using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateJoints : MonoBehaviour
{
    private Transform Base;
    private Transform Knee;
    private Transform Hip;
    private Transform Head;
    private Transform LShoulder;
    private Transform LElbow;
    private Transform LWrist;
    private Transform RShoulder;
    private Transform RElbow;
    private Transform RWrist;

    [Range(-1.0f, 1.0f)] public float Xpos;
    [Range(-1.0f, 1.0f)] public float Zpos;
    [Range(-180.0f, 180.0f)] public float Orientation;
    [Range(-29.5f, 29.5f)] public float KneePitch;
    [Range(-59.5f, 59.5f)] public float HipPitch;
    [Range(-29.5f, 29.5f)] public float HipRoll;
    [Range(-40.5f, 36.5f)] public float HeadPitch;
    [Range(-119.5f, 119.5f)] public float HeadYaw;
    [Range(-119.5f, 119.5f)] public float LShoulderPitch = 90.0f;
    [Range(0.5f, 89.5f)] public float LShoulderRoll;
    [Range(-119.5f, 119.5f)] public float LElbowYaw;
    [Range(-89.5f, -0.5f)] public float LElbowRoll;
    [Range(-104.5f, 104.5f)] public float LWristYaw;
    [Range(-119.5f, 119.5f)] public float RShoulderPitch = 90.0f;
    [Range(-89.5f, -0.5f)] public float RShoulderRoll;
    [Range(-119.5f, 119.5f)] public float RElbowYaw;
    [Range(0.5f, 89.5f)] public float RElbowRoll;
    [Range(-104.5f, 104.5f)] public float RWristYaw;

    private Vector3 offsetLShoulder;
    private Vector3 offsetLElbow;
    private Vector3 offsetLWrist;
    private Vector3 offsetRShoulder;
    private Vector3 offsetRElbow;
    private Vector3 offsetRWrist;

    [HideInInspector] public bool randomize = false;
    private bool lerp = false;
    private float delay = 0.0f;

    float RandXpos;
    float RandZpos;
    float RandOrientation;
    float RandKneePitch;
    float RandHipPitch;
    float RandHipRoll;
    float RandHeadPitch;
    float RandHeadYaw;
    float RandLShoulderPitch;
    float RandLShoulderRoll;
    float RandLElbowYaw;
    float RandLElbowRoll;
    float RandLWristYaw;
    float RandRShoulderPitch;
    float RandRShoulderRoll;
    float RandRElbowYaw;
    float RandRElbowRoll;
    float RandRWristYaw;

    [HideInInspector]
    public float rootXpos;
    [HideInInspector]
    public float rootZpos;
    [HideInInspector]
    public float rootOrientation;

    // Use this for initialization
    void Start()
    {
        Base = transform.Find("base_link");
        Knee = Base.Find("Tibia").Find("Pelvis");
        Hip = Knee.Find("Hip");
        Head = Hip.Find("Torso").Find("Neck");
        LShoulder = Hip.Find("Torso").Find("LShoulder");
        LElbow = LShoulder.Find("LBicep").Find("LElbow");
        LWrist = LElbow.Find("LForeArm").Find("l_wrist");
        RShoulder = Hip.Find("Torso").Find("RShoulder");
        RElbow = RShoulder.Find("RBicep").Find("RElbow");
        RWrist = RElbow.Find("RForeArm").Find("r_wrist");

        offsetLShoulder = LShoulder.localEulerAngles + new Vector3(-90.0f,0.0f,0.0f);
        offsetLElbow = LElbow.localEulerAngles + new Vector3(0.0f, 90.0f, 0.0f);
        offsetLWrist = LWrist.localEulerAngles;
        offsetRShoulder = RShoulder.localEulerAngles + new Vector3(90.0f, 0.0f, 0.0f);
        offsetRElbow = RElbow.localEulerAngles + new Vector3(0.0f, -90.0f, 0.0f);
        offsetRWrist = RWrist.localEulerAngles;

        RShoulderPitch = 90.0f;
        LShoulderPitch = 90.0f;

        rootXpos = Xpos;
        rootZpos = Zpos;
        rootOrientation = Orientation;
    }

    // Update is called once per frame
    void Update()
    {
        Base.localPosition = new Vector3(-Xpos, 0.0f, Zpos);
        Base.localEulerAngles = new Vector3(0.0f, Orientation, 0.0f);
        Knee.localEulerAngles = new Vector3(KneePitch, 0.0f, 0.0f);
        Hip.localEulerAngles = new Vector3(-HipRoll, 0.0f, HipPitch);
        Head.localEulerAngles = new Vector3(0.0f, HeadYaw, -HeadPitch);
        LShoulder.localEulerAngles = new Vector3(LShoulderPitch, 0.0f, -LShoulderRoll) + offsetLShoulder;
        LElbow.localEulerAngles = new Vector3(LElbowRoll, LElbowYaw, 0.0f) + offsetLElbow;
        LWrist.localEulerAngles = new Vector3(0.0f, LWristYaw, 0.0f) + offsetLWrist;
        RShoulder.localEulerAngles = new Vector3(-RShoulderPitch, 0.0f, RShoulderRoll) + offsetRShoulder;
        RElbow.localEulerAngles = new Vector3(RElbowRoll, RElbowYaw, 0.0f) + offsetRElbow;
        RWrist.localEulerAngles = new Vector3(0.0f, RWristYaw, 0.0f) + offsetRWrist;
        
        Randomize();
        Lerp();
    }

    public void SetRandomize() { randomize = true; }

    void Randomize()
    {
        if (!randomize) return;

        RandXpos = Random.Range(-1.0f, 1.0f);
        RandZpos = Random.Range(-1.0f, 1.0f);
        RandOrientation = Random.Range(-180.0f, 180.0f);
        RandKneePitch = Random.Range(-29.5f, 29.5f);
        RandHipPitch = Random.Range(-59.5f, 59.5f);
        RandHipRoll = Random.Range(-29.5f, 29.5f);
        RandHeadPitch = Random.Range(-40.5f, 36.5f);
        RandHeadYaw = Random.Range(-119.5f, 119.5f);
        RandLShoulderPitch = Random.Range(-119.5f, 119.5f);
        RandLShoulderRoll = Random.Range(0.5f, 89.5f);
        RandLElbowYaw = Random.Range(-119.5f, 119.5f);
        RandLElbowRoll = Random.Range(-89.5f, -0.5f);
        RandLWristYaw = Random.Range(-104.5f, 104.5f);
        RandRShoulderPitch = Random.Range(-119.5f, 119.5f);
        RandRShoulderRoll = Random.Range(-89.5f, -0.5f);
        RandRElbowYaw = Random.Range(-119.5f, 119.5f);
        RandRElbowRoll = Random.Range(0.5f, 89.5f);
        RandRWristYaw = Random.Range(-104.5f, 104.5f);

        randomize = false;

        lerp = true;
    }

    void Lerp()
    {
        if (!lerp) return;
        lerp = false;
        float speed = 0.05f;
        float epsilon = 0.1f;

        Xpos = Mathf.Lerp(Xpos, RandXpos, speed);
        Zpos = Mathf.Lerp(Zpos, RandZpos, speed);
        Orientation = Mathf.Lerp( Orientation, RandOrientation, speed);
        KneePitch = Mathf.Lerp( KneePitch, RandKneePitch, speed);
        HipPitch = Mathf.Lerp( HipPitch, RandHipPitch, speed);
        HipRoll = Mathf.Lerp( HipRoll, RandHipRoll, speed);
        HeadPitch = Mathf.Lerp( HeadPitch, RandHeadPitch, speed);
        HeadYaw = Mathf.Lerp( HeadYaw, RandHeadYaw, speed);
        LShoulderPitch = Mathf.Lerp( LShoulderPitch, RandLShoulderPitch, speed);
        LShoulderRoll = Mathf.Lerp(LShoulderRoll ,RandLShoulderRoll , speed);
        LElbowRoll = Mathf.Lerp(LElbowRoll ,RandLElbowRoll , speed);
        LElbowYaw = Mathf.Lerp(LElbowYaw ,RandLElbowYaw , speed);
        LWristYaw = Mathf.Lerp(LWristYaw , RandLWristYaw, speed);
        RShoulderPitch = Mathf.Lerp(RShoulderPitch , RandRShoulderPitch, speed);
        RShoulderRoll = Mathf.Lerp( RShoulderRoll, RandRShoulderRoll, speed);
        RElbowRoll = Mathf.Lerp( RElbowRoll, RandRElbowRoll, speed);
        RElbowYaw = Mathf.Lerp( RElbowYaw, RandRElbowYaw, speed);
        RWristYaw = Mathf.Lerp( RWristYaw, RandRWristYaw, speed);

        if (Mathf.Abs(Xpos - RandXpos) > epsilon) lerp = true;
        if (Mathf.Abs(Zpos - RandZpos) > epsilon) lerp = true;
        if (Mathf.Abs(Orientation - RandOrientation) > epsilon) lerp = true;
        if (Mathf.Abs(KneePitch - RandKneePitch) > epsilon) lerp = true;
        if (Mathf.Abs(HipPitch - RandHipPitch) > epsilon) lerp = true;
    }
}
