﻿// # MIT License

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
using RosSharp.RosBridgeClient;

public class DiffDriveController : MonoBehaviour
{
    public MotorWriter rightMotorWriter;
    public MotorWriter leftMotorWriter;

    public float P_lin, I_lin, D_lin, P_ang, I_ang, D_ang, spacing, radius;
    private Rigidbody rb;

    private float sum_eps_lin, sum_eps_ang, last_eps_lin, last_eps_ang;

    // Start is called before the first frame update
    void Start()
    {
        sum_eps_lin = 0;
        sum_eps_ang = 0;
        last_eps_lin = 0;
        last_eps_ang = 0;
        rb = GetComponent<Rigidbody>();
    }

    public void ControlMotorsClosedLoop(float delta_time, float LinearVelocity, float AngularVelocity)
    {
        if(delta_time == 0) return;

        float eps_lin, eps_ang, V_right, V_left, Cmd_lin, Cmd_ang;

        // Linear closed loop
        eps_lin = LinearVelocity - transform.InverseTransformDirection(rb.velocity).z;

        if(float.IsNaN(eps_lin)) 
        {
            eps_lin = 0;
        }

        float Ki = 0;
        if (I_lin != 0) Ki = delta_time/I_lin;
       
        if(Ki == 0) sum_eps_lin = 0;
        else sum_eps_lin += eps_lin;
        if(float.IsNaN(sum_eps_lin)) sum_eps_lin = 0;
        

        Cmd_lin = P_lin *( eps_lin + Ki * sum_eps_lin + (D_lin/delta_time) * (eps_lin - last_eps_lin));        
        if(float.IsNaN(Cmd_lin)) return;
       
        {
        // safety net
            if(Mathf.Abs(Cmd_lin) > Mathf.Min(leftMotorWriter.MaxTorque, rightMotorWriter.MaxTorque))
            {
                sum_eps_lin -= eps_lin;
            }
            if(Ki * Mathf.Abs(sum_eps_lin) > 10.0f){
                sum_eps_lin = 0;
                Debug.LogWarning("Sum_lin overflow");
            } 
        }

        last_eps_lin = eps_lin;

        // Angular closed loop
        eps_ang = AngularVelocity - (rb.transform.rotation * rb.angularVelocity).y;
        if(float.IsNaN(eps_ang)) eps_ang = 0;
    
        Ki = 0;
        if (I_ang != 0) Ki = delta_time/I_ang;
        
        if(Ki == 0) sum_eps_ang = 0;
        else sum_eps_ang += eps_ang;

        if(float.IsNaN(sum_eps_ang)) sum_eps_ang = 0;
       
        Cmd_ang = P_ang *( eps_ang + Ki * sum_eps_ang + (D_ang/delta_time) * (eps_ang - last_eps_ang));        
        if(float.IsNaN(Cmd_ang)) return;

        {
        // safety net
            if(Mathf.Abs(Cmd_ang) > Mathf.Min(leftMotorWriter.MaxTorque, rightMotorWriter.MaxTorque))
            {
                sum_eps_ang -= eps_ang;
            }
            if(Ki * Mathf.Abs(sum_eps_ang) > 10.0f){
                sum_eps_ang = 0;
                Debug.LogWarning("Sum_ang overflow");
            } 
        }

        last_eps_ang = eps_ang;

        // Inverse kinematic
        
        V_right = (Cmd_lin - Cmd_ang/(2*spacing))/radius;
        V_left = (Cmd_lin + Cmd_ang/(2*spacing))/radius;

        Accelerate(V_left,V_right);

        // Debug.Log("Cmd_lin " + Cmd_lin + " eps_lin " + eps_lin + " sum_lin " + sum_eps_lin);
    
    }


    public void Accelerate(float left, float right)
	{
        rightMotorWriter.Write(right);
        leftMotorWriter.Write(left);   
	}

}
