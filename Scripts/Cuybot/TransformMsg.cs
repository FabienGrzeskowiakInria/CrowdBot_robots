using UnityEngine;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class TransformMsg : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "geometry_msgs/Transform";
        public Messages.Geometry.Vector3 translation;
        public Messages.Geometry.Quaternion rotation;

        public TransformMsg()
        {
            translation = new Messages.Geometry.Vector3();
            rotation = new Messages.Geometry.Quaternion();
            rotation.w = 1;
        }
    }
}