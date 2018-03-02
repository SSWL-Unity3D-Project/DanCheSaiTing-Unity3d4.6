using UnityEngine;

public class GameModeSelect : MonoBehaviour
{
    public enum GameMode
    {
        Null,
        Link,
        NoLink,
    }
    [HideInInspector]
    public GameMode eGameMode = GameMode.Null;
    /// <summary>
    /// 联机模式UI动画.
    /// </summary>
    public Animator LinkAni;
    /// <summary>
    /// 单机模式UI动画.
    /// </summary>
    public Animator NoLinkAni;
    /// <summary>
    /// 开始按键UI.
    /// </summary>
    public GameObject StartBtObj;

    public void Init()
    {
        SetActiveStartBt(false);
    }

    void Update()
    {
        float steerVal = pcvr.GetInstance().mGetSteer;
        if (steerVal > 0f && eGameMode != GameMode.NoLink)
        {
            Debug.Log("player select noLink...");
            if (eGameMode == GameMode.Null)
            {
                SetActiveStartBt(true);
            }
            eGameMode = GameMode.NoLink;
            NoLinkAni.SetBool("IsPlay", true);
            LinkAni.SetBool("IsPlay", false);
        }

        if (steerVal < 0f && eGameMode != GameMode.Link)
        {
            Debug.Log("player select Link...");
            if (eGameMode == GameMode.Null)
            {
                SetActiveStartBt(true);
            }
            eGameMode = GameMode.Link;
            NoLinkAni.SetBool("IsPlay", false);
            LinkAni.SetBool("IsPlay", true);
        }
    }

    /// <summary>
    /// 设置开始按键.
    /// </summary>
    void SetActiveStartBt(bool isActive)
    {
        StartBtObj.SetActive(isActive);
    }

    public void HiddenSelf()
    {
        gameObject.SetActive(false);
    }
}