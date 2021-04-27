using UnityEditor;
using UnityEngine;

namespace crowdbotsim.Urdf.Editor
{
    [CustomEditor(typeof(SimpleUrdfRobot))]
    public class SimpleUrdfRobotEditor : UnityEditor.Editor
    {
        private SimpleUrdfRobot urdfRobot;
        private static GUIStyle buttonStyle;

        public override void OnInspectorGUI()
        {
            if (buttonStyle == null)
                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight) { fixedWidth = 75 };

            urdfRobot = (SimpleUrdfRobot) target;

            GUILayout.Space(5);
            GUILayout.Label("All Joints", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Generate Unique Joint Names");
            if (GUILayout.Button("Generate", new GUIStyle (EditorStyles.miniButton) {fixedWidth = 155}))
                urdfRobot.GenerateUniqueJointNames();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button("Export Simple robot to URDF file"))
            {
                // Get existing open window or if none, make a new one:
                SimpleUrdfExportEditorWindow window = (SimpleUrdfExportEditorWindow)EditorWindow.GetWindow(typeof(SimpleUrdfExportEditorWindow));
                window.urdfRobot = urdfRobot;
                window.minSize = new Vector2(500, 200);
                window.GetEditorPrefs();
                window.Show();
            }
        }
    }
}
