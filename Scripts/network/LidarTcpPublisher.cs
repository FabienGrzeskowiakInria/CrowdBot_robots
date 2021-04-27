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

namespace crowdbotsim
{
public class LidarTcpPublisher : TcpPublisher
{
    [SerializeField]
    [Tooltip ("Sensor providing the data to be published")]
    private LidarProvider Sensor;
    
    [SerializeField]
    [Tooltip ("Frame id for Tf")]
    public string FrameId = "undefined";
    private float scanPeriod;
    private float previousScanTime = 0;
    public bool publish_seg_mask = true;
    private float last_timestamp;
    private string message;

    private string sensor_infos;
    private float[] ranges, intensities;
    private int[] seg_mask;


    protected override void Start()
    {
        base.Start();        
        scanPeriod = Sensor.GetScanTime();
        
        sensor_infos = scanPeriod.ToString("0.000");
        sensor_infos = concat_to(sensor_infos,',',FrameId);
        sensor_infos = concat_to(sensor_infos,',',Sensor.GetStartAngle() * Mathf.Deg2Rad); //angle_min
        sensor_infos = concat_to(sensor_infos,',',Sensor.GetEndAngle() * Mathf.Deg2Rad); //angle_max
        sensor_infos = concat_to(sensor_infos,',',Sensor.GetAngularResolution() * Mathf.Deg2Rad); //angle_increment

        int dataLenth = Mathf.RoundToInt((Sensor.GetEndAngle() - Sensor.GetStartAngle()) / Sensor.GetAngularResolution());
        
        sensor_infos = concat_to(sensor_infos,',', Sensor.GetScanTime() / dataLenth); // time_increment
        sensor_infos = concat_to(sensor_infos,',', Sensor.GetMinRange()); // range_min
        sensor_infos = concat_to(sensor_infos,',', Sensor.GetMaxRange()); // range_max

        ranges = new float[dataLenth];
        intensities = new float[dataLenth];
        seg_mask = new int[dataLenth];

        last_timestamp = -scanPeriod;
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
        if(ToolsTime.TimeSinceTrialStarted >= last_timestamp + scanPeriod)
        {
            message = "ranges";
            ranges = Sensor.GetData();
            intensities = Sensor.GetIntensities();
            if(publish_seg_mask)
            {
                seg_mask = Sensor.GetSegMask();
            }

            if(ranges != null && intensities != null) 
            {
                message = concat_to(message,'#',ranges);
                message = concat_to(message,'#',"intensities");
                message = concat_to(message,'#',intensities);
                if(publish_seg_mask && seg_mask != null)
                {
                    message = concat_to(message,'#',"masks");
                    message = concat_to(message, '#', seg_mask);
                }
            }

            last_timestamp = ToolsTime.TimeSinceTrialStarted;
        }

    }
}
}