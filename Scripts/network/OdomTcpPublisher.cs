using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace crowdbotsim
{
    public class OdomTcpPublisher : TcpPublisher
    {
        public OdomProvider odom;

        [SerializeField]
        [Tooltip ("Frame id for Tf")]
        public string FrameId = "undefined";
        private string message;
        private string sensor_infos = "";
        private OdomProvider.Pose_and_twist pose_And_Twist;
        
        protected override void Start()
        {
            base.Start();
            
            switch (odom.type)
            {
                case OdomProvider.mobile_base_type.differential_drive:
                    sensor_infos = "diff_drive";
                break;
                
                default:
                    sensor_infos = "other";
                break;
            }
        }

        public override string Publish(string data_id, float time)
        {
            string header = concat_to(Topic, '=', data_id);

            // publish infos on first message only
            // if(int.Parse(data_id) == 0)
            // {
            //     header = concat_to(header, '-', sensor_infos);
            // }

            return concat_to(header, '#', message);
        }

        private void LateUpdate()
        {
            UpdateMessage();
        }

        public void UpdateMessage()
        {
            if(ToolsTime.DeltaTime > 0)
            {
                message = "pose";
                pose_And_Twist = odom.get_pose_and_twist();

                message = concat_to(message, ' ', pose_And_Twist.x);
                message = concat_to(message, ' ', pose_And_Twist.y);
                message = concat_to(message, ' ', pose_And_Twist.theta);
                message = concat_to(message, ' ', pose_And_Twist.dxdt);
                message = concat_to(message, ' ', pose_And_Twist.dydt);
                message = concat_to(message, ' ', pose_And_Twist.dthetadt);
            }
        }

    }
}