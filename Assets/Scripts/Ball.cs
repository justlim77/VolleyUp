using UnityEngine;
using System.Collections;
using System;

namespace Volley
{
    public class Ball : MonoBehaviour, IInteractable
    {
        public LayerMask[] destroyLayers;
        public float autoDestructTime = 5.0f;
        public int pointsOnHit = 100;

        Collider col;
        Rigidbody rb;

        Vector3 _cachedVel;
        int[] layerIndices;
        int _points;

        public int Points
        {
            get
            {
                return _points;
            }

            set
            {
                _points = value;
            }
        }

        public void Interact(Vector3 force)
        {
            col.isTrigger = false;
            rb.AddForce(force);
        }

        // Use this for initialization
        void Start ()
        {
            col = this.GetComponent<Collider>();
            rb = this.GetComponent<Rigidbody>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            layerIndices = new int[destroyLayers.Length];
            for(int i = 0; i<layerIndices.Length; i++)
            {
                layerIndices[i] = destroyLayers[i].value;
            }

            Points = pointsOnHit;

            Destroy(this.gameObject, autoDestructTime * 2.0f);
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
            if (col != null)
            {
                col.isTrigger = localVel.y > 0 ? true : false;
            }
	    }

        void OnCollisionEnter(Collision col)
        {
            foreach (int layerIndex in layerIndices)
            {
                if (col.gameObject.layer == layerIndex)
                {
                    Destroy(this.gameObject, autoDestructTime);
                    break;
                }
            }
        }
    }
}
