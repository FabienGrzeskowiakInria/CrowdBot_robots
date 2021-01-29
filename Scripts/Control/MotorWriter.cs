using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(HingeJoint))]
    public class MotorWriter : MonoBehaviour
    {
        private WheelCollider wheel;
        private HingeJoint _hingeJoint;

        public float MaxTorque = 150;

        private void Start()
        {
            _hingeJoint = GetComponent<HingeJoint>();
            _hingeJoint.useMotor = false;

            string st = "base_link/wheel_colliders/"+transform.name;
            wheel = transform.root.Find(st).GetComponent<WheelCollider>();
        }
	    private void UpdateWheelPose()
	    {
		    Vector3 _pos = transform.position;
		    Quaternion _quat = transform.rotation;

		    wheel.GetWorldPose(out _pos, out _quat);

		    transform.position = _pos;
		    transform.rotation = _quat;
	    }

        void LateUpdate()
        {
            UpdateWheelPose();
        }
        
        public void Write(float value)
        {
            wheel.motorTorque = Mathf.Max( Mathf.Min(value, MaxTorque), -MaxTorque);
        }
    }

}
