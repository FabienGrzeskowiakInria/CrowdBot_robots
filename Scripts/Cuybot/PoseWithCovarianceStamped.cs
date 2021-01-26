using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

// Should be part of RosSharp
//namespace RosSharp.RosBridgeClient.Messages.Geometry

namespace Cuybot.ROS
{
 //   public class PoseWithCovarianceStampedMsg : global::RosSharp.RosBridgeClient.Message
    public class PoseWithCovarianceStamped : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "geometry_msgs/PoseWithCovarianceStamped";
        public Messages.Standard.Header header;
        public Messages.Geometry.PoseWithCovariance pose; 

        public PoseWithCovarianceStamped()
        {
            header = new Messages.Standard.Header();
            pose = new Messages.Geometry.PoseWithCovariance();
        }
    }
}