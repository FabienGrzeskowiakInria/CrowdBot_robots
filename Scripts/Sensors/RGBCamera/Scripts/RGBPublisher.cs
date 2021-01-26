using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class RGBPublisher : Publisher<Messages.Sensor.CompressedImage>, Publisher
    {
        [SerializeField]
        private RGBProvider Sensor;

        public string FrameId = "base_link";

        private Messages.Sensor.CompressedImage message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     Sensor.StopCoroutine(Sensor.Sense());
        //     Sensor.Sensing = false;
        // }

        private void FixedUpdate()
        {
            // if (Sensor == null || message == null) return;
            // UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Sensor.CompressedImage();
            message.data = new byte[Sensor.GetWidth() * Sensor.GetHeight() * 3];
            message.header.frame_id = FrameId;
            message.format = "jpeg";
        }

        public void UpdateMessage()
        {
            message.header.Update();
            byte[] data = Sensor.GetRawData();
            if(data != null) message.data = data;
            Publish(message);
        }

    }
}