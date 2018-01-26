
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
    int _DaoDanNum;
    /// <summary>
    /// 导弹数量.
    /// </summary>
    public int DaoDanNum
    {
        set
        {
            if (value > 9)
            {
                value = 9;
            }

            _DaoDanNum = value;
            if (PlayerController.GetInstance() != null)
            {
                PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.UpdateDaoDanInfo(_DaoDanNum);
            }
        }
        get
        {
            return _DaoDanNum;
        }
    }
    int _DiLeiNum;
    /// <summary>
    /// 地雷数量.
    /// </summary>
    public int DiLeiNum
    {
        set
        {
            if (value > 9)
            {
                value = 9;
            }

            _DiLeiNum = value;
            if (PlayerController.GetInstance() != null)
            {
                PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.UpdateDiLeiInfo(_DiLeiNum);
            }
        }
        get
        {
            return _DiLeiNum;
        }
    }
    public PlayerDataManage()
    {
        PlayerCoinNum = System.Convert.ToInt32(ReadGameInfo.GetInstance().ReadInsertCoinNum());
        CoinNumNeed = System.Convert.ToInt32(ReadGameInfo.GetInstance().ReadStarCoinNumSet());
        CoinNumFeiXing = 1;
        GameMode = ReadGameInfo.GetInstance().ReadGameStarMode();
        DaoDanNum = 5; //test
        DiLeiNum = 5; //test
    }
}