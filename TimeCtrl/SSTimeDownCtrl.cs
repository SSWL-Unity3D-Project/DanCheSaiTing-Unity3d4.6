using UnityEngine;

public class SSTimeDownCtrl : MonoBehaviour
{
    float MaxTimeVal = 0f;
    float LastTimeVal = 0f;
    bool IsInitDownTime = false;
    bool IsTimeDownOver = false;
    float mTimeStepVal = 1f;
    bool IsDestroySelf = false;
    /// <summary>
    /// 时间每走一帧触发事件.
    /// timeCur表示当前减少到的时间.
    /// </summary>
    public delegate void TimeDownStepEvent(float timeCur);
    public event TimeDownStepEvent OnTimeDownStepEvent;
    /// <summary>
    /// 时间减少一帧时触发.
    /// </summary>
    void OnTimeDownStep(float timeCur)
    {
        if (OnTimeDownStepEvent != null)
        {
            OnTimeDownStepEvent(timeCur);
        }
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Init(float maxTime, float step = 1f)
    {
        LastTimeVal = Time.realtimeSinceStartup;
        MaxTimeVal = maxTime;
        mTimeStepVal = step;
        IsInitDownTime = true;
    }

    void Update()
    {
        if (!IsInitDownTime || IsDestroySelf)
        {
            return;
        }

        float dTime = Time.realtimeSinceStartup - LastTimeVal;
        if (dTime >= mTimeStepVal)
        {
            LastTimeVal = Time.realtimeSinceStartup;
            MaxTimeVal -= dTime;
            if (MaxTimeVal <= 0f)
            {
                if (!IsTimeDownOver)
                {
                    IsTimeDownOver = true;
                    OnTimeDownStep(0f);
                    Destroy(this);
                    return;
                }
            }
            else
            {
                //时间减少一帧.
                OnTimeDownStep(MaxTimeVal);
            }
        }
    }

    public void DestroySelf()
    {
        if (!IsDestroySelf)
        {
            IsDestroySelf = true;
            Destroy(this);
        }
    }
}