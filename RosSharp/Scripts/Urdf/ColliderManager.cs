using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    /*
    This script search for mesh colliders recursively in the model and enable/disable them
    according to the boolean enable_colliders    
     */
    public bool enable_colliders = false;
    public bool colliders_is_trigger = true;

    // Start is called before the first frame update
    void Start()
    {
            foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>())
            {
                meshCollider.isTrigger = colliders_is_trigger;
                meshCollider.enabled = enable_colliders;                
            }
    }

    // void RecursiveFindColliderInChild(Transform parent)
    // {
    //     foreach (Transform child in parent)
    //     {
    //         MeshCollider mc = child.GetComponent<MeshCollider>();
    //         if (mc != null)
    //         {
    //             mc.enabled = enable_colliders;
    //         }
    //         RecursiveFindColliderInChild(child);
    //     }
    // }
}

