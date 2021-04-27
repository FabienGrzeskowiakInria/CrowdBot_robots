using UnityEngine;
using crowdbotsim;

namespace crowdbotsim.Urdf
{
    public class SimpleUrdfRobot : MonoBehaviour
    {
        public string FilePath;
        
        #region Configure Robot

        public void SetRigidbodiesIsKinematic(bool isKinematic)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = isKinematic;
        }

        public void SetRigidbodiesUseGravity(bool useGravity)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.useGravity = useGravity;
        }

        public void GenerateUniqueJointNames()
        {
            foreach (SimpleUrdfJoint urdfJoint in GetComponentsInChildren<SimpleUrdfJoint>())
                urdfJoint.GenerateUniqueJointName();
        }

        #endregion

        public SimpleUrdfJoint[] GetJoints()
        {
            SimpleUrdfJoint[] links = gameObject.transform.GetComponentsInChildren<SimpleUrdfJoint>();

            return links;
        }
    }
}
