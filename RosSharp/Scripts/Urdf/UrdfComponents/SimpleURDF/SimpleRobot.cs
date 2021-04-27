using UnityEngine;
using crowdbotsim;
using RosSharp.Urdf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace crowdbotsim.Urdf
{
    public class SimpleRobot
    {
        public string _name;
        public string filename;
        public string name;
        public SimpleLink root;
        public List<SimpleLink> links = new List<SimpleLink>();
        public List<SimpleJoint> joints = new List<SimpleJoint>();

        public SimpleRobot(string urdf_file_path)
        {
            filename = urdf_file_path;
            _name = "SimpleRobot";
        }

        public SimpleRobot(string urdf_file_path, string name)
        {
            filename = urdf_file_path;
            _name = name;
        }

        public void WriteToUrdf()
        {
            XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
            
            File.WriteAllText(filename, "");

            using (var stream = File.OpenWrite(filename))
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("robot");
                    writer.WriteAttributeString("name",_name);

                    if(root == null) Debug.LogError("Missing base_link simple urdf link");
                    root.WriteToUrdf(writer);

                    foreach (SimpleLink link in links)
                    {
                        link.WriteToUrdf(writer);
                    }

                    foreach (SimpleJoint joint in joints)
                    {
                        joint.WriteToUrdf(writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }
    }
}
