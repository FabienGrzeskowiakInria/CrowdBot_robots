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
using System;


public class HingeChainControl : MonoBehaviour
{

    // Control related variables
    private HingeJoint[] joints;
    private float[] target;
    private float[] last_error;

    [Header("PI controller (common for all joints)")]
    public float P = 1.0f;
    public float I = 0.01f;
    private PepperJointsControl joints_list;
    void Start()
    {
        joints = GetComponentsInChildren<HingeJoint>();

        joints_list = GetComponent<PepperJointsControl>();

        foreach (HingeJoint hinge in joints)
        {
            var motor = hinge.motor;
            motor.force = 100;
            motor.freeSpin = false;

            hinge.motor = motor;
            hinge.useMotor = true;
        }

        target = new float[joints.Length];
        last_error = new float[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            target[i] = (joints_list == null) ? 0 : joints_list.joints[i];
            last_error[i] = 0;
        }

    }

    void FixedUpdate()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            if (joints_list != null) target[i] = joints_list.joints[i];

            if (target[i] > joints[i].limits.max)
            {
                target[i] = joints[i].limits.max;
                if (joints_list != null) joints_list.limit_joint(target[i], i);
            }
            if (target[i] < joints[i].limits.min)
            {
                target[i] = joints[i].limits.min;
                if (joints_list != null) joints_list.limit_joint(target[i], i);
            }

            var motor = joints[i].motor;
            motor.targetVelocity = P * (target[i] - joints[i].angle) + I * last_error[i];
            last_error[i] = (target[i] - joints[i].angle);
            joints[i].motor = motor;


        }
    }
}
