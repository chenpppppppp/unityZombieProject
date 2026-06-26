using UnityEngine;

public class BigBulletSkill : MonoBehaviour
{
    [SerializeField] private float cooldown = 12f;
    [SerializeField] private float duration = 2f;

    private float cdTimer;
    private float activeTimer;
    private AutoAttack autoAttack;
    public float CooldownPercent => Mathf.Clamp01(1f - cdTimer / cooldown);
    public bool IsActive => activeTimer > 0f;

    private void Awake()
    {
        autoAttack = GetComponent<AutoAttack>();
    }

    private void Update()
    {
        cdTimer -= Time.deltaTime;

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f)
            {
                activeTimer = 0f;
                if (autoAttack != null)
                    autoAttack.bigBulletMode = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && cdTimer <= 0f)
        {
            Activate();
            cdTimer = cooldown;
        }
    }

    private void Activate()
    {
        activeTimer = duration;
        if (autoAttack != null)
            autoAttack.bigBulletMode = true;
    }
}
