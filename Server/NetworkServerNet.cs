using UnityEngine;
using System.Collections;

public class NetworkServerNet : MonoBehaviour
{
    public enum PlayerPortType
    {
        Null,
        Client, //客户端.
        Server, //服务端.
    }
    /// <summary>
    /// 玩家在联机游戏中的状态.
    /// </summary>
    [HideInInspector]
    public PlayerPortType ePlayerPortState = PlayerPortType.Null;

    public enum PlayerGameNetType
    {
        Null,
        MovieIntoGame,   //循环动画进入游戏.
        GameBackMovie,   //游戏返回循环动画.
    }
    string MasterServerIpFile = "./MasterServerIP.info";
    string MasterServerIp = "192.168.0.2";
    private int mPort = 23465;
    bool IsTryToLinkServer = true;
    bool IsCreateServer = true;
    /// <summary>
    /// 游戏场景中网络root脚本.
    /// </summary>
    public NetworkRootGame mNetworkRootGame;
    /// <summary>
    /// 链接到服务器的玩家数量在循环动画场景中.
    /// </summary>
    [HideInInspector]
    public int LinkServerPlayerNum_Movie;
    /// <summary>
    /// 链接到服务器的玩家数量在游戏场景中.
    /// </summary>
    [HideInInspector]
    public int LinkServerPlayerNum_Game;
    /// <summary>
    /// 玩家网络数据索引.
    /// </summary>
    [HideInInspector]
    public int IndexSpawnPlayer;
    float TimeLastCreatServer;
    /// <summary>
    /// 网络游戏类型名称.
    /// </summary>
    [HideInInspector]
    public string mGameTypeName = "MyUniqueGameType";
    /// <summary>
    /// MasterServer控制脚本.
    /// </summary>
    [HideInInspector]
    public RequestMasterServer mRequestMasterServer;

    static NetworkServerNet _Instance;
    public static NetworkServerNet GetInstance()
    {
        if (_Instance == null)
        {
            Debug.Log("creat NetworkServerNet...");
            GameObject obj = new GameObject("_NetworkServerNet");
            _Instance = obj.AddComponent<NetworkServerNet>();
            _Instance.mRequestMasterServer = obj.AddComponent<RequestMasterServer>();
            _Instance.Init();
            DontDestroyOnLoad(obj);
        }
        return _Instance;
    }

    void Init()
    {
        if (!pcvr.bIsHardWare)
        {
            MasterServerIp = HandleJson.GetInstance().ReadFromFilePathXml(MasterServerIpFile, "MasterServerIp");
            if (MasterServerIp == null || MasterServerIp == "")
            {
                MasterServerIp = "192.168.0.2";
                HandleJson.GetInstance().WriteToFilePathXml(MasterServerIpFile, "MasterServerIp", MasterServerIp);
            }
        }
        Debug.Log("MasterServerIp " + MasterServerIp);

        if (MasterServerIp == Network.player.ipAddress)
        {
            XKMasterServerCtrl.CheckMasterServerIP();
        }

        //初始化MasterServer.
        MasterServer.ipAddress = MasterServerIp;
        Network.natFacilitatorIP = MasterServerIp;

        if (mRequestMasterServer != null)
        {
            mRequestMasterServer.Init();
        }
    }

    /// <summary>
    /// 重置信息.
    /// </summary>
    public void ResetInfo()
    {
        ePlayerPortState = PlayerPortType.Null;
    }

    void Update()
    {
        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                if (!IsCreateServer)
                {
                    TryToCreateServer();
                }
                break;

            case NetworkPeerType.Server:
                break;

            case NetworkPeerType.Client:
                break;

            case NetworkPeerType.Connecting:
                break;
        }
    }

    public void SetIndexSpawnPlayer(int index)
    {
        IndexSpawnPlayer = index;
        Debug.Log("SetIndexSpawnPlayer -> index == " + index);
    }

    void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer -> appLevel " + Application.loadedLevel);
        if (mNetworkRootGame != null)
        {
            //游戏场景.
            NetworkEvent.GetInstance().OnConnectedToServerTrigger();
        }
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }

    void OnServerInitialized()
    {
        Debug.Log("OnServerInitialized -> appLevel " + Application.loadedLevel);
        IsCreateServer = true;
        if (NetworkRootMovie.GetInstance() != null)
        {
            //循环动画场景.
            LinkServerPlayerNum_Movie = 0; //初始化.
            LinkServerPlayerNum_Game = 0;
            NetworkRootMovie.GetInstance().CreateNetworkRpc();
        }

        if (mNetworkRootGame != null)
        {
            //游戏场景.
            SetIndexSpawnPlayer(0);
            NetworkEvent.GetInstance().OnServerInitializedTrigger();
        }
    }
    
    void OnPlayerConnected(NetworkPlayer playerNet)
    {
        Debug.Log("NetworkServerNet::OnPlayerConnected -> ip " + playerNet.ipAddress + ", appGameLevel " + Application.loadedLevel);
        if (NetworkRootMovie.GetInstance() != null)
        {
            //循环动画场景.
            if (NetworkRootMovie.GetInstance().mNetworkRpcMsgScript != null)
            {
                NetworkRootMovie.GetInstance().mNetworkRpcMsgScript.NetSetSpawnPlayerIndex(playerNet, Network.connections.Length);
            }
            LinkServerPlayerNum_Movie = Network.connections.Length;
        }

        if (mNetworkRootGame != null)
        {
            //游戏场景.
            LinkServerPlayerNum_Game = Network.connections.Length;
            NetworkEvent.GetInstance().OnPlayerConnectedTrigger();
        }
    }
    
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("NetworkServerNet::OnPlayerDisconnected -> ip " + player.ipAddress);
        RemoveAllRPC(player);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("OnDisconnectedFromServer -> info is " + info + ", peerType " + Network.peerType);
        switch (info)
        {
            case NetworkDisconnection.Disconnected:
                {
                    if (Network.peerType == NetworkPeerType.Client)
                    {
                        RemoveClientHost(NetworkDisconnection.Disconnected);
                    }
                    break;
                }
        }
    }

    public void InitTryToLinkServer()
    {
        if (!IsTryToLinkServer)
        {
            return;
        }
        IsTryToLinkServer = false;
    }

    /// <summary>
    /// 初始化创建主服务器.
    /// </summary>
    public void InitCreateServer()
    {
        if (NetworkRootMovie.GetInstance() != null)
        {
            //循环动画场景间隔一定时间主动创建服务器.
            if (Time.time - TimeLastCreatServer < 9f)
            {
                //Debug.Log("InitCreateServer -> test...");
                return;
            }
            TimeLastCreatServer = Time.time;

            if (NetworkRootMovie.GetInstance().ePlayerGameNetState == PlayerGameNetType.Null)
            {
                StartCoroutine(DelayInitCreateServer((Random.Range(0, 100) % 4) * 3));
            }
            else
            {
                Debug.Log("InitCreateServer -> player into Game!");
            }
            return;
        }
        else
        {
            if (mNetworkRootGame != null)
            {
                //非循环动画场景直接创建服务器.
                if (mNetworkRootGame.ePlayerGameNetState == PlayerGameNetType.Null && ePlayerPortState == PlayerPortType.Server)
                {
                    IsCreateServer = false;
                }
            }
        }
    }

    IEnumerator DelayInitCreateServer(float timeVal)
    {
        Debug.Log("DelayInitCreateServer -> randTime == " + timeVal);
        yield return new WaitForSeconds(timeVal);
        IsCreateServer = false;
    }

    void RemoveAllRPC(NetworkPlayer playerNet)
    {
        if (Network.isServer)
        {
            Network.RemoveRPCs(playerNet);
            Network.DestroyPlayerObjects(playerNet);
        }
    }
    
    /// <summary>
    /// 删除当前主服务器.
    /// </summary>
    public void RemoveMasterServerHost()
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            RemoveAllRPC(Network.player);
            Network.Disconnect(30);
            MasterServer.UnregisterHost();
        }
    }

    /// <summary>
    /// 删除当前客户端.
    /// </summary>
    public void RemoveClientHost(NetworkDisconnection info = NetworkDisconnection.LostConnection)
    {
        if (Network.peerType == NetworkPeerType.Client)
        {
            if (NetworkRootMovie.GetInstance().mNetworkRpcMsgScript != null)
            {
                NetworkRootMovie.GetInstance().mNetworkRpcMsgScript.RemoveSelf();
            }

            if (info == NetworkDisconnection.LostConnection)
            {
                Network.Disconnect();
            }
        }
    }

    void TryToCreateServer()
    {
        if (NetworkRootMovie.GetInstance() != null)
        {
            //循环动画场景.
            if (NetworkRootMovie.GetInstance().ePlayerSelectGameMode != NetworkRootMovie.GameMode.Link)
            {
                //只有当玩家选择了联机游戏时,才允许创建主服务器.
                return;
            }

            if (NetworkRootMovie.GetInstance().ePlayerGameNetState == PlayerGameNetType.MovieIntoGame)
            {
                //如果循环动画正在加载游戏场景时,不允许创建服务器.
                return;
            }
        }

        if (mNetworkRootGame != null)
        {
            //游戏场景
            if (mNetworkRootGame.ePlayerGameNetState == PlayerGameNetType.GameBackMovie)
            {
                //如果游戏场景正在加载循环动画时,不允许创建服务器.
                return;
            }

            if (ePlayerPortState != PlayerPortType.Server)
            {
                //不是Server端的不允许创建服务器.
                return;
            }
        }

        if (IsCreateServer)
        {
            return;
        }
        IsCreateServer = true;

        Debug.Log("start create server, time " + Time.time.ToString("f2") + ", ePlayerPortState " + ePlayerPortState
            + ", level " + Application.loadedLevel);
        Network.InitializeServer(3, mPort, true);

        Debug.Log("masterServer.ip " + MasterServer.ipAddress + ", port " + MasterServer.port + ", updateRate " + MasterServer.updateRate);
        MasterServer.dedicatedServer = true;
        
        switch (mRequestMasterServer.eMasterComment)
        {
            case RequestMasterServer.MasterServerComment.Movie:
                mRequestMasterServer.SetMasterServerIp(Network.player.ipAddress);
                MasterServer.RegisterHost(mGameTypeName, "My game", mRequestMasterServer.MasterServerMovieComment);
                break;

            case RequestMasterServer.MasterServerComment.GameNet:
                MasterServer.RegisterHost(mGameTypeName, "My game", mRequestMasterServer.MasterServerGameNetComment);
                break;
        }
    }

    /// <summary>
    /// 当游戏退出时.
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit...NetServer");
        if (MasterServerIp == Network.player.ipAddress)
        {
            XKMasterServerCtrl.CloseMasterServer();
        }
    }
}