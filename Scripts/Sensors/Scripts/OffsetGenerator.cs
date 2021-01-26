using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetGenerator : AdditiveNoiseGenerator
{
    [SerializeField]
    [Tooltip ("Offset to return")]
    private double Offset = 0;

    public override double GetValue()
    {
        return Offset;
    }
}
