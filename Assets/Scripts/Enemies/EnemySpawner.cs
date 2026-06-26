using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyType
    {
        public string name;
        public Color color;
        public float speed;
        public float maxHp;
        public float scale;
        public int xpReward;
        public float weight; // 生成权重
    }

    public EnemyType[] enemyTypes = new EnemyType[]
    {
        new EnemyType { name = "普通丧尸", color = new Color(0.85f, 0.15f, 0.15f), speed = 3f,  maxHp = 45f,  scale = 1f,   xpReward = 20, weight = 5f },
        new EnemyType { name = "快速丧尸", color = new Color(1f, 0.5f, 0f),     speed = 5.5f, maxHp = 25f,  scale = 0.8f, xpReward = 15, weight = 3f },
        new EnemyType { name = "巨型丧尸", color = new Color(0.6f, 0.1f, 0.5f),  speed = 2f,   maxHp = 120f, scale = 1.8f, xpReward = 50, weight = 1.5f },
    };

    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnMargin = 5f;
    private float timer;
    private float totalWeight;

    private void Start()
    {
        spawnInterval = MainMenu.SpawnInterval;
        foreach (var t in enemyTypes) totalWeight += t.weight;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = spawnInterval;
        }
    }

    private void Spawn()
    {
        var type = PickType();
        Vector3 pos = GetOffScreenPosition();
        var zombie = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        zombie.name = type.name;
        zombie.tag = "Enemy";
        zombie.layer = 7; // Enemy 层，不与 Player 层物理碰撞
        zombie.transform.position = pos;
        zombie.transform.localScale = new Vector3(0.4f * type.scale, 0.5f * type.scale, 0.4f * type.scale);
        zombie.GetComponent<MeshRenderer>().material.color = type.color;

        var agent = zombie.AddComponent<NavMeshAgent>();
        agent.speed = type.speed;
        agent.stoppingDistance = 0.8f;
        agent.radius = 0.35f * type.scale;
        agent.height = 1.5f * type.scale;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 20f, NavMesh.AllAreas))
        {
            zombie.transform.position = hit.position;
            agent.Warp(hit.position);
        }

        // 碰撞体（与场景碰撞）
        var bodyCol = zombie.GetComponent<CapsuleCollider>();
        bodyCol.isTrigger = false;
        bodyCol.height = 2f;
        bodyCol.radius = 0.35f;

        var enemy = zombie.AddComponent<Enemy>();
        enemy.maxHp = type.maxHp * MainMenu.EnemyHpMul;
        enemy.xpReward = type.xpReward;
    }

    private EnemyType PickType()
    {
        float r = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var t in enemyTypes)
        {
            cumulative += t.weight;
            if (r <= cumulative) return t;
        }
        return enemyTypes[0];
    }

    private Vector3 GetOffScreenPosition()
    {
        var cam = Camera.main;
        if (cam == null) return Vector3.zero;
        var player = GameManager.Instance?.Player?.transform;
        if (player == null) return Vector3.zero;

        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, -cam.transform.position.z));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, -cam.transform.position.z));

        int edge = Random.Range(0, 4);
        float x, z;
        switch (edge)
        {
            case 0: x = Random.Range(bl.x, tr.x); z = bl.z - spawnMargin; break;
            case 1: x = Random.Range(bl.x, tr.x); z = tr.z + spawnMargin; break;
            case 2: x = bl.x - spawnMargin;        z = Random.Range(bl.z, tr.z); break;
            default: x = tr.x + spawnMargin;        z = Random.Range(bl.z, tr.z); break;
        }
        return new Vector3(x, 0f, z);
    }
}
