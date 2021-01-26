using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp;
using System;

namespace RosSharp.RosBridgeClient{

    public class UltrasoundPublisher : Publisher<Messages.Standard.Float64>, Publisher
    {
        [SerializeField]
        [Tooltip ("Sensor providing the data to be published")]
        public UltrasoundSensorProvider Sensor;

        private Messages.Standard.Float64 message;

        protected override void Start()
        {
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
            message = new Messages.Standard.Float64();
            message.data = 0.0f;
        }

        private void FixedUpdate()
        {
            // UpdateMessage();
        }

        public void UpdateMessage()
        {
            message.data = Sensor.GetData();
            // message.data = 0.0f;
            Publish(message);
        }
    }
}