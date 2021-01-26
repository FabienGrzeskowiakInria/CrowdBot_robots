using UnityEngine;
using RosSharp;
using RosSharp.RosBridgeClient;
using Messages = RosSharp.RosBridgeClient.Messages;

namespace Cuybot.ROS
{
    public class OdometryTFPublisher: Publisher<TFMessage>, Publisher
    {
        //public OdomProvider odom;
        public string FrameId = "odom";
        public Transform publishedTransform;
        private TFMessage message;

        protected override void Start()
        {
            Debug.Log("Odometry start");
            //base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
            // UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new TFMessage();
            message.transforms = new TransformStamped[] { new TransformStamped() };
            message.transforms[0].header.frame_id = "odom";
            message.transforms[0].child_frame_id = "base_link";
            /*            message.header = new Messages.Standard.Header { frame_id = FrameId };
                        message.header.stamp = new RosBridgeClient.Messages.Standard.Time();

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
            */
        }
        public void UpdateMessage()
        {
            //            if (ToolsTime.TimeSinceTrialStarted < last_timestamp + scanPeriod) return;

            message.transforms[0].header.Update();
            message.transforms[0].transform.translation = GetGeometryVector3(publishedTransform.position.Unity2Ros());
            message.transforms[0].transform.rotation = GetRotation(publishedTransform.rotation.Unity2Ros());

            /*            message.pose.pose.position.x = transform.position.x; // odom.get_pose_and_twist().x;
            message.pose.pose.position.y = transform.position.y; // odom.get_pose_and_twist().y;
            //message.pose.pose.orientation = GetRotation(Quaternion.AngleAxis(odom.get_pose_and_twist().theta*Mathf.Rad2Deg, Vector3.forward));
            // message.pose.pose.orientation = GetRotation(Quaternion.identity);
*/
            //message.twist.twist.linear = GetGeometryVector3(new Vector3(odom.get_pose_and_twist().dxdt, odom.get_pose_and_twist().dydt, 0 ));
            //message.twist.twist.angular = GetGeometryVector3(new Vector3(0,0, odom.get_pose_and_twist().dthetadt * Mathf.Rad2Deg    ));

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
