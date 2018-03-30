using UnityEngine;

public class JinYongAmmoUI : MonoBehaviour
{
    /// <summary>
    /// 响应动画触发器事件消息.
    /// </summary>
    public void OnAnimationEnvent()
    {
        Destroy(gameObject);
    }
}