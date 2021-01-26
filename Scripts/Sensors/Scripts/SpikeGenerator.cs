using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGenerator : AdditiveNoiseGenerator
{
    [SerializeField]
    [Range (0f, 1f)]
    [Tooltip ("Probability of a spike happening. Value between 0 and 1")]
    private double SpikeProbability = 0.01;

    [SerializeField]
    [Tooltip ("Intensity of the spike")]
    private double SpikeIntensity = 2;
    private GaussianGenerator Gaussian;

    public override void Start()
    {
        base.Start();
        Gaussian = new GaussianGenerator(SpikeIntensity, 1);
    }

    public override double GetValue()
    {
        return RandomEngine.NextDouble() <= SpikeProbability ? Gaussian.GetValue() : 0;
    }
}
