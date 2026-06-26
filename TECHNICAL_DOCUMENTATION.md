# 丧尸生存游戏 — 技术文档

> Unity 2022.3.62f3c1 | Built-in RP | 2D 俯视角 | 2026-06-25

---

## 1. 项目概览

丧尸生存游戏原型，玩家 WASD 移动，自动攻击丧尸，通过 Buff/Debuff/AoE/Skill 系统构建玩法深度。

### 核心玩法
- 玩家在封闭地图内移动，丧尸从屏幕外生成追踪
- 自动索敌攻击，击杀掉落 Buff
- 两个主动技能（炸弹/大子弹）
- 升级系统（经验 → 等级 → 伤害提升）
- 三档难度选择

---

## 2. 目录结构

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, LayerSetup
│   ├── Events/         # EventBus (全局事件)
│   ├── Player/         # PlayerController, PlayerStats
│   ├── Camera/         # CameraFollow (Lerp跟随)
│   ├── Enemies/        # Enemy, EnemySpawner
│   ├── Weapons/        # AutoAttack, Projectile, BombSkill, BigBulletSkill
│   ├── Items/          # DropItem (拾取Buff)
│   ├── Buffs/          # BuffData, BuffInstance, StatModifier, StatType
│   └── UI/             # HUD, GameUI (暂停/死亡), DamageNumber, MainMenu
├── Editor/
│   └── SceneBuilder.cs # 编辑器菜单：一键生成场景 + NavMesh烘焙
├── _Game/
│   ├── Scenes/         # MainMenu.unity, 游戏场景
│   ├── Prefabs/        # 预留
│   ├── ScriptableObjects/ # 预留
│   └── Imported/       # 外部素材
└── Zombie Leg/         # 丧尸模型素材（待集成）
```

---

## 3. 游戏流程

```
MainMenu → 选难度 → 游戏场景
                      ├── 击杀丧尸 → XP → 升级(伤害+5,回满血)
                      ├── 掉落 Buff(加速/射速)
                      ├── 技能1 炸弹(范围伤害)
                      ├── 技能2 大子弹(AoE)
                      ├── 死亡 → 画面冻结 → 重新开始
                      └── ESC → 暂停菜单
```

---

## 4. 核心系统

### 4.1 移动与摄像机
- **PlayerController**: Rigidbody + velocity 驱动，FreezePositionY 防漂浮
- **CameraFollow**: Vector3.Lerp 平滑跟随，LateUpdate 执行
- **Player Layer(6)** ↔ **Enemy Layer(7)** 物理断开，玩家撞墙但不被丧尸推动

### 4.2 丧尸系统
| 类型 | 颜色 | 速度 | 血量 | 体型 | 经验 | 权重 |
|------|------|------|------|------|------|------|
| 普通 | 红 | 3 | 45 | 1x | 20 | 53% |
| 快速 | 橙 | 5.5 | 25 | 0.8x | 15 | 32% |
| 巨型 | 紫 | 2 | 120 | 1.8x | 50 | 15% |

- **生成**: ViewportToWorldPoint 计算屏幕四边，+5margin 屏幕外生成
- **寻路**: NavMeshAgent + NavMesh（SceneBuilder 自动烘焙）
- **碰撞**: CapsuleCollider（非Trigger），与场景物体正常碰撞

### 4.3 攻击系统
- **AutoAttack**: 自动找范围内最近 Enemy，按冷却发射子弹
- **Projectile**: 球体 + Rigidbody 飞向目标，碰撞 Enemy 造成伤害
- **大子弹模式**: 子弹变大+品红色，命中 AoE 3单位范围

### 4.4 Buff/Debuff 系统
- **加速 Buff**（青色）: 移速×1.5，持续5s，可叠加（上限3倍）
- **射速 Buff**（黄色）: 射速×1.5，持续5s
- **DOT Debuff**（丧尸触碰）: 每秒3血，持续5s，同源刷新

### 4.5 技能系统
| 技能 | 按键 | CD | 持续 | 效果 |
|------|------|-----|------|------|
| 炸弹 | 1 | 10s | 4s引信 | 5单位范围45伤害 |
| 大子弹 | 2 | 12s | 2s | 子弹变大+命中AoE |

### 4.6 升级系统
- 击杀丧尸直接获得经验（普通20/快速15/巨型50）
- 升级阈值 = 等级 × 100
- 升级效果: 伤害+5，血量回满

### 4.7 难度系统
| 难度 | 生成间隔 | 血量倍率 |
|------|---------|---------|
| 简单 | 2.5s | ×0.7 |
| 普通 | 1.5s | ×1.0 |
| 困难 | 0.8s | ×1.5 |

---

## 5. 事件系统

```csharp
EventBus.OnPlayerDied  // 玩家死亡 → GameUI弹窗 + 时间停止
EventBus.OnGamePaused   // 暂停
EventBus.OnGameResumed  // 恢复
```

---

## 6. 对象池

ObjectPool 脚本已准备但当前未启用（丧尸直接 CreatePrimitive 生成数量不大）。

---

## 7. 物理层配置

| Layer | ID | 用途 |
|-------|-----|------|
| Player | 6 | 玩家，与 Default 碰撞，与 Enemy 不碰撞 |
| Enemy | 7 | 丧尸，与 Default 碰撞，与 Player 不碰撞 |

`LayerSetup.Awake()` 调用 `Physics.IgnoreLayerCollision(6, 7, true)`。

---

## 8. UI 系统

| 组件 | 渲染方式 | 内容 |
|------|---------|------|
| HUD | OnGUI | 等级、血条、XP进度、技能CD、Buff状态 |
| GameUI | OnGUI | 暂停菜单(ESC)、死亡画面 |
| DamageNumber | OnGUI | WorldToScreen 伤害跳字 |
| MainMenu | OnGUI | 标题、开始游戏、难度选择、退出 |

---

## 9. 场景构建

**不要手动修改场景！** 使用编辑器菜单：

- **Game → Build MainMenu** — 生成主菜单场景
- **Game → Build Scene** — 生成游戏场景（含物体+NavMesh烘焙）

---

## 10. 已知问题与后续计划

| 状态 | 事项 |
|------|------|
| ❌ | 丧尸模型未集成（Avatar 缺失，待手动配置 Rig） |
| ❌ | 音效未添加 |
| ❌ | 摇杆输入 |
| ❌ | 掉落物磁铁吸附 |
| ✅ | 核心玩法、Buff/Debuff/AoE/技能/升级/难度 |

---

## 11. 如何添加新内容

### 添加新丧尸类型
`EnemySpawner.enemyTypes` 数组中加一项即可。

### 添加新 Buff
1. `DropItem.ItemType` 枚举加一项
2. `PlayerStats` 加 ApplyXxxBuff 方法
3. `Enemy.SpawnDrops()` 加掉落逻辑

### 添加新技能
1. 新建 Skill 脚本（参考 BombSkill）
2. `HUD` 加对应显示
3. `SceneBuilder.CreatePlayer()` 挂载
