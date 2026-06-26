using UnityEngine;

public class HUD : MonoBehaviour
{
    private PlayerStats stats;
    private BombSkill bomb;
    private BigBulletSkill bigBullet;

    private void Start()
    {
        Invoke(nameof(FindPlayer), 0.3f);
    }

    private void FindPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        stats = player.GetComponent<PlayerStats>();
        bomb = player.GetComponent<BombSkill>();
        bigBullet = player.GetComponent<BigBulletSkill>();
    }

    private Texture2D _overlayTex;

    private void DrawScreenOverlay()
    {
        if (_overlayTex == null)
        {
            _overlayTex = new Texture2D(1, 1);
            _overlayTex.SetPixel(0, 0, Color.white);
            _overlayTex.Apply();
        }

        float sw = Screen.width, sh = Screen.height;
        var old = GUI.color;

        // ── 掉血 DOT → 红色边框脉冲 ──
        if (stats.HasDot)
        {
            float a = 0.3f + Mathf.Sin(Time.time * 6f) * 0.1f;
            GUI.color = new Color(1f, 0f, 0f, a);

            float bw = 12f;
            GUI.DrawTexture(new Rect(0, 0, sw, bw), _overlayTex);           // 上
            GUI.DrawTexture(new Rect(0, sh - bw, sw, bw), _overlayTex);    // 下
            GUI.DrawTexture(new Rect(0, 0, bw, sh), _overlayTex);           // 左
            GUI.DrawTexture(new Rect(sw - bw, 0, bw, sh), _overlayTex);    // 右
        }

        // ── 加速 Buff → 青色边框 ──
        if (stats.HasSpeedBuff)
        {
            GUI.color = new Color(0f, 1f, 1f, 0.25f);
            float bw = 6f;
            GUI.DrawTexture(new Rect(0, 0, sw, bw), _overlayTex);
            GUI.DrawTexture(new Rect(0, sh - bw, sw, bw), _overlayTex);
            GUI.DrawTexture(new Rect(0, 0, bw, sh), _overlayTex);
            GUI.DrawTexture(new Rect(sw - bw, 0, bw, sh), _overlayTex);
        }

        // ── 射速 Buff → 黄色边框 ──
        if (stats.HasRapidFireBuff)
        {
            GUI.color = new Color(1f, 0.9f, 0f, 0.25f);
            float bw = 6f;
            GUI.DrawTexture(new Rect(bw, bw, sw - bw * 2, bw * 0.5f), _overlayTex);
            GUI.DrawTexture(new Rect(bw, sh - bw * 1.5f, sw - bw * 2, bw * 0.5f), _overlayTex);
            GUI.DrawTexture(new Rect(bw, bw, bw * 0.5f, sh - bw * 2), _overlayTex);
            GUI.DrawTexture(new Rect(sw - bw * 1.5f, bw, bw * 0.5f, sh - bw * 2), _overlayTex);
        }

        // ── 低血量 (<25%) → 红色渐变遮罩 ──
        if (stats.HpPercent < 0.25f)
        {
            float a = (1f - stats.HpPercent * 4f) * 0.35f;
            a += Mathf.Sin(Time.time * 4f) * 0.08f;
            GUI.color = new Color(1f, 0f, 0f, a);
            GUI.DrawTexture(new Rect(0, 0, sw, sh), _overlayTex);
        }

        GUI.color = old;
    }

    private void OnGUI()
    {
        if (stats == null) return;

        DrawScreenOverlay();

        float w = 240, h = 170, x = 10, y = 10;
        GUI.Box(new Rect(x, y, w, h), "");

        var big = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold };
        var mid = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        var small = new GUIStyle(GUI.skin.label) { fontSize = 14 };

        // ── Lv ──
        GUI.Label(new Rect(x + 10, y + 8, 120, 30), $"Lv.{stats.Level}", big);

        // ── 状态图标（右侧）──
        float iconX = x + w - 60;
        if (stats.HasDot)
        {
            var s = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            s.normal.textColor = Color.red;
            GUI.Label(new Rect(iconX, y + 12, 50, 20), $"掉血 {stats.TotalDps:F0}/s", s);
        }
        if (stats.HasSpeedBuff)
        {
            var s = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            s.normal.textColor = Color.cyan;
            GUI.Label(new Rect(iconX, y + 32, 50, 20), $"加速 {stats.SpeedBuffRemaining:F1}s", s);
        }
        if (stats.HasRapidFireBuff)
        {
            var s = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            s.normal.textColor = Color.yellow;
            GUI.Label(new Rect(iconX, y + 52, 50, 20), $"射速 {stats.RapidFireRemaining:F1}s", s);
        }

        // ── HP 血条 ──
        float pct = stats.HpPercent;
        GUI.Box(new Rect(x + 10, y + 48, 200, 22), "");
        GUI.color = Color.Lerp(Color.red, Color.green, pct);
        GUI.DrawTexture(new Rect(x + 12, y + 50, 196 * pct, 18), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(x + 15, y + 48, 190, 22), $"{Mathf.CeilToInt(stats.CurrentHp)} / {Mathf.CeilToInt(stats.MaxHp)}", mid);

        // ── XP ──
        var xpS = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        xpS.normal.textColor = Color.yellow;
        GUI.Label(new Rect(x + 10, y + 74, 220, 22), $"XP: {stats.Xp} / {stats.XpToNext}", xpS);

        // ── 技能 ──
        var sk = new GUIStyle(GUI.skin.label) { fontSize = 15 };
        if (bomb != null)
        {
            float cd = bomb.CooldownPercent;
            sk.normal.textColor = cd >= 1f ? Color.green : new Color(1f, 0.5f, 0.2f);
            GUI.Label(new Rect(x + 10, y + 96, 220, 20),
                cd >= 1f ? "[1] 炸弹 就绪" : $"[1] 炸弹 {(1f - cd) * 10f:F1}s", sk);
        }
        if (bigBullet != null)
        {
            float cd = bigBullet.CooldownPercent;
            sk.normal.textColor = bigBullet.IsActive ? Color.magenta :
                cd >= 1f ? Color.green : new Color(1f, 0.5f, 0.2f);
            GUI.Label(new Rect(x + 10, y + 118, 220, 20),
                bigBullet.IsActive ? "[2] 大子弹 激活!" :
                cd >= 1f ? "[2] 大子弹 就绪" : $"[2] 大子弹 {(1f - cd) * 12f:F1}s", sk);
        }
    }
}
