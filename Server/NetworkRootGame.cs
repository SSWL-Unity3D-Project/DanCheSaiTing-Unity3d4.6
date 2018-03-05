using UnityEngine;

public class NetworkRootGame : MonoBehaviour
{
    /// <summary>
    /// 玩家当前的联机游戏状态.
    /// </summary>
    [HideInInspector]
    public NetworkServerNet.PlayerGameNetType ePlayerGameNetState = NetworkServerNet.PlayerGameNetType.Null;
}