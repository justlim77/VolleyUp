using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DrawCollider : MonoBehaviour
{
    public Collider collider;

    Vector3 center;
    Vector3 size;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (collider != null)
        {
            center = collider.bounds.center;
            size = collider.bounds.size;
            Gizmos.DrawWireCube(center, size);
        }
    }

	// Use this for initialization
	void Start ()
    {

	}
}
