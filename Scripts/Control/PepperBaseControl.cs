using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperBaseControl : MonoBehaviour
{
    private WheelCollider[] wheels;
    public GameObject Wheels_reference;

    [Header("Control interface for pepper")]

    [Range(-2, 2)]
    public float x_speed;

    [Range(-2, 2)]
    public float y_speed;

    [Range(-2*Mathf.PI, 2*Mathf.PI)]
    public float theta_speed;

    // Controller

    private float[] last_speed, current_speed, sum, cmd, cmd_angle;
    public float[] PID = new float[3] { 0.1f, 0.01f, 0 };
    private static float RPMtoRadianSecond = (2 * Mathf.PI) / 60;



    // Start is called before the first frame update
    void Start()
    {
        wheels = Wheels_reference.GetComponentsInChildren<WheelCollider>();

        last_speed = new float[wheels.Length];
        cmd = new float[wheels.Length];
        cmd_angle = new float[wheels.Length];
        current_speed = new float[wheels.Length];
        sum = new float[wheels.Length];

        for (int i = 0; i < wheels.Length; i++)
        {
            cmd[i] = 0;
            cmd_angle[i] = 0;
            last_speed[i] = 0;
            current_speed[i] = 0;
            sum[i] = 0;
        }
    }

    void FixedUpdate()
    {
        three_wheels_Control();
    }

    void three_wheels_Control()
    {
        for(int i = 0; i < 3; i++)
        {
            Vector3 wheel_center = new Vector3(wheels[i].gameObject.transform.position.x, 0, wheels[i].gameObject.transform.position.z);
            Vector3 robot_center = wheel_center - Wheels_reference.transform.position;

            float radius = (wheel_center - robot_center).magnitude;

            Vector3 orthogonal_vector = new Vector3(-robot_center.z, 0, robot_center.x).normalized * theta_speed * (wheel_center - robot_center).magnitude / (2*Mathf.PI);

            Vector3 local = new Vector3(y_speed, 0, x_speed);
            Vector3 worldToLocal = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * local;

            Vector3 command = worldToLocal + orthogonal_vector;

            // #region DEBUG
            // {
            //     // line between robot center and wheel
            //     Debug.DrawLine(wheel_center, Wheels_reference.transform.position, Color.black);

            //     // x command line
            //     Debug.DrawLine(wheel_center, wheel_center + new Vector3(x_speed, 0, 0), Color.blue);

            //     // y command line
            //     Debug.DrawLine(wheel_center, wheel_center + new Vector3(0, 0, y_speed), Color.green);

            //     // theta command line
            //     Debug.DrawLine(wheel_center, wheel_center + orthogonal_vector, Color.yellow);

            //     // sum of command lines
            //     Debug.DrawLine(wheel_center, wheel_center + command, Color.red);
            // }
            // #endregion

            float angle = Vector3.SignedAngle(wheels[i].gameObject.transform.forward, command, wheels[i].gameObject.transform.up);
            float redressed_angle = (angle < 0) ? angle + 360 : angle;

            cmd_angle[i] = redressed_angle;
            cmd[i] = command.magnitude;
        }

        PIDcontrol();
    }
    void PIDcontrol()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            current_speed[i] = wheels[i].radius * wheels[i].rpm * RPMtoRadianSecond;
        }

        // for each wheel, PID control using last velocity
        for(int i = 0; i < cmd.Length; i++)
        {
            float diff = cmd[i] - current_speed[i];
            sum[i] += diff;
            cmd[i] = PID[0] * diff;
            if(PID[1] > 0) cmd[i] += PID[1] * sum[i];
            if(PID[2] > 0) cmd[i] += PID[2] * (current_speed[i] - last_speed[i]);        
        }

        // send cmd to wheels
        for(int i = 0; i < cmd.Length; i++)
        {
            if( float.IsNaN(cmd[i])) cmd[i] = 0;
            wheels[i].motorTorque = cmd[i] * Time.fixedDeltaTime;
            //apply angle directly
            wheels[i].steerAngle = cmd_angle[i];
        }    
        // update last speed
        current_speed.CopyTo(last_speed,0);
    }
}
