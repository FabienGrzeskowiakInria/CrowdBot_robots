using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ClockSubscriber : Subscriber<Messages.rosgraph_msgs.Clock>
    {
        private float deltaTime = 0;
        private float lastTime = 0;

        private uint secs = 0;
        private uint nsecs = 0;

        private float currentTime;

        private bool first_message = true;

        public float DeltaTime { get { 
            float dt = Mathf.Max(0.0f, deltaTime);
            deltaTime = 0;
            return dt;
        }}
        public float Time { get { return currentTime; }}

		protected override void Start()
		{
			base.Start();
		}
		
        protected override void ReceiveMessage(Messages.rosgraph_msgs.Clock message)
        {
            secs = message.clock.secs;
            nsecs = message.clock.nsecs;
            currentTime = (float)(secs) + (float)(nsecs)/1e9f;
            deltaTime = first_message ? 0 : currentTime - lastTime;
            first_message = false;
            lastTime = currentTime; 
        }

    }
}
