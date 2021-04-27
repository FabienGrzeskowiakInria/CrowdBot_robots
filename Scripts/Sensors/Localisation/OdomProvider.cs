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
using RosSharp.RosBridgeClient;
using CrowdMP;

namespace crowdbotsim
{
    public class OdomProvider : MonoBehaviour
    {
        public enum mobile_base_type {differential_drive, other}

        public static float repeat_angle(float angle)
        {
            if(angle > 180) return angle - 360.0f;
            else return angle;
        }

        [SerializeField]
        [Tooltip ("Type of wheels.")]
        public mobile_base_type type = mobile_base_type.other;

        [SerializeField]
        [Tooltip ("List of all additive noise generator associated with this sensor.")]
        protected List<AdditiveNoiseGenerator> AdditiveNoiseGenerators;
        
        [Header("For differential drive")]

        [SerializeField]
        [Tooltip ("ticks per wheel turn.")]
        protected float steps = 2578.33f;

        [SerializeField]
        [Tooltip ("Wheels radius (m).")]
        protected float radius = 0.076f;

        [SerializeField]
        [Tooltip ("Spacing between wheels (m).")]
        protected float spacing = 0.3f;

        private Rigidbody base_link;



        [SerializeField]
        [Tooltip ("Right wheel joint")]
        protected JointStateReader r_wheel;

        [SerializeField]
        [Tooltip ("Left wheel joint")]
        protected JointStateReader l_wheel;

        [SerializeField]
        [Tooltip ("Set to true to see debug info in the console")]
        protected bool DebugInfo = false;

        public struct Pose_and_twist{
            public float x;
            public float y;
            public float theta;
            public float dxdt;
            public float dydt;
            public float dthetadt;
            
            public Pose_and_twist(float xx, float yy, float th, float dxx, float dyy, float dth)
            {
                x = xx;
                y = yy;
                theta = th;
                dxdt = dxx;
                dydt = dyy;
                dthetadt = dth;
            }

        }

        // estimated position and velocity of the robot (2D)
        protected Pose_and_twist p_t;

        private float last_time;

        // Use this for initialization
        public virtual void Start () {
            p_t = new Pose_and_twist(0,0,0,0,0,0);
            last_time = ToolsTime.AbsoluteTime;
            base_link = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            if (ToolsTime.DeltaTime > 0)
            {
                Corridor_constraint(24.6f,4.6f);
                switch (type)
                {
                    case mobile_base_type.differential_drive:
                        r_wheel.Read( out _, out float r_p, out float r_v, out float r_effort);
                        l_wheel.Read( out _, out float l_p, out float l_v, out float l_effort);

                        foreach( AdditiveNoiseGenerator n in AdditiveNoiseGenerators)
                        {
                            r_v += (float)(n.GetValue());
                            r_p += (float)(n.GetValue());
                            r_effort += (float)(n.GetValue());
                            l_v += (float)(n.GetValue());
                            l_p += (float)(n.GetValue());
                            l_effort += (float)(n.GetValue());
                        }
                        
                        // http://rossum.sourceforge.net/papers/DiffSteer/
                        // get timestamp

                        // // Quantesize the raw computation with the steps of the odometry sensor
                        // float delta_r_p = (Mathf.Round(r_p - last_r_pos)*(steps/360))/(steps/360);
                        // float delta_l_p = (Mathf.Round(l_p - last_l_pos)*(steps/360))/(steps/360);

                        // // inverse kinematic
                        // float delta_p = (radius*delta_l_p + radius*delta_r_p) / 2;
                        // float delta_curv = (spacing/2) * ((delta_r_p + delta_l_p) / (delta_r_p - delta_l_p));
                        
                        // float delta_o = delta_l_p/delta_curv; // R * dtheta = dX


                        float now = ToolsTime.AbsoluteTime;
                        float dt = now - last_time;

                        // inverse kinematic
                        p_t.dthetadt = radius * (r_v - l_v) / spacing;
                        p_t.theta = p_t.dthetadt  * dt + p_t.theta;

                        if(p_t.theta >= 2*Mathf.PI)
                        {
                            p_t.theta -= 2*Mathf.PI;
                        }
                        if(p_t.theta < 0)
                        {
                            p_t.theta += 2*Mathf.PI;
                        }
                        
                        float v = radius * (r_v + l_v) / 2;
                        p_t.dxdt = v * Mathf.Cos(p_t.theta);
                        p_t.dydt = v * Mathf.Sin(p_t.theta);

                        p_t.x =  p_t.dxdt * dt + p_t.x;
                        p_t.y =  p_t.dydt * dt + p_t.y;

                        last_time = ToolsTime.AbsoluteTime;

                        if(DebugInfo)
                            Debug.Log(p_t.x + " " + p_t.y + " " + p_t.theta*Mathf.Rad2Deg);
                            
                    break;

                    default:
                        Pose_and_twist p_t_now = new Pose_and_twist(
                            transform.position.x,
                            //careful: y and z
                            transform.position.z,
                            // Quaternion.Angle(Quaternion.identity, transform.localRotation),
                            // -transform.localEulerAngles.y,
                            -repeat_angle(transform.localEulerAngles.y+180.0f),
                            0,0,0);

                        p_t.dxdt = (p_t_now.x - p_t.x) / ToolsTime.DeltaTime;
                        p_t.dydt = (p_t_now.y - p_t.y) / ToolsTime.DeltaTime;
                        p_t.dthetadt = (p_t_now.theta - p_t.theta) / ToolsTime.DeltaTime;
                        p_t.x = p_t_now.x;
                        p_t.y = p_t_now.y;
                        p_t.theta = p_t_now.theta;
                    break;
                }
            }
        }

        public Pose_and_twist get_pose_and_twist()
        {
            return p_t;
        }

        public void Corridor_constraint(float x, float y)
        {
            if( p_t.x > x)
            {
                transform.Translate(new Vector3(x - p_t.x, 0 ,0), Space.World);                
            }
            if( p_t.x < -x)
            {
                transform.Translate(new Vector3(-x - p_t.x, 0 ,0), Space.World);                
            }
            if( p_t.y > y)
            {
                // translate on z axis
                transform.Translate(new Vector3(0, 0, y - p_t.y), Space.World);                
            }
            if( p_t.y < -y)
            {
                // translate on z axis
                transform.Translate(new Vector3(0, 0, -y - p_t.y), Space.World);                
            }
        }

    }


}