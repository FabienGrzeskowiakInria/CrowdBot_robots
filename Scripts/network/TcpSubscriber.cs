using UnityEngine;
using RosSharp.RosBridgeClient.Messages.crowdbotsim;
using RosSharp.RosBridgeClient.Messages;

namespace crowdbotsim
{
    [RequireComponent(typeof(TcpConnector))]
    public abstract class TcpSubscriber : MonoBehaviour
    {
        public string Topic;

        protected virtual void Start()
        {
            GetComponent<TcpConnector>().Subscribe(Topic, ReceiveMessage);
        }

        protected abstract void ReceiveMessage(string Message);

    }
}