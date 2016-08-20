using UnityEngine;
using System.Collections;

namespace Volley
{
    public class VolleySpawner : MonoBehaviour
    {
        public static VolleySpawner Instance
        { get; private set; }

        [Header("Ball pool")]
        public FastPool objectPool;

        [Header("Spawn parameters")]
        public HumanBodyBones spawnAtBone;
        public Vector3 spawnOffset;
        public int spawnAmount;
        public bool useOffset;

        [Header("Player hitting hand")]
        public ArmBat armBat;

        [Header("Throwing parameters")]
        public Vector3 throwForce;
        public float throwMultiplier;

        [Header("Debug")]
        public bool testSpawn = false;
        public float spawnDelay = 1.0f;

        public float unitOffset;
        public Transform[] _spawnBoneTransform;

        public static int BallCount
        {
            get; private set;
        }

	    // Use this for initialization
	    void Start ()
        {
            //foreach(var humanBones in GameManager.Instance.players[0].GetComponent<Animator>().bone)
            _spawnBoneTransform[0] = GameManager.Instance.playerAnimator.GetBoneTransform(spawnAtBone);

            objectPool.Init(transform);

            if (testSpawn)
            {
                InvokeRepeating("Spawn", 0, spawnDelay);
            }

            //_volleyPool = FastPoolManager.CreatePool(objectPrefab, true, spawnAmount);
        }

        void Spawn()
        {
            Spawn(Vector3.up * 8);
        }
	
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public GameObject Spawn(Vector3 position)
        {
            GameManager gameManager = GameManager.Instance;
            Vector3 pos = _spawnBoneTransform[0].position;

            if (useOffset)
            {
                Vector3 dir = _spawnBoneTransform[0].TransformPoint(spawnOffset);/* position)*//* + (_spawnBoneTransform[0].forward * unitOffset);*/;
                print(dir);
                //dir.Normalize();
                //pos += dir;
                pos = dir;
            }

            //GameObject ball = (GameObject)Instantiate(objectPrefab, pos, Quaternion.identity);
            GameObject ball = objectPool.FastInstantiate(pos, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();

            Vector3 torque = new Vector3(20, 20, 20);
            rb.AddTorque(torque);

            Vector3 force = throwForce * throwMultiplier;
            rb.AddForce(force);
            BallCount++;
            //Logger.Log(string.Format("{0} Ball Spawned at {1}", BallCount, pos));

            AudioManager.Instance.PlayOneShot(SoundType.Whoosh);     // Play whoosh sound

            return ball;
        }

        public void Despawn(GameObject sceneObject)
        {
            objectPool.FastDestroy(sceneObject);
        }

        public void SetBone(int idx)
        {
            _spawnBoneTransform[idx] = GameManager.Instance.players[idx].GetComponent<Animator>().GetBoneTransform(spawnAtBone);
        }
    }

}
