using UnityEngine;

public class UIAniTimeScale : MonoBehaviour
{
    public Animator mAnimator;
    void Update ()
    {
        if (mAnimator != null)
        {
            float sp = -Time.timeScale + 2f;
            sp = Mathf.Clamp(sp, 0.5f, 1.5f);
            mAnimator.speed = sp;
        }
    }
}