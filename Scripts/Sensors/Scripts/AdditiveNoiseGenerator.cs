using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdditiveNoiseGenerator : MonoBehaviour 
{
    protected System.Random RandomEngine;

    public virtual void Start()
    {
        RandomEngine = new System.Random();
    }

    public abstract double GetValue();
}
