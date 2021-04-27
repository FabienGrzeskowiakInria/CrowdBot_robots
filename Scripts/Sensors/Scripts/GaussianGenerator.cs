// # MIT License

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
