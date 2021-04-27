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
