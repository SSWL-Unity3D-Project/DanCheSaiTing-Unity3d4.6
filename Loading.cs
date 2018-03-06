using UnityEngine;
using System.Collections;
using System;

public class Loading : SSUiRoot
{
	private string CoinNumSet = "1";
	private string InsertCoinNum = "";
    /// <summary>
    /// 选择联机游戏的玩家UI预制文件.
    /// </summary>
    public GameObject LinkPlayerPrefab;
    /// <summary>
    /// 联机玩家信息UI界面.
    /// </summary>
    GameLinkPlayer mGameLinkPlayer;
    /// <summary>
    /// 游戏模式选择UI预制文件.
    /// </summary>
    public GameObject GameModePrefab;
    /// <summary>
    /// 游戏模式选择UI界面.
    /// </summary>
    GameModeSelect mGameModeSelect;
    /// <summary>
    /// UI摄像机.
    /// </summary>
    public UICamera mUICamera;
    public UITexture m_BeginTex;
	public UITexture m_InsertTex;
	public UISprite CoinNumSetTex;
	public UISprite m_InsertNumS;
	public UISprite m_InsertNumG;
	private int m_InserNum = 0;
	private bool m_IsBeginOk = false;

	private float m_PressTimmer = 0.0f;
	private float m_InserTimmer = 0.0f;
	private bool m_IsStartGame =false;
	public AudioSource m_TbSource;
	public AudioSource m_BeginSource;
	public GameObject m_Loading;
	public GameObject m_Tishi;
	private string GameMode = "";

	public UITexture m_pTishiTexture;
	public Texture[] m_pTexture;

	//public MovieTexture m_MovieTex;
	public UITexture m_FreeTexture;
	public GameObject m_ToubiObj;

	private bool timmerstar = false;
	private float timmerforstar = 0.0f;
	public static bool m_HasBegin = false;
	public bool IsLuPingTest;
    void Awake()
    {
        if (NetworkServerNet.GetInstance() != null)
        {
            NetworkServerNet.GetInstance().mRequestMasterServer.SetMasterServerComment(RequestMasterServer.MasterServerComment.Movie);
        }
    }

	void Start ()
    {
        if (NetworkServerNet.GetInstance() != null)
        {
            NetworkServerNet.GetInstance().mRequestMasterServer.SetIsNetScene(true);
        }

        if (IsLuPingTest)
        {
			gameObject.SetActive(false);
		}
		//m_MovieTex.loop = true;
		//m_MovieTex.Play();
		m_HasBegin = false;
		GameMode = ReadGameInfo.GetInstance ().ReadGameStarMode();
		if(GameMode == "oper")
		{
			m_FreeTexture.enabled = false;
			CoinNumSet = ReadGameInfo.GetInstance ().ReadStarCoinNumSet();
			InsertCoinNum = ReadGameInfo.GetInstance ().ReadInsertCoinNum();
			CoinNumSetTex.spriteName = CoinNumSet;
			m_InserNum = Convert.ToInt32(InsertCoinNum);
			UpdateInsertCoin();
			UpdateTex();
		}
		else
		{
			m_ToubiObj.SetActive(false);
			m_IsBeginOk = true;
			m_FreeTexture.enabled = true;
		}
		//Invoke("OnClickBeginBt", UnityEngine.Random.Range(1f, 5f)); //test
		m_Loading.SetActive(false);
		//pcvr.ShaCheBtLight = StartLightState.Mie;

		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickStartBtOneEvent += ClickStartBtOneEvent;
        NetworkEvent.GetInstance().OnRpcSendLoadLevelMsgEvent += OnRpcSendLoadLevelMsgEvent;
    }
    
    void Update ()
	{
		if (!m_IsStartGame) {
			UpdateTex();
		}

		if (pcvr.bIsHardWare)
        {
			if (GlobalData.GetInstance().CoinCur != m_InserNum && GameMode == "oper")
            {
				m_InserNum = GlobalData.GetInstance().CoinCur - 1;
				OnClickInsertBt();
			}
		}
		else
        {
			if(Input.GetKeyDown(KeyCode.T) && GameMode == "oper")
			{
				OnClickInsertBt();
			}
		}
		OnLoadingClicked();
	}

	void ClickStartBtOneEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN) {
			return;
		}
		OnClickBeginBt();
	}

	void ClickSetEnterBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN)
        {
			return;
		}

		if (m_HasBegin)
        {
			return;
		}

        if (NetworkRootMovie.GetInstance().ePlayerSelectGameMode == NetworkRootMovie.GameMode.Link)
        {
            //选择联机游戏后不允许进入设置界面.
            return;
        }

        if (IsLoadingSetPanel)
        {
            return;
        }
        IsLoadingSetPanel = true;


        if (NetworkServerNet.GetInstance() != null)
        {
            NetworkServerNet.GetInstance().mRequestMasterServer.SetIsNetScene(false);
            switch (Network.peerType)
            {
                case NetworkPeerType.Server:
                    {
                        NetworkServerNet.GetInstance().RemoveMasterServerHost();
                        break;
                    }
                case NetworkPeerType.Client:
                    {
                        NetworkServerNet.GetInstance().RemoveClientHost();
                        break;
                    }
            }
        }
        StartCoroutine(DelayLoadingSetPanel());
    }

    /// <summary>
    /// 是否正在加载设置界面场景.
    /// </summary>
    bool IsLoadingSetPanel = false;
    IEnumerator DelayLoadingSetPanel()
    {
        yield return new WaitForSeconds(1f);
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Application.LoadLevel(5); //进入设置界面.
    }

	void UpdateInsertCoin()
	{
		int n = 1;
		int num = m_InserNum;
		int temp = num;
        SSGameCtrl.GetInstance().mPlayerDataManage.PlayerCoinNum = m_InserNum;
        while (num > 9)
		{
			num /= 10;
			n++;
		}

		if(n > 2)
		{
			m_InsertNumS.spriteName = "9";
			m_InsertNumG.spriteName = "9";
		}
		else if(n==2)
		{
			int shiwei = (int)(temp/10);
			int gewei = (int)(temp-shiwei*10);
			m_InsertNumS.spriteName = shiwei.ToString();
			m_InsertNumG.spriteName = gewei.ToString();
		}
		else if(n == 1)
		{
			m_InsertNumS.spriteName = "0";
			m_InsertNumG.spriteName = temp.ToString();
		}
	}

	void UpdateTex()
	{
		if(GameMode == "FREE" || m_InserNum >= Convert.ToInt32(CoinNumSet))
		{
			m_InserTimmer = 0.0f;
			m_IsBeginOk = true;
			m_InsertTex.enabled = false;
			m_BeginTex.enabled =true;
			m_pTishiTexture.enabled = true;
			m_PressTimmer+=(Time.deltaTime / Time.timeScale);
			if(m_PressTimmer >= 0.0f && m_PressTimmer <= 0.5f)
			{
				m_BeginTex.enabled =true;
				m_pTishiTexture.mainTexture = m_pTexture[0];
			}
			else if(m_PressTimmer > 0.5f && m_PressTimmer <= 1.0f)
			{
				m_BeginTex.enabled =false;
				m_pTishiTexture.mainTexture = m_pTexture[1];
			}
			else
			{
				m_PressTimmer = 0.0f;
			}
			//pcvr.StartBtLight = StartLightState.Shan;
		}
		else
		{
			//pcvr.StartBtLight = StartLightState.Mie;
			m_InserTimmer+=(Time.deltaTime / Time.timeScale);
			m_IsBeginOk = false;
			m_InsertTex.enabled = true;
			m_BeginTex.enabled =false;
			m_pTishiTexture.enabled = false;
			m_PressTimmer = 0.0f;
			if(m_InserTimmer >= 0.0f && m_InserTimmer <= 0.4f)
			{
				m_InsertTex.enabled = true;
			}
			else if(m_InserTimmer > 0.4f && m_InserTimmer <= 0.8f)
			{
				m_InsertTex.enabled = false;
			}
			else
			{
				m_InserTimmer = 0.0f;
			}
		}
	}
	IEnumerator loadScene(int num)   
	{
		//XkGameCtrl.IsLoadingLevel = true;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		AsyncOperation async = Application.LoadLevelAsync(num);   
		yield return async;		
	}

	void OnClickInsertBt()
	{
			m_TbSource.Play();
			m_InserNum++;
			ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
			UpdateInsertCoin();
	}
	void OnClickBeginBt()
	{
        switch(NetworkRootMovie.GetInstance().eNetState)
        {
            case NetworkRootMovie.GameNetType.Link:
                {
                    if (mGameModeSelect == null)
                    {
                        SpawnGameModeUI();
                        m_BeginSource.Play();
                        m_IsStartGame = true;
                        if (GameMode == "oper")
                        {
                            m_InserNum -= Convert.ToInt32(CoinNumSet);
                            UpdateInsertCoin();
                            ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
                            if (pcvr.bIsHardWare)
                            {
                                pcvr.GetInstance().mPcvrTXManage.SubPlayerCoin(Convert.ToInt32(CoinNumSet), pcvrTXManage.PlayerCoinEnum.player01);
                            }
                        }
                        m_Tishi.SetActive(false);
                    }
                    else
                    {
                        if (mGameModeSelect.eGameMode == NetworkRootMovie.GameMode.NoLink)
                        {
                            //玩家选择单机游戏.
                            if (m_IsBeginOk && !m_HasBegin)
                            {
                                m_BeginSource.Play();
                                m_Loading.SetActive(true);
                                timmerstar = true;
                                m_HasBegin = true;
                                mGameModeSelect.HiddenSelf();
                                SSGameCtrl.GetInstance().eGameMode = NetworkRootMovie.GameMode.NoLink;
                                NetworkRootMovie.GetInstance().ePlayerSelectGameMode = NetworkRootMovie.GameMode.NoLink;
                                NetworkServerNet.GetInstance().mRequestMasterServer.SetIsNetScene(false);
                                NetworkServerNet.GetInstance().RemoveMasterServerHost();
                            }
                        }

                        if (mGameModeSelect.eGameMode == NetworkRootMovie.GameMode.Link)
                        {
                            if (mGameLinkPlayer == null)
                            {
                                NetworkRootMovie.GetInstance().ePlayerSelectGameMode = NetworkRootMovie.GameMode.Link;
                                m_BeginSource.Play();
                                mGameModeSelect.HiddenSelf();
                                SpawnGameLinkPlayerUI();
                            }
                            else
                            {
                                if (mGameLinkPlayer.StartBtObj.activeInHierarchy)
                                {
                                    //发送网络消息-开始联机游戏.
                                    NetworkRootMovie.GetInstance().mNetworkRpcMsgScript.NetSendLoadLevel(LoadSceneCount);
                                }
                            }
                        }
                    }
                    break;
                }
            case NetworkRootMovie.GameNetType.NoLink:
                {
                    if (PlayerControllerForMoiew.IsLoadMovieLevel)
                    {
                        return;
                    }

                    if (m_IsBeginOk && !m_HasBegin)
                    {
                        m_BeginSource.Play();
                        m_IsStartGame = true;
                        if (GameMode == "oper")
                        {
                            m_InserNum -= Convert.ToInt32(CoinNumSet);
                            UpdateInsertCoin();
                            ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());

                            if (pcvr.bIsHardWare)
                            {
                                pcvr.GetInstance().mPcvrTXManage.SubPlayerCoin(Convert.ToInt32(CoinNumSet), pcvrTXManage.PlayerCoinEnum.player01);
                            }
                        }
                        m_Tishi.SetActive(false);
                        m_Loading.SetActive(true);
                        timmerstar = true;
                        m_HasBegin = true;
                        SSGameCtrl.GetInstance().eGameMode = NetworkRootMovie.GameMode.NoLink;
                    }
                    break;
                }
        }
	}

    static int LoadSceneCount;
	void OnLoadingClicked()
	{
		if(timmerstar)
		{
			timmerforstar += Time.deltaTime;
			if(timmerforstar > 1.5f)
			{
                StartCoroutine (loadScene((LoadSceneCount % (Application.levelCount - 2)) + 1));
				timmerstar = false;
                LoadSceneCount++;
            }
		}
    }

    /// <summary>
    /// 创建游戏模式选择UI界面.
    /// </summary>
    void SpawnGameModeUI()
    {
        GameObject obj = (GameObject)Instantiate(GameModePrefab, mUICamera.transform);
        mGameModeSelect = obj.GetComponent<GameModeSelect>();
        mGameModeSelect.Init();
    }

    /// <summary>
    /// 创建联机玩家信息UI界面.
    /// </summary>
    void SpawnGameLinkPlayerUI()
    {
        GameObject obj = (GameObject)Instantiate(LinkPlayerPrefab, mUICamera.transform);
        mGameLinkPlayer = obj.GetComponent<GameLinkPlayer>();
        mGameLinkPlayer.Init();
    }

    /// <summary>
    /// 收到Rpc加载游戏场景消息.
    /// </summary>
    void OnRpcSendLoadLevelMsgEvent(int level)
    {
        Debug.Log("Loading::OnRpcSendLoadLevelMsgEvent -> level == " + level);
        if (m_IsBeginOk && !m_HasBegin)
        {
            //开始联机游戏.
            Debug.Log("Start link game...");
            LoadSceneCount = level;
            mGameLinkPlayer.OnClickStartBt();
            SSGameCtrl.GetInstance().eGameMode = NetworkRootMovie.GameMode.Link;
            NetworkRootMovie.GetInstance().ePlayerGameNetState = NetworkServerNet.PlayerGameNetType.MovieIntoGame;
            if (NetworkServerNet.GetInstance() != null)
            {
                NetworkServerNet.GetInstance().mRequestMasterServer.SetMasterServerComment(RequestMasterServer.MasterServerComment.GameNet);
            }
            NetworkServerNet.GetInstance().RemoveMasterServerHost();

            m_BeginSource.Play();
            m_Loading.SetActive(true);
            timmerstar = true;
            m_HasBegin = true;
        }
    }
}