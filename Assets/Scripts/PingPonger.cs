using UnityEngine;
using System.Collections;

public class PingPonger : MonoBehaviour
{
    public float range;
    public float xCenter = 0;
    public float speed;

    public bool reversed = false;
    public float initialTime;

	// Use this for initialization
	void Start ()
    {
        xCenter = 0;
        if (reversed)
        {
            initialTime += range;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 targetPos = new Vector3(xCenter + Mathf.PingPong(initialTime + Time.time * speed, range) - (range* 0.5f), transform.position.y, transform.position.z);
        transform.position = targetPos;
	}
}
