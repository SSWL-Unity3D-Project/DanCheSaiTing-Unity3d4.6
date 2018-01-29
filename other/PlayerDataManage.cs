﻿using UnityEngine;
/// <summary>
/// 玩家数据管理.
/// </summary>
public class PlayerDataManage
{
    /// <summary>
    /// 玩家币值信息.
    /// </summary>
    public int PlayerCoinNum = 0;
    /// <summary>
    /// 启动游戏需要消耗的币值.
    /// </summary>
    public int CoinNumNeed = 0;
    /// <summary>
    /// 游戏主角飞行加速需要消耗的币值.
    /// </summary>
    public int CoinNumFeiXing = 0;
    /// <summary>
    /// 游戏运营模式.
    /// </summary>
    public string GameMode = "";
    /// <summary>
    /// 道具障碍物数据管理.
    /// </summary>
    public DaoJuZhangAiWuData mDaoJuZhangAiWuData;
    /// <summary>
    /// AiNpc数据管理.
    /// </summary>
    public AiNpcDataManage mAiNpcData;

    public PlayerDataManage()
    {
        PlayerCoinNum = System.Convert.ToInt32(ReadGameInfo.GetInstance().ReadInsertCoinNum());
        CoinNumNeed = System.Convert.ToInt32(ReadGameInfo.GetInstance().ReadStarCoinNumSet());
        CoinNumFeiXing = 1;
        GameMode = ReadGameInfo.GetInstance().ReadGameStarMode();
    }

    /// <summary>
    /// 创建道具障碍物数据.
    /// </summary>
    public void CreatDaoJuZhangAiWuData()
    {
        if (mDaoJuZhangAiWuData == null)
        {
            GameObject obj = new GameObject("_DaoJuZhangAiWuData");
            mDaoJuZhangAiWuData = obj.AddComponent<DaoJuZhangAiWuData>();
        }
    }
    
    /// <summary>
    /// 创建AiNpc数据.
    /// </summary>
    public void CreatAiNpcData()
    {
        if (mAiNpcData == null)
        {
            GameObject obj = new GameObject("_AiNpcData");
            mAiNpcData = obj.AddComponent<AiNpcDataManage>();
        }
    }
}