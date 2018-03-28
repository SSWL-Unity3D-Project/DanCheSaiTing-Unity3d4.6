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

    float TimeLastSelect = 0f;
    void Update()
    {
        float steerVal = pcvr.GetInstance().mGetSteer;
        if (!pcvr.bIsHardWare)
        {
            if (steerVal < 0f)
            {
                steerVal = -1f;
            }

            if (steerVal > 0f)
            {
                steerVal = 1f;
            }
        }

        if (steerVal < -0.3f || steerVal > 0.3f)
        {
            switch (eGameMode)
            {
                case NetworkRootMovie.GameMode.NoLink:
                    {
                        if (eGameMode != NetworkRootMovie.GameMode.Link && Time.realtimeSinceStartup - TimeLastSelect > 0.5f)
                        {
                            Debug.Log("player select Link...");
                            TimeLastSelect = Time.realtimeSinceStartup;
                            mLoadingCom.m_ModeSource.Play();
                            eGameMode = NetworkRootMovie.GameMode.Link;
                            ModeAni.SetBool("IsDanJi", false);
                            ModeAni.SetBool("IsLianJi", true);
                        }
                        break;
                    }
                case NetworkRootMovie.GameMode.Link:
                    {
                        if (eGameMode != NetworkRootMovie.GameMode.NoLink && Time.realtimeSinceStartup - TimeLastSelect > 0.5f)
                        {
                            Debug.Log("player select noLink...");
                            TimeLastSelect = Time.realtimeSinceStartup;
                            mLoadingCom.m_ModeSource.Play();
                            eGameMode = NetworkRootMovie.GameMode.NoLink;
                            ModeAni.SetBool("IsDanJi", true);
                            ModeAni.SetBool("IsLianJi", false);
                        }
                        break;
                    }
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