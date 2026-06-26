using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [SerializeField] private float range = 12f;
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private float bulletSpeed = 12f;
    public float damage = 15f;
    public float cooldownMultiplier = 1f;
    private float timer;

    // 大子弹模式
    public bool bigBulletMode;
    public float bigBulletScale = 0.8f;
    public float bigBulletAoE = 3f;
    public Color bigBulletColor = new Color(1f, 0.15f, 0.6f); // 品红

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        var target = FindNearestEnemy();
        if (target != null)
        {
            Shoot(target);
            timer = cooldown / cooldownMultiplier;
        }
    }

    private Transform FindNearestEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDist = range;

        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = e.transform;
            }
        }
        return nearest;
    }

    private void Shoot(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = bigBulletMode ? "BigBullet" : "Bullet";
        bullet.transform.position = transform.position + dir * 0.8f;

        var proj = bullet.AddComponent<Projectile>();
        proj.damage = damage;

        if (bigBulletMode)
        {
            bullet.transform.localScale = new Vector3(bigBulletScale, bigBulletScale, bigBulletScale);
            bullet.GetComponent<MeshRenderer>().material.color = bigBulletColor;
            proj.isAoE = true;
            proj.aoeRadius = bigBulletAoE;
        }
        else
        {
            bullet.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            bullet.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.3f, 0f);
        }

        var col = bullet.GetComponent<SphereCollider>();
        col.isTrigger = true;

        var rb = bullet.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = dir * bulletSpeed;

        Destroy(bullet, 3f);
    }
}
