using UnityEngine;
using System.Collections;
using System;

namespace Volley
{
    public class Ball : MonoBehaviour, IInteractable, IFastPoolItem
    {
        public LayerMask[] destroyLayers;
        public float autoDestructTime = 5.0f;
        public int pointsOnHit = 100;

        Collider col;
        Rigidbody rb;

        Vector3 _cachedVel;
        int[] layerIndices;
        int _points;
        bool _hit;
        bool _grounded;

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

        public void Interact(object sender, object args)
        {
            col.isTrigger = false;
            _hit = true;
            if (args is Vector3)
            {
                Vector3 force = (Vector3)args;
                rb.AddForce(force);
            }

            int rand = UnityEngine.Random.Range(0, 2);
            if(rand == 0)
                AudioManager.Instance.PlayRandomClipAtPoint(SoundType.VolleyHit, transform.position);
            else
                AudioManager.Instance.PlayRandomClipAtPoint(SoundType.VolleySpike, transform.position);
        }

        void Awake()
        {
            col = this.GetComponent<Collider>();
            rb = this.GetComponent<Rigidbody>();
        }

        // Use this for initialization
        void Start ()
        {
            layerIndices = new int[destroyLayers.Length];
            for(int i = 0; i<layerIndices.Length; i++)
            {
                layerIndices[i] = destroyLayers[i].value;
            }

            Points = pointsOnHit;
            _grounded = false;

            //Destroy(this.gameObject, autoDestructTime * 2.0f);

            name = "Ball_Instantiated";
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            if ((_hit || _grounded) == true)
                return;

            Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
            if (col != null)
            {
                //col.isTrigger = localVel.y > 0 ? true : false;
            }

        }

        void OnCollisionEnter(Collision col)
        {
            IInteractable i = col.gameObject.GetComponent<IInteractable>();
            if (i != null)
            {
                i.Interact(this, null);
                //Destroy(this.gameObject);
                StartCoroutine(InvokeDespawn(2.0f));
            }

            foreach (int layerIndex in layerIndices)
            {
                if (col.gameObject.layer == layerIndex)
                {
                    //Destroy(this.gameObject, autoDestructTime);
                    _grounded = true;
                    StartCoroutine(InvokeDespawn(2.0f));
                    break;
                }
            }
        }

        IEnumerator InvokeDespawn(float delay)
        {
            yield return new WaitForSeconds(delay);
            VolleySpawner.Instance.Despawn(gameObject);
        }

        void Activate()
        {
            if (col != null)
                col.isTrigger = false;
        }

        public void OnFastInstantiate()
        {
            CancelInvoke();
            name = "Ball_Spawned";
            _hit = false;
            col.isTrigger = true;
            Invoke("Activate", 1.5f);
            StartCoroutine(InvokeDespawn(autoDestructTime));            
        }

        public void OnFastDestroy()
        {
            name = "Ball_Cached";
            if(rb != null)
                rb.velocity = Vector3.zero;
            if (col != null)
                col.isTrigger = true;
            CancelInvoke();
            StopAllCoroutines();
        }
    }
}
