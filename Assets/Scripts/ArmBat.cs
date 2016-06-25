using UnityEngine;
using System.Collections;

namespace Volley
{
    public class ArmBat : MonoBehaviour
    {
        public Transform target;
        public Vector3 hitForce;
        public Vector3 horizontalForce;


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
            rb.MovePosition(target.position);
        }

        void OnTriggerEnter(Collider col)
        {
            IInteractable i = col.gameObject.GetComponent<IInteractable>();
            if (i != null)
            {
                //float hitMag = col.relativeVelocity.magnitude;
                //Debug.Log("Collision magnitude: " + hitMag);
                Vector3 vel = col.attachedRigidbody.velocity.normalized;
                Vector3 hForce = new Vector3(horizontalForce.x * vel.x, 0, 0);
                Debug.Log("Collision velocity: " + vel);
                // Check how close the collision happened
                float dist = Vector3.Distance(transform.position, col.transform.position);
                Debug.Log("Collision distance: " + dist);
                i.Interact(hitForce + hForce);
                Core.BroadcastEvent("OnBallHit", this, dist);
            }
        }
    }
}
