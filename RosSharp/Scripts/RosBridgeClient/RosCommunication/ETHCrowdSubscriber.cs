using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class ETHCrowdSubscriber : Subscriber<Messages.crowdbotsim.TwistArrayStamped>
    {
        private bool isMessageReceived;

        public List<Vector3> crowd_pose;

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.crowdbotsim.TwistArrayStamped message)
        {
            crowd_pose = new List<Vector3>();
            for(int i = 0; i < message.twist.Length; i++){
                crowd_pose.Add(ProcessMessage(message.twist[i]));
            }
            isMessageReceived = true;
        }

        private Vector3 ProcessMessage(Messages.Geometry.Twist agent)
        {
            Vector3 new_pos = new Vector3();
            new_pos.x = agent.linear.x;
            new_pos.y = agent.linear.y;
            new_pos.z = agent.linear.z;
            return new_pos;
        }

        public void GetAgents(List<Vector3> to_fill)
        {
          for(int i = 0; i < crowd_pose.Count; i++)
          {
            to_fill.Add(crowd_pose[i]);
          }
          return;
        }
    }
}
