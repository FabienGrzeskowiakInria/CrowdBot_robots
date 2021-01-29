using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBChallenge : MonoBehaviour
{
    int proximity_count = 0;
    int collision_count = 0;

    float delta = 1.0f;
    float last_time = 0.0f;

    CollisionReport reports_handler;

    public void Awake()
    {
        reports_handler = gameObject.transform.root.GetComponent<CollisionReport>();
    }

    public void increment_collision_count()
    {
        collision_count++;
    }

    // Update is called once per frame
    void Update()
    {
        // if(ToolsTime.AbsoluteTime > last_time + delta )
        // {
        //     last_time = ToolsTime.AbsoluteTime;
        //     Debug.Log("Proximity count: "+proximity_count+" || Collision count: "+collision_count + " || Time: " + ToolsTime.AbsoluteTime);
        //     Debug.Log("------------- Full Report -----------------------");
        //     reports_handler.Get_report();
        //     Debug.Log("------------- End of Full Report ----------------");
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        proximity_count++;
    }
}
