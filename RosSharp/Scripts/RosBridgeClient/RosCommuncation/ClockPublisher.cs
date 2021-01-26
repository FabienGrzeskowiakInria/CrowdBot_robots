using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;
using System;

namespace RosSharp.RosBridgeClient
{
public class ClockPublisher : Publisher<Messages.rosgraph_msgs.Clock>
{
    private Messages.rosgraph_msgs.Clock message;

    protected override void Start()
    {
        base.Start();
        message = new Messages.rosgraph_msgs.Clock(); 
        message.clock = new RosSharp.RosBridgeClient.Messages.Standard.Time();
        message.clock.secs = 0;
        message.clock.nsecs = 0;
    }

    private void FixedUpdate()
    {
        float time = ToolsTime.TrialTime;
        uint secs = (uint)time;
        uint nsecs = (uint)(1e9 *(time-secs));
        message.clock.secs = secs;
        message.clock.nsecs = nsecs;
        Publish(message);
    }
}

}