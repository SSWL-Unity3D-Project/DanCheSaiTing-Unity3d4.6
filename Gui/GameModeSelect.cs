using UnityEngine;

public class GameModeSelect : MonoBehaviour
{
    [HideInInspector]
    public NetworkRootMovie.GameMode eGameMode = NetworkRootMovie.GameMode.Null;
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
        if (steerVal > 0f && eGameMode != NetworkRootMovie.GameMode.NoLink)
        {
            Debug.Log("player select noLink...");
            if (eGameMode == NetworkRootMovie.GameMode.Null)
            {
                SetActiveStartBt(true);
            }
            eGameMode = NetworkRootMovie.GameMode.NoLink;
            NoLinkAni.SetBool("IsPlay", true);
            LinkAni.SetBool("IsPlay", false);
        }

        if (steerVal < 0f && eGameMode != NetworkRootMovie.GameMode.Link)
        {
            Debug.Log("player select Link...");
            if (eGameMode == NetworkRootMovie.GameMode.Null)
            {
                SetActiveStartBt(true);
            }
            eGameMode = NetworkRootMovie.GameMode.Link;
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