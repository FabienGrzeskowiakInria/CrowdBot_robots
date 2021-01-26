using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class MotorSubscriber : Subscriber<Messages.Standard.Float64>
    {
        public MotorWriter MotorWriter;

		protected override void Start()
		{
			base.Start();
		}
		
        protected override void ReceiveMessage(Messages.Standard.Float64 cmd)
        {
            MotorWriter.Write((float)(cmd.data));
        }
    }
}
