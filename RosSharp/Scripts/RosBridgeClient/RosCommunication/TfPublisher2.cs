using UnityEngine;
using crowdbotsim.Urdf;

namespace RosSharp.RosBridgeClient
{
    public class TfPublisher2 : Publisher<Messages.TF2.TFMessage>, Publisher
    {
        public SimpleUrdfRobot robot;
        public string FrameId = "Map";

        [System.NonSerialized]
        private Messages.TF2.TFMessage message;

        [System.NonSerialized]
        private bool initialized = false;

        private SimpleUrdfJoint[] links;
        protected override void Start()
        {
              base.Start();
              InitializeMessage();
        }

        private void FixedUpdate()
        {
            //   UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.TF2.TFMessage();
            links = robot.GetJoints();

            message.transforms = new Messages.Geometry.TransformStamped[links.Length];
            for(int i = 0; i < links.Length ; i++){
                message.transforms[i] = new Messages.Geometry.TransformStamped();
                message.transforms[i].header = new Messages.Standard.Header();
                message.transforms[i].header.stamp = new RosBridgeClient.Messages.Standard.Time();
                message.transforms[i].header.frame_id = links[i].parent.name;
                message.transforms[i].child_frame_id = links[i].transform.name;
                message.transforms[i].transform = new Messages.Geometry.Transform();
                message.transforms[i].transform.translation = new Messages.Geometry.Vector3();
                message.transforms[i].transform.rotation = new Messages.Geometry.Quaternion();
            }
            initialized = true;
        }
        public void UpdateMessage()
        {
            if(links.Length != message.transforms.Length) InitializeMessage();

            for(int i = 0; i < links.Length; i++){

                message.transforms[i].header.Update();
                message.transforms[i].transform.translation = ToRosVector3(links[i].GetLocalPos().Unity2Ros());
                message.transforms[i].transform.rotation = ToRosQuaternion(links[i].GetLocalRot().Unity2Ros());
            }
            Publish(message);
        }       
        private static Messages.Geometry.Vector3 ToRosVector3(Vector3 v){
            Messages.Geometry.Vector3 v3Ros = new Messages.Geometry.Vector3();
            v3Ros.x = v.x;
            v3Ros.y = v.y;
            v3Ros.z = v.z;
            return v3Ros;
        }

        private static Messages.Geometry.Quaternion ToRosQuaternion(Quaternion q){
            Messages.Geometry.Quaternion qRos = new Messages.Geometry.Quaternion();
            qRos.w = q.w;
            qRos.x = q.x;
            qRos.y = q.y;
            qRos.z = q.z;
            return qRos;
        }
    }
}