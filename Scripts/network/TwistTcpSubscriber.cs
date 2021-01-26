
using UnityEngine;
using RosSharp.RosBridgeClient.Messages.crowdbotsim;
using RosSharp.RosBridgeClient.Messages;
using System.Globalization;
using System.Linq;
using System;

namespace crowdbotsim
{
    public class TwistTcpSubscriber : TcpSubscriber
    {

        public enum Mode {kinematic, force, motor_base_velocity, motor_wheel_control};
        public Transform SubscribedTransform;
        public Mode mode = Mode.kinematic;
        public float max_acceleration = 0.8f;
        public float max_angular_acceleration = 20.0f;
        private Rigidbody base_link;
        private float previousRealTime;
        private float f_linearVelocity, f_angularVelocity;
        private Vector3 linearVelocity, angularVelocity, current_forward, current_up;
        private Vector3 current_linear_velocity, current_angular_velocity;
        private Vector3 acceleration = new Vector3(0,0,0);
        private Vector3 angular_acceleration = new Vector3(0,0,0);

        private DiffDriveController controller;

        protected override void Start()
        {
            base.Start();

            current_forward = SubscribedTransform.InverseTransformDirection(SubscribedTransform.forward);
            current_up = SubscribedTransform.InverseTransformDirection(SubscribedTransform.up);
            current_linear_velocity = new Vector3(0,0,0);
            current_angular_velocity = new Vector3(0,0,0);

            base_link = SubscribedTransform.GetComponent<Rigidbody>();

            try
            {
                controller = SubscribedTransform.GetComponent<DiffDriveController>();
            }
            catch (System.Exception)
            {
                mode = Mode.kinematic;                
                throw;
            }
        }

        public void Stop()
        {
            current_angular_velocity = Vector3.zero;
            angular_acceleration = Vector3.zero;
            current_linear_velocity = Vector3.zero;
            acceleration = Vector3.zero;
        }

        protected override void ReceiveMessage(string str_velocity)
        {
            float[] velocity = str_velocity.Split(' ').Select(v => float.Parse(v, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
            f_linearVelocity = velocity[0];
            f_angularVelocity = velocity[1];
        }

        private void Update()
        {
            float deltaTime = ToolsTime.DeltaTime;
            current_forward = SubscribedTransform.InverseTransformDirection(SubscribedTransform.forward);
            current_up = -SubscribedTransform.InverseTransformDirection(SubscribedTransform.up);
            linearVelocity = f_linearVelocity * current_forward;
            angularVelocity = f_angularVelocity * current_up;

            if(deltaTime > 0)
            {
                switch(mode)
                {
                    case Mode.force:
                        if(deltaTime > 0)
                        {
                            if(linearVelocity.magnitude > 0)
                            {
                                Vector3 force = base_link.transform.rotation * (linearVelocity - base_link.velocity) * base_link.mass / deltaTime * 2;
                                base_link.AddForce( new Vector3 (force.x, 0, force.z) );
                            }
                        
                            if (angularVelocity.magnitude > 0)
                            {
                                Vector3 torque = base_link.transform.rotation * (angularVelocity - base_link.angularVelocity) * base_link.mass / deltaTime * 2;
                                base_link.AddTorque( new Vector3 (0, torque.y , 0)   );
                            }

                        }
                    break;
            
                    case Mode.kinematic:
                        current_angular_velocity = Vector3.SmoothDamp(current_angular_velocity, angularVelocity, ref angular_acceleration, 1/max_angular_acceleration, Mathf.Infinity, deltaTime);
                        SubscribedTransform.Rotate(current_angular_velocity * deltaTime);
                        current_linear_velocity = Vector3.SmoothDamp(current_linear_velocity, linearVelocity, ref acceleration, 1/max_acceleration, Mathf.Infinity, deltaTime);
                        SubscribedTransform.Translate(current_linear_velocity * deltaTime);

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
            }


        }

    }
}