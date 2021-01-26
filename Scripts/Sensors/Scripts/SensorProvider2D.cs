using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorProvider2D : SensorProvider
{
    protected float[] SensorValues;
	// Use this for initialization
	public override void Start () {
        base.Start();
        SensorValues = new float[0];
	}

    public override void QuantiseSensorData()
    {
        // for (int i = 0; i < SensorValues.Length; ++i)
        // {
        //     // SensorValues[i] = Mathf.FloorToInt(SensorValues[i] / QuantisationStep) * QuantisationStep;
        // }
    }
    
    public virtual float[] GetData()
    {
        return SensorValues;
    }
}
