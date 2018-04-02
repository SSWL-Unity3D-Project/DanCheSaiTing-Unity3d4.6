using UnityEngine;

public class NetworkEvent : MonoBehaviour
{
    static NetworkEvent _Instance = null;
    public static NetworkEvent GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_NetworkEvent");
            _Instance = obj.AddComponent<NetworkEvent>();
        }
        return _Instance;
    }

    #region Network Event
    /// <summary>
    /// 服务器初始化事件.
    /// </summary>
    public delegate void ServerInitializedEvent();
    public event ServerInitializedEvent OnServerInitializedEvent;
    public void OnServerInitializedTrigger()
    {
        Debug.Log("NetworkEvent::OnServerInitializedTrigger...");
        if (OnServerInitializedEvent != null)
        {
            OnServerInitializedEvent();
        }
    }

    /// <summary>
    /// 有玩家链接到服务器事件(服务端消息).
    /// </summary>
    public delegate void PlayerConnectedEvent();
    public event PlayerConnectedEvent OnPlayerConnectedEvent;
    public void OnPlayerConnectedTrigger()
    {
        Debug.Log("NetworkEvent::OnPlayerConnectedTrigger...");
        if (OnPlayerConnectedEvent != null)
        {
            OnPlayerConnectedEvent();
        }
    }

    /// <summary>
    /// 玩家链接到服务器事件(客户端消息).
    /// </summary>
    public delegate void ConnectedToServerEvent();
    public event ConnectedToServerEvent OnConnectedToServerEvent;
    public void OnConnectedToServerTrigger()
    {
        Debug.Log("NetworkEvent::OnConnectedToServerTrigger...");
        if (OnConnectedToServerEvent != null)
        {
            OnConnectedToServerEvent();
        }
    }
    
    /// <summary>
    /// 玩家链接主服务器失败消息事件.
    /// </summary>
    public delegate void FailedToConnectToMasterServerEvent();
    public event FailedToConnectToMasterServerEvent OnFailedToConnectToMasterServerEvent;
    public void OnFailedToConnectToMasterServerTrigger()
    {
        Debug.Log("NetworkEvent::OnFailedToConnectToMasterServerTrigger...");
        if (OnFailedToConnectToMasterServerEvent != null)
        {
            OnFailedToConnectToMasterServerEvent();
        }
    }

    /// <summary>
    /// Rpc发送加载游戏关卡事件.
    /// </summary>
    public delegate void RpcSendLoadLevelMsgEvent(int level);
    public event RpcSendLoadLevelMsgEvent OnRpcSendLoadLevelMsgEvent;
    public void OnRpcSendLoadLevelMsgTrigger(int level)
    {
        Debug.Log("NetworkEvent::OnRpcSendLoadLevelMsgTrigger -> level == " + level);
        if (OnRpcSendLoadLevelMsgEvent != null)
        {
            OnRpcSendLoadLevelMsgEvent(level);
        }
    }
    #endregion
}