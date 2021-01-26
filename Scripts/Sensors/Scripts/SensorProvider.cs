using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorProvider : MonoBehaviour
{
    public bool isActive = true;

    [HideInInspector]
    public bool Sensing;
    protected float QuantisationStep;

    [SerializeField]
    [Tooltip ("Number of bits used by the sensor to represent the data")]
    protected int Bits = 32;

    [SerializeField]
    [Tooltip ("Frequency in Hz at which the sensor operates")]
    protected float SensingFrequency = 6.25f;

    [SerializeField]
    [Tooltip ("Range under which objects are not detected (deadzone)")]
    protected float MinRange = 0.2f;

    [SerializeField]
    [Tooltip ("Maximum range of the sensor. Objects beyond that range are not detected")]
    protected float MaxRange = 4.5f;

    [SerializeField]
    [Tooltip ("List of all additive noise generator associated with this sensor.")]
    protected List<AdditiveNoiseGenerator> AdditiveNoiseGenerators;

    [SerializeField]
    [Tooltip ("Set to true to see debug info in the console and casted rays")]
    protected bool DebugInfo = false;

    // Use this for initialization
    public virtual void Start () {
        Sensing = false;
        // QuantisationStep = (MaxRange - MinRange) / Mathf.Pow(2, Bits);
        QuantisationStep = Mathf.Pow(2, Bits);
    }
	
	// public virtual void FixedUpdate () {
	// }

    public virtual void Update(){
        if(!isActive) return;
	    if (!Sensing)
        {
            StartCoroutine(Sense());
        }
    }

    public float GetFrequency()
    {
        return SensingFrequency;
    }

    public float GetMinRange()
    {
        return MinRange;
    }

    public float GetMaxRange()
    {
        return MaxRange;
    }

    public abstract IEnumerator Sense();
    public abstract void QuantiseSensorData();



    public static void GetBytesFromFloat(ref byte[] output, float value)
    {
        if (output.Length != 4)
        {
            return;
        }
        uint tmp = (uint)value;
        // unsafe
        // {
        //     tmp = *((uint*)&value);
        // }
        output[0] = (byte)(tmp & 0xFF);
        output[1] = (byte)((tmp >> 8) & 0xFF);
        output[2] = (byte)((tmp >> 16) & 0xFF);
        output[3] = (byte)((tmp >> 24) & 0xFF);
    }

} 
