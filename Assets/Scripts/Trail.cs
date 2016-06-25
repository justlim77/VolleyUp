using UnityEngine;
using System.Collections;

namespace Volley
{
    public class Trail : MonoBehaviour
    {
        [SerializeField] Vector3 trailOffset;
        LineRenderer _lr;

	    // Use this for initialization
	    void Start ()
        {
            _lr = GetComponent<LineRenderer>();
            if (_lr != null)
            {
                _lr.enabled = false;
                _lr.useWorldSpace = true;
            }
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            if (_lr != null && gameObject.activeSelf == true)
            {
                Vector3 currentPosition = transform.position;
                Vector3 endPosition = currentPosition + trailOffset;

                _lr.SetPosition(0, currentPosition);
                _lr.SetPosition(1, endPosition);
            }
	    }
    }
}
