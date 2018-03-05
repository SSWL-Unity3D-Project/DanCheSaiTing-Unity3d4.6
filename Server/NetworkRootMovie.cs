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
    public GameNetType eNetState = GameNetType.NoLink;
    /// <summary>
    /// 玩家选择单机/联机游戏状态.
    /// </summary>
    [HideInInspector]
    public GameNetType ePlayerSelectNetState = GameNetType.NoLink;
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