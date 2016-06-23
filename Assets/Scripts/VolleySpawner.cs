using UnityEngine;
using System.Collections;

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

    [Header("Throwing parameters")]
    public Vector3 throwForce;
    public float throwMultiplier;

    [Header("Hitting parameters")]
    public Vector3 hitForce;
    public float hitMultiplier;

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
        GameObject newBall = (GameObject)Instantiate(CachedPrefab, position, Quaternion.identity);
        Vector3 force = throwForce * throwMultiplier;
        newBall.GetComponent<Rigidbody>().AddForce(force);
    }
}
