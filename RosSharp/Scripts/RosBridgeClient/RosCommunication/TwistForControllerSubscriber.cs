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
using crowdbotsim;

namespace RosSharp.RosBridgeClient
{
    public class TwistForControllerSubscriber : Subscriber<Messages.Geometry.Twist>
    {
        public PepperBaseControl pepper_base;
        public writeMotor2 diff_drive_base;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.Geometry.Twist message)
        {
            if (pepper_base != null)
            {
                pepper_base.theta_speed = message.angular.z;
                pepper_base.x_speed = message.linear.x;// * Mathf.Cos(message.angular.z) + message.linear.y * Mathf.Sin(message.angular.z);
                pepper_base.y_speed = message.linear.y;// * Mathf.Sin(message.angular.z) + message.linear.x * Mathf.Cos(message.angular.z);
            }
            if (diff_drive_base != null)
            {
                diff_drive_base.drive = message.linear.x;
                diff_drive_base.steer = message.angular.z;
            }
        }

        private void Update()
        {
        }
    }
}