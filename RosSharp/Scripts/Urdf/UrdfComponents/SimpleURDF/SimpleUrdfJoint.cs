using UnityEngine;
using RosSharp.Urdf;
using RosSharp;
using System;

namespace crowdbotsim.Urdf
{

    public abstract class SimpleUrdfJoint : MonoBehaviour
    {
        public enum JointTypes
        {
            Fixed,
            Continuous,
            Revolute
        }

        public GameObject parent;
        public abstract Vector3 Axis { get; }
        public string JointName;
        public abstract JointTypes JointType { get; }
        public bool IsRevoluteOrContinuous => JointType == JointTypes.Revolute || JointType == JointTypes.Revolute;
        public Vector3 GetLocalPos()
        {
            var pt = transform.position - parent.transform.position;
            var out_var = Quaternion.Inverse(parent.transform.rotation) * pt;
            return out_var;
        }

        public Quaternion GetLocalRot()
        {
            var q = transform.rotation;
            var q_parent = parent.transform.rotation;
            return Quaternion.Inverse(q_parent) * q;
        }


        public static void Create(GameObject linkObject, JointTypes jointType, SimpleJoint joint = null)
        {
            SimpleUrdfJoint urdfJoint = AddCorrectJointType(linkObject, jointType);
            if (joint != null)
            {
                urdfJoint.JointName = joint._joint_name;
                urdfJoint.ImportSimpleJointData(joint);
            }

        }

        private static SimpleUrdfJoint AddCorrectJointType(GameObject linkObject, JointTypes jointType)
        {
            SimpleUrdfJoint urdfJoint = null;

            switch (jointType)
            {
                case JointTypes.Fixed:
                    urdfJoint = SimpleUrdfJointFixed.Create(linkObject);
                    break;
                case JointTypes.Continuous:
                    urdfJoint = SimpleUrdfJointContinuous.Create(linkObject);
                    break;
                case JointTypes.Revolute:
                    urdfJoint = SimpleUrdfJointRevolute.Create(linkObject);
                    break;
            }

            return urdfJoint;
        }

        #region Import
        protected virtual void ImportSimpleJointData(SimpleJoint joint) { }

        #endregion

        #region Export

        public static SimpleJoint ExportDefaultJoint(Transform transform)
        {
            return new SimpleJoint(
                transform.parent.name + "_" + transform.name + "_joint",
                JointTypes.Fixed.ToString().ToLower(),
                transform.parent.name,
                transform.name,
                UrdfOrigin.ExportOriginData(transform),
                GetAxisData(GetDefaultAxis())
                );
        }

        public SimpleJoint ExportJointData()
        {
            if (parent == null) Debug.Log("undefined parent of " + transform.name);

            GameObject unity_parent = transform.parent.gameObject;

            bool unity_parent_is_urdf_parent = String.Equals(parent.name, unity_parent.name);

            if (!unity_parent_is_urdf_parent)
            {
                transform.parent = parent.transform;
                // tf.Translate(-parent.transform.localPosition);
            }

            SimpleJoint joint = new SimpleJoint(
                JointName,
                JointType.ToString().ToLower(),
                parent.transform.name,
                transform.name,
                UrdfOrigin.ExportOriginData(transform),
                GetAxisData(Axis)
            );


            if (!unity_parent_is_urdf_parent)
            {
                transform.parent = unity_parent.transform;
                // tf.Translate(parent.transform.localPosition);
            }

            return joint;
        }
        #region ExportHelpers

        public void GenerateUniqueJointName()
        {
            JointName = transform.parent.name + "_" + transform.name + "_joint";
        }

        protected static SimpleJoint.Axis GetAxisData(Vector3 axis)
        {
            double[] rosAxis = axis.Unity2Ros().ToRoundedDoubleArray();
            return new SimpleJoint.Axis(rosAxis);
        }

        protected static Vector3 GetDefaultAxis()
        {
            return new Vector3(-1, 0, 0);
        }

        #endregion

        #endregion
    }
}

