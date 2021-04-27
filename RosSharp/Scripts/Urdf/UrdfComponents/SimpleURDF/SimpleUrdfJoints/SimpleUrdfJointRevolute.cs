using System;
using UnityEngine;
using RosSharp.Urdf;

namespace crowdbotsim.Urdf
{
    public class SimpleUrdfJointRevolute : SimpleUrdfJoint
    {
        public override JointTypes JointType => JointTypes.Revolute;
        public override Vector3 Axis => GetAxis();
        public static SimpleUrdfJoint Create(GameObject go)
        {
            SimpleUrdfJointRevolute urdfJoint = go.AddComponent<SimpleUrdfJointRevolute>();
            return urdfJoint;
        }

        protected override void ImportSimpleJointData(SimpleJoint joint) { }

        public Vector3 GetAxis()
        {
            Vector3 a = new Vector3();
            if (gameObject.GetComponent<HingeJoint>().axis == null)
            {
                a = SimpleUrdfJoint.GetDefaultAxis();
            }
            else
            {
                a = gameObject.GetComponent<HingeJoint>().axis;
            }
            return a;
        }


    }
}

