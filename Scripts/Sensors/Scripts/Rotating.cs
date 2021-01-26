using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    private float AngleRotated = 0.0f;

    [SerializeField]
    private float RotatingSpeed = 0.1f;
    
    [SerializeField]
    private Vector3 Axis = Vector3.up;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(RotatingSpeed * Axis * Time.deltaTime);
        AngleRotated += RotatingSpeed * Time.deltaTime;
    }
}
