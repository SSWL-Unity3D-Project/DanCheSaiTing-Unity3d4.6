using UnityEngine;

public class GameLinkPlayer : MonoBehaviour
{
    /// <summary>
    /// 其它端口看到的玩家信息.
    /// </summary>
    public GameObject[] mPlayerOtherMeshs = new GameObject[4];
    Animator[] mPlayerOtherAnimators = new Animator[4];
    /// <summary>
    /// 玩家自己看到的mesh信息列表.
    /// </summary>
    public GameObject[] mPlayerMeshArray = new GameObject[4];
    Animator[] mPlayerAnimators = new Animator[4];
    /// <summary>
    /// 背景Material列表.
    /// </summary>
    public Material[] mBJMaterials = new Material[4];
    /// <summary>
    /// 背景MeshRenderer.
    /// </summary>
    public MeshRenderer mBJMeshRenderer;
    /// <summary>
    /// 服务器正在维护UI提示.
    /// </summary>
    public GameObject UISeverWeiHu;
    /// <summary>
    /// 正在创建服务器UI.
    /// </summary>
    public GameObject UICreateSever;
    /// <summary>
    /// 联机玩家昵称列表父级.
    /// </summary>
    public GameObject LinkNameParent;
    /// <summary>
    /// 开始按键UI.
    /// </summary>
    public GameObject StartBtObj;
    /// <summary>
    /// 联机玩家UI列表.
    /// </summary>
    //public UITexture[] PlayerUITextureArray = new UITexture[4];
    /// <summary>
    /// 玩家昵称UI.
    /// </summary>
    //public Texture[] PlayerTexureArray = new Texture[4];
    /// <summary>
    /// Ai昵称UI
    /// </summary>
    //public Texture[] AiTexureArray = new Texture[4];
    bool IsClickStartBt = false;
    /// <summary>
    /// 记录开始按键时间.
    /// </summary>
    float TimeLastStartBt = 0f;
    Loading mLoadingCom;
    public void Init(Loading loadingScript)
    {
        mLoadingCom = loadingScript;
        for (int i = 0; i < mPlayerOtherMeshs.Length; i++)
        {
            if (mPlayerOtherMeshs.Length > i && mPlayerOtherMeshs[i] != null)
            {
                mPlayerOtherAnimators[i] = mPlayerOtherMeshs[i].GetComponent<Animator>();
            }
        }

        for (int i = 0; i < mPlayerMeshArray.Length; i++)
        {
            if (mPlayerMeshArray.Length > i && mPlayerMeshArray[i] != null)
            {
                mPlayerAnimators[i] = mPlayerMeshArray[i].GetComponent<Animator>();
            }
        }

        SetActiveLinkNameParent(false);
        SetAcitveStartBt(false);
        //SetPlayerUITexture(0);
        SetPlayerUIMesh(0);
        SetActiveUICreateSever(true);
        SetActiveUISeverWeiHu(false);
        NetworkEvent.GetInstance().OnFailedToConnectToMasterServerEvent += OnFailedToConnectToMasterServerEvent;
    }

    /// <summary>
    /// 玩家索引.
    /// </summary>
    int IndexPlayer = 0;
    /// <summary>
    /// 当前联机到服务器的玩家数.
    /// </summary>
    int PlayerLinkServerCount = 0;
    void Update()
    {
        if (IsFailedToConnectMasterServer)
        {
            //链接主服务器失败(主服务器游戏可能没有打开).
            return;
        }

        if (Time.frameCount % 30 == 0)
        {
            if (!StartBtObj.activeSelf && !IsClickStartBt)
            {
                if (NetworkServerNet.GetInstance().mRequestMasterServer.GetMovieMasterServerNum() == 1
                    && NetworkServerNet.GetInstance().mRequestMasterServer.ServerIp == Network.player.ipAddress
                    && Network.peerType == NetworkPeerType.Server)
                {
                    TimeLastStartBt += (Time.deltaTime * 30);
                    if (TimeLastStartBt >= 3f)
                    {
                        //循环动画场景主服务器有且只有1个动画服务端时,才允许显示开始按键.
                        //SetAcitveStartBt(true);
                        //产生选择游戏场景UI.
                        mLoadingCom.SpawnLevelSelectUI();
                        //隐藏创建服务器的UI提示.
                        SetActiveUICreateSever(false);
                    }
                }
                else
                {
                    TimeLastStartBt = 0f;
                }
            }

            if (NetworkServerNet.GetInstance() != null)
            {
                if (IndexPlayer != NetworkServerNet.GetInstance().IndexSpawnPlayer)
                {
                    IndexPlayer = NetworkServerNet.GetInstance().IndexSpawnPlayer;
                    Debug.Log("GameLinkPlayer::update -> IndexPlayer == " + IndexPlayer);
                    if (!LinkNameParent.activeInHierarchy)
                    {
                        SetActiveLinkNameParent(true);
                    }
                    //ChangeUINameScale(IndexPlayer);
                    SetUIPanelBeiJing(IndexPlayer);
                }

                if (PlayerLinkServerCount != NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie)
                {
                    PlayerLinkServerCount = NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie;
                    Debug.Log("GameLinkPlayer::update -> PlayerLinkServerCount == " + PlayerLinkServerCount);
                    //SetPlayerUITexture(PlayerLinkServerCount);
                    SetPlayerUIMesh(PlayerLinkServerCount);
                }
            }
        }
    }

    public void SetAcitveStartBt(bool isActive)
    {
        if (isActive
            && mLoadingCom.mLevelSelectUI != null
            && mLoadingCom.mLevelSelectUI.gameObject.activeInHierarchy)
        {
            return;
        }
        StartBtObj.SetActive(isActive);
    }

    public void SetActiveLinkNameParent(bool isActive)
    {
        if (isActive
            && mLoadingCom.mLevelSelectUI != null
            && mLoadingCom.mLevelSelectUI.gameObject.activeInHierarchy)
        {
            return;
        }

        LinkNameParent.SetActive(isActive);
        if (isActive)
        {
            //隐藏正在创建服务器的UI提示.
            SetActiveUICreateSever(false);
            /*if (Network.peerType == NetworkPeerType.Server)
            {
                SetPlayerUIMesh(PlayerLinkServerCount);
            }*/
        }
    }

    void SetActiveUICreateSever(bool isActive)
    {
        if (UICreateSever != null)
        {
            UICreateSever.SetActive(isActive);
        }
    }

    void SetActiveUISeverWeiHu(bool isActive)
    {
        if (UISeverWeiHu != null)
        {
            UISeverWeiHu.SetActive(isActive);
        }
    }

    /// <summary>
    /// 改变玩家昵称UI大小.
    /// </summary>
    //public void ChangeUINameScale(int index)
    //{
    //    for (int i = 0; i < PlayerUITextureArray.Length; i++)
    //    {
    //        if (index == i)
    //        {
    //            PlayerUITextureArray[i].transform.localScale = new Vector3(1.2f, 1.2f, 1f);
    //        }
    //        else
    //        {
    //            PlayerUITextureArray[i].transform.localScale = Vector3.one;
    //        }
    //    }
    //}

    /// <summary>
    /// 设置联机玩家UI信息.
    /// indexVal [0, 3].
    /// </summary>
    //void SetPlayerUITexture(int indexVal)
    //{
    //    for (int i = 0; i < PlayerUITextureArray.Length; i++)
    //    {
    //        if (i > indexVal)
    //        {
    //            PlayerUITextureArray[i].mainTexture = AiTexureArray[i];
    //        }
    //        else
    //        {
    //            PlayerUITextureArray[i].mainTexture = PlayerTexureArray[i];
    //        }
    //    }
    //}

    /// <summary>
    /// 设置界面背景材质.
    /// index[0,3]
    /// </summary>
    public void SetUIPanelBeiJing(int index)
	{
		Debug.Log("SetUIPanelBeiJing -> index == " + index);
        mBJMeshRenderer.material = mBJMaterials[index];
        for (int i = 0; i < mBJMaterials.Length; i++)
        {
            mPlayerMeshArray[i].SetActive(index == i ? true : false);
            if (i == index)
            {
                if (mPlayerAnimators[i] != null)
                {
                    mPlayerAnimators[i].ResetTrigger("IsPlay");
                    mPlayerAnimators[i].SetTrigger("IsPlay");
                }
            }

            if (i < index)
            {
                mPlayerOtherMeshs[i].SetActive(true);
                if (mPlayerOtherAnimators[i] != null)
                {
                    mPlayerOtherAnimators[i].ResetTrigger("IsPlay");
                    mPlayerOtherAnimators[i].SetTrigger("IsPlay");
                }
            }

            if (i >= index)
            {
                mPlayerOtherMeshs[i].SetActive(false);
            }
        }

		if (Network.peerType == NetworkPeerType.Server)
		{
			SetPlayerUIMesh(PlayerLinkServerCount);
		}
    }

    /// <summary>
    /// 设置联机玩家UI信息.
    /// indexVal [0, 3].
    /// </summary>
    void SetPlayerUIMesh(int indexVal)
    {
		Debug.Log("SetPlayerUIMesh -> indexVal == " + indexVal + ", IndexPlayer == " + IndexPlayer);
        for (int i = 0; i < mBJMaterials.Length; i++)
        {
            if (i > indexVal)
            {
                mPlayerOtherMeshs[i].SetActive(false);
            }
            else
            {
                if (i != IndexPlayer)
                {
                    mPlayerOtherMeshs[i].SetActive(true);
                    if (mPlayerOtherAnimators[i] != null)
                    {
                        mPlayerOtherAnimators[i].ResetTrigger("IsPlay");
                        mPlayerOtherAnimators[i].SetTrigger("IsPlay");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 点击开始按键.
    /// </summary>
    public void OnClickStartBt()
    {
        if (!IsClickStartBt)
        {
            IsClickStartBt = true;
            SetAcitveStartBt(false);
            HiddenSelf();
        }
    }
    
    public void HiddenSelf()
    {
        gameObject.SetActive(false);
    }

    bool IsFailedToConnectMasterServer = false;
    SSTimeUpCtrl mSSTimeUpCom;
    void OnFailedToConnectToMasterServerEvent()
    {
        if (!IsFailedToConnectMasterServer)
        {
            Debug.Log("GameLinkPlayer -> OnFailedToConnectToMasterServerEvent...");
            IsFailedToConnectMasterServer = true;
            //因为网络故障强制切换为单机模式.
            NetworkRootMovie.GetInstance().ePlayerSelectGameMode = NetworkRootMovie.GameMode.NoLink;
            SetActiveUISeverWeiHu(true);
            SetActiveUICreateSever(false);
            SetActiveLinkNameParent(false);
            SetAcitveStartBt(false);
            mSSTimeUpCom = gameObject.AddComponent<SSTimeUpCtrl>();
            mSSTimeUpCom.OnTimeUpOverEvent += OnTimeUpOverServerWeiHuEvent;
            mSSTimeUpCom.Init(5f);
        }
    }

    void OnTimeUpOverServerWeiHuEvent()
    {
        if (mLoadingCom.mGameModeSelect.eGameMode == NetworkRootMovie.GameMode.Link)
        {
            Debug.Log("OnTimeUpOverServerWeiHuEvent...");
            mLoadingCom.mGameModeSelect.eGameMode = NetworkRootMovie.GameMode.NoLink; //强制修改为单机模式.
            if (mLoadingCom.mLevelSelectUI != null)
            {
                Debug.Log("Reinit level select UI...");
                mLoadingCom.mLevelSelectUI.Init(mLoadingCom);
            }
            else
            {
                //产生选择游戏场景UI.
                Debug.Log("spawn level select UI...");
                mLoadingCom.SpawnLevelSelectUI();
            }

            SetActiveUISeverWeiHu(false);
            if (NetworkServerNet.GetInstance().mRequestMasterServer != null)
            {
                NetworkServerNet.GetInstance().mRequestMasterServer.ClearMastServerHostList();
            }
        }
    }
}