﻿/*
© Siemens AG, 2018
Author: Suzannah Smith (suzannah.smith@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using crowdbotsim;

namespace RosSharp.Urdf
{
    public enum GeometryTypes { Box, Cylinder, Sphere, Mesh }

    public class UrdfRobot : MonoBehaviour
    {
        public string FilePath;
        
        #region Configure Robot
        public void SetCollidersConvex(bool convex)
        {
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.enabled = true;
                GameObject parent_go = meshRenderer.gameObject.transform.parent.parent.parent.gameObject;
                string parent_name = parent_go.name;
                if(parent_name == "Collisions")
                {
                    MeshCollider[] meshColliders = meshRenderer.gameObject.GetComponents<MeshCollider>();
                    if(meshColliders != null){
                        foreach(MeshCollider meshCollider in meshColliders)
                        {
                            DestroyImmediate(meshCollider);
                        }
                    } 
                    meshRenderer.gameObject.AddComponent<MeshCollider>();
                    collisionDetection c = parent_go.transform.parent.gameObject.GetComponent<collisionDetection>();
                    if(c == null)
                    {
                        parent_go.transform.parent.gameObject.AddComponent<collisionDetection>();
                    }

                    meshRenderer.enabled = false;
                }
            }
            foreach (MeshCollider meshCollider in GetComponentsInChildren<MeshCollider>())
            {
                meshCollider.convex = convex;
                meshCollider.isTrigger = false;
            }
        }

        public void SetRigidbodiesIsKinematic(bool isKinematic)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = isKinematic;
        }

        public void SetUseUrdfInertiaData(bool useUrdfData)
        {
            foreach (UrdfInertial urdfInertial in GetComponentsInChildren<UrdfInertial>())
                urdfInertial.UseUrdfData = useUrdfData;
        }

        public void SetRigidbodiesUseGravity(bool useGravity)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
                rb.useGravity = useGravity;
        }

        public void GenerateUniqueJointNames()
        {
            foreach (UrdfJoint urdfJoint in GetComponentsInChildren<UrdfJoint>())
                urdfJoint.GenerateUniqueJointName();
        }

        #endregion
    }
}