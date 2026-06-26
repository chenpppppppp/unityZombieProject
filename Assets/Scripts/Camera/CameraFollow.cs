using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 5f;
    private Transform target;
    private Vector3 offset;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (target != null)
            offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
