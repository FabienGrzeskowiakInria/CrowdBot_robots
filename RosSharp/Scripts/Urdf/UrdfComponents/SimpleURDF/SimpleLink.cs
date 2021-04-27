using UnityEngine;
using crowdbotsim;
using RosSharp.Urdf;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace crowdbotsim.Urdf
{
    public class SimpleLink
    {
        public string name;
        // public Inertial inertial;
        // public List<Visual> visuals;
        public List<Collider> collisions;
        public List<SimpleJoint> joints;
        public List<Link.Visual> visuals;


        public SimpleLink()
        {
            name = "defaultSimpleLink";
            visuals = new List<Link.Visual>();
            collisions = new List<Collider>();
            joints = new List<SimpleJoint>();
        }

        public SimpleLink(string link_name) : base()
        {
            name = link_name;
        }

        public void WriteToUrdf(XmlWriter writer)
        {
            writer.WriteStartElement("link");
            writer.WriteAttributeString("name", name);
            foreach(Link.Visual visual in visuals)
            {
                visual.WriteToUrdf(writer);
            }
            writer.WriteEndElement();
        }
    }
}
