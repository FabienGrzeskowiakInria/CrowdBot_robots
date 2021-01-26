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
public class CrowdStamped : Message
{
[JsonIgnore]
public const string RosMessageName = "crowdbotsim/CrowdStamped";

public Messages.Standard.Header header;
public Messages.crowdbotsim.Agent[] crowd;

public CrowdStamped()
{
header = new Messages.Standard.Header();
crowd = new Messages.crowdbotsim.Agent[0];
}
}
}

