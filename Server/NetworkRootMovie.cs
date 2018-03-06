using UnityEngine;

/// <summary>
/// 循环动画场景中的网络标记.
/// </summary>
public class NetworkRootMovie : MonoBehaviour
{
    public enum GameNetType
    {
        NoLink, //单机
        Link,   //联机
    }
    /// <summary>
    /// 游戏设置为单机/联机版本.
    /// </summary>
    public GameNetType eNetState = GameNetType.NoLink;

    public enum GameMode
    {
        Null,
        Link,    //联机
        NoLink,  //单机
    }
    /// <summary>
    /// 玩家选择的游戏模式(单机/联机).
    /// </summary>
    [HideInInspector]
    public GameMode ePlayerSelectGameMode = GameMode.Null;

    /// <summary>
    /// 玩家当前的联机游戏状态.
    /// </summary>
    [HideInInspector]
    public NetworkServerNet.PlayerGameNetType ePlayerGameNetState = NetworkServerNet.PlayerGameNetType.Null;
    /// <summary>
    /// NetworkRpcMsg预制.
    /// </summary>
    public GameObject NetworkRpcMsgPrefab;
    /// <summary>
    /// 循环动画场景里进行网络通信的脚本.
    /// </summary>
    [HideInInspector]
    public NetworkRpcMsgCtrl mNetworkRpcMsgScript;
    static NetworkRootMovie _Instance;
    public static NetworkRootMovie GetInstance()
    {
        return _Instance;
    }

    void Start()
    {
        _Instance = this;
        if (eNetState == GameNetType.Link)
        {
            NetworkServerNet.GetInstance();
        }
    }

    /// <summary>
    /// 创建NetworkRpcMsgCtrl组件(只在游戏的循环动画里进行).
    /// </summary>
    public void CreateNetworkRpc()
    {
        if (NetworkRpcMsgPrefab == null)
        {
            return;
        }

        GameObject obj = (GameObject)Network.Instantiate(NetworkRpcMsgPrefab, Vector3.zero, Quaternion.identity, 0);
        mNetworkRpcMsgScript = obj.GetComponent<NetworkRpcMsgCtrl>();
        mNetworkRpcMsgScript.Init();
    }
}