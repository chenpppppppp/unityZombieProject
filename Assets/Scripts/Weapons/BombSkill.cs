using UnityEngine;

public class BombSkill : MonoBehaviour
{
    [SerializeField] private float cooldown = 10f;
    [SerializeField] private float fuseTime = 4f;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float damage = 45f;

    private float cdTimer;
    public float CooldownPercent => Mathf.Clamp01(1f - cdTimer / cooldown);

    private void Update()
    {
        cdTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha1) && cdTimer <= 0f)
        {
            PlaceBomb();
            cdTimer = cooldown;
        }
    }

    private void PlaceBomb()
    {
        var bomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bomb.name = "Bomb";
        bomb.transform.position = transform.position + Vector3.up * 0.2f;
        bomb.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        bomb.GetComponent<MeshRenderer>().material.color = Color.red;

        Destroy(bomb.GetComponent<SphereCollider>());
        var col = bomb.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = blastRadius;

        var bombScript = bomb.AddComponent<BombBehaviour>();
        bombScript.fuseTime = fuseTime;
        bombScript.damage = damage;
        bombScript.radius = blastRadius;
    }
}

public class BombBehaviour : MonoBehaviour
{
    public float fuseTime = 4f;
    public float damage = 45f;
    public float radius = 5f;
    private float timer;
    private float baseScale;

    private void Start()
    {
        baseScale = transform.localScale.x;
        timer = fuseTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        // 脉冲闪烁
        float pulse = 1f + Mathf.Sin(timer * 8f) * 0.3f;
        transform.localScale = Vector3.one * baseScale * pulse;

        var renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(timer * 4f, 1f));

        if (timer <= 0f)
            Explode();
    }

    private void Explode()
    {
        // 范围伤害
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= radius)
            {
                var enemy = e.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    DamageNumber.Instance?.Show(e.transform.position, damage, new Color(1f, 0.5f, 0f));
                }
            }
        }

        Destroy(gameObject);
    }
}
