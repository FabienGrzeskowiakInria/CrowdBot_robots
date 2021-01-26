using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * StartAngle : angle at which to start each sweep
 * EndAngle : angle at which to end each sweep
 * EndAngle - StartAngle determines the field of view of the sensor
 * MaxRange : Maximum range of the sensor. Objects further than this distance are not detected
 * MinRange : Size of the deadzone near the sensor
 */
public class LidarProvider : SensorProvider2D
{   
    [SerializeField]
    [Tooltip ("Angle between each rays in °")]
    private float AngularResolution = 0.5f;

    [SerializeField]
    [Tooltip ("Angle at which to start the scan in °")]
    private float StartAngle = -60.0f;

    [SerializeField]
    [Tooltip ("Angle at which to stop the scan in °")]
    private float EndAngle = 60.0f;

    protected float[] Intensities;

    protected int[] SegMask;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        Intensities = new float[0];
        SegMask = new int[0];
    }
    
    public static class CoroutineUtil
    {
        public static IEnumerator WaitForRealSeconds(float time)
        {
            float start = ToolsTime.TimeSinceTrialStarted;
            while (ToolsTime.TimeSinceTrialStarted < start + time)
            {
                yield return null;
            }
        }
    }

    public override IEnumerator Sense ()
    {
        if(!isActive) yield return null;

        Sensing = true;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(SensingFrequency == 0 ? 0 : 1f / SensingFrequency));
        int nbMeasures = Mathf.RoundToInt((EndAngle - StartAngle) / AngularResolution);
        SensorValues = new float[nbMeasures];
        Intensities = new float[nbMeasures];
        SegMask = new int[nbMeasures];
        RaycastHit hit;
        for(int i = 0; i < nbMeasures; ++i)
        {
            // float currentAngle = StartAngle + i * AngularResolution;
            float currentAngle = EndAngle - i * AngularResolution;
            Vector3 direction = Quaternion.AngleAxis(currentAngle, Vector3.up) * transform.forward;
            Ray ray_dir = new Ray(transform.position, direction);
            if(Physics.Raycast(ray_dir, out hit, MaxRange) && hit.distance > MinRange)
            {
                float incidenceAngle = Vector3.Angle(hit.normal, -direction);
                Intensities[i] = Mathf.Cos(incidenceAngle*Mathf.Deg2Rad);
                Agent a = hit.transform.root.GetComponent<Agent>();
                if(a != null) SegMask[i] = (int)(a.GetID());
                SensorValues[i] = hit.distance;
                foreach (AdditiveNoiseGenerator gen in AdditiveNoiseGenerators)
                {
                    SensorValues[i] += (float)gen.GetValue();
                }
                if (DebugInfo)
                    Debug.DrawLine(transform.position, hit.point, Color.yellow);
                if (DebugInfo)
                    Debug.Log(hit.collider.gameObject.name);
            }
            else
            {
                SensorValues[i] = MaxRange;
                if (DebugInfo)
                    Debug.DrawRay(transform.position, direction * MaxRange, Color.red);
            }
            direction = Vector3.RotateTowards(direction, -transform.right, AngularResolution * Mathf.Deg2Rad, 0.0f);
        }
        QuantiseSensorData();
        Sensing = false;
	}    

    public float[] GetIntensities()
    {
        return Intensities;
    }
    public int[] GetSegMask()
    {
        return SegMask;
    }
    public float GetAngularResolution()
    {
        return AngularResolution;
    }

    public float GetStartAngle()
    {
        return StartAngle;
    }

    public float GetEndAngle()
    {
        return EndAngle;
    }

    public float GetScanTime()
    {
        return SensingFrequency == 0 ? 0 : 1f / SensingFrequency;
    }
}
