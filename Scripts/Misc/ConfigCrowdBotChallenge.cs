using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using CrowdMP.Core;

public class ConfigCrowdBotChallenge : ConfigExtra
{
    public enum ConnectorType
    {
        ROS,
        Tcp,
        None
    }

    [XmlAttribute("ConnectorType")]
    public ConnectorType connectorType;

    [XmlAttribute("UseLidar")]
    public bool UseLidar;

    [XmlAttribute("UsePointCloud")]
    public bool UsePointCloud;

    [XmlAttribute("UseExternalClock")]
    public bool UseExternalClock;


    // [XmlAttribute("RobotName")]
    // public bool RobotName;

    public ConfigCrowdBotChallenge()
    {
        isUsed = false;
    }

}
