using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum ItemType { SpeedBuff, RapidFireBuff }
    public ItemType itemType;
    public float duration = 5f;
    public float multiplier = 1.5f;

    private Transform player;
    private PlayerStats stats;
    private bool canPickup;

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            stats = go.GetComponent<PlayerStats>();
        }
        Invoke(nameof(EnablePickup), 0.3f);
    }

    private void EnablePickup() => canPickup = true;

    private void Update()
    {
        if (!canPickup || player == null || stats == null) return;
        if (Vector3.Distance(transform.position, player.position) < 1.2f)
            Pickup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup) return;
        if (other.CompareTag("Player"))
            Pickup();
    }

    private void Pickup()
    {
        switch (itemType)
        {
            case ItemType.SpeedBuff:
                stats.ApplySpeedBuff(multiplier, duration);
                break;
            case ItemType.RapidFireBuff:
                stats.ApplyRapidFireBuff(multiplier, duration);
                break;
        }
        Destroy(gameObject);
    }
}
