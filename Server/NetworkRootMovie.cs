using UnityEngine;

public class NetworkRootMovie : MonoBehaviour
{
    public NetworkRpcMsgSpawn mNetworkRpcMsgSpawn;
    static NetworkRootMovie _Instance;
    public static NetworkRootMovie GetInstance()
    {
        return _Instance;
    }

    void Start()
    {
        _Instance = this;
    }
}