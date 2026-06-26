using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public static DamageNumber Instance { get; private set; }

    private class Entry { public Vector3 worldPos; public string text; public Color color; public float time; }
    private readonly List<Entry> entries = new();
    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    public void Show(Vector3 worldPos, float amount, Color color)
    {
        entries.Add(new Entry
        {
            worldPos = worldPos + Random.insideUnitSphere * 0.5f,
            text = Mathf.CeilToInt(amount).ToString(),
            color = color,
            time = 1f
        });
    }

    private void Update()
    {
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            entries[i].time -= Time.deltaTime;
            entries[i].worldPos += Vector3.up * Time.deltaTime * 1.5f; // 上浮
            if (entries[i].time <= 0f)
                entries.RemoveAt(i);
        }
    }

    private void OnGUI()
    {
        if (cam == null) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };

        foreach (var e in entries)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(e.worldPos);
            if (screenPos.z < 0) continue; // 在摄像机后面

            screenPos.y = Screen.height - screenPos.y; // 翻转 Y
            float alpha = Mathf.Clamp01(e.time / 0.3f);
            style.normal.textColor = new Color(e.color.r, e.color.g, e.color.b, alpha);

            GUI.Label(new Rect(screenPos.x - 30, screenPos.y - 10, 60, 20), e.text, style);
        }
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }
}
