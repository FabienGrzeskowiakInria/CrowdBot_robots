using UnityEngine;
using crowdbotsim;
using RosSharp;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class OdometryPublisher: Publisher<Messages.Navigation.Odometry>, Publisher
    {
        //public OdomProvider odom;
        public string FrameId = "odom";
        public Transform publishedTransform;
        private Messages.Navigation.Odometry message;
        private Vector3 lastPose;
        private float lastAngle;
        private bool firstUpdate = true;

        protected override void Start()
        {
            //base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            // UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Navigation.Odometry();
            message.header = new Messages.Standard.Header { frame_id = FrameId };
            message.header.stamp = new Messages.Standard.Time();

            message.twist = new Messages.Geometry.TwistWithCovariance();
            message.twist.covariance = new float[36];
            message.twist.twist = new Messages.Geometry.Twist();
            message.twist.twist.linear = new Messages.Geometry.Vector3();
            message.twist.twist.angular = new Messages.Geometry.Vector3();
            
            message.pose = new Messages.Geometry.PoseWithCovariance();
            message.pose.covariance = new float[36];
            message.pose.pose = new Messages.Geometry.Pose();
            message.pose.pose.position = new Messages.Geometry.Point();
            message.pose.pose.orientation = new Messages.Geometry.Quaternion();
        }
        public void UpdateMessage()
        {
//            if (ToolsTime.TimeSinceTrialStarted < last_timestamp + scanPeriod) return;

            if (firstUpdate)
            {
                firstUpdate = false;
                lastPose = publishedTransform.position;
                lastAngle = publishedTransform.rotation.eulerAngles.y;
                return;
            }

            if (ToolsTime.DeltaTime == 0)
                return;

            message.header.Update();
            message.pose.pose.position = GetGeometryPoint(publishedTransform.position.Unity2Ros());
            message.pose.pose.orientation = GetRotation(publishedTransform.rotation.Unity2Ros());
            //message.pose.pose.orientation = GetRotation(Quaternion.AngleAxis(odom.get_pose_and_twist().theta*Mathf.Rad2Deg, Vector3.forward));
            // message.pose.pose.orientation = GetRotation(Quaternion.identity);

            float angle = publishedTransform.rotation.eulerAngles.y;

            Vector3 pd = (publishedTransform.position - lastPose) / ToolsTime.DeltaTime;
            Vector3 posdiff = Quaternion.AngleAxis(-publishedTransform.rotation.eulerAngles.y, Vector3.up) * pd;

            message.twist.twist.linear = GetGeometryVector3(posdiff.Unity2Ros());
            //            message.twist.twist.linear = GetGeometryVector3(new Vector3(1,2,3));
            message.twist.twist.angular = GetGeometryVector3(new Vector3(0, 0, (-angle + lastAngle) * Mathf.Deg2Rad / ToolsTime.DeltaTime));

            Publish(message);
            lastPose = publishedTransform.position;
            lastAngle = publishedTransform.rotation.eulerAngles.y;
        }

        private Messages.Geometry.Point GetGeometryPoint(Vector3 position)
        {
            Messages.Geometry.Point geometryPoint = new Messages.Geometry.Point();
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
            return geometryPoint;
        }

        private static Messages.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            Messages.Geometry.Vector3 geometryVector3 = new Messages.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }

        private Messages.Geometry.Quaternion GetRotation(Quaternion q)
        {
            Messages.Geometry.Quaternion my_q = new Messages.Geometry.Quaternion();
            my_q.x = q.x;
            my_q.y = q.y;
            my_q.z = q.z;
            my_q.w = q.w;
            return my_q;
        }
    }
}
