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