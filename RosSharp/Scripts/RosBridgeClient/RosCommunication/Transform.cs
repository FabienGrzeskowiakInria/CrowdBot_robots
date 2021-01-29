using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages.Geometry
{
public class Transform : Message
{
[JsonIgnore]
public const string RosMessageName = "geometry_msgs/Transform";
public Messages.Geometry.Vector3 translation;
public Messages.Geometry.Quaternion rotation;
public Transform()
{
translation = new Vector3();
rotation = new Quaternion();
}

}

}