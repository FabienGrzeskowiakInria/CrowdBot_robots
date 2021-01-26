using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;
using System;

namespace RosSharp.RosBridgeClient{
public class InfraredPublisher : Publisher<Messages.Standard.Float64>
{
    [SerializeField]
    [Tooltip ("Sensor that will provide the data to be published")]
    private InfraredSensorProvider Sensor;

    private Messages.Standard.Float64 message;

    protected override void Start()
    {
        base.Start();
        InitializeMessage();
    }
    private void InitializeMessage()
    {
        message = new Messages.Standard.Float64();
        message.data = 0.0f;
    }

    private void FixedUpdate()
    {
        UpdateMessage();
    }

    private void UpdateMessage()
    {
        message.data = Sensor.GetData();
        Publish(message);
    }


}
}