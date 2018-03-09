using UnityEngine;
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
    DaoJuZhangAiWuData _mDaoJuZhangAiWuData;
    /// <summary>
    /// 道具障碍物数据管理.
    /// </summary>
    public DaoJuZhangAiWuData mDaoJuZhangAiWuData
    {
        get
        {
            if (_mDaoJuZhangAiWuData == null)
            {
                _mDaoJuZhangAiWuData = CreatDaoJuZhangAiWuData();
            }
            return _mDaoJuZhangAiWuData;
        }
        set
        {
            _mDaoJuZhangAiWuData = value;
        }
    }
    AiNpcDataManage _mAiNpcData;
    /// <summary>
    /// AiNpc/主角数据列表管理.
    /// </summary>
    public AiNpcDataManage mAiNpcData
    {
        get
        {
            if (_mAiNpcData == null)
            {
                _mAiNpcData = CreatAiNpcData();
            }
            return _mAiNpcData;
        }
        set
        {
            _mAiNpcData = value;
        }
    }

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
    DaoJuZhangAiWuData CreatDaoJuZhangAiWuData()
    {
        if (_mDaoJuZhangAiWuData == null)
        {
            GameObject obj = new GameObject("_DaoJuZhangAiWuData");
            _mDaoJuZhangAiWuData = obj.AddComponent<DaoJuZhangAiWuData>();
        }
        return _mDaoJuZhangAiWuData;
    }

    /// <summary>
    /// 创建AiNpc数据.
    /// </summary>
    AiNpcDataManage CreatAiNpcData()
    {
        if (_mAiNpcData == null)
        {
            GameObject obj = new GameObject("_AiNpcData");
            _mAiNpcData = obj.AddComponent<AiNpcDataManage>();
        }
        return _mAiNpcData;
    }
}