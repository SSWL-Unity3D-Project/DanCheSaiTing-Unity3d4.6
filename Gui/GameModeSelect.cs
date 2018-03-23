using UnityEngine;

public class GameModeSelect : MonoBehaviour
{
    [HideInInspector]
	public NetworkRootMovie.GameMode eGameMode = NetworkRootMovie.GameMode.NoLink;
    /// <summary>
    /// 模式UI动画.
    /// </summary>
    public Animator ModeAni;
    /// <summary>
    /// 开始按键UI.
    /// </summary>
    public GameObject StartBtObj;
    /// <summary>
    /// 循环动画UI总控.
    /// </summary>
    Loading mLoadingCom;

    public void Init(Loading loadingCom)
    {
		eGameMode = NetworkRootMovie.GameMode.NoLink;
        mLoadingCom = loadingCom;
        SetActiveStartBt(true);
    }

    void Update()
    {
        float steerVal = pcvr.GetInstance().mGetSteer;
        if (steerVal < -0.3f)
        {
            if (eGameMode != NetworkRootMovie.GameMode.NoLink)
            {
                Debug.Log("player select noLink...");
                mLoadingCom.m_ModeSource.Play();
                eGameMode = NetworkRootMovie.GameMode.NoLink;
                ModeAni.SetBool("IsDanJi", true);
                ModeAni.SetBool("IsLianJi", false);
            }
        }

        if (steerVal > 0.3f)
        {
            if (eGameMode != NetworkRootMovie.GameMode.Link)
            {
                Debug.Log("player select Link...");
                mLoadingCom.m_ModeSource.Play();
                eGameMode = NetworkRootMovie.GameMode.Link;
                ModeAni.SetBool("IsDanJi", false);
                ModeAni.SetBool("IsLianJi", true);
            }
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