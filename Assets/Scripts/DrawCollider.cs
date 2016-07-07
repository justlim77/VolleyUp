using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DrawCollider : MonoBehaviour
{
    public Collider collider;

    Vector3 center;
    float radius;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (collider != null)
        {
            center = collider.bounds.center;
            radius = collider.bounds.size.x * 0.5f;
            Gizmos.DrawWireCube(center, collider.bounds.size);
        }
    }

	// Use this for initialization
	void Start ()
    {
        if (collider == null)
            collider = GetComponent<Collider>();
	}
}
