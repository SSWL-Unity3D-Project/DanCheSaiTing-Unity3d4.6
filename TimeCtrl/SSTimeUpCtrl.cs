using UnityEngine;

public class SSTimeUpCtrl : MonoBehaviour
{
    float MaxTimeVal = 0f;
    float CurTimeVal = 0f;
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
        IsInitUpTime = true;
    }
    
    void Update()
    {
        if (!IsInitUpTime)
        {
            return;
        }

        CurTimeVal += Time.deltaTime;
        if (CurTimeVal >= MaxTimeVal)
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