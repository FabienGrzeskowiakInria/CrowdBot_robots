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
public class Agent : Message
{
[JsonIgnore]
public const string RosMessageName = "crowdbotsim/Agent";

public Messages.Standard.Int32 id;
public Messages.Geometry.Vector3 position;
public Messages.Geometry.Twist velocity;
public Messages.Geometry.Vector3 goal;

public Agent()
{
id = new Messages.Standard.Int32();
position = new Messages.Geometry.Vector3();
velocity = new Messages.Geometry.Twist();
goal = new Messages.Geometry.Vector3();
}
}
}

