using UnityEngine;
using crowdbotsim;
using RosSharp.Urdf;
using System.Xml;
using System.Xml.Linq;

namespace crowdbotsim.Urdf
{
    public class SimpleJoint
    {
        public string _joint_name, type, parent, child;
        public Origin origin1;
        public Axis axis1;
        public SimpleJoint()
        {
            parent = "";
            child = "";
            type = SimpleUrdfJoint.JointTypes.Fixed.ToString().ToLower();
            _joint_name = parent + "_" + child + "_joint";
        }

        public SimpleJoint(string joint_name, string joint_type, string parent_name, string child_name, Origin origin, Axis axis)
        {
            parent = parent_name;
            child = child_name;
            type = joint_type;
            _joint_name = joint_name;
            origin1 = origin;
            axis1 = axis;

        }
        public class Axis
        {
            public double[] xyz;

            public Axis(double[] xyz)
            {
                this.xyz = new double[xyz.Length];
                xyz.CopyTo(this.xyz, 0);
            }

            public void WriteToUrdf(XmlWriter writer)
            {
                writer.WriteStartElement("axis");
                writer.WriteAttributeString("xyz",xyz[0]+" "+xyz[1]+" "+xyz[2]);
                writer.WriteEndElement();
            }
        }

        public void WriteToUrdf(XmlWriter writer)
        {
            writer.WriteStartElement("joint");
            writer.WriteAttributeString("name", _joint_name);
            writer.WriteAttributeString("type", type);

            writer.WriteStartElement("parent");
            writer.WriteAttributeString("link", parent);
            writer.WriteEndElement();


            writer.WriteStartElement("child");
            writer.WriteAttributeString("link", child);
            writer.WriteEndElement();

            if (origin1 != null) origin1.WriteToUrdf(writer);
            if(type != "fixed") axis1.WriteToUrdf(writer);

            writer.WriteEndElement();
        }
    }
}
