using UnityEngine;
using RosSharp.Urdf;

namespace crowdbotsim.Urdf
{
    public class SimpleUrdfJointContinuous : SimpleUrdfJoint
    {
        public override JointTypes JointType => JointTypes.Continuous;

        public override Vector3 Axis => GetAxis();


        public static SimpleUrdfJoint Create(GameObject go)
        {
            SimpleUrdfJointContinuous urdfJoint = go.AddComponent<SimpleUrdfJointContinuous>();
            return urdfJoint;
        }
        protected override void ImportSimpleJointData(SimpleJoint joint) { }

        public Vector3 GetAxis()
        {
            Vector3 a = new Vector3();
            if (TryGetComponent(out HingeJoint hinge))
            {
                a = hinge.axis;
            }
            else
            {
                a = SimpleUrdfJoint.GetDefaultAxis();
            }
            return a;
        }

    }
}

