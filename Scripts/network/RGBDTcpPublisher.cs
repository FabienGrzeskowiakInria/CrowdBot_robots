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
