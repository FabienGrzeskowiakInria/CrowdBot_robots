/*
© CentraleSupelec, 2017
Author: Dr. Jeremy Fix (jeremy.fix@centralesupelec.fr)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

// Adjustments to new Publication Timing and Execution Framework
// © Siemens AG, 2018, Dr. Martin Bischoff (martin.bischoff@siemens.com)

using UnityEngine;
using RosSharp;

namespace RosSharp.RosBridgeClient
{
    public class TwistSubscriber : Subscriber<Messages.Geometry.Twist>
    {
        public enum Mode {kinematic, force, motor_base_velocity, motor_wheel_control};
        public Transform SubscribedTransform;
        public Mode mode = Mode.kinematic;

        private Rigidbody rigidbody;
        private float previousRealTime;
        private Vector3 linearVelocity;
        private Vector3 angularVelocity;
        private bool isMessageReceived;

        private DiffDriveController controller;

        protected override void Start()
        {
            base.Start();
            rigidbody = SubscribedTransform.GetComponent<Rigidbody>();

            try
            {
                controller = SubscribedTransform.GetComponent<DiffDriveController>();
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        protected override void ReceiveMessage(Messages.Geometry.Twist message)
        {
            linearVelocity = ToVector3(message.linear).Ros2Unity();
            angularVelocity = ToVector3(message.angular).Ros2Unity()*Mathf.Deg2Rad;
            isMessageReceived = true;
        }

        private static Vector3 ToVector3(Messages.Geometry.Vector3 geometryVector3)
        {
            return new Vector3(geometryVector3.x, geometryVector3.y, geometryVector3.z);
        }

        private void Update()
        {
            // if (isMessageReceived)
                ProcessMessage();
        }
        private void ProcessMessage()
        {
            // float deltaTime = Time.realtimeSinceStartup - previousRealTime;
            float deltaTime = ToolsTime.DeltaTime;

            switch(mode)
            {
                case Mode.force:
                    if(deltaTime > 0)
                    {
                        if(linearVelocity.magnitude > 0)
                        {
                            Vector3 force = rigidbody.transform.rotation * (linearVelocity - rigidbody.velocity) * rigidbody.mass / deltaTime * 2;
                            rigidbody.AddForce( new Vector3 (force.x, 0, force.z) );
                        }
                    
                        if (angularVelocity.magnitude > 0)
                        {
                            Vector3 torque = rigidbody.transform.rotation * (angularVelocity - rigidbody.angularVelocity) * rigidbody.mass / deltaTime * 2;
                            rigidbody.AddTorque( new Vector3 (0, torque.y , 0)   );
                        }

                    }
                break;
        
                case Mode.kinematic:
                    SubscribedTransform.Translate(linearVelocity * deltaTime);
                    Vector3 angularVelocityDeg = angularVelocity*Mathf.Rad2Deg;
                    SubscribedTransform.Rotate(Vector3.forward, angularVelocityDeg.x * deltaTime);
                    SubscribedTransform.Rotate(Vector3.up, angularVelocityDeg.y * deltaTime);
                    SubscribedTransform.Rotate(Vector3.left, angularVelocityDeg.z * deltaTime);
                break;
        
                case Mode.motor_base_velocity:
                    if(controller == null) break;
                    controller.ControlMotorsClosedLoop(deltaTime, linearVelocity.z, angularVelocity.y);
                break;

                case Mode.motor_wheel_control:
                    if(controller == null) break;
                    controller.Accelerate(linearVelocity.x, linearVelocity.y);
                break;

            }
            
            previousRealTime = Time.realtimeSinceStartup;

            isMessageReceived = false;
        }
    }
}