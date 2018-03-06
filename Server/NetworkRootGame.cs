using UnityEngine;

/// <summary>
/// 游戏场景中的网络控制脚本.
/// </summary>
public class NetworkRootGame : MonoBehaviour
{
    /// <summary>
    /// 玩家当前的联机游戏状态.
    /// </summary>
    [HideInInspector]
    public NetworkServerNet.PlayerGameNetType ePlayerGameNetState = NetworkServerNet.PlayerGameNetType.Null;
}