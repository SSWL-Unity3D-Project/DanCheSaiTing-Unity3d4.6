using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 主角动画控制脚本.
    /// </summary>
    public PlayerAnimationCtrl mPlayerAnimation;
    /// <summary>
    /// 终点Render.
    /// </summary>
    [HideInInspector]
    public MeshRenderer[] FinishRender;
    bool IsShowZhongDian = false;
    /// <summary>
    /// 联机游戏中是否显示游戏结束UI界面.
    /// </summary>
    [HideInInspector]
    public bool IsNetShowGameEndUI = false;
    /// <summary>
    /// 是否是网络控制端(只有网络控制端才有主动控制权限).
    /// </summary>
    [HideInInspector]
    public bool IsNetControlPort = false;
    /// <summary>
    /// 网络信息同步组件.
    /// </summary>
    public NetworkSynchronizeGame mNetSynGame;
    /// <summary>
    /// 潜艇/坦克对水粒子位置的信息.
    /// </summary>
    [Serializable]
    public class DaoJuToWaterData
    {
        /// <summary>
        /// 水粒子原位置信息列表.
        /// </summary>
        public Transform[] TrArray;
        /// <summary>
        /// 潜艇水粒子位置列表.
        /// </summary>
        public Transform[] QianTingTrArray;
        /// <summary>
        /// 坦克水粒子位置列表.
        /// </summary>
        public Transform[] TankTrArray;
    }
    public DaoJuToWaterData DaoJuToWaterDt;
    /// <summary>
    /// 水粒子列表.
    /// </summary>
    public Transform[] WaterLiZiTrArray;
    /// <summary>
    /// 坦克变型数据.
    /// </summary>
    [Serializable]
    public class TankData
    {
        /// <summary>
        /// 坦克模型.
        /// </summary>
        public GameObject TankObj;
        /// <summary>
        /// 坦克变身特效粒子预置.
        /// </summary>
        public GameObject LiZiPrefab;
        /// <summary>
        /// 坦克变身特效粒子产生点.
        /// </summary>
        public Transform LiZiSpawnPoint;
        /// <summary>
        /// 坦克发射间隔时间.
        /// </summary>
        public float TimeAmmo;
        /// <summary>
        /// 坦克鱼雷预置.
        /// </summary>
        public GameObject AmmoPrefab;
        /// <summary>
        /// 坦克子弹产生点列表.
        /// </summary>
        public Transform[] AmmoSpawnPointArray;
    }
    public TankData TankDt = new TankData();

    /// <summary>
    /// 潜艇变型数据.
    /// </summary>
    [Serializable]
    public class QianTingData
    {
        /// <summary>
        /// 潜艇模型.
        /// </summary>
        public GameObject QianTingObj;
        /// <summary>
        /// 潜艇变身特效粒子预置.
        /// </summary>
        public GameObject LiZiPrefab;
        /// <summary>
        /// 潜艇变身特效粒子产生点.
        /// </summary>
        public Transform LiZiSpawnPoint;
        /// <summary>
        /// 鱼雷发射间隔时间.
        /// </summary>
        public float TimeYuLei;
        /// <summary>
        /// 潜艇鱼雷预置.
        /// </summary>
        public GameObject YuLeiPrefab;
        /// <summary>
        /// 潜艇鱼雷产生点列表.
        /// </summary>
        public Transform[] YuLeiSpawnPointArray;
    }
    public QianTingData QianTingDt = new QianTingData();
    public FeiBanPengZhuangCtrl pFeiBanPengZhuang;
    /// <summary>
    /// 主角不同道具等对象的数据.
    /// </summary>
    public PlayerData[] PlayerDt = new PlayerData[4];
    /// <summary>
    /// 主角模型列表.
    /// </summary>
    public GameObject[] PlayerObjArray;
    /// <summary>
    /// 玩家磁铁道具状态.
    /// </summary>
    [HideInInspector]
    public bool IsOpenCiTieDaoJu;
    float TimeLastCiTie;
    /// <summary>
    /// 磁铁激活时长.
    /// </summary>
    public float TimeOpenCiTie = 5f;
    /// <summary>
    /// 导弹射击npc的有效距离.
    /// </summary>
    public float DaoDanDisNpc = 100f;
    /// <summary>
    /// 提示发射导弹射击npc的最小有效距离.
    /// </summary>
    public float MinDaoDanDisNpc = 50f;
    /// <summary>
    /// 套圈提示导弹射击npc的最小有效距离.
    /// </summary>
    public float MinTaoQuanDaoDanDisNpc = 3f;
    /// <summary>
    /// 最大超级加速的有效距离.
    /// </summary>
    public float MaxChaoJiJiaSuDisNpc = 50f;
    /// <summary>
    /// 最小超级加速的有效距离.
    /// </summary>
    public float MinChaoJiJiaSuDisNpc = 25f;
    /// <summary>
    /// 道具掉落点.
    /// DaoJuDiaoLuoTr[x]: 0 喷气道具, 1 飞行翼道具, 2 风框道具, 3 双翼飞行道具.
    /// </summary>
    public Transform[] DaoJuDiaoLuoTr;
    /// <summary>
    /// 喷气动画列表.
    /// </summary>
    [HideInInspector]
    public Animator[] PenQiAniAy;
    /// <summary>
    /// 喷气道具掉落预置.
    /// </summary>
    public GameObject PenQiPrefab;
    /// <summary>
    /// 飞机翅膀动画列表.
    /// </summary>
    [HideInInspector]
    public Animator[] FiXingYiAniAy;
    /// <summary>
    /// 飞行翅膀道具掉落预置.
    /// </summary>
    public GameObject FeiXingYiPrefab;
    /// <summary>
    /// 双翼飞机翅膀动画列表.
    /// </summary>
    [HideInInspector]
    public Animator[] ShuangYiFeiJiAniAy;
    /// <summary>
    /// 双翼飞机翅膀道具掉落预置.
    /// </summary>
    public GameObject ShuangYiFeiJiPrefab;
    /// <summary>
    /// 双翼飞机风框转动脚本.
    /// </summary>
    [HideInInspector]
    public TweenRotation ShuangYiFeiJiTwRot;
    /// <summary>
    /// 风框动画.
    /// </summary>
    [HideInInspector]
    public Animator FengKuangAni;
    /// <summary>
    /// 风框道具掉落预置.
    /// </summary>
    public GameObject FenKuangPrefab;
    /// <summary>
    /// 道具风框转动脚本.
    /// </summary>
    [HideInInspector]
    public TweenRotation FengKuangTwRot;
    /// <summary>
    /// 主角飞行高度(飞行翼).
    /// </summary>
    public float PlayerHightFeiXing = 1f;
    /// <summary>
    /// 主角飞行高度(双翼飞机).
    /// </summary>
    public float PlayerHightShuangYiFeiJi = 1f;
    /// <summary>
    /// 主角普通运动速度(油门速度).
    /// </summary>
    public float PlayerMvSpeedMin = 50f;
    /// <summary>
    /// 脚踏板最高速度.
    /// </summary>
    public float mTopJiaoTaSpeed = 110f;
    /// <summary>
    /// 脚踏板风扇.
    /// </summary>
    Transform JiaoTaBanFenShanTr;
    /// <summary>
    /// 脚踏板风扇最高转速.
    /// </summary>
    public float JiaoTaBanFenShanTopSpeed = 2f;
    /// <summary>
    /// 主角喷气运动速度.
    /// </summary>
    public float PlayerMvSpeedPenQi = 90f;
    /// <summary>
    /// 主角飞行运动速度(飞行翼).
    /// </summary>
    public float PlayerMvSpeedFeiXing = 100f;
    /// <summary>
    /// 主角飞行运动速度(双翼飞机).
    /// </summary>
    public float PlayerMvSpeedShuangYiFeiJi = 100f;
    /// <summary>
    /// 主角加速风扇运动速度.
    /// </summary>
    public float PlayerMvSpeedJiaSuFengShan = 80f;
    /// <summary>
    /// 玩家速度相关道具状态.
    /// </summary>
    [HideInInspector]
    public DaoJuCtrl.DaoJuType mSpeedDaoJuState;
    float TimeLastDaoJuBianXing;
    /// <summary>
    /// 道具变型时长.
    /// </summary>
    public float DaoJuBianXingTime = 5f;
    /// <summary>
    /// 喷气变形时长.
    /// </summary>
    public float PenQiBianXingTime = 5f;
    /// <summary>
    /// 飞行翼(投币)变形时长.
    /// </summary>
    public float FeiXingYiBianXingTime = 5f;
    /// <summary>
    /// 积分.
    /// </summary>
    [HideInInspector]
    public int PlayerJiFen = 0;
    /// <summary>
    /// 积分产生点.
    /// </summary>
    public Transform SpawnJiFenTr;
    [HideInInspector]
    public float m_pTopSpeed = 100.0f;
    private float throttle = 0.0f;
    private float mJiaoTaBan = 0.0f;
    public bool canDrive = true;
    public WheelCollider m_wheel;
    public Transform m_pLookTarget;
    public Transform m_massCenter;
    public Transform TestCube;
    bool IsIntoFeiBan;
    public bool m_IsFinished = false;
    public float m_timmerFinished = 0.0f;
    public ColorCorrectionCurves m_ColorEffect;
    public UIController m_UIController;
    /// <summary>
    /// 运动的路程.
    /// </summary>
    //[HideInInspector]
    public float mDistanceMove = 0.0f;
    private Vector3 PosRecord;
    [HideInInspector]
    public float m_LimitSpeed = 10.0f;
    public float m_StopTimmerSet = 5.0f;
    static PlayerController _Instance = null;
    private bool m_IsErrorDirection = false;
    private bool m_IsInWarter = false;
    private bool m_IsOnRoad = false;
    [HideInInspector]
    public Transform m_pChuan;

    //feiyuepubu
    [HideInInspector]
    public bool m_IsPubu = false;
    private float m_pubuTimmer = 0.0f;
    [HideInInspector]
    public float m_PubuPower = 2000.0f;
    public float m_HitPower = 2000.0f;
    public float m_GravitySet = -2000.0f;

    //yangjiao
    public float[] m_XangleSet;
    public float[] m_XangleSpeedSet;

    //fujiao
    public float m_XangleFuchongMax = 30.0f;
    public float m_XangleFuchongTime = 2.0f;
    private float m_ResetPlayerTimmer = 0.0f;
    public float m_ResetPlayerTimeSet = 5.0f;
    public GameObject m_OutHedao;
    public GameObject m_ErrorDirection;
    private RaycastHit hit;
    private LayerMask mask;
    public float m_OffSet = 0.5f;

    //qingjiao
    public Transform m_ForfwardPos;
    public Transform m_BehindPos;
    private Vector3 m_ForwradHitPos;
    private Vector3 m_BehindHitPos;
    public float timmerstar = 0.0f;

    //lujingchuli
    [HideInInspector]
    public Transform[] PathPoint;
    public Transform Path;
    public int PathNum = 0;

    //jingtoumohu
    public RadialBlur m_RadialBlurEffect;
    /// <summary>
    /// 开始虚化的初始速度.
    /// </summary>
	public float m_SpeedForEffectStar = 0.0f;
    /// <summary>
    /// 虚化强度控制.
    /// </summary>
	public float m_ParameterForEfferct = 1.0f;
    /// <summary>
    /// 初始虚化参数.
    /// </summary>
    float m_StartForEfferct = 1.0f;
    /// <summary>
    /// 喷气虚化强度控制.
    /// </summary>
	public float m_ForEfferctPenQi = 1.0f;
    /// <summary>
    /// 飞行翼虚化强度控制.
    /// </summary>
	public float m_ForEfferctFeiXing = 1.0f;
    /// <summary>
    /// 双翼飞机虚化强度控制.
    /// </summary>
	public float m_ForEfferctShuangYiFeiJi = 1.0f;
    /// <summary>
    /// 加速风扇虚化强度控制.
    /// </summary>
	public float m_ForEfferctJiaSuFenShan = 1.0f;

    //yangjiaokongzhi
    public float m_SpeedForXangle = 0.0f;
    public float m_ParameterForXangle = 1.0f;

    //zuoyouxuanzhuansudu
    public float m_ParameterForRotate = 50.0f;

    //zuoyouqingjiao
    public float m_SpeedForZangle = 0.0f;
    public float m_ParameterForZangle = 1.0f;

    //shuihua
    public float m_speedForshuihua = 0.0f;
    public GameObject[] m_partical;
    private bool m_IsOffShuihua = false;

    public CameraShake m_CameraShake;
    public CameraSmooth m_CameraSmooth;
    //	private float m_SpeedRecord = 0.0f;

    private Vector3 m_HitDirection = Vector3.forward;

    public AudioSource m_HitStone;
    public GameObject m_HitEffectObj;
    public AudioSource m_HitWater;
    public AudioSource m_YinqingAudio;
    public AudioSource m_ShuihuaAudio;
    public AudioSource m_BeijingAudio;
    public AudioSource m_ErrorDirectionAudio;
    public AudioSource m_HuanjingSenlin;
    public AudioSource m_HuanjingShuiliu;
    public AudioSource m_FeibanAudio;
    public GameObject m_FeibanEffectObj;
    private Vector3 m_WaterDirection;
    private Vector3 m_OldWaterDirection;
    private bool m_HasChanged = false;

    //zhiwujiansu
    private bool m_IsInZhiwuCollider = false;
    public float m_ParameterForZhiwu = 1.0f;
    private Vector3 m_SpeedForZhiwu;

    private bool m_CanDrive = true;

    public GameObject m_HitWaterParticle;
    public float m_BaozhaForward = 0.0f;
    public float m_BaozhaUp = 0.0f;

    [HideInInspector]
    public Animator m_PlayerAnimator;
    private bool m_IsJiasu = false;
    public float m_JiasuTimeSet = 3.0f;
    private float m_JiasuTimmer = 0.0f;
    public GameObject m_JiasuPartical;
    public AudioSource m_JiasuAudio;
    /// <summary>
    /// 变形翼音效.
    /// </summary>
	public AudioSource mBianXingYiAudio;
    public AudioSource m_EatJiasuAudio;
    public float m_JiasuTopSpeed = 0.0f;
    public GameObject m_JiashiGameObject;

    public GameObject m_JiashiPartical;
    public AudioSource m_JiashiAudio;
    public AudioSource m_EatJiashiAudio;
    public AudioSource LaBaAudio;

    /// <summary>
    /// 排名数据.
    /// </summary>
    RankManage.RankData mRankDt = null;
    /// <summary>
    /// 最大圈数.
    /// </summary>
    [Range(1, 10)]
    [HideInInspector]
    public int QuanShuMax = 1;
    int QuanShuCount = 0;
    float TimeQuanShuVal = 0f;

    /// <summary>
    /// 初始化人物数据(只在控制端初始化).
    /// </summary>
    void InitPlayerData()
    {
        if (!IsNetControlPort)
        {
            //只在控制端初始化.
            return;
        }

        SSGameDataManage.PlayerData playerDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerDt;
        m_ColorEffect = playerDt.m_ColorEffect;
        m_UIController = playerDt.m_UIController;
        m_OutHedao = playerDt.m_OutHedao;
        m_ErrorDirection = playerDt.m_ErrorDirection;
        Path = playerDt.Path;
        m_RadialBlurEffect = playerDt.m_RadialBlurEffect;
        m_CameraShake = playerDt.m_CameraShake;
        m_CameraSmooth = playerDt.m_CameraSmooth;
        m_HitStone = playerDt.m_HitStone;
        m_HitWater = playerDt.m_HitWater;
        m_HitWater = playerDt.m_HitWater;
        m_YinqingAudio = playerDt.m_YinqingAudio;
        m_ShuihuaAudio = playerDt.m_ShuihuaAudio;
        m_BeijingAudio = playerDt.m_BeijingAudio;
        m_ErrorDirectionAudio = playerDt.m_ErrorDirectionAudio;
        m_HuanjingSenlin = playerDt.m_HuanjingSenlin;
        m_HuanjingShuiliu = playerDt.m_HuanjingShuiliu;
        m_FeibanAudio = playerDt.m_FeibanAudio;
        m_JiashiGameObject = playerDt.m_JiashiGameObject;
        m_JiasuAudio = playerDt.m_JiasuAudio;
        mBianXingYiAudio = playerDt.mBianXingYiAudio;
        m_EatJiasuAudio = playerDt.m_EatJiasuAudio;
        m_JiashiAudio = playerDt.m_JiashiAudio;
        m_EatJiashiAudio = playerDt.m_EatJiashiAudio;
        LaBaAudio = playerDt.LaBaAudio;

        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerControllerList.Add(this);
        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerController = this;
        m_UIController.m_Player = this;
        m_CameraSmooth.target = transform;
        mNetViewCom = GetComponent<NetworkView>();
        mNetSynGame.Init(mNetViewCom);
    }

    /// <summary>
    /// 网络消息控制器.
    /// </summary>
    NetworkView mNetViewCom;
    /// <summary>
    /// 设置人物索引.
    /// </summary>
    public void SetPlayerIndex(int index)
    {
        InitPlayerData();
        InitPlayerMode(index);
        InitStartInfo();
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            m_LimitSpeed = 0f;
            mNetViewCom.RPC("RpcGetPlayerIndex", RPCMode.OthersBuffered, index);
            NetSendPlayerTrInfo();
        }
    }

    [RPC]
    void RpcGetPlayerIndex(int index)
    {
        InitPlayerMode(index);
    }

    /// <summary>
    /// 初始化人物模型.
    /// </summary>
    void InitPlayerMode(int index)
    {
        Debug.Log("InitPlayerMode -> index == " + index);
        name = "zhuJiao_" + index;
        for (int i = 0; i < PlayerObjArray.Length; i++)
        {
            PlayerObjArray[i].SetActive(index == i ? true : false);
            if (PlayerDt[i] != null && index == i)
            {
                m_pChuan = PlayerObjArray[index].transform;
                m_PlayerAnimator = PlayerObjArray[index].GetComponent<Animator>();
                PenQiAniAy = PlayerDt[i].PenQiAniAy;
                FiXingYiAniAy = PlayerDt[i].FiXingYiAniAy;
                ShuangYiFeiJiAniAy = PlayerDt[i].ShuangYiFeiJiAniAy;
                ShuangYiFeiJiTwRot = PlayerDt[i].ShuangYiFeiJiTwRot;
                FengKuangAni = PlayerDt[i].FengKuangAni;
                FengKuangTwRot = PlayerDt[i].FengKuangTwRot;
                SpawnJiFenTr = PlayerDt[i].SpawnJiFenTr;
                JiaoTaBanFenShanTr = PlayerDt[i].JiaoTaBanFenShanTr;
            }
        }

        if (PenQiAniAy != null && PenQiAniAy.Length > 0)
        {
            for (int i = 0; i < PenQiAniAy.Length; i++)
            {
                if (PenQiAniAy[i] != null)
                {
                    PenQiAniAy[i].transform.localScale = Vector3.zero;
                }
            }
        }

        if (FiXingYiAniAy != null && FiXingYiAniAy.Length > 0)
        {
            for (int i = 0; i < FiXingYiAniAy.Length; i++)
            {
                if (FiXingYiAniAy[i] != null)
                {
                    FiXingYiAniAy[i].transform.localScale = Vector3.zero;
                }
            }
        }

        SSGameDataManage.PlayerData playerDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerDt;
        m_UIController = playerDt.m_UIController;

        mNetViewCom = GetComponent<NetworkView>();
        FinishRender = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.FinishRender;
        QuanShuMax = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.QuanShuMax;
        mNetSynGame.InitData(m_PlayerAnimator);
        SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.AddAiNpcTr(transform);
        SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.AddPlayerTr(transform);
        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.mNetPlayerComList.Add(this);
        mRankDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.AddRankDt((RankManage.RankEnum)index, IsNetControlPort);

        if (QuanShuMax > 1)
        {
            for (int i = 0; i < FinishRender.Length; i++)
            {
                if (FinishRender[i] != null)
                {
                    FinishRender[i].enabled = false;
                }
            }
            IsShowZhongDian = false;
        }
        else
        {
            IsShowZhongDian = true;
        }
        rigidbody.isKinematic = true;
    }

    /// <summary>
    /// 只在控制端初始化.
    /// </summary>
	void InitStartInfo()
    {
        if (!IsNetControlPort)
        {
            //只在控制端初始化.
            return;
        }
        InputEventCtrl.GetInstance().mListenPcInputEvent.ClickFireBtEvent += ClickFireBtEvent;
        InputEventCtrl.GetInstance().mListenPcInputEvent.ClickStartBtOneEvent += ClickStartBtOneEvent;
        m_PlayerAnimator = m_pChuan.GetComponent<Animator>();
        m_StartForEfferct = m_ParameterForEfferct;
        //PlayerMinSpeedVal = ReadGameInfo.GetInstance().ReadPlayerMinSpeedVal();
        PlayerMinSpeedVal = 80f;
        Loading.m_HasBegin = false;
        //pcvr.ShaCheBtLight = StartLightState.Liang;
        //pcvr.IsSlowLoopCom = false;
        //pcvr.CountFXZD = 0;
        //pcvr.CountQNZD = 0;
        //pcvr.OpenFangXiangPanPower();
        Screen.showCursor = false;
        Time.timeScale = 1f;

        //pcvr.GetInstance();
        //m_pTopSpeed = Convert_Miles_Per_Hour_To_Meters_Per_Second(m_pTopSpeed);
        rigidbody.centerOfMass = m_massCenter.localPosition;
        mask = 1 << (LayerMask.NameToLayer("shexianjiance"));

        PathPoint = new Transform[Path.childCount];
        //		Debug.Log (Path.childCount);
        for (int i = 0; i < Path.childCount; i++)
        {
            string str = (i + 1).ToString();
            PathPoint[i] = Path.FindChild(str);
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            transform.position = PathPoint[0].position;
            transform.eulerAngles = new Vector3(PathPoint[0].eulerAngles.x, PathPoint[0].eulerAngles.y, PathPoint[0].eulerAngles.z);
        }
        m_WaterDirection = m_OldWaterDirection = PathPoint[1].position - PathPoint[0].position;
        //		m_SpeedRecord = rigidbody.velocity.magnitude;
        _Instance = this;
        //InputEventCtrl.GetInstance().ClickShaCheBtEvent += ClickShaCheBtEvent;
        InputEventCtrl.GetInstance().mListenPcInputEvent.ClickLaBaBtEvent += ClickLaBaBtEvent;
        Invoke("DelayCallClickShaCheBtEvent", 0.5f);

        StartCoroutine(GameObjectHide());   //gzknu
    }

    void ClickFireBtEvent(InputEventCtrl.ButtonState val)
    {
        if (val != InputEventCtrl.ButtonState.DOWN)
        {
            return;
        }

        if (!IsNetControlPort)
        {
            return;
        }

        if (m_IsFinished || m_UIController.m_IsGameOver)
        {
            return;
        }

        if (IsAmmoHitPlayer)
        {
            if (m_UIController.mPlayerDaoJuManageUI.DaoDanNum > 0 || m_UIController.mPlayerDaoJuManageUI.DiLeiNum > 0)
            {
                m_UIController.SpawnJinYongAmmoUI();
                return;
            }
        }

        if (m_UIController.mTaoQuanUI == null)
        {
            return;
        }

        if (m_UIController.mPlayerDaoJuManageUI.DaoDanNum > 0)
        {
            m_UIController.mPlayerDaoJuManageUI.DaoDanNum--;
            //GameObject zhangAiWuObj = SSGameCtrl.GetInstance().mPlayerDataManage.mDaoJuZhangAiWuData.FindZhangAiWu(transform);
            //OnPlayerHitDaoDanDaoJu(zhangAiWuObj);
            OnPlayerHitDaoDanDaoJu(null);
        }
        else if (m_UIController.mPlayerDaoJuManageUI.DiLeiNum > 0)
        {
            m_UIController.mPlayerDaoJuManageUI.DiLeiNum--;
            //GameObject zhangAiWuObj = SSGameCtrl.GetInstance().mPlayerDataManage.mDaoJuZhangAiWuData.FindZhangAiWu(transform);
            OnPlayerHitDiLeiDaoJu(null);
        }

        if (m_UIController.mPlayerDaoJuManageUI.DaoDanNum <= 0 && m_UIController.mPlayerDaoJuManageUI.DiLeiNum <= 0)
        {
            if (m_UIController.mTaoQuanUI != null)
            {
                m_UIController.mTaoQuanUI.DestroySelf();
            }
        }
    }

    void ClickStartBtOneEvent(InputEventCtrl.ButtonState val)
    {
        if (val != InputEventCtrl.ButtonState.DOWN)
        {
            return;
        }

        if (timmerstar < 5f)
        {
            return;
        }

        if (m_IsFinished || m_UIController.m_IsGameOver)
        {
            return;
        }

        if (mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi)
        {
            return;
        }

        if (SSGameCtrl.GetInstance().mPlayerDataManage.PlayerCoinNum < SSGameCtrl.GetInstance().mPlayerDataManage.CoinNumFeiXing)
        {
            m_UIController.PlayInsertCoinAniOnClickJiaSu();
            return;
        }
        SSGameCtrl.GetInstance().mPlayerDataManage.PlayerCoinNum -= SSGameCtrl.GetInstance().mPlayerDataManage.CoinNumFeiXing;
        m_UIController.mTouBiInfo.UpdateInsertCoin();
        OpenPlayerDaoJuAni(DaoJuCtrl.DaoJuType.FeiXingYi);
        m_UIController.RemoveChaoJiJiaSuUI();
    }

    public static int GameGradeVal = 2;
    IEnumerator GameObjectHide()        //gzknu
    {
        yield return new WaitForSeconds(1.0f);

        GameGradeVal = PlayerPrefs.GetInt("Grade");
        switch (GameGradeVal)
        {
            case 1: //简单
                {
                    string[] ObjNameToHide = {
                    "Obstacle/Stone_1/_newCreation_Tris537_201",
                    "Obstacle/Stone_1/_newCreation_Tris601_p",
                    "Obstacle/Stone_2/UC_SmallRock_05_p",
                    "Obstacle/Stone_2/_newCreation_Tris537_p",
                    "Obstacle/wood_3/_newCreation_Tris537_p",
                    "Obstacle/Stone_3/O2/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris601_p",

                    "Obstacle/wood_2",
                    "Obstacle/wood_3",
                    "Obstacle/Stone_4",
                    "Obstacle/Stone_5",
                    "Obstacle/Stone_6",
                    "Obstacle/Stone_7",
                    "Obstacle/Stone_8",
                    "Obstacle/Stone_9",
                    "Obstacle/Stone_10",

                                              };

                    foreach (string go in ObjNameToHide)
                    {
                        GameObject game_obj = GameObject.Find(go);
                        if (game_obj != null)
                        {
                            game_obj.SetActive(false);
                        }
                    }
                }
                break;

            case 2: //中等
                {
                    string[] ObjNameToHide = {
                    "Obstacle/Stone_1/_newCreation_Tris537_201",
                    "Obstacle/Stone_1/_newCreation_Tris601_p",
                    "Obstacle/Stone_2/UC_SmallRock_05_p",
                    "Obstacle/Stone_2/_newCreation_Tris537_p",
                    "Obstacle/wood_3/_newCreation_Tris537_p",
                    "Obstacle/Stone_3/O2/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris537_p",
                    "Obstacle/Stone_4/_newCreation_Tris601_p",

                                              };

                    foreach (string go in ObjNameToHide)
                    {
                        GameObject game_obj = GameObject.Find(go);
                        if (game_obj != null)
                        {
                            game_obj.SetActive(false);
                        }
                    }
                }
                break;

            case 3: //难
                break;

            default:
                break;
        }

    }

    void DelayCallClickShaCheBtEvent()
    {
        ClickShaCheBtEvent(InputEventCtrl.ButtonState.UP);
    }

    public static PlayerController GetInstance()
    {
        return _Instance;
    }

    bool IsClickShaCheBt;
    void ClickShaCheBtEvent(InputEventCtrl.ButtonState val)
    {
        if (Application.loadedLevel != 1)
        {
            return;
        }

        if (val == InputEventCtrl.ButtonState.DOWN)
        {
            IsClickShaCheBt = true;
            //pcvr.ShaCheBtLight = StartLightState.Liang;
        }
        else
        {
            IsClickShaCheBt = false;
            //pcvr.ShaCheBtLight = StartLightState.Shan;
        }
    }

    void ClickLaBaBtEvent(InputEventCtrl.ButtonState val)
    {
        if (val != InputEventCtrl.ButtonState.DOWN)
        {
            LaBaAudio.Stop();
            return;
        }
        LaBaAudio.Play();
    }

    /// <summary>
    /// 回头动画参数.
    /// </summary>
    bool IsPlayHuiTouAni = false;
    float TimeLastHuiTou = 0f;
    float TimeRandHuiTou = 0f;

    /// <summary>
    /// 发射导弹UI产生时间记录.
    /// </summary>
    float TimeLastFaShaDaoDanUI = -100f;
    /// <summary>
    /// 超级加速UI产生时间记录.
    /// </summary>
    float TimeLastChaoJiJiaSuUI = -15f;

    bool IsInitStartGameInfo = false;
    void InitStartGameInfo()
    {
        IsInitStartGameInfo = true;
        rigidbody.isKinematic = false;
    }

    void Update()
    {
        if (!IsNetControlPort)
        {
            return;
        }

        if (timmerstar < 5.0f)
        {
            if (m_UIController.IsGameStart)
            {
                timmerstar += Time.deltaTime;
            }

            if (timmerstar >= 5f)
            {
                float timeVal = Time.time;
                SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.SetTimeStartVal(timeVal);
                SendPlayerPlayerRankTimeServerStartVal(timeVal);
            }
            PosRecord = transform.position;
        }
        else
        {
            //if (IsAmmoHitPlayer)
            //{
            //    if (Time.realtimeSinceStartup - TimeLastAmmoHit >= 3f)
            //    {
            //        IsAmmoHitPlayer = false;
            //        m_UIController.RemoveJinYongAmmo();
            //    }
            //}

            if (Time.frameCount % 3 == 0 && !m_IsFinished && !m_UIController.m_IsGameOver)
            {
                CheckPlayerIsCanFire();
            }

            if (!m_UIController.m_IsGameOver && !m_IsFinished)
            {
                if (Time.time - TimeLastChaoJiJiaSuUI > 1f
                    && m_UIController.ChaoJiJiaSuObj == null
                    && mSpeedDaoJuState != DaoJuCtrl.DaoJuType.FeiXingYi)
                {
                    GameObject aimNpc = SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.FindAiNpc(transform, MaxChaoJiJiaSuDisNpc, MinChaoJiJiaSuDisNpc);
                    if (aimNpc != null)
                    {
                        //30秒触发一次.
                        TimeLastChaoJiJiaSuUI = Time.time;
                        m_UIController.SpawnChaoJiJiaSuUI();
                    }
                }
            }

            if (m_UIController.mPlayerDaoJuManageUI.DaoDanNum > 0 || m_UIController.mPlayerDaoJuManageUI.DiLeiNum > 0)
            {
                if (Time.time - TimeLastFaShaDaoDanUI > 1f
                    && m_UIController.ChaoJiJiaSuObj == null
                    && m_UIController.FaSheDaoDanObj == null
                    && !m_UIController.m_IsGameOver
                    && !m_IsFinished)
                {
                    GameObject aimNpc = SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.FindAiNpc(transform, DaoDanDisNpc, MinDaoDanDisNpc);
                    if (aimNpc != null)
                    {
                        //10秒内不允许再提示发射导弹.
                        TimeLastFaShaDaoDanUI = Time.time;
                        m_UIController.SpawnFaSheDaoDanUI();
                    }
                }
            }

            if (QuanShuCount >= QuanShuMax - 1 && Time.time - TimeQuanShuVal > 5f && !IsShowZhongDian)
            {
                //显示终点.
                for (int i = 0; i < FinishRender.Length; i++)
                {
                    FinishRender[i].enabled = true;
                }
                IsShowZhongDian = true;
            }

            if (IsPlayHuiTouAni)
            {
                IsPlayHuiTouAni = false;
                m_IsHitshake = true;
                /*if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetTrigger("IsDiaoluo");
                    mNetSynGame.SynNetAnimator("IsDiaoluo", NetworkSynchronizeGame.AnimatorType.Trigger);
                }*/
                TimeRandHuiTou = UnityEngine.Random.Range(3, 15);
                TimeLastHuiTou = Time.time;
            }
            else
            {
                if (Time.time - TimeLastHuiTou >= TimeRandHuiTou)
                {
                    IsPlayHuiTouAni = true;
                }
            }

            if (IsOpenCiTieDaoJu)
            {
                if (Time.time - TimeLastCiTie >= TimeOpenCiTie)
                {
                    IsOpenCiTieDaoJu = false;
                }
            }

            if (mSpeedDaoJuState != DaoJuCtrl.DaoJuType.Null)
            {
                if (Time.time - TimeLastDaoJuBianXing >= DaoJuBianXingTime)
                {
                    ClosePlayerDaoJuAni(mSpeedDaoJuState);
                }
            }

            //if (SpeedMovePlayer > 105f && !m_IsFinished)
            //{
                //if (!m_IsHitshake)
                //{
                    //if (pcvr.m_IsOpneLeftQinang || pcvr.m_IsOpneRightQinang) {
                    //	pcvr.m_IsOpneForwardQinang = false;
                    //	pcvr.m_IsOpneBehindQinang = false;
                    //}
                    //else {
                    //pcvr.m_IsOpneForwardQinang = true;
                    //pcvr.m_IsOpneBehindQinang = false;
                    //}
                //}
            //}
            //else
            //{
            //    if (!m_IsHitshake)
            //    {
                    //pcvr.m_IsOpneForwardQinang = false;
                    //pcvr.m_IsOpneBehindQinang = false;
            //    }
            //}

            if (!m_BeijingAudio.isPlaying)
            {
                m_BeijingAudio.Play();
            }
            CalculateState();

            if (!m_IsFinished && !m_UIController.m_IsGameOver)
            {
                UpdateCameraEffect();
                UpdateShuihua();
                //				m_SpeedRecord = rigidbody.velocity.magnitude;
                m_YinqingAudio.volume = rigidbody.velocity.magnitude * 3.6f / 120.0f;
                ResetPlayer();
                OnHitShake();
            }
            else
            {
                m_HuanjingSenlin.Stop();
                m_HuanjingShuiliu.Stop();
                m_ShuihuaAudio.Stop();
                m_YinqingAudio.Stop();
                if (m_UIController.m_IsGameOver && m_BeijingAudio.isPlaying)
                {
                    m_BeijingAudio.Stop();
                }

                m_ErrorDirectionAudio.Stop();
                m_OutHedao.SetActive(false);
                m_ErrorDirection.SetActive(false);
                m_timmerFinished += Time.deltaTime;
                if (m_timmerFinished > 1.5f)
                {
                    m_ColorEffect.saturation = 0.0f;
                }
            }

            if (!m_IsFinished)
            {
                float length = Vector3.Distance(PosRecord, transform.position);
                mDistanceMove += length;
                PosRecord = transform.position;
                if (mRankDt != null)
                {
                    if (!m_UIController.m_IsGameOver)
                    {
                        mRankDt.UpdataDisMoveValue(mDistanceMove);
                        SendPlayerDisMoveVal(mDistanceMove);
                    }
                }
            }

            if (!m_CanDrive && canDrive)
            {
                m_IsHitshake = true;
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetTrigger("IsZhuang");
                    mNetSynGame.SynNetAnimator("IsZhuang", NetworkSynchronizeGame.AnimatorType.Trigger);
                }
                m_CameraShake.setCameraShakeImpulseValue();
                if (m_IsInWarter && !m_IsOnRoad)
                {
                    m_HitWater.Play();
                    GameObject Tobject = (GameObject)Instantiate(m_HitWaterParticle, transform.position + transform.forward * m_BaozhaForward + Vector3.up * m_BaozhaUp, transform.rotation);
                    Destroy(Tobject, 0.5f);
                }
                else
                {
                    m_HitStone.Play();
                }
            }
            m_CanDrive = canDrive;
        }
    }

    void FixedUpdate()
    {
        if (!IsNetControlPort)
        {
            if (_Instance != null && _Instance.timmerstar >= 5.0f)
            {
                if (!IsInitStartGameInfo)
                {
                    InitStartGameInfo();
                }

                if (JiaoTaBanFenShanTr != null)
                {
                    //转动脚踏板风扇.
                    JiaoTaBanFenShanTr.Rotate(Vector3.forward * JiaoTaBanFenShanTopSpeed * mJiaoTaBan);
                }
            }
            return;
        }

        if (timmerstar >= 5f)
        {
            if (!IsInitStartGameInfo)
            {
                InitStartGameInfo();
            }
        }


        if (m_IsFinished || m_UIController.m_IsGameOver)
		{
			if (m_PlayerAnimator.gameObject.activeInHierarchy)
			{
				m_PlayerAnimator.SetBool("IsRun1", false);
				m_PlayerAnimator.SetBool("IsRun2", false);
				mNetSynGame.SynNetAnimator("IsRun1", NetworkSynchronizeGame.AnimatorType.Bool, false);
				mNetSynGame.SynNetAnimator("IsRun2", NetworkSynchronizeGame.AnimatorType.Bool, false);
			}
		}

        if (!m_IsFinished && !m_UIController.m_IsGameOver && timmerstar >= 5.0f)
        {
            //Debug.Log("rigidbody.velocity.magnitude*3.6f" + rigidbody.velocity.magnitude*3.6f);
            GetInput();
            m_pLookTarget.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
            CalculateEnginePower(canDrive);
        }

        if (!m_IsFinished && !m_UIController.m_IsGameOver && timmerstar >= 5f)
        {
            if (m_IsInWarter && rigidbody.velocity.magnitude * 3.6f < m_LimitSpeed && canDrive && !IsAmmoHitPlayer)
            {
                m_OldWaterDirection = Vector3.Lerp(m_OldWaterDirection, m_WaterDirection, Time.deltaTime);
                rigidbody.velocity = m_OldWaterDirection.normalized * m_LimitSpeed / 3.6f;
            }
        }

        if (m_IsJiasu && !IsAmmoHitPlayer)
        {
            m_JiasuTimmer += Time.deltaTime;
            if (m_JiasuTimmer < m_JiasuTimeSet)
            {
                rigidbody.velocity = 1.4f * transform.forward * m_JiasuTopSpeed / 3.6f;
            }
            else
            {
                m_IsJiasu = false;
                m_JiasuTimmer = 0f;
            }
        }
    }

    //float Convert_Miles_Per_Hour_To_Meters_Per_Second(float value)
    //{
    //    return value * 0.44704f;
    //}

    float mSteer;
    float mSteerTimeCur;
    float SteerOffset = 0.05f;
    /// <summary>
    /// 控制脚踏板动画切换的变量.
    /// </summary>
    [Range(0.1f, 0.9f)]
    public float JiaoTaBanAniVal = 0.5f;
    /// <summary>
    /// 脚踏板电量恢复数值.
    /// </summary>
    public float DianLiangHuiFuVal = 10f;
    /// <summary>
    /// 电池电量减少速度.
    /// </summary>
    public float DianLiangSubSpeed = 0.1f;
    void GetInput()
    {
        mJiaoTaBan = pcvr.GetInstance().mGetJiaoTaBan;
        NetSendPlayerJiaoTaBanVal(mJiaoTaBan);
        mSteer = pcvr.GetInstance().mGetSteer;
        if (!IsClickShaCheBt)
        {
            if (m_UIController.mPlayerDaoJuManageUI.DianLiangVal > 0f)
            {
                throttle = pcvr.GetInstance().mGetPower;
            }
            else
            {
                throttle = 0f;
            }
        }
        else
        {
            throttle = 0f;
            if (!m_IsJiasu && !IsIntoFeiBan && !IsAmmoHitPlayer)
            {
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, Time.deltaTime * 3f);
            }
        }

        if (throttle > 0f)
        {
            if (mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi || mSpeedDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu)
            {
                //喷气加速和飞行翼状态时不消耗电量.
            }
            else
            {
                if (m_UIController.mPlayerDaoJuManageUI.DianLiangVal > 0f)
                {
                    m_UIController.mPlayerDaoJuManageUI.DianLiangVal -= (Time.deltaTime * DianLiangSubSpeed);
                    if (m_UIController.mPlayerDaoJuManageUI.DianLiangVal < 0f)
                    {
                        m_UIController.mPlayerDaoJuManageUI.DianLiangVal = 0f;
                    }
                }
            }
        }

        if (m_IsFinished || m_UIController.m_IsGameOver)
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsRun1", false);
                m_PlayerAnimator.SetBool("IsRun2", false);
                mNetSynGame.SynNetAnimator("IsRun1", NetworkSynchronizeGame.AnimatorType.Bool, false);
                mNetSynGame.SynNetAnimator("IsRun2", NetworkSynchronizeGame.AnimatorType.Bool, false);
            }
        }
        else
        {
            if (mJiaoTaBan > JiaoTaBanAniVal)
            {
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetBool("IsRun2", true);
                    m_PlayerAnimator.SetBool("IsRun1", false);
                    mNetSynGame.SynNetAnimator("IsRun2", NetworkSynchronizeGame.AnimatorType.Bool, true);
                    mNetSynGame.SynNetAnimator("IsRun1", NetworkSynchronizeGame.AnimatorType.Bool, false);
                }
                m_UIController.mPlayerDaoJuManageUI.DianLiangVal += (mJiaoTaBan * DianLiangHuiFuVal);
            }
            else if (mJiaoTaBan > 0f)
            {
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetBool("IsRun1", true);
                    m_PlayerAnimator.SetBool("IsRun2", false);
                    mNetSynGame.SynNetAnimator("IsRun1", NetworkSynchronizeGame.AnimatorType.Bool, true);
                    mNetSynGame.SynNetAnimator("IsRun2", NetworkSynchronizeGame.AnimatorType.Bool, false);
                }
                m_UIController.mPlayerDaoJuManageUI.DianLiangVal += (mJiaoTaBan * DianLiangHuiFuVal);
            }
			else
			{
				if (m_PlayerAnimator.gameObject.activeInHierarchy)
				{
					m_PlayerAnimator.SetBool("IsRun1", false);
					m_PlayerAnimator.SetBool("IsRun2", false);
					mNetSynGame.SynNetAnimator("IsRun1", NetworkSynchronizeGame.AnimatorType.Bool, false);
					mNetSynGame.SynNetAnimator("IsRun2", NetworkSynchronizeGame.AnimatorType.Bool, false);
				}
			}
        }

        if (JiaoTaBanFenShanTr != null)
        {
            //转动脚踏板风扇.
            JiaoTaBanFenShanTr.Rotate(Vector3.forward * JiaoTaBanFenShanTopSpeed * mJiaoTaBan);
        }

        if (mSteer < -SteerOffset)
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsTurnleft", true);
                mNetSynGame.SynNetAnimator("IsTurnleft", NetworkSynchronizeGame.AnimatorType.Bool, true);
            }
            if (SpeedMovePlayer > 15f && !m_IsHitshake)
            {
                //pcvr.m_IsOpneRightQinang = true;
            }
        }
        else
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsTurnleft", false);
                mNetSynGame.SynNetAnimator("IsTurnleft", NetworkSynchronizeGame.AnimatorType.Bool, false);
            }
            if (!m_IsHitshake)
            {
                //pcvr.m_IsOpneRightQinang = false;
            }
        }

        if (mSteer > SteerOffset)
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsTurnRight", true);
                mNetSynGame.SynNetAnimator("IsTurnRight", NetworkSynchronizeGame.AnimatorType.Bool, true);
            }
            if (SpeedMovePlayer > 15f && !m_IsHitshake)
            {
                //pcvr.m_IsOpneLeftQinang = true;
            }
        }
        else
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsTurnRight", false);
                mNetSynGame.SynNetAnimator("IsTurnRight", NetworkSynchronizeGame.AnimatorType.Bool, false);
            }
            if (!m_IsHitshake)
            {
                //pcvr.m_IsOpneLeftQinang = false;
            }
        }
        //if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        if (Mathf.Abs(mSteer) < SteerOffset)
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsRoot", true);
                mNetSynGame.SynNetAnimator("IsRoot", NetworkSynchronizeGame.AnimatorType.Bool, true);
            }
        }
        else
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetBool("IsRoot", false);
                mNetSynGame.SynNetAnimator("IsRoot", NetworkSynchronizeGame.AnimatorType.Bool, false);
            }
        }
        if (canDrive && !m_IsPubu)
        {
            if (Physics.Raycast(m_ForfwardPos.position, -Vector3.up, out hit, 100.0f, mask.value))
            {
                //Debug.DrawLine(m_ForfwardPos.position,hit.point,Color.red);
                m_ForwradHitPos = hit.point;
            }
            if (Physics.Raycast(m_BehindPos.position, -Vector3.up, out hit, 100.0f, mask.value))
            {
                //Debug.DrawLine(m_BehindPos.position,hit.point,Color.red);
                m_BehindHitPos = hit.point;
            }
            float ytemp = Mathf.Abs(m_BehindHitPos.y - m_ForwradHitPos.y);
            if (ytemp <= 5.0f)
            {
                Vector3 CurrentDirection = Vector3.Normalize(m_ForwradHitPos - m_BehindHitPos);
                transform.forward = Vector3.Lerp(transform.forward, CurrentDirection, 10.0f * Time.deltaTime);
            }
        }

        //chuantouyangjiao
        if (rigidbody.velocity.magnitude * 3.6f > m_SpeedForXangle && rigidbody.velocity.magnitude * 3.6f < 90.0f)
        {
            m_pChuan.localEulerAngles = new Vector3(-m_ParameterForXangle * rigidbody.velocity.magnitude * 3.6f * rigidbody.velocity.magnitude * 3.6f,
                m_pChuan.localEulerAngles.y, m_pChuan.localEulerAngles.z);
        }
        else if (rigidbody.velocity.magnitude * 3.6f >= 90.0f)
        {
            m_pChuan.localEulerAngles = new Vector3(-m_ParameterForXangle * 90.0f * 90.0f,
                m_pChuan.localEulerAngles.y, m_pChuan.localEulerAngles.z);
        }
        if (!canDrive)
        {
            //Debug.Log("transform.localEulerAngles.x" + transform.localEulerAngles.x);
            if (transform.localEulerAngles.x < m_XangleFuchongMax || (transform.localEulerAngles.x >= 270.0f && transform.localEulerAngles.x <= 360.0f))
            {
                transform.Rotate(new Vector3(Time.deltaTime * m_XangleFuchongMax / m_XangleFuchongTime, 0.0f, 0.0f));
            }
            else
            {
                transform.localEulerAngles = new Vector3(42.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
            }
            //Debug.Log("transform.localEulerAngles.x" + transform.localEulerAngles.x);
            //transform.localEulerAngles = new Vector3(30.0f,transform.localEulerAngles.y,transform.localEulerAngles.z);
        }
        if (Mathf.Abs(mSteer) < 0.05f)
        {
            mSteer = 0f;
        }

        float rotSpeed = m_ParameterForRotate * mSteer * Time.smoothDeltaTime;
        transform.Rotate(0, rotSpeed, 0);
        float angleZ = 0.0f;
        if (rigidbody.velocity.magnitude * 3.6f >= m_SpeedForZangle)
        {
            angleZ = -mSteer * m_ParameterForZangle * rigidbody.velocity.magnitude * 3.6f * rigidbody.velocity.magnitude * 3.6f;
            if (angleZ < -42f)
            {
                angleZ = -42f;
            }
            else if (angleZ > 42f)
            {
                angleZ = 42f;
            }
        }
        m_pChuan.localEulerAngles = new Vector3(m_pChuan.localEulerAngles.x, m_pChuan.localEulerAngles.y, angleZ);
    }

    private bool m_hasplay = false;
    void CalculateState()
    {
        if (Physics.Raycast(m_massCenter.position + Vector3.up * 10.0f, -Vector3.up, out hit, 100.0f, mask.value))
        {
            if (Vector3.Distance(m_massCenter.position, hit.point) > m_OffSet)
            {
                //Debug.Log("Vector3.Distance(m_massCenter.position,hit.point)" + Vector3.Distance(m_massCenter.position,hit.point));
                if (!m_FeibanAudio.isPlaying && !m_hasplay)
                {
                    m_FeibanAudio.Play();
                    Instantiate(m_FeibanEffectObj, transform.localPosition, transform.rotation);
                    m_hasplay = true;
                }
                canDrive = false;
                m_IsOffShuihua = true;
                if (Vector3.Angle(m_HitDirection, Vector3.forward) >= -0.01f && Vector3.Angle(m_HitDirection, Vector3.forward) <= 0.01f)
                {
                    m_HitDirection = transform.forward;
                }
            }
            else
            {
                m_hasplay = false;
                m_HitDirection = Vector3.forward;
                m_IsOffShuihua = false;
                canDrive = true;
                if (m_pubuTimmer > 1.0f)
                {
                    m_IsPubu = false;
                    m_pubuTimmer = 0.0f;
                }
            }
        }
        if (m_IsOnRoad && !m_IsInWarter)
        {
            m_IsOffShuihua = true;
        }
    }

    /// <summary>
    /// 玩家运动的速度.
    /// </summary>
    [HideInInspector]
    public float SpeedMovePlayer;
    void OnGUI()
    {
        float wVal = Screen.width;
        float hVal = 20f;
        /*string strC = "m_IsOpneForwardQinang "+pcvr.m_IsOpneForwardQinang
			+", m_IsOpneBehindQinang "+pcvr.m_IsOpneBehindQinang
				+", m_IsOpneLeftQinang "+pcvr.m_IsOpneLeftQinang
				+", m_IsOpneRightQinang "+pcvr.m_IsOpneRightQinang;
		GUI.Box(new Rect(0f, hVal * 2f, wVal, hVal), strC);*/

        //if (pcvr.IsJiOuJiaoYanFailed) {
        //	//JiOuJiaoYanFailed
        //	string jiOuJiaoYanStr = "*********************************************************\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4r8t416saf4bf164ve7t868\n"
        //		+ "1489+1871624537416876467816684dtrsd3541sy3t6f654s68dkfgt4saf4JOJYStr45dfssd\n"
        //		+ "*********************************************************";
        //	GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), jiOuJiaoYanStr);
        //}
        //else if (pcvr.IsJiaMiJiaoYanFailed) {

        //	string JMJYStr = "*********************************************************\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "sdkgfksfgsdfggf64h76hg4j35dhghdga3f5sd34f3ds35135d4g5ds6g4sd6a4fg564dafg64f\n"
        //		+ "gh4j1489+1871624537416876467816684dtrsd3541sy3t6f654s68t4saf4JMJYStr45dfssd\n"
        //		+ "*********************************************************";
        //	GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), JMJYStr);
        //}

        float sp = rigidbody.velocity.magnitude * 3.6f;
        sp = Mathf.Floor(sp);
        float dSpeed = SpeedMovePlayer - sp;
        if (dSpeed > 30f)
        {
            m_IsHitshake = true;
            //pcvr.GetInstance().OpenFangXiangPanZhenDong();
        }
        SpeedMovePlayer = sp;
        PlayerMoveSpeed = (int)sp;

#if UNITY_EDITOR
        if (!pcvr.bIsHardWare)
        {
            string strA = sp.ToString() + "km/h";
            GUI.Box(new Rect(0f, 0f, wVal, hVal), strA);

            string strB = "throttle " + throttle.ToString("f3") + ", steer " + pcvr.GetInstance().mGetSteer.ToString("f3")
                + ", jiaoTaBan " + pcvr.GetInstance().mGetJiaoTaBan.ToString("f2");
            GUI.Box(new Rect(0f, hVal, wVal, hVal), strB);
        }
#endif
    }

    int _PlayerMoveSpeed;
    /// <summary>
    /// 主角运动速度.
    /// </summary>
    public int PlayerMoveSpeed
    {
        set
        {
            if (value != _PlayerMoveSpeed)
            {
                _PlayerMoveSpeed = value;
                if (m_UIController != null)
                {
                    m_UIController.UpdatePlayerMoveSpeed(_PlayerMoveSpeed);
                }
            }
        }
        get
        {
            return _PlayerMoveSpeed;
        }
    }

    float PlayerMinSpeedVal = 80f;
    void CalculateEnginePower(bool canDrive)
    {
        if (mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi || mSpeedDaoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu)
        {
            //喷气加速和飞行翼状态时不用踩脚踏板或加油门.
            if (SpeedMovePlayer <= m_pTopSpeed && !m_IsPubu && !IsAmmoHitPlayer)
            {
                float speedVal = m_pTopSpeed;
                speedVal /= 3.2f;   //gzkun//3.6f;
                rigidbody.velocity = speedVal * transform.forward;
            }
        }
        else
        {
            if (throttle > 0f || mJiaoTaBan > 0f)
            {
                if (SpeedMovePlayer <= m_pTopSpeed && !m_IsPubu && !IsAmmoHitPlayer)
                {
                    float youMenSpeedVal = m_pTopSpeed * throttle;
                    float jiaoTaBanSpeedVal = mTopJiaoTaSpeed < m_pTopSpeed ? (mTopJiaoTaSpeed * mJiaoTaBan) : m_pTopSpeed * mJiaoTaBan;
                    float speedVal = youMenSpeedVal > jiaoTaBanSpeedVal ? youMenSpeedVal : jiaoTaBanSpeedVal;
                    speedVal = speedVal < PlayerMinSpeedVal ? PlayerMinSpeedVal : speedVal;
                    speedVal /= 3.2f;   //gzkun//3.6f;
                    rigidbody.velocity = speedVal * transform.forward;
                }
            }
        }

        if (m_IsPubu)
        {
            m_pubuTimmer += Time.deltaTime;
            float throttleForce = rigidbody.mass * m_PubuPower;
            rigidbody.AddForce(transform.forward * Time.deltaTime * (throttleForce));
        }

        if (!canDrive && !m_IsPubu)
        {
            //float throttleForce = rigidbody.mass * m_HitPower;
            rigidbody.AddForce(Vector3.up * m_GravitySet * rigidbody.mass * Time.deltaTime);
        }

        if (m_IsInZhiwuCollider && !IsAmmoHitPlayer)
        {
            if (rigidbody.velocity.magnitude > 15.0f)
            {
                rigidbody.velocity = transform.forward * m_SpeedForZhiwu.magnitude * m_ParameterForZhiwu;
                //Debug.Log("rigidbody.velocity" + rigidbody.velocity.magnitude);
            }
            else
            {
                rigidbody.velocity = transform.forward * 15.0f / 3.6f;
            }
        }

        if (IsIntoFeiBan && !IsAmmoHitPlayer)
        {
            if (Time.realtimeSinceStartup - TimeFeiBan > 0.8f)
            {
                ResetIsIntoFeiBan();
            }
            rigidbody.velocity = (transform.forward * 200f) / 3.6f;

            //gzknu
            //if (!m_IsHitshake)
            //{
                //pcvr.m_IsOpneForwardQinang = true;
                //pcvr.m_IsOpneBehindQinang = false;
            //}
        }
        //OnNpcHitPlayer();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsNetControlPort)
        {
            return;
        }

        DaoJuCtrl daoJuCom = other.GetComponent<DaoJuCtrl>();
        if (daoJuCom != null)
        {
            if (IsNetControlPort)
            {
                //玩家主控制端碰上障碍物道具时,通知其他客户端销毁该物体.
                NetSendAmmoHitZhangAiWuInfo(daoJuCom);
            }
            daoJuCom.OnDestroyThis(true);
            return;
        }

        if (other.tag == "bianxian")
        {
            m_IsJiasu = false;
        }

        if (other.tag == "finish" && m_UIController.m_pGameTime > 0f && m_UIController.TimeNetEndVal > 0f)
        {
            if (Time.time - TimeQuanShuVal >= 20f)
            {
                TimeQuanShuVal = Time.time;
                QuanShuCount++;
                if (QuanShuMax <= QuanShuCount)
                {
                    if (!m_IsFinished)
                    {
                        if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client)
                        {
                            if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                            {
                                //没有玩家选择链接服务器进行联机游戏.
                            }
                            else
                            {
                                //到达终点.
                                SendOnPlayerMoveToFinishPoint();
                            }
                        }
                    }

                    mRankDt.UpdateRankDtTimeFinish(Time.time);
                    SendPlayerUpdateRankDtTimeFinish(Time.time);
                    m_IsFinished = true;
                    if (m_PlayerAnimator.gameObject.activeInHierarchy)
                    {
                        m_PlayerAnimator.SetBool("IsFinish", true);
                        mNetSynGame.SynNetAnimator("IsFinish", NetworkSynchronizeGame.AnimatorType.Bool, true);
                    }
                    GlobalData.GetInstance().AddYiWanChengLevel(Application.loadedLevel);

                    if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
                    {
                        if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                        {
                            //没有玩家选择链接服务器进行联机游戏.
                        }
                        else
                        {
                            //多人联机.
                            m_UIController.SpawnTiaoZhanChengGongUI();
                        }
                    }
                }
            }
        }

        if (other.tag == "water")
        {
            m_IsInWarter = true;
        }
        if (other.tag == "road")
        {
            if (!m_IsOnRoad)
            {
                //pcvr.GetInstance().OpenFangXiangPanZhenDong();
            }
            m_IsOnRoad = true;
        }
        if (other.tag == "pathpoint")
        {
            PathNum = Convert.ToInt32(other.name) - 1;
            //Debug.Log("PathNum PathNum PathNum" + PathNum);
            mRankDt.UpdateRankDtPathPoint(Convert.ToInt32(other.name), Time.time);
            SendPlayerUpdateRankDtPathPoint(Convert.ToInt32(other.name), Time.time);
        }
        if (other.tag == "zhangai")
        {
            if (/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude * 3.6f > 30.0f)
            {
                //IsHitRock = true;
                m_IsHitshake = true;
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetTrigger("IsZhuang");
                    mNetSynGame.SynNetAnimator("IsZhuang", NetworkSynchronizeGame.AnimatorType.Trigger);
                }
                m_CameraShake.setCameraShakeImpulseValue();
                m_HitStone.Play();
                Instantiate(m_HitEffectObj, transform.position, transform.rotation);
            }
        }
        if (other.tag == "zhaoshi")
        {
            if (/*m_SpeedRecord*3.6f - */rigidbody.velocity.magnitude * 3.6f > 30.0f)
            {
                m_IsHitshake = true;
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetTrigger("IsZhuang");
                    mNetSynGame.SynNetAnimator("IsZhuang", NetworkSynchronizeGame.AnimatorType.Trigger);
                }
                m_CameraShake.setCameraShakeImpulseValue();
                //m_HitStone.Play();
                //GameObject temp = (GameObject)Instantiate(m_HitEffectObj,transform.position,transform.rotation);
            }
        }
        if (other.tag == "qiao")
        {
            if (m_IsInWarter)
            {
                //IsHitRock = true;
                m_IsHitshake = true;
                m_CameraShake.setCameraShakeImpulseValue();
                m_HitStone.Play();
                Instantiate(m_HitEffectObj, other.transform.position, other.transform.rotation);
            }
        }
        //gzknu
        //if(other.tag == "zhiwu")
        //{
        //	m_IsHitshake = true;
        //	m_IsInZhiwuCollider = true;
        //	m_SpeedForZhiwu = rigidbody.velocity;
        //	//pcvr.GetInstance().OpenFangXiangPanZhenDong();
        //}
        if (other.tag == "feibananimtor")
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetTrigger("IsQifei");
                mNetSynGame.SynNetAnimator("IsQifei", NetworkSynchronizeGame.AnimatorType.Trigger);
            }
        }
        if (other.tag == "pubuanimtor")
        {
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetTrigger("IsDiaoluo");
                mNetSynGame.SynNetAnimator("IsDiaoluo", NetworkSynchronizeGame.AnimatorType.Trigger);
            }
        }

        NpcController npcScript = other.GetComponent<NpcController>();
        if (npcScript)
        {
            if (rigidbody.velocity.magnitude * 3.6f > 30.0f)
            {
                m_IsHitshake = true;
                if (m_PlayerAnimator.gameObject.activeInHierarchy)
                {
                    m_PlayerAnimator.SetTrigger("IsZhuang");
                    mNetSynGame.SynNetAnimator("IsZhuang", NetworkSynchronizeGame.AnimatorType.Trigger);
                }
                m_CameraShake.setCameraShakeImpulseValue();
                m_HitStone.Play();
                Instantiate(m_HitEffectObj, transform.position, transform.rotation);
            }
            npcScript.m_IsHit = true;
            npcScript.m_PlayerHit = transform.position;
            npcScript.m_NpcPos = npcScript.transform.position;
            OnNpcHitPlayer(npcScript);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsNetControlPort)
        {
            return;
        }

        if (other.tag == "water")
        {
            m_IsInWarter = false;
        }
        if (other.tag == "road")
        {
            m_IsOnRoad = false;
        }
        if (other.tag == "pubu")
        {
            m_HasChanged = true;
            m_CameraSmooth.PositionForward = 6.0f;
            m_CameraSmooth.PositionUp = 5.0f;
            m_CameraSmooth.speed = 5.0f;
        }
        if (other.tag == "zhiwu")
        {
            m_IsInZhiwuCollider = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!IsNetControlPort)
        {
            return;
        }

        if (other.tag == "water")
        {
            m_IsInWarter = true;
        }
        if (other.tag == "road")
        {
            m_IsOnRoad = true;
        }
        if (other.tag == "feiban")
        {
            if (!IsIntoFeiBan)
            {
                TimeFeiBan = Time.realtimeSinceStartup;
                IsIntoFeiBan = true;
                m_IsHitshake = true;
                //pcvr.GetInstance().OpenFangXiangPanZhenDong();
            }
        }
        if (other.tag == "dan")
        {
            m_IsHitshake = true;
            //pcvr.GetInstance().OpenFangXiangPanZhenDong();
            m_IsJiasu = true;
            m_EatJiasuAudio.Play();
            m_JiasuAudio.Play();
            GameObject temp = (GameObject)Instantiate(m_JiasuPartical, other.transform.position, other.transform.rotation);
            Destroy(temp, 0.5f);
            Destroy(other.gameObject);
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            //只在单机状态下加时.
            if (other.tag == "zhong" && !m_UIController.m_IsGameOver)
            {
                m_IsHitshake = true;
                //pcvr.GetInstance().OpenFangXiangPanZhenDong();
                GameObject temp = (GameObject)Instantiate(m_JiashiPartical, other.transform.position, other.transform.rotation);
                Destroy(other.gameObject);
                Destroy(temp, 0.5f);
                m_EatJiashiAudio.Play();
                m_JiashiAudio.Play();
                m_JiashiGameObject.SetActive(true);
            }
        }

        if (other.tag == "pubu" && !m_UIController.m_IsGameOver)
        {
            if (!m_HasChanged)
            {
                m_CameraSmooth.PositionForward = -1.0f;
                m_CameraSmooth.PositionUp = test;
                m_CameraSmooth.speed = 300.0f;
            }
        }
        //		if(other.tag == "zhiwu")
        //		{
        //			m_IsInZhiwuCollider = true;
        //		}
    }
    public float test = 15.0f;
    void ResetPlayer()
    {
        if (PathNum == PathPoint.Length - 1)
        {
            m_WaterDirection = PathPoint[0].position - PathPoint[PathNum].position;
        }
        else
        {
            m_WaterDirection = PathPoint[PathNum + 1].position - PathPoint[PathNum].position;
        }
        float angle = Vector3.Angle(m_WaterDirection, transform.forward);
        if (Mathf.Abs(angle) >= 90.0f)
        {
            m_IsErrorDirection = true;
        }
        if (Mathf.Abs(angle) < 90.0f)
        {
            m_IsErrorDirection = false;
        }

        if (m_IsOnRoad && !m_IsInWarter || m_IsErrorDirection)
        {
            if (!m_ErrorDirectionAudio.isPlaying)
            {
                m_ErrorDirectionAudio.Play();
            }
            if (m_IsOnRoad && !m_IsInWarter)
            {
                if (!m_OutHedao.activeSelf)
                {
                    m_OutHedao.SetActive(true);
                }
            }
            else
            {
                m_OutHedao.SetActive(false);
            }
            if (m_IsErrorDirection)
            {
                if (!m_OutHedao.activeSelf && !m_ErrorDirection.activeSelf)
                {
                    m_ErrorDirection.SetActive(true);
                }
                else if (m_OutHedao.activeSelf)
                {
                    m_ErrorDirection.SetActive(false);
                }
            }
            m_ResetPlayerTimmer += Time.deltaTime;
        }
        else/* if(m_IsInWarter)*/
        {
            m_ErrorDirectionAudio.Stop();
            m_OutHedao.SetActive(false);
            m_ErrorDirection.SetActive(false);
            m_ResetPlayerTimmer = 0.0f;
        }
        if (m_ResetPlayerTimmer >= m_ResetPlayerTimeSet)
        {
            //chonghzi
            m_OutHedao.SetActive(false);
            m_ErrorDirection.SetActive(false);
            m_ResetPlayerTimmer = 0.0f;
            m_IsOnRoad = false;
            m_IsInWarter = false;
            transform.position = PathPoint[PathNum].position;
            transform.localEulerAngles = PathPoint[PathNum].localEulerAngles;
        }
    }
    void UpdateCameraEffect()
    {
        if (rigidbody.velocity.magnitude * 3.6f >= m_SpeedForEffectStar)
        {
            m_RadialBlurEffect.SampleStrength = Mathf.Lerp(m_RadialBlurEffect.SampleStrength, m_ParameterForEfferct, 10f * Time.deltaTime);
            //m_RadialBlurEffect.SampleStrength = m_ParameterForEfferct*rigidbody.velocity.magnitude*3.6f*3.6f*rigidbody.velocity.magnitude;
            //float rv = m_ParameterForEfferct * Mathf.Pow(rigidbody.velocity.magnitude * 3.6f, 2f);
            //if (Mathf.Abs(rv - m_RadialBlurEffect.SampleStrength) >= 0.05f)
            //{
            //    m_RadialBlurEffect.SampleStrength = m_ParameterForEfferct * Mathf.Pow(rigidbody.velocity.magnitude * 3.6f, 2f);
            //}
        }
        else
        {
            //m_RadialBlurEffect.SampleStrength = 0.0f;
            m_RadialBlurEffect.SampleStrength = Mathf.Lerp(m_RadialBlurEffect.SampleStrength, 0f, 30f * Time.deltaTime);
        }
    }

    enum ChuanShuiHua
    {
        Null,
        Open,
        Close,
    }
    ChuanShuiHua mChuanShuiHua = ChuanShuiHua.Null;
    void UpdateShuihua()
    {

        if (rigidbody.velocity.magnitude * 3.6f >= m_speedForshuihua && !m_IsOffShuihua)
        {
            if (mChuanShuiHua != ChuanShuiHua.Open)
            {
                mChuanShuiHua = ChuanShuiHua.Open;
                m_partical[0].SetActive(true);
                m_partical[1].SetActive(true);
                NetSendActiveChuanShuiHuaZL(true);
            }

            if (!m_ShuihuaAudio.isPlaying)
            {
                m_ShuihuaAudio.Play();
            }
            m_ShuihuaAudio.volume = rigidbody.velocity.magnitude * 3.6f / 120.0f;
        }
        else
        {
            if (mChuanShuiHua != ChuanShuiHua.Close)
            {
                mChuanShuiHua = ChuanShuiHua.Close;
                m_partical[0].SetActive(false);
                m_partical[1].SetActive(false);
                NetSendActiveChuanShuiHuaZL(false);
            }
            m_ShuihuaAudio.Stop();
        }

        if (!m_IsOffShuihua != m_partical[2].activeInHierarchy)
        {
            m_partical[2].SetActive(!m_IsOffShuihua);
        }
    }

    /// <summary>
    /// 通过网络消息发送玩家的tr信息.
    /// </summary>
    public void NetSendPlayerTrInfo()
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcNetSendPlayerTrInfo", RPCMode.OthersBuffered, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// 发送玩家的tr信息.
    /// </summary>
    [RPC]
    void RpcNetSendPlayerTrInfo(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    /// <summary>
    /// 通过网络消息发送船水花粒子的显示隐藏.
    /// </summary>
    public void NetSendActiveChuanShuiHuaZL(bool isActive)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcNetSendActiveChuanTouShuiHua", RPCMode.Others, isActive == true ? 1 : 0);
        }
    }

    /// <summary>
    /// 发送船水花粒子的显示隐藏信息.
    /// </summary>
    [RPC]
    void RpcNetSendActiveChuanShuiHuaZL(int activeVal)
    {
        bool isActive = activeVal == 1 ? true : false;
        Debug.Log("RpcNetSendActiveChuanShuiHuaZL -> " + name + ":: isActive == " + isActive);
        m_partical[0].SetActive(isActive);
        m_partical[1].SetActive(isActive);
    }

    float m_HitshakeTimmerSet = 0.2f;
    private float m_HitshakeTimmer = 0.0f;
    [HideInInspector]
    public bool m_IsHitshake = false;
    //static bool IsHitRock = false;
    void OnHitShake()
    {
        if (m_IsHitshake)
        {
            if (m_HitshakeTimmer < m_HitshakeTimmerSet)
            {
                TimeHitRock = 0f;
                m_HitshakeTimmer += Time.deltaTime;
                if (m_HitshakeTimmer < m_HitshakeTimmerSet * 0.25f || (m_HitshakeTimmer >= m_HitshakeTimmerSet * 0.5f && m_HitshakeTimmer < m_HitshakeTimmerSet * 0.75f))
                {
                    //pcvr.m_IsOpneForwardQinang = IsHitRock;
                    //pcvr.m_IsOpneBehindQinang = IsHitRock;
                    //pcvr.m_IsOpneLeftQinang = false;
                    //pcvr.m_IsOpneRightQinang = true;
                }
                else if ((m_HitshakeTimmer >= m_HitshakeTimmerSet * 0.25f && m_HitshakeTimmer < m_HitshakeTimmerSet * 0.5f) || m_HitshakeTimmer >= m_HitshakeTimmerSet * 0.75f)
                {
                    //pcvr.m_IsOpneForwardQinang = IsHitRock;
                    //pcvr.m_IsOpneBehindQinang = IsHitRock;
                    //pcvr.m_IsOpneLeftQinang = true;
                    //pcvr.m_IsOpneRightQinang = false;
                }
            }
            else
            {
                TimeHitRock += Time.deltaTime;
                //pcvr.m_IsOpneLeftQinang = false;
                //pcvr.m_IsOpneRightQinang = false;
                if (TimeHitRock >= 2f)
                {
                    //pcvr.CountQNZD++;
                    TimeHitRock = 0f;
                    m_HitshakeTimmer = 0.0f;
                    m_IsHitshake = false;
                    //IsHitRock = false;
                    //pcvr.m_IsOpneForwardQinang = false;
                    //pcvr.m_IsOpneBehindQinang = false;
                    //					pcvr.m_IsOpneLeftQinang = false;
                    //					pcvr.m_IsOpneRightQinang = false;
                }
            }
            //pcvr.GetInstance().OpenFangXiangPanZhenDong();
        }
    }

    float TimeHitRock;
    public void OnNpcHitPlayer(NpcController npcScript = null)
    {
        if (!m_IsFinished && !m_UIController.m_IsGameOver && timmerstar >= 5.0f)
        {
            if (npcScript != null && npcScript.m_IsHit && npcScript.m_HitTimmer < 0.4f)
            {
                Debug.Log(npcScript.name + " hit player!");
                rigidbody.AddForce(Vector3.Normalize(npcScript.m_PlayerHit - npcScript.m_NpcPos) * 80000.0f, ForceMode.Force);
            }
        }
    }

    bool IsAmmoHitPlayer = false;
    //float TimeLastAmmoHit = 0f;
    public void ResetIsAmmoHitPlayer()
    {
        IsAmmoHitPlayer = false;
        m_UIController.RemoveJinYongAmmo();
    }
    /// <summary>
    /// 眩晕特效粒子.
    /// </summary>
    public GameObject XuanYunTXPrefab;
    /// <summary>
    /// 眩晕特效产生点.
    /// </summary>
    public Transform XuanYunTxSpawnTr;
    /// <summary>
    /// 子弹爆炸粒子特效.
    /// </summary>
    public GameObject AmmoLiZiPrefab;
    /// <summary>
    /// 子弹爆炸产生点(联机情况下在主控主角端口使用).
    /// </summary>
    public Transform AmmoExpSpawnTr;
    /// <summary>
    /// 眩晕特效.
    /// </summary>
    GameObject XuanYunTXObj;
    /// <summary>
    /// 当玩家被子弹攻击时.
    /// </summary>
    public void OnAmmoHitPlayer()
    {
        PlayerController playerScript = _Instance;
        if (playerScript != null
            && !playerScript.m_IsFinished
            && !playerScript.m_UIController.m_IsGameOver
            && playerScript.timmerstar >= 5.0f)
        {
            Debug.Log(name + "::OnAmmoHitPlayer...");
            NetSendAmmoHitPlayer();
        }
    }
    
    /// <summary>
    /// 通过网络消息发送玩家被子弹击中的信息.
    /// </summary>
    void NetSendAmmoHitPlayer()
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (mNetViewCom != null)
            {
                mNetViewCom.RPC("RpcNetSendAmmoHitPlayer", RPCMode.All);
            }
        }
        else
        {
            RpcNetSendAmmoHitPlayer();
        }
    }

    /// <summary>
    /// 发送玩家被子弹击中的信息.
    /// </summary>
    [RPC]
    void RpcNetSendAmmoHitPlayer()
    {
        Debug.Log("RpcNetSendAmmoHitPlayer -> " + name + "::OnAmmoHitPlayer...");
        if (XuanYunTXPrefab != null && XuanYunTxSpawnTr != null)
        {
            if (XuanYunTXObj == null)
            {
                XuanYunTXObj = (GameObject)Instantiate(XuanYunTXPrefab);
                XuanYunTXObj.transform.parent = XuanYunTxSpawnTr;
                XuanYunTXObj.transform.localPosition = Vector3.zero;
            }
        }

        if (IsNetControlPort)
        {
            if (!IsAmmoHitPlayer)
            {
                IsAmmoHitPlayer = true;
                NetSendPlayAmmoHitPlayerAnimation();
                Instantiate(AmmoLiZiPrefab, AmmoExpSpawnTr.position, AmmoExpSpawnTr.rotation);
                if (m_UIController.mTaoQuanUI != null)
                {
                    //受攻击后禁用导弹.
                    m_UIController.mTaoQuanUI.DestroySelf();
                }
            }
            m_UIController.SpawnAmmoHitPlayerUI();
        }
    }

    float TimeFeiBan;
    void ResetIsIntoFeiBan()
    {
        IsIntoFeiBan = false;
    }

    /// <summary>
    /// 船头水花特效.
    /// </summary>
    public GameObject ChuanTouShuiHuaTX;
    /// <summary>
    /// 当速度增大时,打开水花; 当速度变回初始值时,关闭水花; 
    /// </summary>
    void SetAcitveChuanTouShuiHuaTX(bool isActive)
    {
        ChuanTouShuiHuaTX.SetActive(isActive);
    }
    
    /// <summary>
    /// 通过网络消息发送船头水花的显示隐藏.
    /// </summary>
    public void NetSendActiveChuanTouShuiHua(bool isActive)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcNetSendActiveChuanTouShuiHua", RPCMode.Others, isActive == true ? 1 : 0);
        }
    }

    /// <summary>
    /// 发送船头水花的显示隐藏信息.
    /// </summary>
    [RPC]
    void RpcNetSendActiveChuanTouShuiHua(int activeVal)
    {
        SetAcitveChuanTouShuiHuaTX(activeVal == 1 ? true : false);
    }

    /// <summary>
    /// 使玩家道具掉落.
    /// </summary>
    void ClosePlayerDaoJuAni(DaoJuCtrl.DaoJuType daoJuState, bool isTimeOver = true)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0 && Network.peerType == NetworkPeerType.Server)
            {
                //没有玩家选择链接服务器进行联机游戏.
            }
            else
            {
                SendClosePlayerDaoJuAni(daoJuState);
            }
        }

        mSpeedDaoJuState = DaoJuCtrl.DaoJuType.Null;
        m_pTopSpeed = PlayerMvSpeedMin;
        m_ParameterForEfferct = m_StartForEfferct;
        switch (daoJuState)
        {
            case DaoJuCtrl.DaoJuType.PenQiJiaSu:
                {
                    SetAcitveChuanTouShuiHuaTX(false);
                    Instantiate(PenQiPrefab, DaoJuDiaoLuoTr[0].position, DaoJuDiaoLuoTr[0].rotation);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.zero;
                        PenQiAniAy[i].SetBool("IsPlay", false);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.FeiXingYi:
                {
                    m_pChuan.localPosition -= new Vector3(0f, PlayerHightFeiXing, 0f);
                    m_CameraSmooth.SetCameraUpPos(-PlayerHightFeiXing);
                    Instantiate(FeiXingYiPrefab, DaoJuDiaoLuoTr[1].position, DaoJuDiaoLuoTr[1].rotation);
                    for (int i = 0; i < FiXingYiAniAy.Length; i++)
                    {
                        FiXingYiAniAy[i].transform.localScale = Vector3.zero;
                        FiXingYiAniAy[i].SetBool("IsPlay", false);
                    }

                    Instantiate(PenQiPrefab, DaoJuDiaoLuoTr[0].position, DaoJuDiaoLuoTr[0].rotation);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.zero;
                        PenQiAniAy[i].SetBool("IsPlay", false);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.ShuangYiFeiJi:
                {
                    m_pChuan.localPosition -= new Vector3(0f, PlayerHightShuangYiFeiJi, 0f);
                    m_CameraSmooth.SetCameraUpPos(-PlayerHightShuangYiFeiJi);
                    Instantiate(ShuangYiFeiJiPrefab, DaoJuDiaoLuoTr[3].position, DaoJuDiaoLuoTr[3].rotation);
                    ShuangYiFeiJiTwRot.enabled = false;
                    for (int i = 0; i < ShuangYiFeiJiAniAy.Length; i++)
                    {
                        ShuangYiFeiJiAniAy[i].transform.localScale = Vector3.zero;
                        ShuangYiFeiJiAniAy[i].SetBool("IsPlay", false);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.JiaSuFengShan:
                {
                    FengKuangTwRot.enabled = false;
                    FengKuangAni.enabled = true;
                    SetAcitveChuanTouShuiHuaTX(false);
                    Instantiate(FenKuangPrefab, DaoJuDiaoLuoTr[2].position, DaoJuDiaoLuoTr[2].rotation);
                    FengKuangAni.transform.localScale = Vector3.zero;
                    FengKuangAni.SetBool("IsPlay", false);
                    break;
                }
            case DaoJuCtrl.DaoJuType.QianTing:
                {
                    if (pFeiBanPengZhuang != null)
                    {
                        pFeiBanPengZhuang.SetIsEnablePengZhuang(true);
                    }
                    Instantiate(QianTingDt.LiZiPrefab, QianTingDt.LiZiSpawnPoint.position, QianTingDt.LiZiSpawnPoint.rotation);
                    m_pChuan.gameObject.SetActive(true);
                    QianTingDt.QianTingObj.SetActive(false);
                    break;
                }
            case DaoJuCtrl.DaoJuType.Tank:
                {
                    Instantiate(TankDt.LiZiPrefab, TankDt.LiZiSpawnPoint.position, TankDt.LiZiSpawnPoint.rotation);
                    m_pChuan.gameObject.SetActive(true);
                    TankDt.TankObj.SetActive(false);
                    break;
                }
        }

        if (isTimeOver)
        {
            ChangeDaoJuToWaterPos();
        }
    }

    /// <summary>
    /// 打开玩家变型动画.
    /// </summary>
    public void OpenPlayerDaoJuAni(DaoJuCtrl.DaoJuType daoJuState)
    {
        if (mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi)
        {
            //道具加速为飞行翼时不重置时间.
            return;
        }
        else
        {
            TimeLastDaoJuBianXing = Time.time;
            m_IsJiasu = true;
            m_JiasuTimmer = 0f;
        }

        if (mSpeedDaoJuState == daoJuState)
        {
            return;
        }

        if (mSpeedDaoJuState == DaoJuCtrl.DaoJuType.FeiXingYi && daoJuState == DaoJuCtrl.DaoJuType.PenQiJiaSu)
        {
            return;
        }

        if (mSpeedDaoJuState != DaoJuCtrl.DaoJuType.Null)
        {
            ClosePlayerDaoJuAni(mSpeedDaoJuState, false);
        }

        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0 && Network.peerType == NetworkPeerType.Server)
            {
                //没有玩家选择链接服务器进行联机游戏.
            }
            else
            {
                SendOnPlayerHitDaoJuBianXing(daoJuState);
            }
        }

        mSpeedDaoJuState = daoJuState;
        switch (mSpeedDaoJuState)
        {
            case DaoJuCtrl.DaoJuType.PenQiJiaSu:
                {
                    m_JiasuTimeSet = PenQiBianXingTime;
                    DaoJuBianXingTime = PenQiBianXingTime;
                    m_ParameterForEfferct = m_ForEfferctPenQi;
                    m_pTopSpeed = PlayerMvSpeedPenQi;
                    SetAcitveChuanTouShuiHuaTX(true);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.one;
                        PenQiAniAy[i].SetBool("IsPlay", true);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.FeiXingYi:
                {
                    m_JiasuTimeSet = FeiXingYiBianXingTime;
                    DaoJuBianXingTime = FeiXingYiBianXingTime;
                    m_ParameterForEfferct = m_ForEfferctFeiXing;
                    m_pChuan.localPosition += new Vector3(0f, PlayerHightFeiXing, 0f);
                    m_CameraSmooth.SetCameraUpPos(PlayerHightFeiXing);
                    m_pTopSpeed = PlayerMvSpeedFeiXing;
                    for (int i = 0; i < FiXingYiAniAy.Length; i++)
                    {
                        FiXingYiAniAy[i].transform.localScale = Vector3.one;
                        FiXingYiAniAy[i].SetBool("IsPlay", true);
                    }

                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.one;
                        PenQiAniAy[i].SetBool("IsPlay", true);
                    }
                    mBianXingYiAudio.Play();
                    break;
                }
            case DaoJuCtrl.DaoJuType.ShuangYiFeiJi:
                {
                    m_ParameterForEfferct = m_ForEfferctShuangYiFeiJi;
                    m_pChuan.localPosition += new Vector3(0f, PlayerHightShuangYiFeiJi, 0f);
                    m_CameraSmooth.SetCameraUpPos(PlayerHightShuangYiFeiJi);
                    m_pTopSpeed = PlayerMvSpeedShuangYiFeiJi;
                    ShuangYiFeiJiTwRot.enabled = false;
                    for (int i = 0; i < ShuangYiFeiJiAniAy.Length; i++)
                    {
                        ShuangYiFeiJiAniAy[i].enabled = true;
                        ShuangYiFeiJiAniAy[i].transform.localScale = Vector3.one;
                        ShuangYiFeiJiAniAy[i].SetBool("IsPlay", true);
                    }
                    //Invoke("OnDaoJuShaungYiFeiJiAniOver", 0.45f);
                    break;
                }
            case DaoJuCtrl.DaoJuType.JiaSuFengShan:
                {
                    m_ParameterForEfferct = m_ForEfferctJiaSuFenShan;
                    m_pTopSpeed = PlayerMvSpeedJiaSuFengShan;
                    FengKuangTwRot.enabled = false;
                    FengKuangAni.enabled = true;
                    SetAcitveChuanTouShuiHuaTX(true);
                    FengKuangAni.transform.localScale = Vector3.one;
                    FengKuangAni.SetBool("IsPlay", true);
                    break;
                }
            case DaoJuCtrl.DaoJuType.QianTing:
                {
                    if (pFeiBanPengZhuang != null)
                    {
                        pFeiBanPengZhuang.SetIsEnablePengZhuang(false);
                    }
                    Instantiate(QianTingDt.LiZiPrefab, QianTingDt.LiZiSpawnPoint.position, QianTingDt.LiZiSpawnPoint.rotation);
                    m_pChuan.gameObject.SetActive(false);
                    QianTingDt.QianTingObj.SetActive(true);
                    StartCoroutine(SpawnQianTingYuLei());
                    break;
                }
            case DaoJuCtrl.DaoJuType.Tank:
                {
                    Instantiate(TankDt.LiZiPrefab, TankDt.LiZiSpawnPoint.position, TankDt.LiZiSpawnPoint.rotation);
                    m_pChuan.gameObject.SetActive(false);
                    TankDt.TankObj.SetActive(true);
                    StartCoroutine(SpawnTankAmmo());
                    break;
                }
        }
        ChangeDaoJuToWaterPos();
    }

    public void OnDaoJuFengKuangAniOver()
    {
        FengKuangAni.enabled = false;
        FengKuangTwRot.enabled = true;
    }

    public void OnDaoJuShaungYiFeiJiAniOver()
    {
        for (int i = 0; i < ShuangYiFeiJiAniAy.Length; i++)
        {
            ShuangYiFeiJiAniAy[i].enabled = false;
        }
        ShuangYiFeiJiTwRot.enabled = true;
    }

    /// <summary>
    /// 导弹预置.
    /// </summary>
    public GameObject DaoDanPrefab;
    /// <summary>
    /// 导弹产生点.
    /// </summary>
    public Transform[] SpawnDaoDanTr;
    /// <summary>
    /// 障碍物道具.
    /// </summary>
    //GameObject mZhangAiWuObj;
    int DaoDanSpawnCount = 0;
    /// <summary>
    /// 当主角吃上导弹道具.
    /// </summary>
    public void OnPlayerHitDaoDanDaoJu(GameObject zhangAiWu)
    {
        DaoDanSpawnCount = 0;
        //mZhangAiWuObj = zhangAiWu;
        SpawnDaoDanAmmo();
        //Invoke("SpawnDaoDanAmmo", 0.5f);
    }

    public void SpawnDaoDanAmmo()
    {
        bool isFollowNpc = false;
        Transform aimNpcTr = null;
        GameObject ammo = null;
        int indexVal = DaoDanSpawnCount % SpawnDaoDanTr.Length;
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            ammo = (GameObject)Network.Instantiate(DaoDanPrefab, SpawnDaoDanTr[indexVal].position, SpawnDaoDanTr[indexVal].rotation, 0);
        }
        else
        {
            ammo = (GameObject)Instantiate(DaoDanPrefab, SpawnDaoDanTr[indexVal].position, SpawnDaoDanTr[indexVal].rotation);
        }

        AmmoMoveCtrl ammoMoveCom = ammo.GetComponent<AmmoMoveCtrl>();
        ammoMoveCom.SetIsNetControlPort(true);
        AmmoMoveCtrl.AmmoDt ammoDt = new AmmoMoveCtrl.AmmoDt();
        //GameObject aimNpc = SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.FindAiNpc(transform, DaoDanDisNpc, MinTaoQuanDaoDanDisNpc);
        GameObject aimNpc = null;
        if (m_UIController.mTaoQuanUI != null && m_UIController.mTaoQuanUI.mAimTr != null)
        {
            aimNpc = m_UIController.mTaoQuanUI.mAimTr.gameObject;
        }

        if (aimNpc != null)
        {
            isFollowNpc = true;
            aimNpcTr = aimNpc.transform;
        }

        if (isFollowNpc)
        {
            ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.GenZongDan;
            ammoDt.PosHit = aimNpcTr.position;
            ammoDt.AimTr = aimNpcTr;
            ammoMoveCom.InitMoveAmmo(ammoDt);
        }
        else
        {
            ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.PuTong;
            ammoDt.PosHit = (ammo.transform.forward * UnityEngine.Random.Range(50f, 120f)) + ammo.transform.position;
            ammoMoveCom.InitMoveAmmo(ammoDt);
            //if (mZhangAiWuObj != null)
            //{
            //    ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.PuTong;
            //    ammoDt.PosHit = mZhangAiWuObj.transform.position;
            //    ammoDt.AimTr = mZhangAiWuObj.transform;
            //    ammoMoveCom.InitMoveAmmo(ammoDt);
            //}
            //else
            //{
            //    ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.PuTong;
            //    ammoDt.PosHit = (ammo.transform.forward * UnityEngine.Random.Range(50f, 120f)) + ammo.transform.position;
            //    ammoMoveCom.InitMoveAmmo(ammoDt);
            //}
        }
        DaoDanSpawnCount++;
    }


    /// <summary>
    /// 地雷数据.
    /// </summary>
    [Serializable]
    public class DiLeiData
    {
        /// <summary>
        /// 子弹预置.
        /// </summary>
        public GameObject AmmoPrefab;
        /// <summary>
        /// 子弹产生点.
        /// </summary>
        public Transform[] SpawnAmmoTr;
        /// <summary>
        /// 障碍物道具.
        /// </summary>
        [HideInInspector]
        public GameObject mZhangAiWuObj;
        [HideInInspector]
        public int AmmoSpawnCount = 0;
    }
    public DiLeiData DiLeiDt;
    /// <summary>
    /// 当主角吃上地雷道具.
    /// </summary>
    public void OnPlayerHitDiLeiDaoJu(GameObject zhangAiWu)
    {
        DiLeiDt.AmmoSpawnCount = 0;
        DiLeiDt.mZhangAiWuObj = zhangAiWu;
        SpawnDiLeiAmmo();
        //Invoke("SpawnDiLeiAmmo", 0.5f);
    }

    public void SpawnDiLeiAmmo()
    {
        GameObject ammo = null;
        int indexVal = DiLeiDt.AmmoSpawnCount % DiLeiDt.SpawnAmmoTr.Length;
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            ammo = (GameObject)Network.Instantiate(DiLeiDt.AmmoPrefab, DiLeiDt.SpawnAmmoTr[indexVal].position, DiLeiDt.SpawnAmmoTr[indexVal].rotation, 0);
        }
        else
        {
            ammo = (GameObject)Instantiate(DiLeiDt.AmmoPrefab, DiLeiDt.SpawnAmmoTr[indexVal].position, DiLeiDt.SpawnAmmoTr[indexVal].rotation);
        }
        AmmoMoveCtrl ammoMoveCom = ammo.GetComponent<AmmoMoveCtrl>();
        ammoMoveCom.SetIsNetControlPort(true);
        AmmoMoveCtrl.AmmoDt ammoDt = new AmmoMoveCtrl.AmmoDt();
        ammoDt.HightVal = UnityEngine.Random.Range(2.5f, 5f);
        ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.DiLei;
        //GameObject aimNpc = SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.FindAiNpc(transform, DaoDanDisNpc, MinTaoQuanDaoDanDisNpc);
        GameObject aimNpc = null;
        if (m_UIController.mTaoQuanUI != null && m_UIController.mTaoQuanUI.mAimTr != null)
        {
            aimNpc = m_UIController.mTaoQuanUI.mAimTr.gameObject;
        }

        if (aimNpc != null)
        {
            ammoDt.PosHit = aimNpc.transform.position;
            ammoDt.AimTr = aimNpc.transform;
            ammoMoveCom.InitMoveAmmo(ammoDt);
        }
        else
        {
            //if (DiLeiDt.mZhangAiWuObj != null)
            //{
            //    ammoDt.PosHit = DiLeiDt.mZhangAiWuObj.transform.position;
            //    ammoDt.AimTr = DiLeiDt.mZhangAiWuObj.transform;
            //    ammoMoveCom.InitMoveAmmo(ammoDt);
            //}
            //else
            //{
            //    ammoDt.PosHit = (ammo.transform.forward * UnityEngine.Random.Range(40f, 65f)) + ammo.transform.position;
            //    ammoMoveCom.InitMoveAmmo(ammoDt);
            //}
            ammoDt.PosHit = (ammo.transform.forward * UnityEngine.Random.Range(40f, 65f)) + ammo.transform.position;
            ammoMoveCom.InitMoveAmmo(ammoDt);
        }
        DiLeiDt.AmmoSpawnCount++;
    }

    public void OpenPlayerCiTieDaoJu()
    {
        IsOpenCiTieDaoJu = true;
        TimeLastCiTie = Time.time;
    }
    /// <summary>
    /// 产生道具积分.
    /// </summary>
    public void SpawnDaoJuJiFen(GameObject jiFenPrefab)
    {
        Instantiate(jiFenPrefab, SpawnJiFenTr.position, SpawnJiFenTr.rotation);
    }

    /// <summary>
    /// 产生潜艇的鱼雷.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnQianTingYuLei()
    {
        yield return new WaitForSeconds(0.1f);
        int indexVal = 0;
        GameObject ammo;
        AmmoMoveCtrl moveAmmo;
        AmmoMoveCtrl.AmmoDt ammoDt = new AmmoMoveCtrl.AmmoDt();
        ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.YuLei;
        do
        {
            if (mSpeedDaoJuState != DaoJuCtrl.DaoJuType.QianTing)
            {
                yield break;
            }
            ammo = (GameObject)Instantiate(QianTingDt.YuLeiPrefab, QianTingDt.YuLeiSpawnPointArray[indexVal].position, QianTingDt.YuLeiSpawnPointArray[indexVal].rotation);
            moveAmmo = ammo.GetComponent<AmmoMoveCtrl>();
            ammoDt.PosHit = ammo.transform.position + (ammo.transform.forward * UnityEngine.Random.Range(30f, 65f));
            moveAmmo.InitMoveAmmo(ammoDt);
            indexVal++;
            if (indexVal >= QianTingDt.YuLeiSpawnPointArray.Length
                || mSpeedDaoJuState != DaoJuCtrl.DaoJuType.QianTing)
            {
                indexVal = 0;
                yield return new WaitForSeconds(UnityEngine.Random.Range(QianTingDt.TimeYuLei * 1.6f, QianTingDt.TimeYuLei * 2.5f));
                continue;
            }
            yield return new WaitForSeconds(QianTingDt.TimeYuLei);
        } while (true);
    }

    /// <summary>
    /// 产生坦克炮弹.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTankAmmo()
    {
        yield return new WaitForSeconds(0.1f);
        int indexVal = 0;
        GameObject ammo;
        AmmoMoveCtrl moveAmmo;
        AmmoMoveCtrl.AmmoDt ammoDt = new AmmoMoveCtrl.AmmoDt();
        ammoDt.AmmoState = AmmoMoveCtrl.AmmoType.TankPaoDan;
        do
        {
            if (mSpeedDaoJuState != DaoJuCtrl.DaoJuType.Tank)
            {
                yield break;
            }
            ammoDt.HightVal = UnityEngine.Random.Range(1, 4);
            ammo = (GameObject)Instantiate(TankDt.AmmoPrefab, TankDt.AmmoSpawnPointArray[indexVal].position, TankDt.AmmoSpawnPointArray[indexVal].rotation);
            moveAmmo = ammo.GetComponent<AmmoMoveCtrl>();
            ammoDt.PosHit = ammo.transform.position + (ammo.transform.forward * UnityEngine.Random.Range(25f, 75f));
            moveAmmo.InitMoveAmmo(ammoDt);
            indexVal++;
            if (indexVal >= TankDt.AmmoSpawnPointArray.Length
                || mSpeedDaoJuState != DaoJuCtrl.DaoJuType.Tank)
            {
                indexVal = 0;
                yield return new WaitForSeconds(UnityEngine.Random.Range(TankDt.TimeAmmo * 1.6f, TankDt.TimeAmmo * 2.5f));
                continue;
            }
            yield return new WaitForSeconds(TankDt.TimeAmmo);
        } while (true);
    }

    /// <summary>
    /// 改变对应道具时水粒子的位置.
    /// </summary>
    void ChangeDaoJuToWaterPos()
    {
        switch (mSpeedDaoJuState)
        {
            case DaoJuCtrl.DaoJuType.QianTing:
                {
                    for (int i = 0; i < WaterLiZiTrArray.Length; i++)
                    {
                        WaterLiZiTrArray[i].position = DaoJuToWaterDt.QianTingTrArray[i].position;
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.Tank:
                {
                    for (int i = 0; i < WaterLiZiTrArray.Length; i++)
                    {
                        WaterLiZiTrArray[i].position = DaoJuToWaterDt.TankTrArray[i].position;
                    }
                    break;
                }
            default:
                {
                    for (int i = 0; i < WaterLiZiTrArray.Length; i++)
                    {
                        WaterLiZiTrArray[i].position = DaoJuToWaterDt.TrArray[i].position;
                    }
                    break;
                }
        }
    }

    public void OnPlayerHitDaoJuZhangAiWu()
    {
        if (rigidbody.velocity.magnitude * 3.6f > 10f)
        {
            //IsHitRock = true;
            m_IsHitshake = true;
            if (m_PlayerAnimator.gameObject.activeInHierarchy)
            {
                m_PlayerAnimator.SetTrigger("IsZhuang");
                mNetSynGame.SynNetAnimator("IsZhuang", NetworkSynchronizeGame.AnimatorType.Trigger);
            }
            m_CameraShake.setCameraShakeImpulseValue();
        }
    }

    public void SetIsNetControlPort(bool isNetControl)
    {
        IsNetControlPort = isNetControl;
    }

    /// <summary>
    /// 通过网络消息发送将要销毁的障碍物
    /// </summary>
    public void NetSendAmmoHitZhangAiWuInfo(DaoJuCtrl daoJuCom)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (daoJuCom != null && mNetViewCom != null)
            {
                mNetViewCom.RPC("RpcOnAmmoHitZhangAiWu", RPCMode.Others, daoJuCom.IndexZhangAiWu);
            }
        }
    }

    /// <summary>
    /// 发送子弹击中障碍物道具的数据索引信息.
    /// </summary>
    [RPC]
    void RpcOnAmmoHitZhangAiWu(int index)
    {
        GameObject obj = SSGameCtrl.GetInstance().mPlayerDataManage.mDaoJuZhangAiWuData.FindZhangAiWu(index);
        if (obj != null)
        {
            DaoJuCtrl daoJuCom = obj.GetComponent<DaoJuCtrl>();
            if (daoJuCom != null)
            {
                daoJuCom.OnDestroyThis();
            }
        }
    }

    /// <summary>
    /// 通过网络消息发送脚踏板信息.
    /// </summary>
    public void NetSendPlayerJiaoTaBanVal(float jiaoTaBan)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            mNetViewCom.RPC("RpcNetSendPlayerJiaoTaBanVal", RPCMode.Others, jiaoTaBan);
        }
    }

    /// <summary>
    /// 发送脚踏板信息.
    /// </summary>
    [RPC]
    void RpcNetSendPlayerJiaoTaBanVal(float jiaoTaBan)
    {
        mJiaoTaBan = jiaoTaBan;
    }

    /// <summary>
    /// 发送有玩家到达终点的消息.
    /// </summary>
    void SendOnPlayerMoveToFinishPoint()
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                m_UIController.SetActiveIsOpenTimeNetEndUI();
                mNetViewCom.RPC("RpcOnPlayerMoveToFinishPoint", RPCMode.Others);
            }
        }
    }

    /// <summary>
    /// 通知其他客户端有玩家到达终点了.
    /// </summary>
    [RPC]
    void RpcOnPlayerMoveToFinishPoint()
    {
        if (_Instance.m_UIController != null)
        {
            Debug.Log("RpcOnPlayerMoveToFinishPoint...");
            _Instance.m_UIController.SetActiveIsOpenTimeNetEndUI();
        }
    }
    
    /// <summary>
    /// 发送玩家碰上道具变形消息.
    /// </summary>
    void SendOnPlayerHitDaoJuBianXing(DaoJuCtrl.DaoJuType daoJuState)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                mNetViewCom.RPC("RpcOnPlayerHitDaoJuBianXing", RPCMode.Others, (int)daoJuState);
            }
        }
    }

    /// <summary>
    /// 通知其他客户端玩家变形信息.
    /// </summary>
    [RPC]
    void RpcOnPlayerHitDaoJuBianXing(int daoJuState)
    {
        DaoJuCtrl.DaoJuType daoJuType = (DaoJuCtrl.DaoJuType)daoJuState;
        //Debug.Log("RpcOnPlayerHitDaoJuBianXing -> daoJuType " + daoJuType);
        switch (daoJuType)
        {
            case DaoJuCtrl.DaoJuType.PenQiJiaSu:
                {
                    SetAcitveChuanTouShuiHuaTX(true);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.one;
                        PenQiAniAy[i].SetBool("IsPlay", true);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.FeiXingYi:
                {
                    m_pChuan.localPosition += new Vector3(0f, PlayerHightFeiXing, 0f);
                    for (int i = 0; i < FiXingYiAniAy.Length; i++)
                    {
                        FiXingYiAniAy[i].transform.localScale = Vector3.one;
                        FiXingYiAniAy[i].SetBool("IsPlay", true);
                    }

                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.one;
                        PenQiAniAy[i].SetBool("IsPlay", true);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// 发送关闭玩家变形消息.
    /// </summary>
    void SendClosePlayerDaoJuAni(DaoJuCtrl.DaoJuType daoJuState)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                mNetViewCom.RPC("RpcClosePlayerDaoJuAni", RPCMode.Others, (int)daoJuState);
            }
        }
    }

    /// <summary>
    /// 通知其他客户端关闭玩家变形消息.
    /// </summary>
    [RPC]
    void RpcClosePlayerDaoJuAni(int daoJuState)
    {
        DaoJuCtrl.DaoJuType daoJuType = (DaoJuCtrl.DaoJuType)daoJuState;
        //Debug.Log("RpcClosePlayerDaoJuAni -> daoJuType " + daoJuType);
        switch (daoJuType)
        {
            case DaoJuCtrl.DaoJuType.PenQiJiaSu:
                {
                    SetAcitveChuanTouShuiHuaTX(false);
                    Instantiate(PenQiPrefab, DaoJuDiaoLuoTr[0].position, DaoJuDiaoLuoTr[0].rotation);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.zero;
                        PenQiAniAy[i].SetBool("IsPlay", false);
                    }
                    break;
                }
            case DaoJuCtrl.DaoJuType.FeiXingYi:
                {
                    m_pChuan.localPosition -= new Vector3(0f, PlayerHightFeiXing, 0f);
                    Instantiate(FeiXingYiPrefab, DaoJuDiaoLuoTr[1].position, DaoJuDiaoLuoTr[1].rotation);
                    for (int i = 0; i < FiXingYiAniAy.Length; i++)
                    {
                        FiXingYiAniAy[i].transform.localScale = Vector3.zero;
                        FiXingYiAniAy[i].SetBool("IsPlay", false);
                    }

                    Instantiate(PenQiPrefab, DaoJuDiaoLuoTr[0].position, DaoJuDiaoLuoTr[0].rotation);
                    for (int i = 0; i < PenQiAniAy.Length; i++)
                    {
                        PenQiAniAy[i].transform.localScale = Vector3.zero;
                        PenQiAniAy[i].SetBool("IsPlay", false);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// 是否发送过激活IsNetShowGameEndUI的消息.
    /// </summary>
    [HideInInspector]
    public bool IsSendActiveNetShowGameEndUI = false;
    /// <summary>
    /// 发送激活IsNetShowGameEndUI的消息.
    /// </summary>
    public void SendActiveIsNetShowGameEndUI()
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    if (!m_IsFinished)
                    {
                        IsSendActiveNetShowGameEndUI = true;
                        mNetViewCom.RPC("RpcActiveIsNetShowGameEndUI", RPCMode.All);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 接收激活IsNetShowGameEndUI的消息.
    /// </summary>
    [RPC]
    void RpcActiveIsNetShowGameEndUI()
    {
        Debug.Log("RpcActiveIsNetShowGameEndUI...");
        IsNetShowGameEndUI = true;
    }

    /// <summary>
    /// 所有玩家都没到终点时显示游戏结束UI界面.
    /// </summary>
    public bool GetIsNetShowGameEndUI()
    {
        bool isNetShowEndUI = true;
        if (SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.mNetPlayerComList != null)
        {
            PlayerController[] playerComArray = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.mNetPlayerComList.ToArray();
            for (int i = 0; i < playerComArray.Length; i++)
            {
                if (playerComArray[i] != null && !playerComArray[i].IsNetShowGameEndUI)
                {
                    isNetShowEndUI = false;
                }
            }
        }
        return isNetShowEndUI;
    }

    /// <summary>
    /// 发送Player更新排名路径的消息.
    /// </summary>
    void SendPlayerUpdateRankDtPathPoint(int index, float time)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcPlayerUpdateRankDtPathPoint", RPCMode.Others, index, time);
                }
            }
        }
    }

    /// <summary>
    /// 接收Player更新排名路径的消息.
    /// </summary>
    [RPC]
    void RpcPlayerUpdateRankDtPathPoint(int index, float time)
    {
        //Debug.Log("RpcPlayerUpdateRankDtPathPoint...");
        mRankDt.UpdateRankDtPathPoint(index, time);
    }

    /// <summary>
    /// 发送Player更新到达终点的消息.
    /// </summary>
    void SendPlayerUpdateRankDtTimeFinish(float time)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcPlayerUpdateRankDtTimeFinish", RPCMode.Others, time);
                }
            }
        }
    }

    /// <summary>
    /// 接收Player更新到达终点的消息.
    /// </summary>
    [RPC]
    void RpcPlayerUpdateRankDtTimeFinish(float time)
    {
        //Debug.Log("RpcPlayerUpdateRankDtTimeFinish...");
        mRankDt.UpdateRankDtTimeFinish(time);
    }

    /// <summary>
    /// 发送服务端Player比赛开始时间的消息.
    /// </summary>
    void SendPlayerPlayerRankTimeServerStartVal(float time)
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                mNetViewCom.RPC("RpcPlayerRankTimeServerStartVal", RPCMode.All, time);
            }
        }
        else if (Network.peerType == NetworkPeerType.Disconnected)
        {
            SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.SetTimeServerStartVal(time);
        }
    }

    /// <summary>
    /// 接收服务端Player比赛开始时间的消息.
    /// </summary>
    [RPC]
    void RpcPlayerRankTimeServerStartVal(float time)
    {
        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.SetTimeServerStartVal(time);
    }

    /// <summary>
    /// 发送Player比赛运动路程的消息.
    /// </summary>
    void SendPlayerDisMoveVal(float disVal)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                if (Network.peerType == NetworkPeerType.Server && NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
                {
                    //没有玩家选择链接服务器进行联机游戏.
                }
                else
                {
                    mNetViewCom.RPC("RpcPlayerDisMoveVal", RPCMode.Others, disVal);
                }
            }
        }
    }

    /// <summary>
    /// 接收Player比赛运动路程的消息.
    /// </summary>
    [RPC]
    void RpcPlayerDisMoveVal(float disVal)
    {
        if (mRankDt != null)
        {
            mRankDt.UpdataDisMoveValue(disVal);
        }
    }

    /// <summary>
    /// 通过网络消息发送打开UI界面的信息.
    /// </summary>
    public void NetSendPlayerAcitveUIPanel()
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            if (IsNetControlPort)
            {
                mNetViewCom.RPC("RpcGetPlayerAcitveUIPanel", RPCMode.All);
            }
        }
    }

    /// <summary>
    /// 通过网络消息接收打开UI界面的信息.
    /// </summary>
    [RPC]
    void RpcGetPlayerAcitveUIPanel()
    {
        Debug.Log("RpcGetPlayerAcitveUIPanel...");
        if (m_UIController == null)
        {
            SSGameDataManage.PlayerData playerDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mPlayerDt;
            m_UIController = playerDt.m_UIController;
        }

        if (m_UIController != null)
        {
            m_UIController.SetActiveUIRoot(true);
        }
    }
    
    /// <summary>
    /// 通过网络消息发送播放被子弹击中的动画信息.
    /// </summary>
    public void NetSendPlayAmmoHitPlayerAnimation()
    {
        if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client)
        {
            if (IsNetControlPort)
            {
                mNetViewCom.RPC("RpcGetPlayAmmoHitPlayerAnimation", RPCMode.All);
            }
        }
        else
        {
             RpcGetPlayAmmoHitPlayerAnimation();
        }
    }

    /// <summary>
    /// 通过网络消息接收播放被子弹击中的动画信息.
    /// </summary>
    [RPC]
    void RpcGetPlayAmmoHitPlayerAnimation()
    {
        Debug.Log("RpcGetPlayAmmoHitPlayerAnimation...");
        mPlayerAnimation.PlayAmmoHitAnimation();
    }

    void CheckPlayerIsCanFire()
    {
        if (m_UIController.mPlayerDaoJuManageUI.DaoDanNum <= 0 && m_UIController.mPlayerDaoJuManageUI.DiLeiNum <= 0)
        {
            return;
        }

        if (IsAmmoHitPlayer)
        {
            return;
        }

        GameObject aimNpc = SSGameCtrl.GetInstance().mPlayerDataManage.mAiNpcData.FindAiNpc(transform, DaoDanDisNpc, MinTaoQuanDaoDanDisNpc);
        if (aimNpc != null)
        {
            bool isCreatTaoQuanUI = false;
            if (m_UIController.mTaoQuanUI == null)
            {
                isCreatTaoQuanUI = true;
            }
            else
            {
                if (m_UIController.mTaoQuanUI.mAimTr != aimNpc.transform)
                {
                    isCreatTaoQuanUI = true;
                }
            }

            if (isCreatTaoQuanUI)
            {
                m_UIController.SpawnTaoQuanUI(aimNpc.transform, transform, MinTaoQuanDaoDanDisNpc, DaoDanDisNpc);
            }
        }
        else
        {
            if (m_UIController.mTaoQuanUI != null)
            {
                m_UIController.mTaoQuanUI.DestroySelf();
            }
        }
    }
}