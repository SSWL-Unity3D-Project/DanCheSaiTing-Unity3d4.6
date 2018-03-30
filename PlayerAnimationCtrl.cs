using UnityEngine;

public class PlayerAnimationCtrl : MonoBehaviour
{
    /// <summary>
    /// 主角主控制脚本.
    /// </summary>
    public PlayerController mPlayerCtrl;
    /// <summary>
    /// 动画控制组件.
    /// </summary>
    public Animator mAnimator;

    /// <summary>
    /// 被子弹攻击动画事件响应.
    /// </summary>
    public void OnAmmoHitAnimationEnvent()
    {
        if (mPlayerCtrl.IsNetControlPort)
		{
			Debug.Log("OnAmmoHitAnimationEnvent...");
            mPlayerCtrl.ResetIsAmmoHitPlayer();
        }
    }

    /// <summary>
    /// 播放被子弹攻击的动画.
    /// </summary>
    public void PlayAmmoHitAnimation()
    {
        mAnimator.SetTrigger("IsAmmoHit");
    }
}