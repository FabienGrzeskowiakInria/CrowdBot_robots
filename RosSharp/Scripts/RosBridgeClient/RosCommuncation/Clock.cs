using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages.rosgraph_msgs
{
public class Clock : Message
{
[JsonIgnore]
public const string RosMessageName = "rosgraph_msgs/Clock";

public Time clock;

public Clock()
{
clock = new Time();
}
}
}