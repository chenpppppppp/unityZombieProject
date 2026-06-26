using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 15f;
    public bool isAoE;
    public float aoeRadius = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (isAoE)
                DealAoE();
            else
            {
                other.GetComponent<Enemy>()?.TakeDamage(damage);
                DamageNumber.Instance?.Show(transform.position, damage, Color.yellow);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player"))
        {
            if (isAoE)
                DealAoE();
            Destroy(gameObject);
        }
    }

    private void DealAoE()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist <= aoeRadius)
            {
                e.GetComponent<Enemy>()?.TakeDamage(damage);
                DamageNumber.Instance?.Show(e.transform.position, damage, Color.yellow);
            }
        }
    }
}
