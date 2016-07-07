using UnityEngine;
using System.Collections;

namespace Volley
{
    public class ArmBat : MonoBehaviour
    {
        public bool countScore = true;
        public Vector3 hitForce;
        public Vector3 horizontalForce;
        public Vector3 throwingForce;

        public HumanBodyBones armBone;
        public bool followTarget;
        public Transform handTransform;

        public Vector3 FrameVelocity { get; set; }
        public Vector3 PrevPosition { get; set; }

        Collider col;
        Vector3 _cachedVel;
        Transform _cachedBoneTransform;

        // Use this for initialization
        void Start ()
        {
            col = this.GetComponent<Collider>();

            _cachedBoneTransform = GameManager.Instance.playerAnimator.GetBoneTransform(armBone);
	    }

        void Update()
        {
            if (followTarget && handTransform != null)
            {
                FollowTarget();
            }
        }

        void FixedUpdate()
        {
            // Keep an average velocity due to fixed update irregularity, else we will occassionally get 0 velocity
            Vector3 currFrameVelocity = (transform.position - PrevPosition) / Time.deltaTime;
            FrameVelocity = Vector3.Lerp(FrameVelocity, currFrameVelocity, 0.1f);
            PrevPosition = transform.position;
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

               // Vector3 externalForce = new Vector3(horizontalForce.x * vel.x, horizontalForce.y * vel.y, horizontalForce.z * vel.z);
                Vector3 externalForce = new Vector3(horizontalForce.x * _cachedBoneTransform.forward.z, horizontalForce.y * vel.y, horizontalForce.z * vel.z);

                //Logger.Log(_cachedBoneTransform.forward.z);
                //Logger.Log("Collision velocity: " + vel);
                // Check how close the collision happened
                //float dist = Vector3.Distance(transform.position, col.transform.position);
                float dist = Mathf.Abs(transform.position.y - col.transform.position.y);
                //Logger.Log("Collision distance: " + dist);
                Vector3 force = hitForce + externalForce;
                i.Interact(this, force);

                // Audio feedback
                AudioManager.Instance.PlayRandomClipAtPoint(SoundType.VolleyHit, transform.position);
            }
        }

        void FollowTarget()
        {
            transform.position = handTransform.position;
        }
    }
}


/*

    Original joint)shoulderLT rot -1.9427, -0.0498, 1.6662
    oringnal jiont shodler RT rot -1.9427, -0.0498, 1.6661

*/
