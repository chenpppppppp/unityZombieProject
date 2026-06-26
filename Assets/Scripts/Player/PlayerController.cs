using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    public float speedMultiplier = 1f;
    private Rigidbody rb;
    private float fixedY;

    private void Awake()
    {
        gameObject.layer = 6; // Player 层
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        fixedY = transform.position.y;
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;
        float mul = Mathf.Min(speedMultiplier, 3f); // 上限 3 倍速
        rb.velocity = new Vector3(dir.x * speed * mul, 0, dir.z * speed * mul);
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }
}
