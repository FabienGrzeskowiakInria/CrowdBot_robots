using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianGenerator : AdditiveNoiseGenerator
{
    [SerializeField]
    [Tooltip ("Mean of the Gaussian distribution")]
    private double Mean = 0;

    [SerializeField]
    [Tooltip ("Standard deviation of the Gaussian distribution")]
    private double StandardDeviation = 1;

    public GaussianGenerator(double mean, double standardDeviation) : base()
    {
        Mean = mean;
        StandardDeviation = standardDeviation;
    }

    public override double GetValue()
    {
        double Value1 = RandomEngine.NextDouble();
        double Value2 = RandomEngine.NextDouble();
        return Mean + StandardDeviation * (Math.Sqrt(-2.0 * Math.Log(Value1)) * Math.Sin(2.0 * Math.PI * Value2));
    }
}
