using UnityEngine;
using System.Collections;

namespace Volley
{
    public class ArmBat : MonoBehaviour
    {
        public Transform target;
        public Vector3 hitForce;


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

        void OnCollisionEnter(Collision col)
        {
            IInteractable i = col.gameObject.GetComponent<IInteractable>();
            if (i != null)
            {
                float hitMag = col.relativeVelocity.magnitude;
                Debug.Log("Collision magnitude: " + hitMag);
                i.Interact(hitForce);
                int points = (int)hitMag * i.Points;
                Core.BroadcastEvent("OnBallHit", this, points);
            }
        }
    }
}
