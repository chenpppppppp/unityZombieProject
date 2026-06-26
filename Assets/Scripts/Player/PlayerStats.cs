using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    private AutoAttack autoAttack;
    private PlayerController controller;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public int Xp { get; private set; }
    public int Level { get; private set; } = 1;
    public int XpToNext => Level * 100;
    public float HpPercent => maxHp > 0 ? currentHp / maxHp : 0;

    // ── Buff/Debuff 状态（HUD 用）──
    public bool HasDot => dots.Count > 0;
    public float TotalDps => dots.Count > 0 ? dots[0].dps : 0f;
    public bool HasSpeedBuff => speedBuffs.Count > 0;
    public bool HasRapidFireBuff => fireBuffs.Count > 0;
    public float SpeedBuffRemaining => speedBuffs.Count > 0 ? speedBuffs[0].timer.remaining : 0f;
    public float RapidFireRemaining => fireBuffs.Count > 0 ? fireBuffs[0].timer.remaining : 0f;

    // ── Buff / Debuff ──
    private class TimedEffect { public float remaining; public GameObject source; }
    private readonly List<(TimedEffect timer, float speedMul)> speedBuffs = new();
    private readonly List<(TimedEffect timer, float fireMul)> fireBuffs = new();
    private readonly List<(TimedEffect timer, float dps)> dots = new();

    private void Awake()
    {
        currentHp = maxHp;
        autoAttack = GetComponent<AutoAttack>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // 计算总速度系数
        float totalSpeedMul = 1f;
        TickList(speedBuffs, ref totalSpeedMul, (a, b) => a * b);
        if (controller != null)
            controller.speedMultiplier = totalSpeedMul;

        // 计算总射速系数
        float totalFireMul = 1f;
        TickList(fireBuffs, ref totalFireMul, (a, b) => a * b);
        if (autoAttack != null)
            autoAttack.cooldownMultiplier = totalFireMul;

        // 计算总 DOT
        float totalDps = 0f;
        TickList(dots, ref totalDps, (a, b) => a + b);
        if (totalDps > 0f)
            TakeDamage(totalDps * Time.deltaTime);
    }

    private void TickList<T>(List<(TimedEffect timer, T val)> list, ref T accumulator,
        System.Func<T, T, T> combine)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list[i].timer.remaining -= Time.deltaTime;
            if (list[i].timer.remaining <= 0f)
                list.RemoveAt(i);
        }
        if (list.Count == 0) return; // 空列表不修改 accumulator
        bool first = true;
        foreach (var item in list)
        {
            accumulator = first ? item.val : combine(accumulator, item.val);
            first = false;
        }
    }

    // ── 公开接口 ──
    public void ApplySpeedBuff(float multiplier, float duration)
    {
        speedBuffs.Add((new TimedEffect { remaining = duration }, multiplier));
    }

    public void ApplyRapidFireBuff(float multiplier, float duration)
    {
        fireBuffs.Add((new TimedEffect { remaining = duration }, multiplier));
    }

    public void ApplyDot(float dps, float duration, GameObject source)
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            if (dots[i].timer.source == source)
            {
                dots[i] = ((new TimedEffect { remaining = duration, source = source }, dps));
                return;
            }
        }
        dots.Add((new TimedEffect { remaining = duration, source = source }, dps));
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        if (currentHp <= 0f)
        {
            currentHp = 0f;
            Time.timeScale = 0f;
            EventBus.PublishPlayerDied();
        }
    }

    public void Heal(float amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
    }

    public void GrantXp(int amount)
    {
        Xp += amount;
        while (Xp >= XpToNext) { Xp -= XpToNext; LevelUp(); }
    }

    private void LevelUp()
    {
        Level++;
        currentHp = maxHp;
        if (autoAttack != null) autoAttack.damage += 5f;
    }
}
