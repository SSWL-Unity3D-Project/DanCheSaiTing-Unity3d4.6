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
    #endregion
}