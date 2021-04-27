using UnityEngine;
using RosSharp;
using RosSharp.Urdf;
using RosSharp.Urdf.Editor;

namespace crowdbotsim.Urdf.Editor
{
    public static class SimpleUrdfLinkExtensions
    { 
        public static SimpleUrdfLink Create(Transform parent, SimpleLink link = null, SimpleJoint joint = null)
        {
            GameObject linkObject = new GameObject("link");
            linkObject.transform.SetParentAndAlign(parent);
            SimpleUrdfLink urdfLink = linkObject.AddComponent<SimpleUrdfLink>();

            // if (link != null)
            //     urdfLink.ImportLinkData(link, joint);
            // else
            // {
                UnityEditor.EditorGUIUtility.PingObject(linkObject);
            // }

            return urdfLink;
        }
        
        public static SimpleLink ExportLinkData(this SimpleUrdfLink urdfLink)
        {
            if(urdfLink.transform.localScale != Vector3.one)
                Debug.LogWarning("Only visuals should be scaled. Scale on link \"" + urdfLink.gameObject.name + "\" cannot be saved to the URDF file.", urdfLink.gameObject);

            // UrdfInertial urdfInertial = urdfLink.gameObject.GetComponent<UrdfInertial>();
            SimpleLink link = new SimpleLink(urdfLink.gameObject.name)
            {
                visuals = urdfLink.GetComponentInChildren<UrdfVisuals>().ExportVisualsData(),
            };
            
            return link;
        }
    }
}