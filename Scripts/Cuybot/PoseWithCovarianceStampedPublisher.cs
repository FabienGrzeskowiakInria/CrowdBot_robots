using UnityEngine;
using RosSharp;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class PoseWithCovarianceStampedPublisher : Publisher<PoseWithCovarianceStamped>, Publisher
    {
        public Transform publishedTransform;
        public string FrameId = "Unity";

        private PoseWithCovarianceStamped message;

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
            message = new PoseWithCovarianceStamped();
            message.header.frame_id = FrameId;
        }

        public void UpdateMessage()
        {
            message.header.Update();
            message.pose.pose.position = GetGeometryPoint(publishedTransform.position.Unity2Ros());
            message.pose.pose.orientation = GetGeometryQuaternion(publishedTransform.rotation.Unity2Ros());

            Publish(message);
        }

        private Messages.Geometry.Point GetGeometryPoint(Vector3 position)
        {
            Messages.Geometry.Point geometryPoint = new Messages.Geometry.Point();
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
            return geometryPoint;
        }

        private Messages.Geometry.Quaternion GetGeometryQuaternion(Quaternion quaternion)
        {
            Messages.Geometry.Quaternion geometryQuaternion = new Messages.Geometry.Quaternion();
            geometryQuaternion.x = quaternion.x;
            geometryQuaternion.y = quaternion.y;
            geometryQuaternion.z = quaternion.z;
            geometryQuaternion.w = quaternion.w;
            return geometryQuaternion;
        }

    }
}
