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
using UnityEngine;
using System;
using System.Linq;

namespace crowdbotsim
{
public class RGBDTcpPublisher : TcpPublisher
{
    [SerializeField]
    [Tooltip ("Sensor providing the data to be published")]
    private RGBProvider RGB_sensor;

    [SerializeField]
    [Tooltip ("Sensor providing the data to be published")]
    private RealsenseProvider Depth_sensor;
    
    [SerializeField]
    [Tooltip ("Frame id for Tf")]
    public string FrameId = "RGBD";
    private string message;
    private string sensor_infos;

    protected override void Start()
    {
        base.Start();        
        sensor_infos = "image_size";
        sensor_infos = concat_to(sensor_infos,',',RGB_sensor.GetWidth());
        sensor_infos = concat_to(sensor_infos,',',RGB_sensor.GetHeight());
    }

    public override string Publish(string data_id, float time)
    {
        string header = concat_to(Topic, '=', data_id);

        // publish infos on first message only
        // if(int.Parse(data_id) == 0)
        // {
        //     header = concat_to(header, '-', sensor_infos);
        // }

        return concat_to(header, '#', message);
    }

    public void LateUpdate()
    {
        UpdateMessage();
    }

    private void UpdateMessage()
    {
        if(ToolsTime.DeltaTime > 0)
        {
            if(RGB_sensor.GetRawData() == null ) return;
            message = "bytesRGB";
            message = concat_to(message,'#', BitConverter.ToString(RGB_sensor.GetRawData()));
            message = concat_to(message, '#', "bytesD");
            message = concat_to(message,'#', BitConverter.ToString(Depth_sensor.GetCvData()));
        }

    }
}
}
