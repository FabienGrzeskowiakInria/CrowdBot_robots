// # MIT License

// # Copyright (C) 2018-2021  CrowdBot H2020 European project
// # Inria Rennes Bretagne Atlantique - Rainbow - Julien Pettré

// # Permission is hereby granted, free of charge, to any person obtaining
// # a copy of this software and associated documentation files (the
// # "Software"), to deal in the Software without restriction, including
// # without limitation the rights to use, copy, modify, merge, publish,
// # distribute, sublicense, and/or sell copies of the Software, and to
// # permit persons to whom the Software is furnished to do so, subject
// # to the following conditions:

// # The above copyright notice and this permission notice shall be
// # included in all copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// # OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// # LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace crowdbotsim
{
public class collisionDetection : MonoBehaviour
{
    CollisionReport reports_handler;

    TwistTcpSubscriber controller;

    List<Rigidbody> rigidbodies;

    // Start is called before the first frame update
    void Start()
    {
        reports_handler = gameObject.transform.root.GetComponent<CollisionReport>();
        // challenge = gameObject.transform.root.GetComponentInChildren<CBChallenge>();
        rigidbodies = new List<Rigidbody>(transform.root.GetComponentsInChildren<Rigidbody>());
        try
        {
            controller = gameObject.transform.root.Find("TcpConnector").GetComponent<TwistTcpSubscriber>();
        }
        catch (System.NullReferenceException)
        {
            foreach (var TwistTcpSubscriber in GameObject.Find("TcpConnector").GetComponents<TwistTcpSubscriber>())
            {
                if( TwistTcpSubscriber.SubscribedTransform.root.name == gameObject.transform.root.name)
                {
                    controller = TwistTcpSubscriber;
                    break;
                }                 
            }                        
        }
    }

    // void OnCollisionEnter(Collision collision)
    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.transform.root.tag == "VirtualHumanActive")
        {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
            }

            if(controller !=null)
            {
                controller.Stop();
            }

            string report = collision.gameObject.transform.root.GetComponent<Agent>().GetID().ToString();
            report += " (";
            report += collision.gameObject.name.Remove(0,6); //removinving "Bip01 " at the begining of the agents body parts
            report += " ";
            report += '~' + gameObject.name;
            report += ") ";
            Vector3 human_vel = collision.gameObject.transform.root.GetComponentInChildren<AnimationController>().velocity;
            OdomProvider.Pose_and_twist robot_p_t = this.transform.root.GetComponentInChildren<OdomProvider>().get_pose_and_twist();
            Vector3 robot_vel = new Vector3(robot_p_t.dxdt, 0, robot_p_t.dydt);
            if(robot_vel.magnitude > 0.001)
                report += (robot_vel-human_vel).ToString("F3");


            // if( collision.relativeVelocity.sqrMagnitude > 0.001)
            // {
            //     report += " ";
            //     report += collision.relativeVelocity.ToString("F3");
            // }

            // if( collision.impulse.sqrMagnitude > 0.001)
            // {
            //     report += " ";
            //     report += collision.impulse.ToString("F3");
            // }

            reports_handler.Append_report(report);
            // Debug.Log(report);

            // challenge.increment_collision_count();
            // foreach (var col in gameObject.transform.root.GetComponentsInChildren<Collider>())
            // {
                // Physics.IgnoreCollision(col, collision.collider, true);
                // col.isTrigger = true;
            // }
        }
    }

    // void OnCollisionExit()
    void OnTriggerExit()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}

}