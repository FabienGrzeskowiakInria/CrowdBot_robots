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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperLidarProvider : SensorProvider
{
    [SerializeField]
    [Tooltip("Angle between each rays in °")]
    private float AngularResolution = 0.5f;

    [SerializeField]
    [Tooltip("Angle at which to start the scan in °")]
    private float StartAngle = -60.0f;

    [SerializeField]
    [Tooltip("Angle at which to stop the scan in °")]
    private float EndAngle = 60.0f;

    private float[] Xvalues;
    private float[] Yvalues;
    private int nbMeasures;
    private byte[] RawData;

    public override void Start()
    {
        base.Start();
        nbMeasures = Mathf.Abs(Mathf.RoundToInt((EndAngle - StartAngle) / AngularResolution));
        nbMeasures = nbMeasures == 0 ? 1 : nbMeasures;
        Xvalues = new float[nbMeasures];
        Yvalues = new float[nbMeasures];
        RawData = new byte[nbMeasures * 8];
}

    public override IEnumerator Sense()
    {
        Sensing = true;
        yield return new WaitForSeconds(SensingFrequency == 0 ? 0 : 1f / SensingFrequency);
        RaycastHit hit;
        for (int i = 0; i < nbMeasures; ++i)
        {
            float currentAngle = EndAngle - i * AngularResolution;
            Vector3 direction = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
            Ray ray_dir = new Ray(transform.position, direction);
            if (Physics.Raycast(ray_dir, out hit, MaxRange) && hit.distance > MinRange)
            {
                float incidenceAngle = Vector3.Angle(hit.normal, -direction);

                /*Vector3 point = new Vector3(Xvalues[i], Yvalues[i], 0.0f);

                // Position in the point in the base frame
                //point = Quaternion.Inverse(transform.parent.rotation) * (hit.point - transform.parent.position);
                point = hit.point;
                //Xvalues[i] = hit.distance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                //Yvalues[i] = hit.distance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
                Xvalues[i] = point.x;
                Yvalues[i] = point.z;*/

                Xvalues[i] = hit.distance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                Yvalues[i] = hit.distance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

                foreach (AdditiveNoiseGenerator gen in AdditiveNoiseGenerators)
                {
                    Xvalues[i] += (float)gen.GetValue();
                    Yvalues[i] += (float)gen.GetValue();
                }

                Xvalues[i] *= 3;
                Yvalues[i] *= 3;

                if (DebugInfo)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.yellow);
                    Debug.Log(new Vector2(Xvalues[i], Yvalues[i]));
                }
            }
            else
            {
                Xvalues[i] = MaxRange * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                Yvalues[i] = MaxRange * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
                if (DebugInfo)
                    Debug.DrawRay(transform.position, direction * MaxRange, Color.red);
            }
            direction = Vector3.RotateTowards(direction, -transform.right, AngularResolution * Mathf.Deg2Rad, 0.0f);
        }
        QuantiseSensorData();
        Sensing = false;
    }

    public byte[] GetData()
    {
        byte[] currentX = new byte[4];
        byte[] currentY = new byte[4];
        for (int i = 0; i < nbMeasures; ++i)
        {
            int tmp_index = i * 8;
            GetBytesFromFloat(ref currentX, Xvalues[i]);
            GetBytesFromFloat(ref currentY, Yvalues[i]);
            for (int offset = 0; offset < 4; ++offset)
            {
                RawData[tmp_index + offset] = currentX[offset];
                RawData[tmp_index + offset + 4] = currentY[offset];
            }
        }
        return RawData;
    }

    public override void QuantiseSensorData()
    {
        for (int i = 0; i < nbMeasures; ++i)
       {
            Xvalues[i] = Mathf.FloorToInt(Xvalues[i] / QuantisationStep) * QuantisationStep;
            Yvalues[i] = Mathf.FloorToInt(Yvalues[i] / QuantisationStep) * QuantisationStep;
        }
    }
}