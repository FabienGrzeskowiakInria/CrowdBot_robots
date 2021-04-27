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