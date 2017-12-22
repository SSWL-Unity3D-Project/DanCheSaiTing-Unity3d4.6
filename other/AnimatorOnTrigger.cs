using UnityEngine;
using System.Collections;

/// <summary>
/// 动画控制器时间响应脚本.
/// </summary>
public class AnimatorOnTrigger : MonoBehaviour
{
    public enum AniType
    {
        Null,
        JiaSuFengShan,
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
        }
    }
}