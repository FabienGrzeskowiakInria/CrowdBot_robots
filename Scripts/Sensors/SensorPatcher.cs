using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crowdbotsim;
using RosSharp.RosBridgeClient;

public class SensorPatcher : MonoBehaviour
{
    // Start is called before the first frame update
    public void CreatePublishers(bool create)
    {
        if(create)
        {
            GameObject[] sensors = GameObject.FindGameObjectsWithTag("Sensor");
            int i = 0;
            foreach(GameObject sensor in sensors)
            {
                UltrasoundSensorProvider sense = sensor.GetComponent<UltrasoundSensorProvider>();
                if(sense != null)
                {
                    UltrasoundPublisher pub = transform.gameObject.AddComponent<UltrasoundPublisher>();
                    pub.Topic = "us"+i.ToString();
                    pub.Sensor = sense;
                    pub.enabled = false;
                }
                i++;
            }
        }
        else
        {
            foreach(UltrasoundPublisher pub in transform.gameObject.GetComponents<UltrasoundPublisher>())
            {
                DestroyImmediate(pub);
            }
        }

    }
}
