using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages.TF2
{
public class TFMessage : Message
{
[JsonIgnore]
public const string RosMessageName = "tf2_msgs/TFMessage";
public Messages.Geometry.TransformStamped[] transforms;

public TFMessage()
{
transforms = new Messages.Geometry.TransformStamped[0];
}

}

}