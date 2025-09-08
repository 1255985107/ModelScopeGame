using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Target")]
    public Transform target;
    [Tooltip("Offset")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Tooltip("Follow Speed")]
    [Range(0.1f, 10f)]
    public float followSpeed = 2f;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow: No Camera component found on this GameObject.");
        }
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned. Please assign a target Transform.");
        }
        transform.position = target.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && cam != null)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10; // Ensure the camera stays at z = -10
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}