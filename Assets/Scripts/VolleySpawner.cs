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

        [Header("Spawn parameters")]
        public HumanBodyBones spawnAtBone;
        public Vector3 spawnOffset;

        [Header("Player hitting hand")]
        public ArmBat armBat;

        [Header("Throwing parameters")]
        public Vector3 throwForce;
        public float throwMultiplier;

        Transform _spawnBoneTransform;

        static int ballCount;
        public static int BallCount
        {
            get { return ballCount; }
            set { ballCount = value; }
        }

	    // Use this for initialization
	    void Start ()
        {
            _spawnBoneTransform = GameManager.Instance.playerAnimator.GetBoneTransform(spawnAtBone);

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

        public GameObject Spawn(Vector3 position)
        {
            Vector3 pos = _spawnBoneTransform.position + spawnOffset;

            GameObject ball = (GameObject)Instantiate(objectPrefab, pos, Quaternion.identity);
            BallCount++;
            Vector3 force = throwForce * throwMultiplier;
            ball.GetComponent<Rigidbody>().AddForce(force);
            //Logger.Log(string.Format("{0} Ball Spawned at {1}", BallCount, pos));
            return ball;
        }
    }

}
