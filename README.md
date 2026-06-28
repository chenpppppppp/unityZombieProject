# 🧟 丧尸生存 (Zombie Survival)

> Unity 2022.3 | 2D 俯视角 | 丧尸生存射击游戏

一款 2D 俯视角丧尸生存游戏原型。玩家在封闭地图中移动，自动攻击追踪而来的丧尸，通过 Buff/Debuff/技能/升级系统构建丰富的玩法深度。

---


## 🎯 核心玩法

- **移动射击**：WASD 移动，自动锁定并攻击范围内的丧尸
- **三种丧尸**：普通（红色）、快速（橙色）、巨型（紫色），各有不同属性
- **两个主动技能**：炸弹（范围爆炸）和大子弹（穿透 AoE）
- **Buff 系统**：击杀丧尸掉落加速/射速 Buff，可叠加
- **升级成长**：经验值 → 等级提升 → 攻击力+5 & 满血回复
- **三档难度**：简单 / 普通 / 困难，影响生成速度和丧尸血量

---

## 🕹️ 操作说明

| 按键 | 功能 |
|------|------|
| `W` `A` `S` `D` | 移动 |
| `1` | 炸弹技能（CD 10s，4s 引信后爆炸） |
| `2` | 大子弹技能（CD 12s，持续 2s，命中后 AoE） |
| `ESC` | 暂停 / 返回菜单 |

---

## 🧬 丧尸类型

| 类型 | 颜色 | 速度 | 血量 | 体型 | 经验 | 生成权重 |
|------|------|------|------|------|------|----------|
| 普通丧尸 | 🔴 红 | 3 | 45 | 1× | 20 | 53% |
| 快速丧尸 | 🟠 橙 | 5.5 | 25 | 0.8× | 15 | 32% |
| 巨型丧尸 | 🟣 紫 | 2 | 120 | 1.8× | 50 | 15% |

---

## ⚔️ 技能系统

| 技能 | 按键 | CD | 持续时间 | 效果 |
|------|------|-----|----------|------|
| 💣 炸弹 | `1` | 10s | 4s 引信 | 5 单位范围 45 点伤害 |
| 🔮 大子弹 | `2` | 12s | 2s | 子弹变大 + 命中后 3 单位 AoE |

---

## 🧪 Buff / Debuff

| 类型 | 效果 | 持续 | 叠加 |
|------|------|------|------|
| 🩵 加速 Buff | 移速 ×1.5 | 5s | 可叠加（上限 ×3） |
| 💛 射速 Buff | 射速 ×1.5 | 5s | — |
| 🟥 DOT Debuff | 每秒 3 血 | 5s | 触碰刷新 |

---

## 📈 升级系统

- 击杀丧尸直接获得经验（普通 20 / 快速 15 / 巨型 50）
- 升级阈值 = 等级 × 100
- 每次升级：**攻击力 +5**，**血量回满**

---

## 🎚️ 难度选择

| 难度 | 生成间隔 | 血量倍率 |
|------|---------|---------|
| 简单 | 2.5s | ×0.7 |
| 普通 | 1.5s | ×1.0 |
| 困难 | 0.8s | ×1.5 |

---

## 🏗️ 项目结构

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager（全局单例）、LayerSetup（物理层配置）
│   ├── Events/         # EventBus（全局事件系统）
│   ├── Player/         # PlayerController（移动）、PlayerStats（属性/Buff）
│   ├── Camera/         # CameraFollow（Lerp 平滑跟随）
│   ├── Enemies/        # Enemy（丧尸 AI）、EnemySpawner（屏幕外生成）
│   ├── Weapons/        # AutoAttack、Projectile、BombSkill、BigBulletSkill
│   ├── Items/          # DropItem（击杀掉落 Buff 拾取物）
│   └── UI/             # HUD、GameUI（暂停/死亡画面）、DamageNumber、MainMenu
├── Editor/
│   └── SceneBuilder.cs # 一键构建场景 + NavMesh 烘焙
├── _Game/
│   ├── Scenes/         # MainMenu.unity、游戏场景
│   ├── Prefabs/        # 预留
│   ├── ScriptableObjects/ # 预留
│   └── Imported/       # 外部素材
└── Zombie Leg/         # 丧尸模型素材（待集成）
```

---

## 🚀 快速开始

### 环境要求
- **Unity 2022.3.62f3c1** 或更高版本
- Windows / macOS

### 运行步骤

1. **Clone 仓库**
   ```bash
   git clone https://github.com/chenpppppppp/unityZombieProject.git
   ```

2. **用 Unity Hub 打开项目**
   - 添加项目 → 选择 clone 的文件夹
   - 等待 Unity 导入资源（首次打开会自动重建 Library）

3. **生成场景**（如场景为空）
   - 打开 Unity 编辑器菜单 → **Game → Build Scene**
   - 这会自动创建游戏场景并烘焙 NavMesh

4. **开始游戏**
   - 打开 `MainMenu` 场景
   - 点击 Play 运行

---

## ⚙️ 技术要点

| 技术 | 说明 |
|------|------|
| 渲染管线 | Built-in Render Pipeline |
| 物理 | 2D 伪 3D（Rigidbody + FreezePositionY） |
| 寻路 | Unity NavMesh + NavMeshAgent |
| 层级隔离 | Player (6) / Enemy (7)，物理碰撞隔离 |
| UI | OnGUI（即时模式） |
| 丧尸生成 | ViewportToWorldPoint 计算屏幕外坐标 |
| 摄像机 | LateUpdate 中 Lerp 平滑跟随玩家 |

---

## 📄 许可证

本项目仅供学习和交流使用。

---

*Made with Unity & ❤️*
