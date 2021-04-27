using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using RosSharp.Urdf;

namespace crowdbotsim.Urdf.Editor
{
    public static class SimpleUrdfRobotExtensions
    {
        public static void Create()
        {
            GameObject robotGameObject = new GameObject("SimpleRobot");
            robotGameObject.AddComponent<SimpleUrdfRobot>();

            SimpleUrdfLink urdfLink = SimpleUrdfLinkExtensions.Create(robotGameObject.transform);
            urdfLink.name = "base_link";
            urdfLink.IsBaseLink = true;
        }

        #region Export

        public static void ExportRobotToUrdf(this SimpleUrdfRobot urdfRobot, string exportRootFolder, string exportDestination)
        {
            UrdfExportPathHandler.SetExportPath(exportRootFolder, exportDestination);

            urdfRobot.FilePath = Path.Combine(UrdfExportPathHandler.GetExportDestination(), urdfRobot.name + ".urdf");

            SimpleRobot robot = urdfRobot.ExportRobotData();
            if (robot == null) return;

            robot.WriteToUrdf();

            Debug.Log(robot.name + " was exported to " + UrdfExportPathHandler.GetExportDestination());

            UrdfExportPathHandler.Clear();
        }

        private static SimpleRobot ExportRobotData(this SimpleUrdfRobot urdfRobot)
        {
            SimpleRobot robot = new SimpleRobot(urdfRobot.FilePath, urdfRobot.gameObject.name);

            List<string> linkNames = new List<string>();

            foreach (SimpleUrdfLink urdfLink in urdfRobot.GetComponentsInChildren<SimpleUrdfLink>())
            {
                //Link export
                if (linkNames.Contains(urdfLink.name))
                {
                    EditorUtility.DisplayDialog("URDF Export Error",
                        "URDF export failed. There are several links with the name " +
                        urdfLink.name + ". Make sure all link names are unique before exporting this robot.",
                        "Ok");
                    return null;
                }
                if (urdfLink.IsBaseLink)
                {
                    robot.root = urdfLink.ExportLinkData();
                }
                else
                {
                    robot.links.Add(urdfLink.ExportLinkData());
                }
                linkNames.Add(urdfLink.name);

                //Joint export
                SimpleUrdfJoint urdfJoint = urdfLink.gameObject.GetComponent<SimpleUrdfJoint>();
                if (urdfJoint != null)
                    robot.joints.Add(urdfJoint.ExportJointData());
                else if (!urdfLink.IsBaseLink)
                    //Make sure that links with no rigidbodies are still connected to the robot by a default joint
                    robot.joints.Add(SimpleUrdfJoint.ExportDefaultJoint(urdfLink.transform));
            }

            return robot;
        }

        #endregion
    }
}
