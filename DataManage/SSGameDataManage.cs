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
}