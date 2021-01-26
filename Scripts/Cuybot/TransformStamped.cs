using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class TransformStamped : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "geometry_msgs/TransformStamped";
        public Messages.Standard.Header header;
        public string child_frame_id;
        public TransformMsg transform;

        public TransformStamped()
        {
            header = new Messages.Standard.Header();
            child_frame_id = "";
            transform = new TransformMsg();
        }
    }
}