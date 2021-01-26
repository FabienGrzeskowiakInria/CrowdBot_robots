using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class SensorProvider1D : SensorProvider
{
    protected float SensorValue;

	// Use this for initialization
	public override void Start () {
        base.Start();
        SensorValue = 0.0f;
	}

    public virtual float GetData()
    {
        return SensorValue;
    }

    public override void QuantiseSensorData()
    {
        // SensorValue = Mathf.FloorToInt(SensorValue / QuantisationStep) * QuantisationStep;
    }
}
