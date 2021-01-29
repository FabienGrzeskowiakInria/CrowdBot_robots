using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class TwistArrayStampedPublisher : Publisher<Messages.crowdbotsim.TwistArrayStamped>, Publisher
    {
        [System.NonSerialized]
        public List<Agent> agents;
        private Messages.crowdbotsim.TwistArrayStamped message;

        [System.NonSerialized]
        private bool initialized = false;

        protected override void Start()
        {
              base.Start();
              InitializeMessage();
        }

        private void FixedUpdate()
        {
            //   UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.crowdbotsim.TwistArrayStamped();
            message.header = new Messages.Standard.Header();
            message.header.stamp = new RosBridgeClient.Messages.Standard.Time();
            message.twist = new Messages.Geometry.Twist[agents.Count];
            for(int i = 0; i < agents.Count; i++){
                message.twist[i] = new Messages.Geometry.Twist();
                message.twist[i].linear = new Messages.Geometry.Vector3();
                message.twist[i].angular = new Messages.Geometry.Vector3();
            }
            initialized = true;
        }
        public void UpdateMessage()
        {
            if(agents.Count != message.twist.Length) InitializeMessage();

            for(int i = 0; i < agents.Count; i++){
                message.twist[i].angular = ToRosVector3(- agents[i].transform.rotation.eulerAngles.Unity2Ros());
                message.twist[i].linear = ToRosVector3(agents[i].transform.position.Unity2Ros());
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
    }
}