using UnityEngine;
using System.Collections.Generic;

namespace RosSharp.RosBridgeClient
{
    public class TwistArrayStampedSubscriber : Subscriber<Messages.crowdbotsim.TwistArrayStamped>
    {
        [System.NonSerialized]
        public UnityTwist[] ut;
        [System.NonSerialized]
        public Rigidbody[] agent;
        private bool isMessageReceived;

        [System.NonSerialized]
        public bool started = false;
        

        public struct UnityTwist {
            public Vector3 linear;
            public Vector3 angular;
        }

        private static UnityTwist ToUnityTwist(Messages.Geometry.Twist twist){
            UnityTwist ut = new UnityTwist();
            ut.linear = new Vector3(twist.linear.x, twist.linear.y, twist.linear.z);
            ut.angular = new Vector3(twist.angular.x, twist.angular.y, twist.angular.z);
            return ut;
        }

        public void startup(){
            if(started) return;
            else{
                foreach(Rigidbody rb in agent){
                    rb.drag = 0;
                }
                started = true;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void ReceiveMessage(Messages.crowdbotsim.TwistArrayStamped message)
        {
            for(int i = 0; i < message.twist.Length; i++){
                ut[i] = ToUnityTwist(message.twist[i]); 
            }
            isMessageReceived = true;
        }

        private void Update()
        {
            if (started)
                ProcessMessage();
        }

        private void ProcessMessage()
        {
            for(int i = 0; i < agent.Length; i++){
                agent[i].transform.rotation = Quaternion.LookRotation(ut[i].linear);
                agent[i].velocity = ut[i].linear;
            }
            isMessageReceived = false;
        }
    }
}