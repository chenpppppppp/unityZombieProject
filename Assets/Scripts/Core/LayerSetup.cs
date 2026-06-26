using UnityEngine;

public class LayerSetup : MonoBehaviour
{
    private void Awake()
    {
        // 禁用 Player(6) 和 Enemy(7) 之间的物理碰撞
        Physics.IgnoreLayerCollision(6, 7, true);
    }
}
