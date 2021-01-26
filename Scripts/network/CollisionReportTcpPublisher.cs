
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace crowdbotsim
{
    public class CollisionReportTcpPublisher : TcpPublisher
    {
        public CollisionReport collisionReport;

        [SerializeField]
        [Tooltip ("Frame id for Tf")]
        public string FrameId = "undefined";
        private string message;
        
        protected override void Start()
        {
            base.Start();
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
                message = collisionReport.Get_report();
            }
        }

    }
}