using UnityEngine;
using System.Collections;
using System;

public class UIController : SSUiRoot
{
    /// <summary>
    /// 画布摄像机.
    /// </summary>
    public UICamera mUICamera;
    /// <summary>
    /// 发射导弹提示UI预制.
    /// </summary>
    public GameObject FaSheDaoDanPrefab;
    GameObject FaSheDaoDanObj;
    /// <summary>
    /// 使用超级加速UI预制.
    /// </summary>
    public GameObject ChaoJiJiaSuPrefab;
    GameObject ChaoJiJiaSuObj;
    /// <summary>
    /// 播放请投币动画控制组件当玩家想耗币加速时.
    /// </summary>
    public Animator JiaSuCoinAni;
    /// <summary>
    /// 投币控制.
    /// </summary>
    public TouBiInfoCtrl mTouBiInfo;
    /// <summary>
    /// 游戏结束时要关闭的对象.
    /// </summary>
    public GameObject[] HiddenObjArray;
    /// <summary>
    /// 主角运动速度.
    /// </summary>
    public UISprite[] PlayerMoveSpeedArray;
    /// <summary>
    /// 电量UISprite.
    /// </summary>
    public UISprite DianLiangUISprite;
    /// <summary>
    /// 充电图标.
    /// </summary>
    public GameObject ChongDianObj;
    /// <summary>
    /// 电量警告图标.
    /// </summary>
    public GameObject DianLiangJiangGaoObj;
    /// <summary>
    /// 玩家道具(导弹/地雷)UI管理.
    /// </summary>
    public PlayerDaoJuManageUI mPlayerDaoJuManageUI;
    /// <summary>
    /// 结算积分对象.
    /// </summary>
    public GameObject JieSuanJiFenObj;
    /// <summary>
    /// 结算积分图集列表.
    /// </summary>
    public UISprite[] JieSuanJiFenSpriteArray;
    /// <summary>
    /// 积分图集列表.
    /// </summary>
    public UISprite[] JiFenSpriteArray;
    /// <summary>
    /// 游戏时长信息.
    /// </summary>
    public float m_pGameTime = 300.0f;
	public UISprite m_pMiaoBaiwei;
	public UISprite m_pMiaoshiwei;
	public UISprite m_pMiaogewei;
	public UISprite m_pMiaobiaozhi;
	public UIAtlas m_pNormalAtlas;
	public UIAtlas m_pWarnAtlas;
	public TweenScale m_pScale;
    /// <summary>
    /// 联机游戏最终倒计时UI预制.
    /// </summary>
    public GameObject mNetEndDaoJiShiPrefab;
    NetEndDaoJiShi mNetEndDaoJiShiCom;
    private bool m_pHasChange = false;
	public PlayerController m_Player;
	public GameObject m_CongratulateJiemian;
	public GameObject m_FinishiJiemian;
	public GameObject m_OverJiemian;
	public GameObject m_CongratulateZitiObj;
	public GameObject m_FinishiZitiObj;
	public GameObject m_OverZitiObj;
	public GameObject m_JiluObj;
	public GameObject m_JindutiaoObj;
	public GameObject m_daojishiObj;
	public GameObject m_biaodituObj;
	private float m_CongratulateTimmer = 0.0f;
	private bool m_IsCongratulate = false;
	public CameraShake m_CameraShake;
	private bool m_HasShake = false;
    bool _IsGameOver = false;
	public bool m_IsGameOver
    {
        set
        {
            _IsGameOver = value;
            if (_IsGameOver)
            {
                if (m_Player != null)
                {
                    m_Player.SortPlayerRankList();
                }
            }
        }
        get
        {
            return _IsGameOver;
        }
    }
    /// <summary>
    /// 玩家当前局所用时间.
    /// </summary>
    private int m_Score = 0;
	private int m_totalTime = 0;
	private int m_JiluRecord = 0;
	
	public UISprite m_ScoreFenGewei;
	public UISprite m_ScoreMiaoShiwei;
	public UISprite m_ScoreMiaoGewei;
	public UISprite m_RecordFenGewei;
	public UISprite m_RecordMiaoShiwei;
	public UISprite m_RecordMiaoGewei;

	public float Distance = 6400;
	public UISprite m_JinduTiao;
	public Transform m_ChuanTuBiao;

	public UITexture m_BeginDaojishi;
	public Texture[] m_BeginDaojishiTexture;

	//youmentishi
	public UITexture m_YoumenTishi;
	public Texture[] m_YoumenTishiTexture;
	private float m_YoumenTimmer = 0.0f;
	public AudioSource m_GameOverAudio;
	public AudioSource m_FinishiAudio;
	public AudioSource m_NewRecordAudio;
	public AudioSource m_NewRecordHitAudio;
	private bool m_HasTishi = false;
	public AudioSource m_BeginDaojishiAudio;
	private bool m_HasPlay = false;

	void Start()
    {
        if (SSGameCtrl.GetInstance().mSSGameRoot != null)
        {
            m_pGameTime = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.m_pGameTime;
            Distance = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.DistancePath;
            TimeNetEndVal = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.TimeNetEndVal;
        }
        //m_pGameTime = 120.0f;   //gzknu
        //m_pMiaoshiwei.spriteName = "2"; //gzknu

        chile = 0;
		m_pScale.enabled = false;
		m_totalTime = (int)m_pGameTime;
		//XkGameCtrl.IsLoadingLevel = false;
        ShowJiFenInfo(0);
        UpdateDianLiangUI(1f);
        UpdatePlayerMoveSpeed(0);
        ChongDianObj.SetActive(false);

        m_pGameTime += 1f;
        UpdateGameTime();
    }

	bool IsCloseYouMenTiShi;
	/*void ClickPlayerYouMenBtEvent(ButtonState val)
	{
		if (val == ButtonState.UP) {
			return;
		}
		IsCloseYouMenTiShi = true;
	}*/

	void Update ()
	{
		if(PlayerController.GetInstance().timmerstar < 5.0f)
		{
            //gzkun void CloseAllQiNang()
            if (!SetPanel.IsOpenSetPanel)
            {                
                //pcvr.m_IsOpneForwardQinang = false;
                //pcvr.m_IsOpneBehindQinang = false;
                //pcvr.m_IsOpneLeftQinang = false;
                //pcvr.m_IsOpneRightQinang = false;
            }
            UpdateBeginDaojishi();
		}
		else
		{
            //充电动画控制.
            if (pcvr.GetInstance().mGetJiaoTaBan > 0f && !ChongDianObj.activeInHierarchy)
            {
                ChongDianObj.SetActive(true);
            }
            else if (pcvr.GetInstance().mGetJiaoTaBan <= 0f && ChongDianObj.activeInHierarchy)
            {
                ChongDianObj.SetActive(false);
            }

            //电量低动画控制.
            if (PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.DianLiangVal <= 0.15f && !DianLiangJiangGaoObj.activeInHierarchy)
            {
                DianLiangJiangGaoObj.SetActive(true);
            }
            else if (PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.DianLiangVal > 0.15f && DianLiangJiangGaoObj.activeInHierarchy)
            {
                DianLiangJiangGaoObj.SetActive(false);
            }

            if (m_BeginDaojishi.enabled)
			{
				m_BeginDaojishi.enabled = false;
				m_BeginDaojishiAudio.Stop();
			}

            if (Mathf.Abs(pcvr.GetInstance().mGetPower) > 0f && !IsCloseYouMenTiShi)
            {
                //关闭方向盘提示.
                IsCloseYouMenTiShi = true;
                m_HasTishi = true;
                m_YoumenTishi.enabled = false;
                m_YoumenTimmer = 0.0f;
            }

            if (m_pGameTime >= 0.0f && !m_Player.m_IsFinished)
            {
                if (IsCloseYouMenTiShi && m_HasTishi)
                {
                    if (Mathf.Abs(pcvr.GetInstance().mGetPower) == 0f)
                    {
                        m_YoumenTimmer += Time.deltaTime;
                        if (m_YoumenTimmer >= 5f)
                        {
                            //打开油门提示.
                            IsCloseYouMenTiShi = false;
                            m_HasTishi = false;
                        }
                    }
                    else
                    {
                        m_YoumenTimmer = 0.0f;
                    }
                }
            }
            else
            {
                m_YoumenTishi.gameObject.SetActive(false);
            }

            if (!IsCloseYouMenTiShi && !m_HasTishi)
			{
				m_YoumenTishi.enabled = true;
				UpdateYoumenTishi();
			}
            
            if (IsOpenTimeNetEndUI && TimeNetEndVal > 0f)
            {
                NetUpdateEndGameTime();
            }

            if (m_pGameTime >= 0.0f && !m_Player.m_IsFinished)
			{
				UpdateJinduTiao();
                if (!IsOpenTimeNetEndUI)
                {
                    UpdateGameTime();
                }
			}
			else
			{
				if(m_pGameTime <= 0.0f && !m_IsGameOver)
				{
					m_IsGameOver = true;
					//TouBiInfoCtrl.IsCloseQiNang = true;
				}
				m_pScale.enabled = false;
			}

			if(m_Player.m_timmerFinished > 2.0f && !m_IsCongratulate)
			{
                bool isShowOverUI = false; //是否显示游戏结束界面.
                if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
                {
                    //联机模式游戏.
                    if (NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0 && Network.peerType == NetworkPeerType.Server)
                    {
                        //没有玩家选择链接服务器.
                        isShowOverUI = true;
                    }
                    else
                    {
                        if (IsOpenTimeNetEndUI)
                        {
                            //打开联机游戏最终倒计时界面.
                            if (TimeNetEndVal <= 0f)
                            {
                                //联机最终倒计时结束,显示游戏结束界面.
                                isShowOverUI = true;
                            }
                        }
                        else
                        {
                            if (m_Player.m_IsFinished)
                            {
                                //玩家已经到达终点.
                            }
                            else
                            {
                                if (!m_Player.IsSendActiveNetShowGameEndUI)
                                {
                                    m_Player.SendActiveIsNetShowGameEndUI();
                                }
                                else
                                {
                                    if (m_Player.GetIsNetShowGameEndUI())
                                    {
                                        //所有玩家都没到终点时显示游戏结束UI界面.
                                        isShowOverUI = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //单机.
                    isShowOverUI = true;
                }

                if (isShowOverUI)
                {
                    if (m_Player.m_IsFinished)
                    {
                        //到达终点.
                        m_Score = (int)(m_totalTime + chile * addChiLe - m_pGameTime);
                        m_JiluRecord = ReadGameInfo.GetInstance().ReadGameRecord();
                        if (m_JiluRecord == 0 || m_Score < m_JiluRecord)
                        {
                            //玩家创纪录(玩家当前用时小于记录时间).
                            if (!m_NewRecordAudio.isPlaying)
                            {
                                m_NewRecordAudio.Play();
                            }
                            m_CongratulateJiemian.SetActive(true);
                            ReadGameInfo.GetInstance().WriteGameRecord(m_Score);
                        }
                        else
                        {
                            //到达终点.
                            if (!m_FinishiAudio.isPlaying)
                            {
                                m_FinishiAudio.Play();
                            }
                            m_FinishiJiemian.SetActive(true);
                        }
                        m_JiluObj.SetActive(true);
                        UpdateMyScore();
                        UpdateRecord();
                    }
                    else
                    {
                        //玩家没有到达终点.
                        if (!m_GameOverAudio.isPlaying)
                        {
                            m_GameOverAudio.Play();
                        }
                        m_OverJiemian.SetActive(true);
                    }

                    if (!m_IsCongratulate)
                    {
                        //游戏结束后无需NetworkSynchronizeGame进行npc或主角的信息同步.
                        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
                        {
                            if (NetworkServerNet.GetInstance().mNetworkRootGame != null)
                            {
                                NetworkServerNet.GetInstance().mNetworkRootGame.ePlayerGameNetState = NetworkServerNet.PlayerGameNetType.GameBackMovie;
                            }
                        }
                    }

                    m_IsCongratulate = true;
                    m_JindutiaoObj.SetActive(false);
                    m_daojishiObj.SetActive(false);
                    m_biaodituObj.SetActive(false);
                    HiddenJiFen();
                    HiddenUi();
                    JieSuanJiFenObj.SetActive(true);
                }
            }

			if(m_IsCongratulate)
			{
				m_CongratulateTimmer+=Time.deltaTime;
			}

			if(m_CongratulateTimmer > 1.0f)
			{
				if(m_Player.m_IsFinished)
				{
					if(m_Score < m_JiluRecord || m_JiluRecord == 0)
					{
						if(!m_NewRecordHitAudio.isPlaying && !m_HasPlay)
						{
							m_HasPlay = true;
							m_NewRecordHitAudio.Play();
						}
						m_CongratulateZitiObj.SetActive(true);
					}
					else
					{
						m_FinishiZitiObj.SetActive(true);
					}
				}
				else
				{
					m_OverZitiObj.SetActive(true);
				}
			}

			if(m_Player.m_IsFinished && m_CongratulateTimmer>1.2f && !m_HasShake)
			{
				m_HasShake = true;
				m_CameraShake.setCameraShakeImpulseValue();
			}

			if(m_CongratulateTimmer > 5f && !IsLoadMovie)
			{
				//MyCOMDevice.GetInstance().ForceRestartComPort();
				LoadMovieLevel();
			}

            if (PlayerController.GetInstance().PlayerMoveSpeed != LastPlayerMoveSpeedVal)
            {
                UpdatePlayerMoveSpeed(PlayerController.GetInstance().PlayerMoveSpeed);
            }
        }
	}

	bool IsLoadMovie;
	void LoadMovieLevel()
	{
		if (IsLoadMovie) {
			return;		
		}
		IsLoadMovie = true;

        if (Network.peerType == NetworkPeerType.Client)
        {
            NetworkServerNet.GetInstance().RemoveClientHost();
        }

        if (Network.peerType == NetworkPeerType.Server)
        {
            NetworkServerNet.GetInstance().RemoveMasterServerHost();
        }
        StartCoroutine(CheckUnloadUnusedAssets());
	}

	IEnumerator CheckUnloadUnusedAssets()
	{
		bool isLoop = true;
		GC.Collect();
		AsyncOperation asyncVal = Resources.UnloadUnusedAssets();
		float timeLast = Time.realtimeSinceStartup;

		do {
			yield return new WaitForSeconds(0.5f);
			if (Time.realtimeSinceStartup - timeLast > 5f)
            {
				isLoop = false;
				//XkGameCtrl.IsLoadingLevel = true;
				//Debug.Log("CheckUnloadUnusedAssets -> loading movie level, asyncVal.isDone "+asyncVal.isDone);
				Application.LoadLevel(0);
				yield break;
			}

			if (!asyncVal.isDone)
            {
				yield return new WaitForSeconds(0.5f);
			}
			else
            {
				isLoop = false;
				//XkGameCtrl.IsLoadingLevel = true;
				Application.LoadLevel(0);
				yield break;
			}
		} while (isLoop);
	}

    /// <summary>
    /// 更新正常倒计时.
    /// </summary>
	void UpdateGameTime()
	{
		m_pGameTime -= Time.deltaTime;
		int TimeMiaoBaiwei = (int)(m_pGameTime / 100);
		int TimeMiaoshiwei = (int)((m_pGameTime - TimeMiaoBaiwei*100)/10);
		int TimeMiaogewei = (int)(m_pGameTime - TimeMiaoBaiwei*100 - TimeMiaoshiwei*10);
		if(m_pGameTime <= 11.0f && !m_pHasChange)
		{
			m_pScale.enabled = true;
			m_pMiaoBaiwei.atlas = m_pWarnAtlas;
			m_pMiaoshiwei.atlas = m_pWarnAtlas;
			m_pMiaogewei.atlas = m_pWarnAtlas;
			m_pMiaobiaozhi.atlas = m_pWarnAtlas;
			m_pHasChange = true;
		}
		m_pMiaoBaiwei.spriteName = TimeMiaoBaiwei.ToString();
		m_pMiaoshiwei.spriteName = TimeMiaoshiwei.ToString();
		m_pMiaogewei.spriteName = TimeMiaogewei.ToString();
    }

    /// <summary>
    /// 联机游戏最终倒计时(多人联机时启用).
    /// </summary>
    [HideInInspector]
    public float TimeNetEndVal = 0;
    /// <summary>
    /// 是否打开了联机游戏最终倒计时.
    /// </summary>
    bool IsOpenTimeNetEndUI = false;
    public void SetActiveIsOpenTimeNetEndUI()
    {
        if (!IsOpenTimeNetEndUI)
        {
            IsOpenTimeNetEndUI = true;
            SpawnNetEndDaoJiShiUI();
        }
    }

    /// <summary>
    /// 产生网络联机游戏最终倒计时界面.
    /// </summary>
    void SpawnNetEndDaoJiShiUI()
    {
        if (mNetEndDaoJiShiPrefab != null)
        {
            Debug.Log("SpawnNetEndDaoJiShiUI...");
            GameObject obj = (GameObject)Instantiate(mNetEndDaoJiShiPrefab, mUICamera.transform);
            mNetEndDaoJiShiCom = obj.GetComponent<NetEndDaoJiShi>();
        }
    }

    /// <summary>
    /// 联机状态下更新最后倒计时.
    /// </summary>
    void NetUpdateEndGameTime()
    {
        if (TimeNetEndVal <= 0f)
        {
            return;
        }

        float timeVal = TimeNetEndVal;
        timeVal -= Time.deltaTime;
        if (timeVal <= 0f)
        {
            timeVal = 0f;
            m_pScale.enabled = false;

            if (!m_Player.m_IsFinished && !m_IsGameOver)
            {
                //联机最终倒计时结束,玩家未到达终点.
                m_IsGameOver = true;
            }
        }
        TimeNetEndVal = timeVal;

        int TimeMiaoBaiwei = (int)(timeVal / 100);
        int TimeMiaoshiwei = (int)((timeVal - TimeMiaoBaiwei * 100) / 10);
        int TimeMiaogewei = (int)(timeVal - TimeMiaoBaiwei * 100 - TimeMiaoshiwei * 10);
        if (!m_pHasChange)
        {
            //改变倒计时的图片.
            m_pScale.enabled = true;
            m_pMiaoBaiwei.atlas = m_pWarnAtlas;
            m_pMiaoshiwei.atlas = m_pWarnAtlas;
            m_pMiaogewei.atlas = m_pWarnAtlas;
            m_pMiaobiaozhi.atlas = m_pWarnAtlas;
            m_pHasChange = true;
        }
        m_pMiaoBaiwei.spriteName = TimeMiaoBaiwei.ToString();
        m_pMiaoshiwei.spriteName = TimeMiaoshiwei.ToString();
        m_pMiaogewei.spriteName = TimeMiaogewei.ToString();

        //更新最终倒计时.
        if (mNetEndDaoJiShiCom != null)
        {
            mNetEndDaoJiShiCom.m_pNetMiaoshiwei.spriteName = TimeMiaoshiwei.ToString();
            mNetEndDaoJiShiCom.m_pNetMiaogewei.spriteName = TimeMiaogewei.ToString();
        }
    }


    void UpdateMyScore()
	{
		int fen = m_Score/60;
		if(fen > 0)
		{
			m_ScoreFenGewei.spriteName = fen.ToString();
		}
		else
		{
			m_ScoreFenGewei.spriteName = "0";
		}
		int miao = m_Score - fen*60;
		int miaoshiwei = miao/10;
		int miaogewei = miao - miaoshiwei*10;
		m_ScoreMiaoShiwei.spriteName = miaoshiwei.ToString();
		m_ScoreMiaoGewei.spriteName = miaogewei.ToString();
	}


	void UpdateRecord()
	{
		int fen = m_JiluRecord/60;
		if(fen > 0)
		{
			m_RecordFenGewei.spriteName = fen.ToString();
		}
		else
		{
			m_RecordFenGewei.spriteName = "0";
		}
		int miao = m_JiluRecord - fen*60;
		int miaoshiwei = miao/10;
		int miaogewei = miao - miaoshiwei*10;
		m_RecordMiaoShiwei.spriteName = miaoshiwei.ToString();
		m_RecordMiaoGewei.spriteName = miaogewei.ToString();
	}

	void UpdateJinduTiao()
	{
		m_JinduTiao.fillAmount = (m_Player.m_distance)/Distance;
		if(m_JinduTiao.fillAmount > 1.0f)
		{
			m_JinduTiao.fillAmount = 1.0f;
		}
		m_ChuanTuBiao.localPosition = new Vector3(m_JinduTiao.fillAmount *(355+375.0f)-375.0f ,-25.0f,0.0f);
	}

	void UpdateBeginDaojishi()
	{
		if(!m_BeginDaojishi.enabled)
		{
			m_BeginDaojishi.enabled = true;
		}
		if (!m_BeginDaojishiAudio.isPlaying)
				m_BeginDaojishiAudio.Play ();
		int index = (int)(6.0f - PlayerController.GetInstance().timmerstar);
		if(index>=6)
		{
			index = 5;
		}
		m_BeginDaojishi.mainTexture = m_BeginDaojishiTexture[index-1];
	}

	void UpdateYoumenTishi()
	{
		m_YoumenTimmer+=Time.deltaTime;
		if(m_YoumenTimmer<0.3f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[0];
		}
		else if(m_YoumenTimmer>=0.3f &&m_YoumenTimmer<0.6f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[1];
		}
		else if(m_YoumenTimmer>=0.6f &&m_YoumenTimmer<0.9f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[2];
		}
		else if(m_YoumenTimmer>=0.9f &&m_YoumenTimmer<1.2f)
		{
			m_YoumenTishi.mainTexture =  m_YoumenTishiTexture[3];
		}
		else
		{
			m_YoumenTimmer = 0.0f;
		}
	}
	public GameObject m_JiashiTexture;
	public TweenScale m_Scale;
	public TweenPosition m_Position;
	private int chile = 0;
	private float addChiLe = 5.0f;
	public void ResetJiashi()
	{
		m_JiashiTexture.transform.localPosition = new Vector3 (0.0f, 0.0f, 500.0f);
		m_JiashiTexture.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
		if (!m_IsGameOver) {
			m_pGameTime += addChiLe;
			chile++;
		}
		//Debug.Log ("chileeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee " +chile);
		m_Scale.ResetToBeginning ();
		m_Position.ResetToBeginning ();
		m_Scale.enabled = true;
		m_Position.enabled = true;
		m_JiashiTexture.SetActive (false);
	}

    /// <summary>
    /// 显示玩家积分.
    /// </summary>
    public void ShowJiFenInfo(int jiFen)
    {
        int jiFenTmp = 0;
        string jiFenStr = jiFen.ToString();
        for (int i = 0; i < 6; i++)
        {
            if (jiFenStr.Length > i)
            {
                jiFenTmp = jiFen % 10;
                JiFenSpriteArray[i].spriteName = jiFenTmp.ToString();
                JieSuanJiFenSpriteArray[i].spriteName = jiFenTmp.ToString();
                jiFen = (int)(jiFen / 10f);
                JiFenSpriteArray[i].enabled = true;
                JieSuanJiFenSpriteArray[i].enabled = true;
            }
            else
            {
                JiFenSpriteArray[i].enabled = false;
                JieSuanJiFenSpriteArray[i].enabled = false;
            }
        }
    }

    void HiddenUi()
    {
        for (int i = 0; i < HiddenObjArray.Length; i++)
        {
            HiddenObjArray[i].SetActive(false);
        }
    }

    void HiddenJiFen()
    {
        for (int i = 0; i < JiFenSpriteArray.Length; i++)
        {
            JiFenSpriteArray[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 电量数据.
    /// </summary>
    [Serializable]
    public class DianLiangData
    {
        [Range(0f, 1f)]
        public float minAmount = 0f;
        [Range(0f, 1f)]
        public float maxAmount = 1f;
    }
    public DianLiangData mDianLiangData;
    /// <summary>
    /// 更新玩家电量UI.
    /// </summary>
    public void UpdateDianLiangUI(float dianLiang)
    {
        float disVal = mDianLiangData.maxAmount - mDianLiangData.minAmount;
        float val = mDianLiangData.minAmount + disVal * dianLiang;
        DianLiangUISprite.fillAmount = val;
    }

    float TimeLastSpeed = -1f;
    int LastPlayerMoveSpeedVal = 0;
    /// <summary>
    /// 更新玩家速度数据.
    /// </summary>
    public void UpdatePlayerMoveSpeed(int speed)
    {
        if (Time.time - TimeLastSpeed < 0.08f)
        {
            return;
        }
        TimeLastSpeed = Time.time;
        LastPlayerMoveSpeedVal = speed;

        int tmpVal = 0;
        string valStr = speed.ToString();
        for (int i = 0; i < 3; i++)
        {
            if (valStr.Length > i)
            {
                tmpVal = speed % 10;
                PlayerMoveSpeedArray[i].spriteName = tmpVal.ToString();
                speed = (int)(speed / 10f);
                PlayerMoveSpeedArray[i].enabled = true;
            }
            else
            {
                PlayerMoveSpeedArray[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 播放请投币动画当玩家想耗币加速时.
    /// </summary>
    public void PlayInsertCoinAniOnClickJiaSu()
    {
        JiaSuCoinAni.gameObject.SetActive(true);
        JiaSuCoinAni.SetTrigger("IsPlay");
    }

    public void SetActiveUIRoot(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 产生发射导弹提示UI.
    /// </summary>
    public void SpawnFaSheDaoDanUI()
    {
        if (ChaoJiJiaSuObj != null)
        {
            //有超级加速提示时,不提示发射导弹.
            return;
        }

        if (FaSheDaoDanObj != null)
        {
            return;
        }

        if (mPlayerDaoJuManageUI.DaoDanNum > 0 || mPlayerDaoJuManageUI.DiLeiNum > 0)
        {
            Debug.Log("SpawnFaSheDaoDanUI -> time " + Time.time);
            FaSheDaoDanObj = (GameObject)Instantiate(FaSheDaoDanPrefab, mUICamera.transform);
        }
    }

    /// <summary>
    /// 删除发射导弹提示UI.
    /// </summary>
    public void RemoveFaSheDaoDanUI()
    {
        if (FaSheDaoDanObj != null)
        {
            Destroy(FaSheDaoDanObj);
        }
    }
    /// <summary>
    /// 产生使用超级加速UI.
    /// </summary>
    public void SpawnChaoJiJiaSuUI()
    {
        if (ChaoJiJiaSuObj != null)
        {
            return;
        }
        Debug.Log("SpawnChaoJiJiaSuUI -> time " + Time.time);
        RemoveFaSheDaoDanUI();
        ChaoJiJiaSuObj = (GameObject)Instantiate(ChaoJiJiaSuPrefab, mUICamera.transform);
    }

    /// <summary>
    /// 删除使用超级加速UI.
    /// </summary>
    public void RemoveChaoJiJiaSuUI()
    {
        if (ChaoJiJiaSuObj != null)
        {
            Destroy(ChaoJiJiaSuObj);
        }
    }
}