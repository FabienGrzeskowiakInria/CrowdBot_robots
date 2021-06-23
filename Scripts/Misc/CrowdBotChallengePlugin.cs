using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crowdbotsim;
using CrowdMP.Core;

public class CrowdBotChallengePlugin : PluginManager
{
    public ConfigCrowdBotChallenge parameters;
    public override bool LoadPlugin()
    {
        bool is_used = false;
        GameObject RosConnector = transform.Find("RosConnector").gameObject;
        GameObject TcpConnector = transform.Find("TcpConnector").gameObject;

        // Look the config for the plugin parameters
        foreach (ConfigExtra ce in LoaderConfig.addons)
        {
            if (typeof(ConfigCrowdBotChallenge).IsAssignableFrom(ce.GetType()))
            {
                parameters = (ConfigCrowdBotChallenge)ce;
                if (ce.isUsed)
                {
                    switch (parameters.connectorType)
                    {
                        case ConfigCrowdBotChallenge.ConnectorType.ROS:
                            is_used = true;
                            break;

                        case ConfigCrowdBotChallenge.ConnectorType.Tcp:
                            is_used = true;
                            break;

                        case ConfigCrowdBotChallenge.ConnectorType.None:
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    RosConnector.SetActive(false);
                    TcpConnector.SetActive(false);
                    foreach (GameObject sensor in GameObject.FindGameObjectsWithTag("Sensor"))
                    {
                        sensor.SetActive(false);
                    }
                }

                // If not used, deactivate the whole plugin
                if (!is_used)
                {
                    this.enabled = false;
                }

                return is_used;

            }
        }
        return is_used;
    }

    public override void UnloadPlugin()
    {
        return;
    }
}
