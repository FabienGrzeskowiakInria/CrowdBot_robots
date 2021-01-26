using UnityEngine;
using RosSharp.RosBridgeClient.Messages.crowdbotsim;
using RosSharp.RosBridgeClient.Messages;
using System.Linq;

namespace crowdbotsim
{
    [RequireComponent(typeof(TcpConnector))]
    public abstract class TcpPublisher : MonoBehaviour
    {
        public string Topic;

        protected virtual void Start()
        {
            GetComponent<TcpConnector>().Advertise(this);
        }

        public abstract string Publish(string id="-1", float time = -1);
        public static string concat_to(string a, char sep, float[] b)
        {
            return a + sep + string.Join(" ", b.Select(f => f.ToString("0.000"))) ;
        }
        public static string concat_to(string a, char sep, int[] b)
        {
            return a + sep + string.Join(" ", b.Select(i => i.ToString())) ;
        }
        public static string concat_to(string a, char sep, float b)
        {
            return a + sep + b.ToString("0.000");
        }
        public static string concat_to(string a, char sep, string b)
        {
            return a + sep + b;
        }

    }
}