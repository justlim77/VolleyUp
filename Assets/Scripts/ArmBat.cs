﻿using UnityEngine;
using System.Collections;

public class ArmBat : MonoBehaviour
{
    public Transform target;

    Collider col;
    Rigidbody rb;

    Vector3 _cachedVel;

    // Use this for initialization
    void Start ()
    {
        col = this.GetComponent<Collider>();
        rb = this.GetComponent<Rigidbody>();

        if (target == null)
        {
            Debug.Log("No target found!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (target != null)
        {
            Follow();
        }

        // Velocity check

	}

    void Follow()
    {
        this.transform.position = target.position;
    }
}
