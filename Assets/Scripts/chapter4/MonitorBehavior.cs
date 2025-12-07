using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorBehavior : MonoBehaviour
{

    public BoxCollider2D monitorCollider;
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    public Transform imageTarget;

    private Transform currentTarget;

    void Start()
    {
        Debug.Assert(monitorCollider != null, "Monitor Collider is not assigned!");
        Debug.Assert(pointA != null, "Point A is not assigned!");
        Debug.Assert(pointB != null, "Point B is not assigned!");
        Debug.Assert(imageTarget != null, "Image Target is not assigned!");
        currentTarget = pointB;
    }

    void Update()
    {
        if (pointA == null || pointB == null)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.0001f)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
            }
            else
            {
                currentTarget = pointB;
            }
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 newScale = imageTarget.localScale;
        newScale.x *= -1;
        imageTarget.transform.localScale = newScale;
    }
}
