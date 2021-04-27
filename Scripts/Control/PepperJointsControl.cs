using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperJointsControl : MonoBehaviour
{

    [Header("Pepper joints controls")]
    public float Pelvis;
    public float Hip;
    public float torso;
    public float Neck;
    public float Head;
    public float LShoulder;
    public float LBiceps;
    public float LElbow;
    public float LForeArm;
    public float LWrist;
    public float RShoulder;
    public float RBiceps;
    public float RElbow;
    public float RForeArm;
    public float RWrist;

    [HideInInspector]
    public float[] joints;
    void Start()
    {
        joints = new float[15];
        set_joints();
    }

    void FixedUpdate()
    {
        set_joints();
    }

    void set_joints()
    {
        joints[0] = Pelvis;
        joints[1] = Hip;
        joints[2] = torso;
        joints[3] = Neck;
        joints[4] = Head;
        joints[5] = LShoulder;
        joints[6] = LBiceps;
        joints[7] = LElbow;
        joints[8] = LForeArm;
        joints[9] = LWrist;
        joints[10] = RShoulder;
        joints[11] = RBiceps;
        joints[12] = RElbow;
        joints[13] = RForeArm;
        joints[14] = RWrist;
    }

    public void limit_joint(float val, int i)
    {
        switch (i)
        {
            case 0:
                Pelvis = val;
                break;
            case 1:
                Hip = val;
                break;
            case 2:
                torso = val;
                break;
            case 3:
                Neck = val;
                break;
            case 4:
                Head = val;
                break;
            case 5:
                LShoulder = val;
                break;
            case 6:
                LBiceps = val;
                break;
            case 7:
                LElbow = val;
                break;
            case 8:
                LForeArm = val;
                break;
            case 9:
                LWrist = val;
                break;
            case 10:
                RShoulder = val;
                break;
            case 11:
                RBiceps = val;
                break;
            case 12:
                RElbow = val;
                break;
            case 13:
                RForeArm = val;
                break;
            case 14:
                RWrist = val;
                break;
            default:
                break;
        }
    }

}
