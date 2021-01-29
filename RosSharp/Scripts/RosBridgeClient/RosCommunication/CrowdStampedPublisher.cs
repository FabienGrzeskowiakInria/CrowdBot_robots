using UnityEngine;
using System.Collections.Generic;
using CrowdMP;

namespace RosSharp.RosBridgeClient
{
    public class CrowdStampedPublisher : Publisher<Messages.crowdbotsim.CrowdStamped>, Publisher
    {
        [System.NonSerialized]
        public CrowdBotAgent[] agents;

        public struct CrowdBotAgent
        {
            public int id;
            public Vector3 position;
            public Vector3 velocity_linear;
            public Vector3 velocity_angular;
            public Vector3 goal;
            public CrowdBotAgent(int id_, Vector3 pos_, 
            Vector3 v_l_, Vector3 v_a_, Vector3 goal_)
            {
                id = id_;
                position = pos_;
                velocity_linear = v_l_;
                velocity_angular = v_a_;
                goal = goal_;
            }
        }
        private Messages.crowdbotsim.CrowdStamped message;

        [System.NonSerialized]
        private bool initialized = false;

        private Vector3[] old_pos;
        private Vector3[] old_vel;
        private float old_timestamp;

        protected override void Start()
        {
            base.Start();
            old_pos = new Vector3[agents.Length];
            old_vel = new Vector3[agents.Length];
            for(int i = 0; i < agents.Length; i++){
                old_pos[i] = Vector3.zero;
                old_vel[i] = Vector3.zero;
            }
            old_timestamp = ToolsTime.TrialTime;
            InitializeMessage();
        }


        private void InitializeMessage()
        {
            message = new Messages.crowdbotsim.CrowdStamped();
            message.header = new Messages.Standard.Header();
            message.header.stamp = new RosBridgeClient.Messages.Standard.Time();
            message.crowd = new Messages.crowdbotsim.Agent[agents.Length];

            for(int i = 0; i < agents.Length; i++){
                message.crowd[i] = new Messages.crowdbotsim.Agent();
                message.crowd[i].position = new Messages.Geometry.Vector3();
                message.crowd[i].velocity = new Messages.Geometry.Twist();
                message.crowd[i].goal = new Messages.Geometry.Vector3();
            }
            initialized = true;
        }

        public void UpdateAgents(List<Vector3> currPos, List<Agent> CrowdMPAgentsGoals)
        {
            if(!initialized) Start();
            
            if(old_pos.Length != agents.Length)
            {
                old_pos = new Vector3[agents.Length];
                old_vel = new Vector3[agents.Length];
                for(int i = 0; i < agents.Length; i++)
                {
                    old_pos[i] = agents[i].position;
                    old_vel[i] = agents[i].velocity_linear;
                }            
            }

            float dt = ToolsTime.TrialTime - old_timestamp;
            old_timestamp = ToolsTime.TrialTime;

            for(int i = 0; i < agents.Length; i++)
            {
                agents[i].id = (int)(CrowdMPAgentsGoals[i].id);
                agents[i].position = currPos[i];
                agents[i].goal = CrowdMPAgentsGoals[i].Position;
                agents[i].velocity_linear = (currPos[i] - old_pos[i])/dt;
                agents[i].velocity_angular = new Vector3(0,
                    Vector3.SignedAngle(old_vel[i], (currPos[i] - old_pos[i])/dt, Vector3.up)*Mathf.Deg2Rad,0);
                old_pos[i] = agents[i].position;
                old_vel[i] = agents[i].velocity_linear;
            }
        }

        public void UpdateMessage()
        {
            if(agents.Length != message.crowd.Length) InitializeMessage();

            for(int i = 0; i < agents.Length; i++){
                message.crowd[i].position = ToRosVector3(agents[i].position);
                message.crowd[i].velocity.linear = ToRosVector3(agents[i].velocity_linear);
                message.crowd[i].velocity.angular = ToRosVector3(agents[i].velocity_angular);
                message.crowd[i].goal = ToRosVector3(agents[i].goal);
            }
            Publish(message);
        }       

        private static Messages.Geometry.Vector3 ToRosVector3(Vector3 v){
            Messages.Geometry.Vector3 v3Ros = new Messages.Geometry.Vector3();
            v3Ros.x = v.x;
            v3Ros.y = v.y;
            v3Ros.z = v.z;
            return v3Ros;
        }

        private static Messages.Geometry.Vector3 substract(Messages.Geometry.Vector3 a, Messages.Geometry.Vector3 b)
        {
            Messages.Geometry.Vector3 c = new Messages.Geometry.Vector3();
            c.x = a.x - b.x;
            c.y = a.y - b.y;
            c.z = a.z - b.z;
            return c;
        }

        private static Messages.Geometry.Vector3 divide(Messages.Geometry.Vector3 a, float b)
        {
            Messages.Geometry.Vector3 c = new Messages.Geometry.Vector3();
            if(Mathf.Abs(b) > 0)
            {
                c.x = a.x/b;
                c.y = a.y/b;
                c.z = a.z/b;
            }
            else
            {
                c.x = 0;
                c.y = 0;
                c.z = 0;
            }
            return c;
        }
    }
}