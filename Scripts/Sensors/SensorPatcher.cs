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
