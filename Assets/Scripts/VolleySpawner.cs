using UnityEngine;
using System.Collections;

namespace Volley
{
    public class VolleySpawner : MonoBehaviour
    {
        static VolleySpawner _Instance;
        public static VolleySpawner Instance
        {
            get
            {
                return _Instance;
            }
        }

        [Header("Ball object prefab")]
        public GameObject objectPrefab;

        [Header("Player hitting hand")]
        public HumanBodyBones leftHandBone;

        [Header("Throwing parameters")]
        public Vector3 throwForce;
        public float throwMultiplier;

        [Header("Hitting parameters")]
        public Vector3 hitForce;
        public float hitMultiplier;

        Transform _leftHandTransform;

        GameObject _cachedPrefab;
        GameObject CachedPrefab
        {
            get
            {
                if (_cachedPrefab == null)
                {
                    _cachedPrefab = (GameObject)Instantiate(objectPrefab);
                }
                return _cachedPrefab;
            }
        }

	    // Use this for initialization
	    void Start ()
        {
            _leftHandTransform = GameManager.Instance.playerAnimator.GetBoneTransform(leftHandBone);

        }
	
	    // Update is called once per frame
	    void Update ()
        {
	    
	    }

        void Awake()
        {
            _Instance = this;
        }

        void OnDestroy()
        {
            _Instance = null;
        }

        public void Spawn(Vector3 position)
        {
            Vector3 pos = _leftHandTransform.position;
            GameObject newBall = (GameObject)Instantiate(CachedPrefab, pos, Quaternion.identity);
            Vector3 force = throwForce * throwMultiplier;
            newBall.GetComponent<Rigidbody>().AddForce(force);

            Debug.Log("Ball Spawned at " + pos);
        }
    }

}
