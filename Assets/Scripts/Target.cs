using UnityEngine;
using System.Collections;
using System;

namespace Volley
{
    public class Target : MonoBehaviour, IInteractable
    {
        public int Multiplier = 1;
        public float TimeToRespawn = 2f;
        public Vector3 torqueForce;

        Collider col;
        MeshRenderer mr;
        Vector3 originalScale;

        float ExpandFactor { get { return originalScale.x * 6f; } }
        float ShrinkFactor { get { return originalScale.x * 3f; } }

        public int Points { get; set; }
        public bool HasInteracted { get; private set; }

        public void Interact(object sender, object args)
        {
            if (HasInteracted)
                return;

            if (args is float)
            {
                //Logger.Log(args);
                float dist = (float)args;
                Core.BroadcastEvent("OnBallHit", this, dist);
            }

            int totalPoints = Points * Multiplier;

            Core.BroadcastEvent("OnTargetHit", this, totalPoints);
            Core.BroadcastEvent("OnTargetHitUpdate", this, null);

            AudioManager.Instance.PlayOneShot(SoundType.Timer);

            Deactivate();
        }

        void Awake()
        {
            col = GetComponent<Collider>();
            mr = GetComponent<MeshRenderer>();

            HasInteracted = col.enabled = mr.enabled = false;
            originalScale = this.transform.localScale;
        }

        void Start ()
        {
            Points = 10;
	    }

        public void Activate()
        {
            HasInteracted = false;
            StartCoroutine(Expand());
        }

        public void Deactivate()
        {
            HasInteracted = true;
            StartCoroutine(Shrink());
        }

        IEnumerator Expand()
        {
            col.enabled = mr.enabled = true;

            Vector3 scale = this.transform.localScale;

            while (transform.localScale.x < originalScale.x - 0.1f)
            {
                scale.x += ExpandFactor * Time.deltaTime;
                scale.z += ExpandFactor * Time.deltaTime;
                transform.localScale = scale;
                yield return null;
            }

            transform.localScale = originalScale;

            yield return null;
        }

        IEnumerator Shrink()
        {
            col.enabled = false;

            Vector3 scale = this.transform.localScale;

            while (transform.localScale.x > 0.1f)
            {
                scale.x -= ShrinkFactor * Time.deltaTime;                
                scale.z -= ShrinkFactor * Time.deltaTime;
                transform.localScale = scale;
                yield return null;
            }

            mr.enabled = false;

            transform.localScale = Vector3.zero;

            yield return null;
        }
    }

}
