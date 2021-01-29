using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PosePublisher : Publisher<Messages.Geometry.Twist>, Publisher
    {
        public Transform PublishedTransform;

        private Messages.Geometry.Twist message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            // UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Geometry.Twist();
            message.linear = new Messages.Geometry.Vector3();
            message.angular = new Messages.Geometry.Vector3();
        }
        public void UpdateMessage()
        {
            message.linear = GetGeometryVector3(PublishedTransform.position.Unity2Ros()); ;
            message.angular = GetGeometryVector3(- PublishedTransform.rotation.eulerAngles.Unity2Ros());

            Publish(message);
        }

        private static Messages.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            Messages.Geometry.Vector3 geometryVector3 = new Messages.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }
    }
}
