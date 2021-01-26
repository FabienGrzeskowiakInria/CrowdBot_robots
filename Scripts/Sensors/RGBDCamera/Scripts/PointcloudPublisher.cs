using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PointcloudPublisher : Publisher<Messages.Sensor.PointCloud2>, Publisher
    {
        [SerializeField]
        private RealsenseProvider Sensor;

        private string FrameId = "map";

        private Messages.Sensor.PointField[] PointFields;
        Messages.Sensor.PointCloud2 message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }


        private void FixedUpdate()
        {
            // if (Sensor == null) return;
            // UpdateMessage();
        }

        private void InitializeMessage()
        {
            PointFields = new Messages.Sensor.PointField[4];
            for(int i = 0; i< 4; ++i)
            {
                PointFields[i] = new Messages.Sensor.PointField();
            }

            PointFields[0].name = "x";
            PointFields[0].offset = 0;
            PointFields[0].datatype = Messages.Sensor.PointField.FLOAT32;
            PointFields[0].count = 1;

            PointFields[1].name = "y";
            PointFields[1].offset = 4;
            PointFields[1].datatype = Messages.Sensor.PointField.FLOAT32;
            PointFields[1].count = 1;

            PointFields[2].name = "z";
            PointFields[2].offset = 8;
            PointFields[2].datatype = Messages.Sensor.PointField.FLOAT32;
            PointFields[2].count = 1;

            PointFields[3].name = "rgb";
            PointFields[3].offset = 12;
            PointFields[3].datatype = Messages.Sensor.PointField.FLOAT32;
            PointFields[3].count = 1;

            message = new Messages.Sensor.PointCloud2();
            message.header = new Messages.Standard.Header();
            message.header.frame_id = FrameId;
            message.height = Sensor.GetHeight();
            message.width = Sensor.GetWidth();
            message.is_dense = true;
            message.fields = PointFields;
            message.point_step = 16;
            message.row_step = message.point_step * message.width;
            message.data = Sensor.GetPoints();
            message.is_bigendian = !BitConverter.IsLittleEndian;
        }

        public void UpdateMessage()
        {
            message.header.Update();
            message.data = Sensor.GetCvData();
            Publish(message);
        }
    }
}