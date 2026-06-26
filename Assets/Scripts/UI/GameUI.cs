using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    private bool paused;
    private bool playerDead;

    private void Start()
    {
        EventBus.OnPlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        EventBus.OnPlayerDied -= OnPlayerDied;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playerDead) return;
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
        }
    }

    private void OnPlayerDied()
    {
        playerDead = true;
    }

    private void OnGUI()
    {
        if (paused)
            DrawPauseScreen();

        if (playerDead)
            DrawDeathScreen();
    }

    private void DrawPauseScreen()
    {
        float sw = Screen.width, sh = Screen.height;
        GUI.Box(new Rect(sw / 2 - 120, sh / 2 - 60, 240, 120), "暂停");

        GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        GUI.Label(new Rect(sw / 2 - 100, sh / 2 - 50, 200, 40), "游戏暂停", big);

        if (GUI.Button(new Rect(sw / 2 - 80, sh / 2, 160, 30), "继续游戏 (ESC)"))
        {
            paused = false;
            Time.timeScale = 1f;
        }

        if (GUI.Button(new Rect(sw / 2 - 80, sh / 2 + 35, 160, 30), "重新开始"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void DrawDeathScreen()
    {
        float sw = Screen.width, sh = Screen.height;

        // 暗红遮罩
        var old = GUI.color;
        GUI.color = new Color(0.8f, 0f, 0f, 0.5f);
        GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
        GUI.color = old;

        GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = 40, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        big.normal.textColor = Color.white;
        GUI.Label(new Rect(sw / 2 - 150, sh / 2 - 80, 300, 60), "你死了", big);

        GUIStyle mid = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
        mid.normal.textColor = Color.yellow;
        var stats = GameManager.Instance?.Player?.GetComponent<PlayerStats>();
        if (stats != null)
            GUI.Label(new Rect(sw / 2 - 100, sh / 2, 200, 30), $"Lv.{stats.Level}  击杀经验:{stats.Xp}", mid);

        if (GUI.Button(new Rect(sw / 2 - 80, sh / 2 + 50, 160, 40), "重新开始"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
