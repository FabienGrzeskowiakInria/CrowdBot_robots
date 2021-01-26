using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class RealsensePublisher : Publisher<Messages.Sensor.Image>
    {
        [SerializeField]
        [Tooltip("Sensor providing the data to be published")]
        private RealsenseProvider Sensor;

        public string FrameId = "base_link";

        private Messages.Sensor.Image DepthMap;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            if (Sensor == null || DepthMap == null) return;
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            DepthMap = new Messages.Sensor.Image();
            DepthMap.header = new Messages.Standard.Header();
            DepthMap.header.frame_id = FrameId;
            DepthMap.height = Sensor.GetHeight();
            DepthMap.width = Sensor.GetWidth();
            DepthMap.encoding = "16UC1";
            DepthMap.is_bigendian = Convert.ToByte(!BitConverter.IsLittleEndian);
            DepthMap.step = DepthMap.width * 2;
        }

        private void UpdateMessage()
        {
            DepthMap.header.Update();
            DepthMap.data = Sensor.GetCvData();
            // DepthMap.data = Sensor.GetPoints();
            Publish(DepthMap);
        }
    }
}