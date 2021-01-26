using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAwayFrom : MonoBehaviour
{
    private Vector3 StartPosition;

    [SerializeField]
    private float MaxRange = 1.0f;
    [SerializeField]
    private Vector3 Direction = -Vector3.back;
    [SerializeField]
    private float Speed = 10.0f;

	// Use this for initialization
	void Start () {
        StartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.SqrMagnitude(StartPosition - transform.position) <= MaxRange * MaxRange)
        {
            transform.Translate(Direction * Speed * Time.deltaTime);
        }
	}
}
