using UnityEngine;

/// <summary>
/// 播放请投币动画事件响应当玩家想耗币加速时.
/// </summary>
public class JiaSuCoinAniTrigger : MonoBehaviour
{
    /// <summary>
    /// 动画事件回调.
    /// </summary>
    public void OnAnimationEnvent(int index)
    {
        gameObject.SetActive(false);
    }
}