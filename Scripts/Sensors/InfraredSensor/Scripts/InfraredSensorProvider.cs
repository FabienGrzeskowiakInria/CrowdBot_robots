﻿// # MIT License

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
using System.Collections.Generic;
using System.Collections;

public class InfraredSensorProvider : SensorProvider1D
{
    [SerializeField]
    [Tooltip ("Angle of incidence beyond which an object is no longer detected")]
    private float MaxDetectableIncidenceAngle = 45f;

    [SerializeField]
    [Tooltip ("Angular dispersion in ° along the sensors axis")]
    private float AngularDispersion = 0.5f;

    [SerializeField]
    [Tooltip ("Number of rays to use. A higher number will be more realistic but requires more resources")]
    private int NumberOfSamples = 100;

    private float Radius;
    private System.Random RandomGenerator;

    public override void Start()
    {
        base.Start();
        Radius = MaxRange * Mathf.Tan(AngularDispersion * Mathf.Deg2Rad);
        RandomGenerator = new System.Random();
    }

    public override IEnumerator Sense()
    {
        Sensing = true;
        yield return new WaitForSeconds(SensingFrequency == 0 ? 0 : 1f / SensingFrequency);
        RaycastHit hit;
        float value = Mathf.Infinity;
        for (int i = 0; i < NumberOfSamples; ++i)
        {
            float r = Radius * (float)RandomGenerator.NextDouble();
            float theta = 2 * Mathf.PI * (float)RandomGenerator.NextDouble();
            Vector3 endpoint = transform.forward * MaxRange;
            endpoint.x = r * Mathf.Cos(theta) + transform.position.x;
            endpoint.y = r * Mathf.Sin(theta) + transform.position.y;
            Vector3 ray_dir = endpoint - transform.position;
            if (Physics.Raycast(transform.position, ray_dir, out hit, MaxRange))
            {
                float Angle = Vector3.Angle(-ray_dir, hit.normal);
                if (DebugInfo)
                    Debug.Log(Angle);
                // Check if incidence angle is in the detectable range. Also check if object is in the deadzone
                if (Angle <= MaxDetectableIncidenceAngle && Angle >= -MaxDetectableIncidenceAngle && hit.distance >= MinRange)
                {
                    if (hit.distance < value)
                    {
                        value = hit.distance;
                    }
                }
            }
            if (DebugInfo)
                Debug.DrawLine(transform.position, endpoint, Color.red);
        }
        SensorValue = value;
        foreach (AdditiveNoiseGenerator gen in AdditiveNoiseGenerators)
        {
            SensorValue += (float)gen.GetValue();
        }
        QuantiseSensorData();
        Sensing = false;
    }
    
}
