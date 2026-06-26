using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private int selectedLevel = 0;
    private bool showLevelSelect;
    private readonly string[] levels = { "简单", "普通", "困难" };
    private readonly float[] spawnIntervals = { 2.5f, 1.5f, 0.8f };
    private readonly float[] enemyHpMuls = { 0.7f, 1f, 1.5f };

    public static float SpawnInterval = 1.5f;
    public static float EnemyHpMul = 1f;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void OnGUI()
    {
        float sw = Screen.width, sh = Screen.height;
        float cx = sw / 2, cy = sh / 2;

        // 背景
        GUI.Box(new Rect(0, 0, sw, sh), "");
        var old = GUI.color;
        GUI.color = new Color(0.05f, 0.08f, 0.03f, 0.9f);
        GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
        GUI.color = old;

        // 标题
        var title = new GUIStyle(GUI.skin.label) { fontSize = 48, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        title.normal.textColor = Color.red;
        GUI.Label(new Rect(cx - 200, cy - 160, 400, 80), "丧 尸 生 存", title);

        if (!showLevelSelect)
        {
            DrawMainButtons(cx, cy);
        }
        else
        {
            DrawLevelSelect(cx, cy);
        }
    }

    private void DrawMainButtons(float cx, float cy)
    {
        if (GUI.Button(new Rect(cx - 100, cy - 20, 200, 50), "开始游戏"))
        {
            showLevelSelect = true;
        }
        if (GUI.Button(new Rect(cx - 100, cy + 50, 200, 50), "退出游戏"))
        {
            Application.Quit();
        }
    }

    private void DrawLevelSelect(float cx, float cy)
    {
        var lbl = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
        lbl.normal.textColor = Color.white;
        GUI.Label(new Rect(cx - 150, cy - 80, 300, 40), "选择关卡", lbl);

        for (int i = 0; i < levels.Length; i++)
        {
            if (GUI.Button(new Rect(cx - 80, cy - 30 + i * 55, 160, 45), levels[i]))
            {
                SpawnInterval = spawnIntervals[i];
                EnemyHpMul = enemyHpMuls[i];
                SceneManager.LoadScene("SampleScene");
            }
        }

        if (GUI.Button(new Rect(cx - 60, cy + 140, 120, 35), "返回"))
        {
            showLevelSelect = false;
        }
    }
}
