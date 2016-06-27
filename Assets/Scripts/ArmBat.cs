using UnityEngine;
using System.Collections;

namespace Volley
{
    public class ArmBat : MonoBehaviour
    {
        public Transform target;
        public Vector3 hitForce;
        public Vector3 horizontalForce;

        public Vector3 FrameVelocity { get; set; }
        public Vector3 PrevPosition { get; set; }

        Collider col;
        Rigidbody rb;

        Vector3 _cachedVel;
        Transform _cachedTorsoTransform;

        // Use this for initialization
        void Start ()
        {
            col = this.GetComponent<Collider>();
            rb = this.GetComponent<Rigidbody>();

            _cachedTorsoTransform = GameManager.Instance.playerAnimator.GetBoneTransform(HumanBodyBones.Chest);

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

            if (_cachedTorsoTransform != null)
            {
                //Logger.Log(_cachedTorsoTransform.forward);
            }

            // Velocity check
            // Keep an average velocity due to fixed update irregularity, else we will occassionally get 0 velocity
            Vector3 currFrameVelocity = (transform.position - PrevPosition) / Time.deltaTime;
            FrameVelocity = Vector3.Lerp(FrameVelocity, currFrameVelocity, 0.1f);
            PrevPosition = transform.position;
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
                //Vector3 vel = col.attachedRigidbody.velocity.normalized;
                Vector3 vel = FrameVelocity.normalized;

                Vector3 hForce = new Vector3(horizontalForce.x * _cachedTorsoTransform.forward.z, 0, horizontalForce.z * vel.z);
                Logger.Log("Collision velocity: " + vel);
                // Check how close the collision happened
                //float dist = Vector3.Distance(transform.position, col.transform.position);
                float dist = Mathf.Abs(transform.position.y - col.transform.position.y);
                Logger.Log("Collision distance: " + dist);
                i.Interact(hitForce + hForce);
                Core.BroadcastEvent("OnBallHit", this, dist);
            }
        }
    }
}
