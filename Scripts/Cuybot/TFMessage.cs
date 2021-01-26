using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class TFMessage: Message
    {
        [JsonIgnore]
        public const string RosMessageName = "tf2_msgs/TFMessage";
        public TransformStamped[] transforms;

        public TFMessage()
        {
        }
    }
}