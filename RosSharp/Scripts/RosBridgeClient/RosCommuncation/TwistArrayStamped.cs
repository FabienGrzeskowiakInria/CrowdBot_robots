/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/ 

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Messages.crowdbotsim
{
public class TwistArrayStamped : Message
{
[JsonIgnore]
public const string RosMessageName = "crowdbotsim/TwistArrayStamped";

public Messages.Standard.Header header;
public Messages.Geometry.Twist[] twist;

public TwistArrayStamped()
{
header = new Messages.Standard.Header();
twist = new Messages.Geometry.Twist[0];
}
}
}

