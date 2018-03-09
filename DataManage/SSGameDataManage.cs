using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据管理.
/// </summary>
public class SSGameDataManage : MonoBehaviour
{
    /// <summary>
    /// 玩家数据.
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        /// <summary>
        /// 画面颜色控制.
        /// </summary>
        public ColorCorrectionCurves m_ColorEffect;
        /// <summary>
        /// 游戏UI控制.
        /// </summary>
        public UIController m_UIController;
        /// <summary>
        /// 出河道UI提示.
        /// </summary>
        public GameObject m_OutHedao;
        /// <summary>
        /// 运动方向错误UI提示
        /// </summary>
        public GameObject m_ErrorDirection;
        /// <summary>
        /// 路径.
        /// </summary>
        public Transform Path;
        /// <summary>
        /// 镜头镜像模糊.
        /// </summary>
        public RadialBlur m_RadialBlurEffect;
        /// <summary>
        /// 镜头抖动控制.
        /// </summary>
        public CameraShake m_CameraShake;
        /// <summary>
        /// 镜头平滑移动控制.
        /// </summary>
        public CameraSmooth m_CameraSmooth;
        /// <summary>
        /// 撞击石头音效.
        /// </summary>
        public AudioSource m_HitStone;
        /// <summary>
        /// 撞击水音效.
        /// </summary>
        public AudioSource m_HitWater;
        /// <summary>
        /// 发动机引擎音效.
        /// </summary>
        public AudioSource m_YinqingAudio;
        /// <summary>
        /// 水花音效.
        /// </summary>
        public AudioSource m_ShuihuaAudio;
        /// <summary>
        /// 背景音效.
        /// </summary>
        public AudioSource m_BeijingAudio;
        /// <summary>
        /// 方向错误音效.
        /// </summary>
        public AudioSource m_ErrorDirectionAudio;
        /// <summary>
        /// 环境森林音效.
        /// </summary>
        public AudioSource m_HuanjingSenlin;
        /// <summary>
        /// 环境水流音效.
        /// </summary>
        public AudioSource m_HuanjingShuiliu;
        /// <summary>
        /// 加速飞板音效.
        /// </summary>
        public AudioSource m_FeibanAudio;
        public AudioSource m_JiasuAudio;
        /// <summary>
        /// 变形翼音效.
        /// </summary>
        public AudioSource mBianXingYiAudio;
        /// <summary>
        /// 得到加速道具音效.
        /// </summary>
        public AudioSource m_EatJiasuAudio;
        /// <summary>
        /// 加时UI.
        /// </summary>
        public GameObject m_JiashiGameObject;
        /// <summary>
        /// 加时音效.
        /// </summary>
        public AudioSource m_JiashiAudio;
        /// <summary>
        /// 得到加时道具音效.
        /// </summary>
        public AudioSource m_EatJiashiAudio;
        /// <summary>
        /// 喇叭音效.
        /// </summary>
        public AudioSource LaBaAudio;
    }
    public PlayerData mPlayerDt;

    /// <summary>
    /// 玩家控制脚本列表.
    /// </summary>
    [HideInInspector]
    public List<PlayerController> mPlayerControllerList = new List<PlayerController>();
    /// <summary>
    /// 游戏主角控制脚本.
    /// </summary>
    [HideInInspector]
    public PlayerController mPlayerController;

    /// <summary>
    /// Npc数据.
    /// </summary>
    [System.Serializable]
    public class NpcData
    {
        /// <summary>
        /// npc路径.
        /// </summary>
        public Transform[] m_NpcPathArray = new Transform[3];
    }
    public NpcData mNpcDt;

    /// <summary>
    /// 游戏配置数据.
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        /// <summary>
        /// 主角预制.
        /// </summary>
        public GameObject mPlayerPrefab;
        /// <summary>
        /// Npc预制.
        /// </summary>
        public GameObject mNpcPrefab;
        /// <summary>
        /// 主角/npc产生点.
        /// </summary>
        public Transform[] SpawnPointArray = new Transform[4];
        /// <summary>
        /// 联机状态需要隐藏的对象.
        /// </summary>
        public GameObject[] HiddenObjLinkArray;
        /// <summary>
        /// 联机游戏最终倒计时(多人联机时启用).
        /// </summary>
        [Range(1, 10)]
        public int TimeNetEndVal = 5;
        /// <summary>
        /// 游戏时长信息.
        /// </summary>
        public float m_pGameTime = 300.0f;
        /// <summary>
        /// 游戏当前关卡的路径长度信息.
        /// </summary>
        public float DistancePath = 6400;
        /// <summary>
        /// 联机游戏中玩家的控制脚本.
        /// </summary>
        public List<PlayerController> mNetPlayerComList = new List<PlayerController>();
        /// <summary>
        /// 创建主角.
        /// </summary>
        public void SpawnPlayer(int indexVal)
        {
            PlayerController playerScript = null;
            GameObject obj = null;
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                //单机模式.
                obj = (GameObject)Instantiate(mPlayerPrefab, SpawnPointArray[indexVal].position, SpawnPointArray[indexVal].rotation);
                NetworkView netView = obj.GetComponent<NetworkView>();
                netView.enabled = false;
                playerScript = obj.GetComponent<PlayerController>();
            }
            else
            {
                //联机模式.
                obj = (GameObject)Network.Instantiate(mPlayerPrefab, SpawnPointArray[indexVal].position, SpawnPointArray[indexVal].rotation, 0);
                playerScript = obj.GetComponent<PlayerController>();
                if (HiddenObjLinkArray != null && HiddenObjLinkArray.Length > 0)
                {
                    for (int i = 0; i < HiddenObjLinkArray.Length; i++)
                    {
                        if (HiddenObjLinkArray[i] != null)
                        {
                            HiddenObjLinkArray[i].SetActive(false);
                        }
                    }
                }
            }
            playerScript.SetIsNetControlPort(true);
            playerScript.SetPlayerIndex(indexVal);
        }

        /// <summary>
        /// 创建Npc.
        /// </summary>
        public void SpawnNpc(int indexVal)
        {
            GameObject obj = null;
            NpcController npcScript = null;
            NpcController.NpcEnum npcState = (NpcController.NpcEnum)indexVal;
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                obj = (GameObject)Instantiate(mNpcPrefab, SpawnPointArray[indexVal].position, SpawnPointArray[indexVal].rotation);
                NetworkView netView = obj.GetComponent<NetworkView>();
                netView.enabled = false;
            }
            else
            {
                obj = (GameObject)Network.Instantiate(mNpcPrefab, SpawnPointArray[indexVal].position, SpawnPointArray[indexVal].rotation, 0);
            }

            npcScript = obj.GetComponent<NpcController>();
            npcScript.SetIsNetControlPort(true);
            npcScript.SetNpcState(npcState);
            npcScript.SetNpcIndex(indexVal);
        }
    }
    public GameData mGameData;
}