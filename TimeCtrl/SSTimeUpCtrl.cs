using UnityEngine;

public class SSTimeUpCtrl : MonoBehaviour
{
    float MaxTimeVal = 0f;
    float LastTimeVal = 0f;
    bool IsInitUpTime = false;
    bool IsTimeUpOver = false;
    /// <summary>
    /// 时间上升结束事件.
    /// </summary>
    public delegate void TimeUpOverEvent();
    public event TimeUpOverEvent OnTimeUpOverEvent;
    public void OnTimeUpOver()
    {
        if (OnTimeUpOverEvent != null)
        {
            OnTimeUpOverEvent();
        }
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Init(float maxTime)
    {
        MaxTimeVal = maxTime;
        LastTimeVal = Time.realtimeSinceStartup;
        IsInitUpTime = true;
    }
    
    void Update()
    {
        if (!IsInitUpTime)
        {
            return;
        }
        
        if (Time.realtimeSinceStartup - LastTimeVal >= MaxTimeVal)
        {
            if (!IsTimeUpOver)
            {
                IsTimeUpOver = true;
                OnTimeUpOver();
                Destroy(this);
                return;
            }
        }
    }
}