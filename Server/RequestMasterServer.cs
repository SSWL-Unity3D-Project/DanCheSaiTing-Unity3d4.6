#define SHOW_NET_INFO

using UnityEngine;

public class RequestMasterServer : MonoBehaviour
{
    public enum GameScene : int
    {
        Movie = 0,
        Scene01 = 1,
        Scene01_Net = 2,
        SetPanel = 3
    }
    bool IsClickConnect;
    public static string MasterServerMovieComment = "Movie Scence";
    public static string MasterServerGameNetComment = "GameNet Scence";
    string ServerIp = "";
    float TimeConnect;
    /// <summary>
    /// 访问Host的记录时间.
    /// </summary>
    float TimeLastRequesHost = 0f;
    /// <summary>
    /// 检测MasterServer的记录时间.
    /// </summary>
    float TimeLastCheckMasterServer = 0f;

    static RequestMasterServer _Instance;
    public static RequestMasterServer GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_RequestMasterServer");
            _Instance = obj.AddComponent<RequestMasterServer>();
            DontDestroyOnLoad(obj);
        }
        return _Instance;
    }

    //void Start()
    //{
        //InitLoopRequestHostList();

        //CancelInvoke("CheckMasterServerList");
        //InvokeRepeating("CheckMasterServerList", 3f, 0.1f);
    //}

    //void InitLoopRequestHostList()
    //{
    //    CancelInvoke("RequestHostListLoop");
    //    InvokeRepeating("RequestHostListLoop", 0f, 3f);
    //}

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
    public static float TimeConnectServer = 0f;
    void OnGUI()
    {
        //GameScene levelVal = (GameScene)Application.loadedLevel;
        GameScene levelVal = (GameScene)GlobalData.GetInstance().gameLeve;
        HostData[] data = MasterServer.PollHostList();

        // Go through all the hosts in the host list
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

            //if (element.comment == MasterServerGameNetComment
            //    && ServerIp == element.ip[0]
            //    && Toubi.GetInstance() != null
            //    && !Toubi.GetInstance().IsIntoPlayGame)
            //{
            //    Toubi.GetInstance().IsIntoPlayGame = true;
            //}

            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                if (!IsClickConnect)
                {
                    bool isConnectServer = false;
                    if (levelVal == GameScene.Scene01_Net
                          && element.comment == MasterServerGameNetComment
                          && element.ip[0] != Network.player.ipAddress
                          && ServerIp == element.ip[0])
                    {
                        if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime)
                        {
                            isConnectServer = true;
                            TimeConnectServer = Time.realtimeSinceStartup;
                            RandConnectTime = Random.Range(3f, 10f);
                        }
                    }
                    //else if (levelVal == GameScene.Movie
                    //             && element.comment == MasterServerMovieComment
                    //             && element.ip[0] != Network.player.ipAddress
                    //             && element.connectedPlayers < element.playerLimit
                    //             && Toubi.GetInstance() != null
                    //             && Toubi.GetInstance().CheckIsLoopWait())
                    //{

                    //    if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime)
                    //    {
                    //        isConnectServer = true;
                    //        TimeConnectServer = Time.realtimeSinceStartup;
                    //        RandConnectTime = Random.Range(3f, 10f);
                    //    }
                    //}

                    if (isConnectServer)
                    {
                        // Connect to HostData struct, internally the correct method is used (GUID when using NAT).
                        Network.RemoveRPCs(Network.player);
                        Network.DestroyPlayerObjects(Network.player);

                        MasterServer.dedicatedServer = false;
                        Network.Connect(element);
                        IsClickConnect = true;
                        if (levelVal == GameScene.Movie)
                        {
                            ServerIp = element.ip[0];
                            TimeConnect = 0f;
                        }
                        Debug.Log("Connect element.ip -> " + element.ip[0]
                                  + ", element.comment " + element.comment
                                  + ", gameLeve " + levelVal
                                  + ", time " + Time.realtimeSinceStartup.ToString("f2"));
                    }
                }
                else
                {
                    if (levelVal == GameScene.Scene01_Net)
                    {
                        if (element.comment == MasterServerGameNetComment && ServerIp == element.ip[0])
                        {
                            TimeConnect += Time.deltaTime;
                            if (TimeConnect >= 2f)
                            {
                                TimeConnect = 0f;
                                IsClickConnect = false;
                            }
                        }
                    }
                    else if (levelVal == GameScene.Movie)
                    {
                        TimeConnect += Time.deltaTime;
                        if (TimeConnect >= 2f)
                        {
                            TimeConnect = 0f;
                            IsClickConnect = false;
                            Debug.Log("reconnect masterServer...");
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
        IsClickConnect = false;
    }

    public void SetMasterServerIp(string ip)
    {
        ServerIp = ip;
    }

    public int GetMovieMasterServerNum()
    {
        int masterServerNum = 0;
        HostData[] data = MasterServer.PollHostList();

        // Go through all the hosts in the host list
        foreach (var element in data)
        {
            if (element.comment == MasterServerMovieComment)
            {
                masterServerNum++;
            }
        }
        return masterServerNum;
    }
    
    void CheckMasterServerList()
    {
        int masterServerNum = 0;
        bool isCreatMasterServer = true;
        HostData[] data = MasterServer.PollHostList();

        // Go through all the hosts in the host list
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

        //GameScene levelVal = (GameScene)Application.loadedLevel;
        GameScene levelVal = (GameScene)GlobalData.GetInstance().gameLeve;
        if (levelVal == GameScene.Scene01 || levelVal == GameScene.SetPanel)
        {
            //不需要网络链接的游戏场景.
            isCreatMasterServer = false;
        }

        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                if (isCreatMasterServer)
                {
                    if (levelVal == GameScene.Movie)
                    {
                        //if ((Toubi.GetInstance() != null && !Toubi.GetInstance().CheckIsLoopWait())
                        //    || Toubi.GetInstance() == null)
                        //{
                        //    return;
                        //}
                        ServerIp = "";
                    }
                    NetworkServerNet.GetInstance().InitCreateServer();
                }
                break;

            case NetworkPeerType.Server:
                if (!isCreatMasterServer)
                {
                    NetworkServerNet.GetInstance().RemoveMasterServerHost();
                }
                else
                {
                    if (levelVal == GameScene.Movie)
                    {
                        //if (Toubi.GetInstance() != null && !Toubi.GetInstance().CheckIsLoopWait())
                        //{
                        //    NetworkServerNet.GetInstance().ResetMasterServerHost();
                        //}
                    }
                }
                break;
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        Debug.Log("Could not connect to master server: " + info);
        //if (Application.loadedLevel == (int)GameLeve.Movie) {
        //	ServerLinkInfo.GetInstance().SetServerLinkInfo("Cannot Link MasterServer");
        //}
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        //Debug.Log("OnMasterServerEvent: " + msEvent + ", time " + Time.time);
        if (msEvent == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("MasterServer registered, GameLevel " + (GameScene)Application.loadedLevel);
            if ((GameScene)Application.loadedLevel == GameScene.Movie)
            {
                //只在循环动画场景执行!
                //ServerLinkInfo.GetInstance().HiddenServerLinkInfo();
                NetworkRootMovie.GetInstance().mNetworkRpcMsgSpawn.CreateNetworkRpc();
            }
        }
    }
}