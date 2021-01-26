using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidar2DProvider : SensorProvider2D
{
    [SerializeField]
    [Tooltip ("Angle between each rays in °")]
    private float AngularResolution = 0.5f;

    [SerializeField]
    [Tooltip ("Angle on the horizontal plane to start the scan at in °")]
    private float HorizontalStartAngle = -60.0f;

    [SerializeField]
    [Tooltip ("Angle on the horizontal plane to stop the scan at in °")]
    private float HorizontalEndAngle = 60.0f;

    [SerializeField]
    [Tooltip ("Angle on the vertical plane to start the scan at in °")]
    private float VerticalStartAngle = -30f;

    [SerializeField]
    [Tooltip ("Angle on the vertical plane to end the scan at in °")]
    private float VerticalEndAngle = 30f;

    private int nbHMeasures;
    private int nbVMeasures;

    protected float[] Intensities;

    protected byte[] PointCloud;

    public override void Start()
    {
        base.Start();
        nbHMeasures = Mathf.RoundToInt((HorizontalEndAngle - HorizontalStartAngle) / AngularResolution);
        nbVMeasures = Mathf.RoundToInt((VerticalEndAngle - VerticalStartAngle) / AngularResolution);
        SensorValues = new float[nbHMeasures*nbVMeasures];
        Intensities = new float[nbHMeasures*nbVMeasures];
        PointCloud = new byte[(nbHMeasures*nbVMeasures)*16];
    }


    public override IEnumerator Sense()
    {
        Sensing = true;
        yield return new WaitForSeconds(SensingFrequency == 0 ? 0 : 1f / SensingFrequency);
        RaycastHit hit;
        Vector3 transformPosition = transform.position;
        PointCloud = new byte[(nbHMeasures*nbVMeasures)*16];
        for (int i = 0; i < nbHMeasures; ++i)
        {
            for (int j = 0; j < nbVMeasures; ++j)
            {
                float currentHAngle = HorizontalStartAngle + i * AngularResolution;
                float currentVAngle = VerticalStartAngle + j * AngularResolution;
                Quaternion rotation = Quaternion.AngleAxis(currentVAngle, Vector3.left) * Quaternion.AngleAxis(currentHAngle, Vector3.up);
                Vector3 direction = rotation * transform.forward;
                Ray ray_dir = new Ray(transformPosition, direction);
                if (Physics.Raycast(ray_dir, out hit, MaxRange) && hit.distance > MinRange)
                {
                    float incidenceAngle = Vector3.Angle(hit.normal, -direction);
                    Intensities[i+j] = Mathf.Cos(incidenceAngle*Mathf.Deg2Rad);
                    SensorValues[i+j] = hit.distance;
                    foreach (AdditiveNoiseGenerator gen in AdditiveNoiseGenerators)
                    {
                        SensorValues[i] += (float)gen.GetValue();
                    }
                    if (DebugInfo)
                        Debug.DrawLine(transformPosition, hit.point, Color.yellow);

                    // this should take the noise into account accurately
                    Vector3 point = hit.point - transform.position;

                    byte[] bytePointx = GetBytesFromFloat(-point.z);
                    byte[] bytePointy = GetBytesFromFloat(point.x);
                    byte[] bytePointz = GetBytesFromFloat(point.y);
                    byte[] byteIntensities = GetBytesFromFloat(Intensities[i+j]);

                    for(int k = 0; k < 4; ++k){
                        PointCloud[(i*nbVMeasures+j)*16+k] = bytePointx[k];
                        PointCloud[(i*nbVMeasures+j)*16+k+4] = bytePointy[k];
                        PointCloud[(i*nbVMeasures+j)*16+k+8] = bytePointz[k];
                        PointCloud[(i*nbVMeasures+j)*16+k+12] = byteIntensities[k];
                    }
                }
                else
                {
                    SensorValues[i] = 0;
                    if (DebugInfo)
                        Debug.DrawRay(transformPosition, direction * MaxRange, Color.red);
                }
            }
        }
        QuantiseSensorData();
        Sensing = false;
    }

    public float[] GetIntensities()
    {
        return Intensities;
    }

    public byte[] GetPoints()
    {
        return PointCloud;
    }

    public float GetAngularResolution()
    {
        return AngularResolution;
    }

    public float GetStartAngle()
    {
        return HorizontalStartAngle;
    }

    public float GetEndAngle()
    {
        return HorizontalEndAngle;
    }

    public float GetScanTime()
    {
        return SensingFrequency == 0 ? 0 : 1f / SensingFrequency;
    }

    private byte[] GetBytesFromFloat(float value)
    {
        var floatArray = new float[] {value};
        var output = new byte[4];
        Buffer.BlockCopy(floatArray, 0, output, 0, output.Length);
        // Array.Reverse(output,0,output.Length);
        return output;
    }

    public uint GetHeight()
    {
        return (uint)nbVMeasures;
    }

    public uint GetWidth()
    {
        return (uint)nbHMeasures;
    }
}
