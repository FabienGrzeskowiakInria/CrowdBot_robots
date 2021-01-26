using UnityEngine;
using System.Linq;

namespace crowdbotsim
{
public class RGBTcpPublisher : TcpPublisher
{
    [SerializeField]
    [Tooltip ("Sensor providing the data to be published")]
    private RGBProvider Sensor;
    
    [SerializeField]
    [Tooltip ("Frame id for Tf")]
    public string FrameId = "undefined";
    private string message;
    private string sensor_infos;

    protected override void Start()
    {
        base.Start();        
        sensor_infos = "image_size";
        sensor_infos = concat_to(sensor_infos,',',Sensor.GetWidth());
        sensor_infos = concat_to(sensor_infos,',',Sensor.GetHeight());
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
            message = "JPG";
            message = concat_to(message,'#', string.Join(" ", Sensor.GetRawData().Select(b => b.ToString()) ));
        }

    }
}
}