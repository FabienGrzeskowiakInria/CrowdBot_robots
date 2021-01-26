using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class CrowdStampedSubscriber : Subscriber<Messages.crowdbotsim.CrowdStamped>
    {
        private bool isMessageReceived;

        public Vector3[] crowd_pose;

        protected override void Start()
        {
            int length = GameObject.FindGameObjectsWithTag("VirtualHumanActive").Length;
            crowd_pose = new Vector3[length+1]; // + 1 for robot
            for(int i = 0; i < length+1; i++)
            {
                crowd_pose[i] = new Vector3(0,0,0);
            }
            base.Start();
        }

        protected override void ReceiveMessage(Messages.crowdbotsim.CrowdStamped message)
        {
            for(int i = 0; i < message.crowd.Length; i++){
                crowd_pose[i] = ProcessMessage(message.crowd[i]);
            }
            isMessageReceived = true;
        }

        private Vector3 ProcessMessage(Messages.crowdbotsim.Agent agent)
        {
            Vector3 new_pos = new Vector3();
            new_pos.x = agent.position.x;
            new_pos.y = agent.position.z;
            new_pos.z = agent.position.y;
            return new_pos;
        }

        public void GetAgents(List<Vector3> to_fill)
        {
          for(int i = 0; i < crowd_pose.Length; i++)
          {
            to_fill.Add(crowd_pose[i]);
          }
          return;
        }
    }
}
