using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages.Geometry
{

public class TransformStamped : Message
{
[JsonIgnore]
public const string RosMessageName = "geometry_msgs/TransformStamped";

public Messages.Standard.Header header;
public string child_frame_id;
public Messages.Geometry.Transform transform;
public TransformStamped()
{
header = new Messages.Standard.Header();
child_frame_id = "";
transform = new Messages.Geometry.Transform();
}
}

}