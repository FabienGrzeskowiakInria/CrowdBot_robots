// # MIT License

// # Copyright (C) 2018-2021  CrowdBot H2020 European project
// # Inria Rennes Bretagne Atlantique - Rainbow - Julien Pettré

// # Permission is hereby granted, free of charge, to any person obtaining
// # a copy of this software and associated documentation files (the
// # "Software"), to deal in the Software without restriction, including
// # without limitation the rights to use, copy, modify, merge, publish,
// # distribute, sublicense, and/or sell copies of the Software, and to
// # permit persons to whom the Software is furnished to do so, subject
// # to the following conditions:

// # The above copyright notice and this permission notice shall be
// # included in all copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// # OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// # LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;
using System;
using System.Linq;

namespace RosSharp.RosBridgeClient{
public class LidarPublisher : Publisher<Messages.Sensor.LaserScan>, Publisher
{
    [SerializeField]
    [Tooltip ("Sensor providing the data to be published")]
    private LidarProvider Sensor;

    public string FrameId = "Unity";

    private Messages.Sensor.LaserScan message;
    private float scanPeriod;
    private float previousScanTime = 0;

    public bool publish_seg_mask = false;

    private float last_timestamp;

    protected override void Start()
    {
        last_timestamp = ToolsTime.TimeSinceTrialStarted;
        base.Start();
        InitializeMessage();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Sensor.StopCoroutine(Sensor.Sense());
        Sensor.Sensing = false;
    }

    private void InitializeMessage()
    {
        scanPeriod = Sensor.GetScanTime();

        message = new Messages.Sensor.LaserScan();
        message.header = new Messages.Standard.Header { frame_id = FrameId };
        message.angle_min       = Sensor.GetStartAngle() * Mathf.Deg2Rad;
        message.angle_max       = Sensor.GetEndAngle() * Mathf.Deg2Rad;
        message.angle_increment = Sensor.GetAngularResolution() * Mathf.Deg2Rad;
        int dataLenth = Mathf.RoundToInt((Sensor.GetEndAngle() - Sensor.GetStartAngle()) / Sensor.GetAngularResolution());
        message.time_increment = Sensor.GetScanTime() / dataLenth; 
        message.range_min = Sensor.GetMinRange();
        message.range_max = Sensor.GetMaxRange();
        message.ranges = new float[dataLenth];
        message.intensities = new float[dataLenth];
    }
    private void FixedUpdate()
    {
        // if (Time.realtimeSinceStartup >= previousScanTime + scanPeriod)
        // {
            // UpdateMessage();
            // previousScanTime = Time.realtimeSinceStartup;
        // }
    }

    public void UpdateMessage()
    {
        if(ToolsTime.TimeSinceTrialStarted < last_timestamp + scanPeriod) return;

        message.header.Update();
        message.ranges = Sensor.GetData();
        if(publish_seg_mask)
        {
            message.intensities = Sensor.GetSegMask().Select(a => (float)a).ToArray();
        }
        else
        {
            message.intensities = Sensor.GetIntensities();
        }
        if(message.ranges != null) Publish(message);
        int dataLenth = Mathf.RoundToInt((Sensor.GetEndAngle() - Sensor.GetStartAngle()) / Sensor.GetAngularResolution());

        last_timestamp = ToolsTime.TimeSinceTrialStarted;
    }
}

}