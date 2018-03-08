#define SHOW_NET_INFO

using UnityEngine;

public class RequestMasterServer : MonoBehaviour
{
    public enum MasterServerComment
    {
        Null,
        Movie,
        GameNet,
    }
    /// <summary>
    /// 主服务器创建时的注解标记.
    /// </summary>
    [HideInInspector]
    public MasterServerComment eMasterComment = MasterServerComment.Null;

    public enum GameScene : int
    {
        Movie = 0,
        Scene01 = 1,
        Scene01_Net = 2,
        SetPanel = 3
    }
    bool IsClickConnectServer;
    /// <summary>
    /// 循环动画场景中主服务器的注解.
    /// </summary>
    [HideInInspector]
    public string MasterServerMovieComment = "Movie Scene";
    /// <summary>
    /// 联机游戏场景中主服务器的注解.
    /// </summary>
    [HideInInspector]
    public string MasterServerGameNetComment = "GameNet Scene";
    /// <summary>
    /// 玩家链接游戏的服务器IP.
    /// </summary>
    [HideInInspector]
    public string ServerIp = "";
    float TimeConnect;
    /// <summary>
    /// 访问Host的记录时间.
    /// </summary>
    float TimeLastRequesHost = 0f;
    /// <summary>
    /// 检测MasterServer的记录时间.
    /// </summary>
    float TimeLastCheckMasterServer = 0f;

    public void Init()
    {
        eMasterComment = MasterServerComment.Movie;
        SetIsNetScene(true);
    }

    void Update()
    {
        if (Time.time - TimeLastRequesHost >= 3f)
        {
            TimeLastRequesHost = Time.time;
            RequestHostListLoop();
        }

        if (Time.time - TimeLastCheckMasterServer >= 0.1f)
        {
            TimeLastCheckMasterServer = Time.time;
            CheckMasterServerList();
        }
    }

    void RequestHostListLoop()
    {
        MasterServer.RequestHostList(NetworkServerNet.GetInstance().mGameTypeName);
    }

    float RandConnectTime = Random.Range(3f, 10f);
    float TimeConnectServer = 0f;
    void OnGUI()
    {
        HostData[] data = MasterServer.PollHostList();
        foreach (var element in data)
        {
#if SHOW_NET_INFO
            var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
            GUILayout.BeginHorizontal();
            GUILayout.Box(name);
            GUILayout.Space(5);

            var hostInfo = "[";
            foreach (var host in element.ip)
            {
                hostInfo = hostInfo + host + ":" + element.port + " ";
            }
            hostInfo = hostInfo + "]";
            GUILayout.Box(hostInfo);
            GUILayout.Space(5);
            GUILayout.Box(element.comment);
            GUILayout.Space(5);
            GUILayout.FlexibleSpace();
#endif
            
            if (Network.peerType == NetworkPeerType.Disconnected && IsNetScene)
            {
                if (!IsClickConnectServer)
                {
                    bool isConnectServer = false;
                    if (NetworkServerNet.GetInstance().mNetworkRootGame != null
                        && NetworkServerNet.GetInstance().ePlayerPortState == NetworkServerNet.PlayerPortType.Client
                        && element.comment == MasterServerGameNetComment
                        && element.ip[0] != Network.player.ipAddress
                        && ServerIp == element.ip[0])
                    {
                        //游戏场景中.
                        if (NetworkServerNet.GetInstance().mNetworkRootGame.ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.GameBackMovie)
                        {
                            //如果游戏场景正在加载循环动画时,不允许链接服务器.
                        }
                        else
                        {
                            if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime)
                            {
                                isConnectServer = true;
                                TimeConnectServer = Time.realtimeSinceStartup;
                                RandConnectTime = (Random.Range(0, 100) % 5) + 3f;
                            }
                        }
                    }

                    if (NetworkRootMovie.GetInstance() != null
                        && NetworkRootMovie.GetInstance().ePlayerSelectGameMode == NetworkRootMovie.GameMode.Link
                        && element.comment == MasterServerMovieComment
                        && element.ip[0] != Network.player.ipAddress
                        && element.connectedPlayers < element.playerLimit)
                    {
                        //循环动画场景中.
                        if (NetworkRootMovie.GetInstance().ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.MovieIntoGame)
                        {
                            //如果循环动画正在加载游戏场景时,不允许链接服务器.
                        }
                        else
                        {
                            if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime)
                            {
                                isConnectServer = true;
                                TimeConnectServer = Time.realtimeSinceStartup;
                                RandConnectTime = (Random.Range(0, 100) % 5) + 3f;
                            }
                        }
                    }

                    if (isConnectServer)
                    {
                        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
                        {
                            //清理网络垃圾信息.
                            Network.RemoveRPCs(Network.player);
                            Network.DestroyPlayerObjects(Network.player);
                        }

                        MasterServer.dedicatedServer = false;
                        Network.Connect(element);
                        IsClickConnectServer = true;
                        if (NetworkRootMovie.GetInstance() != null)
                        {
                            //循环动画场景中.
                            SetMasterServerIp(element.ip[0]);
                            TimeConnect = 0f;
                        }
                        Debug.Log("Connect ip -> " + element.ip[0]
                                  + ", comment " + element.comment
                                  + ", time " + Time.time.ToString("f2"));
                    }
                }
                else
                {
                    if (NetworkServerNet.GetInstance().mNetworkRootGame != null)
                    {
                        //游戏场景中.
                        if (element.comment == MasterServerGameNetComment && ServerIp == element.ip[0])
                        {
                            TimeConnect += Time.deltaTime;
                            if (TimeConnect >= 2f)
                            {
                                TimeConnect = 0f;
                                IsClickConnectServer = false;
                            }
                        }
                    }

                    if (NetworkRootMovie.GetInstance() != null)
                    {
                        //循环动画场景中.
                        TimeConnect += Time.deltaTime;
                        if (TimeConnect >= 2f)
                        {
                            TimeConnect = 0f;
                            IsClickConnectServer = false;
                        }
                    }
                }
            }
#if SHOW_NET_INFO
            GUILayout.EndHorizontal();
#endif
        }
    }

    public void ResetIsClickConnect()
    {
        IsClickConnectServer = false;
    }

    public void SetMasterServerIp(string ip)
    {
        if (ServerIp != ip)
        {
            ServerIp = ip;
            Debug.Log("SetMasterServerIp -> ServerIp == " + ServerIp);
        }
    }

    /// <summary>
    /// 获取当前在主服务器创建MasterServerMovieComment服务的数量.
    /// 在游戏循环动画界面一般只需要创建一个MasterServerMovieComment服务即可.
    /// </summary>
    public int GetMovieMasterServerNum()
    {
        int masterServerNum = 0;
        string ip = "";
        HostData[] data = MasterServer.PollHostList();
        foreach (var element in data)
        {
            if (element.comment == MasterServerMovieComment)
            {
                masterServerNum++;
                ip = element.ip[0];
            }
        }

        if (masterServerNum == 1)
        {
            SetMasterServerIp(ip);
        }
        return masterServerNum;
    }

    /// <summary>
    /// 是否是网络场景.
    /// </summary>
    bool IsNetScene = false;
    /// <summary>
    /// 设置IsNetScene属性.
    /// 当游戏切换到非联机场景时设置为false.
    /// 当游戏切换到联机游戏场景时设置为true.
    /// </summary>
    public void SetIsNetScene(bool isNet)
    {
        IsNetScene = isNet;
    }

    void CheckMasterServerList()
    {
        int masterServerNum = 0;
        bool isCreatMasterServer = true;
        HostData[] data = MasterServer.PollHostList();

        if (NetworkRootMovie.GetInstance() != null)
        {
            //循环动画场景.
            foreach (var element in data)
            {
                if (element.comment == MasterServerMovieComment)
                {
                    masterServerNum++;
                    if (Network.peerType == NetworkPeerType.Disconnected)
                    {
                        if (masterServerNum > 0)
                        {
                            isCreatMasterServer = false;
                        }
                    }
                    else if (Network.peerType == NetworkPeerType.Server)
                    {
                        if (masterServerNum > 1 && Random.Range(0, 100) % 2 == 1)
                        {
                            //随机删除1个循环动画场景的masterServer.
                            isCreatMasterServer = false;
                            Debug.Log("random remove masterServer...");
                        }
                    }
                }
            }
        }

        if (!IsNetScene)
        {
            //不需要网络链接的游戏场景中不进行主服务器MasterServer的创建.
            isCreatMasterServer = false;
        }

        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                {
                    if (isCreatMasterServer)
                    {
                        if (NetworkRootMovie.GetInstance() != null)
                        {
                            //循环动画场景.
                            if (NetworkRootMovie.GetInstance().ePlayerSelectGameMode != NetworkRootMovie.GameMode.Link)
                            {
                                //只有当玩家选择了联机游戏时,才允许创建主服务器.
                                return;
                            }

                            if (NetworkRootMovie.GetInstance().ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.MovieIntoGame)
                            {
                                //如果循环动画正在加载游戏场景时,不允许创建服务器.
                                return;
                            }
                        }
                        NetworkServerNet.GetInstance().InitCreateServer();
                    }
                    break;
                }
            case NetworkPeerType.Server:
                {
                    if (!isCreatMasterServer)
                    {
                        NetworkServerNet.GetInstance().RemoveMasterServerHost();
                    }
                    break;
                }
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        Debug.Log("Could not connect to master server: " + info);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        //Debug.Log("OnMasterServerEvent: " + msEvent + ", time " + Time.time);
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("MasterServer registered, GameLevel " + Application.loadedLevel);
        }
    }

    /// <summary>
    /// 设置主服务器创建时的注解标记.
    /// </summary>
    public void SetMasterServerComment(MasterServerComment comment)
    {
        Debug.Log("SetMasterServerComment -> comment " + comment);
        eMasterComment = comment;
    }
}