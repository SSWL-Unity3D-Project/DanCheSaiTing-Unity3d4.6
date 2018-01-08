using UnityEngine;
using System.Collections;

/// <summary>
/// 动画控制器时间响应脚本.
/// </summary>
public class AnimatorOnTrigger : MonoBehaviour
{
    /// <summary>
    /// JiaSuFengShan 加速风扇
    /// ShuangYiFeiJi 双翼飞机
    /// </summary>
    public enum AniType
    {
        Null,
        JiaSuFengShan,
        ShuangYiFeiJi,
    }
    public AniType AniState = AniType.Null;
    public void OnAnimatorTrigger(int index)
    {
        switch (AniState)
        {
            case AniType.JiaSuFengShan:
                {
                    PlayerController.GetInstance().OnDaoJuFengKuangAniOver();
                    break;
                }
            case AniType.ShuangYiFeiJi:
                {
                    PlayerController.GetInstance().OnDaoJuShaungYiFeiJiAniOver();
                    break;
                }
        }
    }
}