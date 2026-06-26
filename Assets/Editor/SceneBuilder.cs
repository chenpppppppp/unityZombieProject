using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public class SceneBuilder : EditorWindow
{
    // ── 配色方案 ──
    private static readonly Color GroundColor     = Color.white;
    private static readonly Color FenceColor      = new Color(0.70f, 0.55f, 0.35f);
    private static readonly Color WallColor       = new Color(0.55f, 0.60f, 0.68f);
    private static readonly Color PlayerColor     = new Color(0.15f, 0.75f, 0.95f);
    private static readonly Color CrateColor      = new Color(0.72f, 0.48f, 0.22f);
    private static readonly Color BarrelColor     = new Color(0.25f, 0.55f, 0.30f);
    private static readonly Color DebrisColor     = new Color(0.60f, 0.35f, 0.35f);
    private static readonly Color PillarColor     = new Color(0.75f, 0.72f, 0.60f);
    private static readonly Color HallColor       = new Color(0.62f, 0.45f, 0.50f);

    private static readonly Color[] RoomColors = {
        new Color(0.75f, 0.55f, 0.55f),  // 粉红
        new Color(0.55f, 0.65f, 0.78f),  // 天蓝
        new Color(0.60f, 0.78f, 0.55f),  // 草绿
        new Color(0.78f, 0.75f, 0.55f),  // 暖黄
        new Color(0.70f, 0.55f, 0.75f),  // 淡紫
    };

    private const float MapMin = -14f, MapMax = 14f;
    private const float WallHeight = 2.5f;
    private const float WallThick = 0.3f;
    private static int roomColorIndex;

    [MenuItem("Game/Build MainMenu")]
    static void BuildMainMenu()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            UnityEditor.SceneManagement.NewSceneMode.Single);

        var cam = new GameObject("Main Camera");
        cam.tag = "MainCamera";
        cam.AddComponent<Camera>().orthographic = true;
        cam.GetComponent<Camera>().orthographicSize = 5;
        cam.GetComponent<Camera>().backgroundColor = new Color(0.05f, 0.08f, 0.03f);
        cam.AddComponent<AudioListener>();
        cam.transform.position = new Vector3(0, 1, -10);

        var menu = new GameObject("MainMenu");
        menu.AddComponent<MainMenu>();

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,
            "Assets/_Game/Scenes/MainMenu.unity");
        Debug.Log("MainMenu scene built!");
    }

    [MenuItem("Game/Build Scene")]
    static void BuildScene()
    {
        roomColorIndex = 0;
        ClearScene();
        CreateCamera();
        CreateLight();
        CreateGround();
        CreatePlayer();
        CreateBoundaryFence();
        CreateRooms();
        CreateObstacles();
        CreatePillars();
        CreateFloorDecals();
        CreateUI();
        CreateManagers();
        BakeNavMesh();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Scene built and colored!");
    }

    // ═══════════════════════════════════════════════
    static void ClearScene()
    {
        foreach (var go in FindObjectsOfType<GameObject>())
            Undo.DestroyObjectImmediate(go);
    }

    // ═══════════════════ 摄像机 ═══════════════════
    static void CreateCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.orthographic = false;
        cam.fieldOfView = 50;
        cam.backgroundColor = new Color(0.25f, 0.28f, 0.25f);
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 200f;
        go.transform.position = new Vector3(0, 10, -8.5f);
        go.transform.rotation = Quaternion.Euler(50, 0, 0);
        go.AddComponent<AudioListener>();
        go.AddComponent<CameraFollow>();
        Undo.RegisterCreatedObjectUndo(go, "Camera");
    }

    // ═══════════════════ 光照 ═══════════════════
    static void CreateLight()
    {
        // 环境光
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.35f, 0.35f, 0.38f);

        // 主方向光
        var go = new GameObject("Directional Light");
        var lt = go.AddComponent<Light>();
        lt.type = LightType.Directional;
        lt.color = new Color(1f, 0.98f, 0.95f);
        lt.intensity = 1.5f;
        go.transform.rotation = Quaternion.Euler(50, -30, 0);
        Undo.RegisterCreatedObjectUndo(go, "Light");

        // 补光
        var fill = new GameObject("Fill Light");
        var fl = fill.AddComponent<Light>();
        fl.type = LightType.Directional;
        fl.color = new Color(1f, 0.9f, 0.7f);
        fl.intensity = 0.7f;
        fill.transform.rotation = Quaternion.Euler(30, 120, 0);
        Undo.RegisterCreatedObjectUndo(fill, "Fill Light");
    }

    // ═══════════════════ 地面 ═══════════════════
    static void CreateGround()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = "Ground";
        go.transform.localScale = new Vector3(3, 1, 3);
        go.transform.position = Vector3.zero;
        SetColor(go, GroundColor);
        Undo.RegisterCreatedObjectUndo(go, "Ground");
    }

    // ═══════════════════ 玩家 ═══════════════════
    static void CreatePlayer()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "Player";
        go.tag = "Player";
        go.transform.position = new Vector3(0, 0.5f, 0);
        go.transform.localScale = new Vector3(0.7f, 0.5f, 0.7f);
        SetColor(go, PlayerColor);

        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.drag = 5f;

        go.AddComponent<PlayerController>();
        go.AddComponent<PlayerStats>();
        go.AddComponent<AutoAttack>();
        go.AddComponent<BombSkill>();
        go.AddComponent<BigBulletSkill>();
        Undo.RegisterCreatedObjectUndo(go, "Player");
    }

    // ═══════════════════ 外围围栏 ═══════════════════
    static void CreateBoundaryFence()
    {
        var parent = NewParent("BoundaryFence");
        CreateWall(parent, 0, MapMax, MapMax * 2, WallThick, FenceColor);
        CreateWall(parent, 0, MapMin, MapMax * 2, WallThick, FenceColor);
        CreateWall(parent, MapMin, 0, WallThick, MapMax * 2, FenceColor);
        CreateWall(parent, MapMax, 0, WallThick, MapMax * 2, FenceColor);
    }

    // ═══════════════════ 房间 ═══════════════════
    static void CreateRooms()
    {
        BuildRoom("Room1", -9, 6,  5, 5, Color.white, doorSouth: true, doorEast: true);
        BuildRoom("Room2",  4, 6,  7, 5, Color.white, doorWest: true, doorSouth: true);
        BuildRoom("Room3", -9, -9, 6, 5, Color.white, doorEast: true, doorNorth: true);
        BuildRoom("Room4",  5, -9, 5, 5, Color.white, doorNorth: true, doorWest: true);
        BuildRoom("Room5", -2.5f, 6, 4, 3, Color.white, doorSouth: true);

        var p = NewParent("Hallways");
        CreateWall(p, -2.5f, 2,  WallThick, 3.5f, HallColor);
        CreateWall(p,  2.5f, -2, WallThick, 3.5f, HallColor);
        CreateWall(p, -5,    -3, 6,         WallThick, HallColor);
    }

    static void BuildRoom(string name, float cx, float cz, float w, float d, Color innerColor,
        bool doorNorth = false, bool doorSouth = false, bool doorEast = false, bool doorWest = false)
    {
        var parent = NewParent(name);
        float hw = w / 2f, hd = d / 2f;

        // 房间地板色块
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.parent = parent.transform;
        floor.transform.position = new Vector3(cx, 0.005f, cz);
        floor.transform.localScale = new Vector3(w * 0.1f, 1, d * 0.1f);
        SetColor(floor, RoomColors[roomColorIndex % RoomColors.Length]);
        roomColorIndex++;

        // 房间墙
        var wc = WallColor;
        if (!doorNorth) CreateWall(parent, cx, cz + hd, w, WallThick, wc);
        if (!doorSouth) CreateWall(parent, cx, cz - hd, w, WallThick, wc);
        if (!doorEast)  CreateWall(parent, cx + hw, cz, WallThick, d, wc);
        if (!doorWest)  CreateWall(parent, cx - hw, cz, WallThick, d, wc);

        float doorW = 1.5f;
        if (doorNorth) CreateWall(parent, cx - hw + doorW / 2, cz + hd, w - doorW, WallThick, wc);
        if (doorSouth) CreateWall(parent, cx + hw - doorW / 2, cz - hd, w - doorW, WallThick, wc);
        if (doorEast)  CreateWall(parent, cx + hw, cz + hd - doorW / 2, WallThick, d - doorW, wc);
        if (doorWest)  CreateWall(parent, cx - hw, cz - hd + doorW / 2, WallThick, d - doorW, wc);

        // 房间内障碍物（1-3 个）
        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            float ox = cx + Random.Range(-hw + 1, hw - 1);
            float oz = cz + Random.Range(-hd + 1, hd - 1);
            if (Random.value > 0.5f)
                CreateCrate(parent, ox, oz);
            else
                CreateBarrel(parent, ox, oz);
        }
    }

    // ═══════════════════ 室外障碍物 ═══════════════════
    static void CreateObstacles()
    {
        var parent = NewParent("Obstacles");

        float[,] crates = {
            {  2, 0 }, { -3, 2 }, { 0, -5 }, { 5, -3 }, { -6, -6 },
            {  8, 0 }, { -8, -3 }, { 3, 3 }, { -4, 8 }, { 1, -8 },
            { -10, 2 }, { 10, -2 }, { -2, -10 }, { 6, 8 }, { -7, 5 },
        };
        for (int i = 0; i < crates.GetLength(0); i++)
            CreateCrate(parent, crates[i, 0], crates[i, 1]);

        float[,] barrels = {
            { 1, 2 }, { -1, -3 }, { 4, 1 }, { -5, 0 }, { 7, -6 },
            { -9, 1 }, { 3, -7 }, { -3, 10 }, { 9, 5 }, { -11, -5 },
        };
        for (int i = 0; i < barrels.GetLength(0); i++)
            CreateBarrel(parent, barrels[i, 0], barrels[i, 1]);

        float[,] debris = {
            { 0, -2 }, { 2, -4 }, { -2, 5 }, { -5, -8 }, { 8, -8 },
            { 11, 3 }, { -11, -8 }, { 1, 10 }, { 6, -5 }, { -6, 4 },
            { 10, -10 }, { -10, -11 }, { 10, 10 }, { -10, 10 }, { 0, 11 },
        };
        for (int i = 0; i < debris.GetLength(0); i++)
            CreateDebris(parent, debris[i, 0], debris[i, 1]);
    }

    // ═══════════════════ 柱子 ═══════════════════
    static void CreatePillars()
    {
        var parent = NewParent("Pillars");
        float[,] pos = {
            {  0,  4 }, {  0, -4 }, { -4,  0 }, { 4, 0 },
            { -7,  3 }, {  7,  3 }, { -7, -3 }, { 7, -3 },
            { 12,  12 }, { -12, 12 }, { 12, -12 }, { -12, -12 },
        };
        for (int i = 0; i < pos.GetLength(0); i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Pillar";
            go.transform.parent = parent.transform;
            go.transform.position = new Vector3(pos[i, 0], 1.5f, pos[i, 1]);
            go.transform.localScale = new Vector3(0.6f, 1.5f, 0.6f);
            SetColor(go, PillarColor);
        }
    }

    // ═══════════════════ 地面血迹/纹理标记 ═══════════════════
    static void CreateFloorDecals()
    {
        var parent = NewParent("FloorDecals");
        float[,] spots = {
            { 3, -1 }, { -4, -4 }, { 7, 2 }, { -8, 0 }, { 0, 8 },
            { 10, 5 }, { -10, -6 }, { 5, -9 }, { -3, 7 }, { -12, 3 },
            { 11, -7 }, { -5, -10 }, { 8, -3 }, { -1, -7 }, { 2, 5 },
        };
        for (int i = 0; i < spots.GetLength(0); i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "Decal";
            go.transform.parent = parent.transform;
            go.transform.position = new Vector3(spots[i, 0], 0.003f, spots[i, 1]);
            float s = Random.Range(0.02f, 0.06f);
            go.transform.localScale = new Vector3(s, 1, s);
            go.transform.rotation = Quaternion.Euler(90, Random.Range(0, 360), 0);
            float v = Random.Range(0.30f, 0.40f);
            SetColor(go, new Color(v, v * 0.85f, v * 0.75f));
        }
    }

    // ═══════════════════ UI ═══════════════════
    static void CreateUI()
    {
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(es, "EventSystem");

        var hudGO = new GameObject("HUD");
        hudGO.AddComponent<HUD>();
        Undo.RegisterCreatedObjectUndo(hudGO, "HUD");

        var uiGO = new GameObject("GameUI");
        uiGO.AddComponent<GameUI>();
        Undo.RegisterCreatedObjectUndo(uiGO, "GameUI");
    }

    // ═══════════════════ NavMesh ═══════════════════
    static void BakeNavMesh()
    {
        // 标记所有障碍物为导航静态
        foreach (var go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name is "Wall" or "Crate" or "Barrel" or "Debris" or "Pillar" or "Ground")
            {
                UnityEditor.StaticEditorFlags flags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(go);
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, flags | UnityEditor.StaticEditorFlags.NavigationStatic);
            }
        }

        // 烘焙
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        Debug.Log("NavMesh baked!");
    }

    // ═══════════════════ 管理器 ═══════════════════
    static void CreateManagers()
    {
        var ls = new GameObject("LayerSetup");
        ls.AddComponent<LayerSetup>();
        Undo.RegisterCreatedObjectUndo(ls, "LayerSetup");

        var dn = new GameObject("DamageNumber");
        dn.AddComponent<DamageNumber>();
        Undo.RegisterCreatedObjectUndo(dn, "DamageNumber");

        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        Undo.RegisterCreatedObjectUndo(gm, "GameManager");

        var sp = new GameObject("EnemySpawner");
        sp.AddComponent<EnemySpawner>();
        Undo.RegisterCreatedObjectUndo(sp, "EnemySpawner");
    }

    // ═══════════════════ 工具 ═══════════════════
    static void SetColor(GameObject go, Color color)
    {
        var r = go.GetComponent<MeshRenderer>();
        r.sharedMaterial = new Material(Shader.Find("Standard"));
        r.sharedMaterial.color = color;
    }

    static GameObject NewParent(string name)
    {
        var go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, name);
        return go;
    }

    static void CreateWall(GameObject parent, float cx, float cz, float w, float d, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Wall";
        go.transform.parent = parent.transform;
        go.transform.position = new Vector3(cx, WallHeight / 2f, cz);
        go.transform.localScale = new Vector3(w, WallHeight, d);
        SetColor(go, color);
    }

    static void CreateCrate(GameObject parent, float x, float z)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Crate";
        go.transform.parent = parent.transform;
        float s = Random.Range(0.6f, 1.2f);
        go.transform.position = new Vector3(x, s / 2f, z);
        go.transform.localScale = new Vector3(s, s, s);
        go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        SetColor(go, CrateColor);
    }

    static void CreateBarrel(GameObject parent, float x, float z)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Barrel";
        go.transform.parent = parent.transform;
        go.transform.position = new Vector3(x, 0.5f, z);
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SetColor(go, BarrelColor);
    }

    static void CreateDebris(GameObject parent, float x, float z)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Debris";
        go.transform.parent = parent.transform;
        float s = Random.Range(0.2f, 0.5f);
        go.transform.position = new Vector3(
            x + Random.Range(-0.3f, 0.3f), s / 2f, z + Random.Range(-0.3f, 0.3f));
        go.transform.localScale = new Vector3(s, s * 0.5f, s);
        go.transform.rotation = Random.rotation;
        SetColor(go, DebrisColor);
    }
}
