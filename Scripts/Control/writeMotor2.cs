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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class writeMotor2 : MonoBehaviour
{
    // Start is called before the first frame update

    public WheelCollider[] wheelColliders;

    // public float MaxTorque;

    [Range(-1,1)]
    public float drive;

    [Range(-1,1)]
    public float steer;

    // Controller

    private float[] last_speed, current_speed, sum, cmd;
    public float[] PID = new float[3] {0.1f, 0.01f, 0};
    private static float RPMtoRadianSecond = (2*Mathf.PI)/60;
    
    void Start()
    {
        last_speed = new float[wheelColliders.Length];
        cmd = new float[wheelColliders.Length];
        current_speed = new float[wheelColliders.Length];
        sum = new float[wheelColliders.Length];
        for(int i = 0; i < wheelColliders.Length; i++)
        {
            cmd[i] = 0;
            last_speed[i] = 0;
            current_speed[i] = 0;
            sum[i] = 0;
        }
    }

    void LateUpdate()
    {
        // TODO update visuals of wheels
        // Debug.Log(current_speed[0] + " " + current_speed[1]);
    }

    void FixedUpdate()
    {
        DifferentialControl();
    }

    void DifferentialControl()
    {
        // for 2 wheels get velocity input and current speed
        cmd[0] = drive - steer;
        cmd[1] = drive + steer;
        float alpha = 1.0f;//1.5f*Time.fixedDeltaTime
        for(int i = 0; i < wheelColliders.Length; i++)
        {
            current_speed[i] = alpha * (wheelColliders[i].rpm * RPMtoRadianSecond * wheelColliders[i].radius) + (1-alpha)*(last_speed[i]);
        }

        // for both wheel, PID control using last velocity
        for(int i = 0; i < cmd.Length; i++)
        {
            float diff = cmd[i] - current_speed[i];
            sum[i] += diff;
            cmd[i] = PID[0] * diff;
            if(PID[1] > 0) cmd[i] += PID[1] * sum[i];
            if(PID[2] > 0) cmd[i] += PID[2] * (current_speed[i] - last_speed[i]);        
            // Debug.Log("diff = " + diff);
        }
        
        // send cmd to wheels
        for(int i = 0; i < cmd.Length; i++)
        {
            if( float.IsNaN(cmd[i]) || float.IsInfinity(cmd[i])) cmd[i] = 0;
            wheelColliders[i].motorTorque = cmd[i] * Time.fixedDeltaTime;
            // Debug.Log(wheelColliders[i].motorTorque);
        }    
        // update last speed
        current_speed.CopyTo(last_speed,0);
    }
}
