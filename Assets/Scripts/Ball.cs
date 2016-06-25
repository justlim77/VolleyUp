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
        bool _permSolid;

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

        public void Interact(object args)
        {
            col.isTrigger = false;
            _permSolid = true;
            if (args is Vector3)
            {
                Vector3 force = (Vector3)args;
                rb.AddForce(force);
            }
        }

        // Use this for initialization
        void Start ()
        {
            col = this.GetComponent<Collider>();
            rb = this.GetComponent<Rigidbody>();
            if (col != null)
            {
                col.isTrigger = true;
                col.enabled = false;
            }
            Invoke("Activate", 1f);

            layerIndices = new int[destroyLayers.Length];
            for(int i = 0; i<layerIndices.Length; i++)
            {
                layerIndices[i] = destroyLayers[i].value;
            }

            Points = pointsOnHit;
            _permSolid = false;

            Destroy(this.gameObject, autoDestructTime * 2.0f);
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            if (_permSolid == true)
                return;

            Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
            if (col != null)
            {
                col.isTrigger = localVel.y > 0 ? true : false;
            }
	    }

        void OnCollisionEnter(Collision col)
        {
            IInteractable i = col.gameObject.GetComponent<IInteractable>();
            if (i != null)
            {
                i.Interact(null);
            }

            foreach (int layerIndex in layerIndices)
            {
                if (col.gameObject.layer == layerIndex)
                {
                    Destroy(this.gameObject, autoDestructTime);
                    break;
                }
            }
        }

        void Activate()
        {
            if (col != null)
                col.enabled = true;
        }
    }
}
