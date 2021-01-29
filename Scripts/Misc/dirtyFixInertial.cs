using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dirtyFixInertial : MonoBehaviour
{
    public Vector3 CenterOfMass;

    public bool UrdfInertialEnabled = false;

    bool firstUpdate = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<RosSharp.Urdf.UrdfInertial>().enabled = UrdfInertialEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        if(firstUpdate)
        {
            transform.GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
            firstUpdate = false;
        }
    }
}
