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
    /// ������������ʱ��ע����.
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
    /// ѭ����������������������ע��.
    /// </summary>
    [HideInInspector]
    public string MasterServerMovieComment = "Movie Scene";
    /// <summary>
    /// ������Ϸ����������������ע��.
    /// </summary>
    [HideInInspector]
    public string MasterServerGameNetComment = "GameNet Scene";
    /// <summary>
    /// ���������Ϸ�ķ�����IP.
    /// </summary>
    [HideInInspector]
    public string ServerIp = "";
    float TimeConnect;
    /// <summary>
    /// ����Host�ļ�¼ʱ��.
    /// </summary>
    float TimeLastRequesHost = 0f;
    /// <summary>
    /// ���MasterServer�ļ�¼ʱ��.
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
                        //��Ϸ������.
                        if (NetworkServerNet.GetInstance().mNetworkRootGame.ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.GameBackMovie)
                        {
                            //�����Ϸ�������ڼ���ѭ������ʱ,���������ӷ�����.
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
                        //ѭ������������.
                        if (NetworkRootMovie.GetInstance().ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.MovieIntoGame)
                        {
                            //���ѭ���������ڼ�����Ϸ����ʱ,���������ӷ�����.
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
                            //��������������Ϣ.
                            Network.RemoveRPCs(Network.player);
                            Network.DestroyPlayerObjects(Network.player);
                        }

                        MasterServer.dedicatedServer = false;
                        Network.Connect(element);
                        IsClickConnectServer = true;
                        if (NetworkRootMovie.GetInstance() != null)
                        {
                            //ѭ������������.
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
                        //��Ϸ������.
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
                        //ѭ������������.
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
    /// ��ȡ��ǰ��������������MasterServerMovieComment���������.
    /// ����Ϸѭ����������һ��ֻ��Ҫ����һ��MasterServerMovieComment���񼴿�.
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
    /// �Ƿ������糡��.
    /// </summary>
    bool IsNetScene = false;
    /// <summary>
    /// ����IsNetScene����.
    /// ����Ϸ�л�������������ʱ����Ϊfalse.
    /// ����Ϸ�л���������Ϸ����ʱ����Ϊtrue.
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
            //ѭ����������.
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
                            //���ɾ��1��ѭ������������masterServer.
                            isCreatMasterServer = false;
                            Debug.Log("random remove masterServer...");
                        }
                    }
                }
            }
        }

        if (!IsNetScene)
        {
            //����Ҫ�������ӵ���Ϸ�����в�������������MasterServer�Ĵ���.
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
                            //ѭ����������.
                            if (NetworkRootMovie.GetInstance().ePlayerSelectGameMode != NetworkRootMovie.GameMode.Link)
                            {
                                //ֻ�е����ѡ����������Ϸʱ,����������������.
                                return;
                            }

                            if (NetworkRootMovie.GetInstance().ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.MovieIntoGame)
                            {
                                //���ѭ���������ڼ�����Ϸ����ʱ,��������������.
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
    /// ����������������ʱ��ע����.
    /// </summary>
    public void SetMasterServerComment(MasterServerComment comment)
    {
        Debug.Log("SetMasterServerComment -> comment " + comment);
        eMasterComment = comment;
    }
}