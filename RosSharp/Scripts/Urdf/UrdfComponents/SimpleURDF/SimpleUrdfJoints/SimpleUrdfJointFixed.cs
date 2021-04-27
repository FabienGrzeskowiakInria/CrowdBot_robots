using UnityEngine;
using RosSharp.Urdf;
namespace crowdbotsim.Urdf
{
    public class SimpleUrdfJointFixed : SimpleUrdfJoint
    {
        public override JointTypes JointType => JointTypes.Fixed;
        public override Vector3 Axis => SimpleUrdfJoint.GetDefaultAxis();

        public static SimpleUrdfJoint Create(GameObject go)
        {
            SimpleUrdfJointFixed urdfJoint = go.AddComponent<SimpleUrdfJointFixed>();
            return urdfJoint;
        }
        protected override void ImportSimpleJointData(SimpleJoint joint) { }

    }
}