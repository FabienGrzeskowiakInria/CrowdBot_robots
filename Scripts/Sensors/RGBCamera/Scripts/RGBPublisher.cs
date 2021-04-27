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