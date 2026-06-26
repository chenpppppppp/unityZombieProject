using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public float maxHp = 45f;
    [HideInInspector] public int xpReward = 20;
    private float currentHp;
    private Transform player;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHp = maxHp;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            player = GameManager.Instance.Player?.transform;
    }

    private void Update()
    {
        if (player == null || agent == null) return;
        if (!agent.isOnNavMesh) return;
        agent.SetDestination(player.position);

        // 距离检测 DOT（替代 Trigger，因为 Physics.IgnoreLayerCollision 也屏蔽 Trigger）
        if (Vector3.Distance(transform.position, player.position) < 1.2f)
        {
            player.GetComponent<PlayerStats>()?.ApplyDot(3f, 5f, gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0f)
        {
            agent.isStopped = true;
            Die();
            Destroy(gameObject, 0.5f);
        }
    }

    private void Die()
    {
        var stats = GameManager.Instance?.Player?.GetComponent<PlayerStats>();
        if (stats != null) stats.GrantXp(xpReward);

        SpawnDrops();

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    private void SpawnDrops()
    {
        if (Random.value > 0.4f) return;

        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(0.5f, 1.5f);
            Vector3 dropPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;

            bool isSpeed = Random.value < 0.5f;
            var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = isSpeed ? "SpeedBuff" : "RapidFire";
            item.transform.position = dropPos;
            item.transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);

            item.GetComponent<MeshRenderer>().material.color = isSpeed ? Color.cyan : Color.yellow;
            item.GetComponent<BoxCollider>().isTrigger = true;

            var drop = item.AddComponent<DropItem>();
            drop.itemType = isSpeed ? DropItem.ItemType.SpeedBuff : DropItem.ItemType.RapidFireBuff;
            drop.duration = 5f;
            drop.multiplier = 1.5f;

            var rb = item.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            Destroy(item, 15f);
        }
    }
}
