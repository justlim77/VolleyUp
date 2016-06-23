using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    Collider col;
    Rigidbody rb;

    Vector3 _cachedVel;

	// Use this for initialization
	void Start ()
    {
        col = this.GetComponent<Collider>();
        rb = this.GetComponent<Rigidbody>();
        if (col != null)
        {
            col.isTrigger = true;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        if (col != null)
        {
            col.isTrigger = localVel.y > 0 ? true : false;
            Debug.Log(localVel.y);
        }
	}
}
