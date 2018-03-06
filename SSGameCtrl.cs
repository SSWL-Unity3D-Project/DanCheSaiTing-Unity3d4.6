using UnityEngine;

/// <summary>
/// 游戏控制脚本,始终不销毁.
/// </summary>
public class SSGameCtrl : MonoBehaviour
{
    /// <summary>
    /// 游戏场景总控制脚本.
    /// </summary>
    [HideInInspector]
    public SSGameRoot mSSGameRoot;
    /// <summary>
    /// 玩家选择的游戏模式.
    /// </summary>
    [HideInInspector]
    public NetworkRootMovie.GameMode eGameMode = NetworkRootMovie.GameMode.Null;
    /// <summary>
    /// 玩家數據管理.
    /// </summary>
    [HideInInspector]
    public PlayerDataManage mPlayerDataManage;
    static SSGameCtrl _Instance;
    public static SSGameCtrl GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_SSGameCtrl");
            _Instance = obj.AddComponent<SSGameCtrl>();
            _Instance.mPlayerDataManage = new PlayerDataManage();
            DontDestroyOnLoad(obj);
        }
        return _Instance;
    }
}