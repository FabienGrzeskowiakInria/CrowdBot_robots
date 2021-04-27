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

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(HingeJoint))]
    public class MotorWriter : MonoBehaviour
    {
        private WheelCollider wheel;
        private HingeJoint _hingeJoint;

        public float MaxTorque = 150;

        [Range(-1.0f, 1.0f)]
        public float torque;

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

        void Update()
        {
            Write(torque);
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
