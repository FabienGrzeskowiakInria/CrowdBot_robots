using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Radius : Radius at the "fattest" part of the teardrop
 * MinRange : Deadzone. Distances smaller than this value are ignored
 * MaxRange : Maximum range of the sensor, measured on the main teardrop axis
 * SensingFrequency : frequency of the measures in Hz. Set to zero to scan as fast as possible
 * Bits : number of bits that should be used to represent the data. Used to simulate quantisation of the data
 */
public class UltrasoundSensorProvider : SensorProvider1D
{
    [SerializeField]
    [Tooltip ("Radius of the teardrop at its widest part")]
    private float Radius = 1.0f;

    [SerializeField]
    [Tooltip ("Angle between each rays in °")]
    private float AngularResolution = 0.1f;

    // This way may not be performance friendly
    public override IEnumerator Sense()
    {
        Sensing = true;
        yield return new WaitForSeconds(SensingFrequency == 0 ? 0 : 1 / SensingFrequency);
        float distance = Mathf.Infinity;
        // Maximum radius obtained for Theta = 120°
        // Effective radius is RadiusMultiplier x 3sqrt(3)/8
        float RadiusMultiplier = Radius * 8f / 3f / Mathf.Sqrt(3);
        Vector3 hitPoint = Vector3.back;
        Vector3 transformPosition = transform.position;
        Quaternion transformRotation = transform.rotation;
        for (float Theta = 0f; Theta < Mathf.PI; Theta += AngularResolution)
        {
            float cosT = Mathf.Cos(Theta);
            float sinT = Mathf.Sin(Theta);
            for (float Phi = 0f; Phi < Mathf.PI * 2; Phi += AngularResolution)
            {
                Vector3 endPoint;
                endPoint.x = 0.5f * (1 - cosT) * sinT * Mathf.Cos(Phi) * RadiusMultiplier;
                endPoint.y = 0.5f * (1 - cosT) * sinT * Mathf.Sin(Phi) * RadiusMultiplier;
                // The teardrop has a total length of MaxRange. The translation puts the tail of the drop at the origin
                endPoint.z = cosT * MaxRange / 2f - MaxRange / 2f;
                // Translate the point from the world origin to the local origin
                endPoint *= -1;
                endPoint = transformRotation * endPoint;
                RaycastHit hit;
                Vector3 direction = endPoint + transformPosition;
                float MaxDistance = Vector3.Magnitude(direction);
                MaxDistance = MaxDistance > MaxRange ? MaxRange : MaxDistance;
                if (MaxDistance > MinRange && Physics.Raycast(transformPosition, direction - transformPosition, out hit, MaxDistance))
                {
                    if (DebugInfo)
                        Debug.DrawLine(transformPosition, hit.point, Color.green);
                    if (hit.distance < distance && hit.distance > MinRange)
                    {
                        distance = hit.distance;
                        hitPoint = hit.point;
                    }
                }
                else
                {
                    if (DebugInfo)
                        Debug.DrawLine(transformPosition, direction, Color.red);
                }
            }
        }
        //Debug.DrawLine(transformPosition, hitPoint, Color.yellow);
        SensorValue = distance;
        foreach (AdditiveNoiseGenerator gen in AdditiveNoiseGenerators)
        {
            SensorValue += (float)gen.GetValue();
        }
        QuantiseSensorData();
        if (DebugInfo)
            Debug.Log("Quantised distance : " + SensorValue + "Non quantised distance : " + distance);
        Sensing = false;
	}
}
