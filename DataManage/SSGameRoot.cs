﻿using UnityEngine;

/// <summary>
/// 游戏总控制.
/// </summary>
public class SSGameRoot : MonoBehaviour
{
    /// <summary>
    /// 游戏中网络root脚本.
    /// </summary>
    public NetworkRootGame mNetworkRootGame;
    /// <summary>
    /// 游戏中数据管理脚本.
    /// </summary>
    public SSGameDataManage mSSGameDataManage;
    /// <summary>
    /// 游戏UI总控.
    /// </summary>
    public UIController mUIController;
    SSTimeUpCtrl mSSTimeUpCom;
    void Awake()
    {
        Debug.Log("SSGameRoot::Awake -> peerType " + Network.peerType + ", eGameMode " + SSGameCtrl.GetInstance().eGameMode);
        SSGameCtrl.GetInstance().mSSGameRoot = this;
        NetworkServerNet.GetInstance().mNetworkRootGame = mNetworkRootGame;

        switch (SSGameCtrl.GetInstance().eGameMode)
        {
            case NetworkRootMovie.GameMode.Null:
            case NetworkRootMovie.GameMode.NoLink:
                {
                    mSSGameDataManage.mGameData.SpawnPlayer(0);
                    mSSGameDataManage.mGameData.SpawnNpc(1);
                    mSSGameDataManage.mGameData.SpawnNpc(2);
                    mSSGameDataManage.mGameData.SpawnNpc(3);
                    mUIController.SetActiveUIRoot(true);
                    break;
                }
            case NetworkRootMovie.GameMode.Link:
                {
                    mUIController.SetActiveUIRoot(false);
                    mSSTimeUpCom = gameObject.AddComponent<SSTimeUpCtrl>();
                    mSSTimeUpCom.OnTimeUpOverEvent += OnTimeUpOverNotConnectEvent;
                    mSSTimeUpCom.Init(35f);
                    break;
                }
        }
        NetworkEvent.GetInstance().OnServerInitializedEvent += OnServerInitializedEvent;
        NetworkEvent.GetInstance().OnPlayerConnectedEvent += OnPlayerConnectedEvent;
        NetworkEvent.GetInstance().OnConnectedToServerEvent += OnConnectedToServerEvent;
        NetworkEvent.GetInstance().OnFailedToConnectToMasterServerEvent += OnFailedToConnectToMasterServerEvent;
    }
    
    /// <summary>
    /// 当游戏联机超时后,尚未链接时.
    /// </summary>
    void OnTimeUpOverNotConnectEvent()
    {
        if (!mUIController.IsGameStart)
        {
            Debug.Log("SSGameRoot::OnTimeUpOverNotConnectEvent...");
            OnFailedToConnectToMasterServerEvent();
        }
    }

    /// <summary>
    /// 游戏场景里服务器被初始化.
    /// </summary>
    void OnServerInitializedEvent()
    {
        Debug.Log("SSGameRoot::OnServerInitializedEvent -> creat server player...");
        if (NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0)
        {
            //没有其他玩家链接服务器.
            mSSGameDataManage.mGameData.SpawnPlayer(0);
            mSSGameDataManage.mGameData.SpawnNpc(1);
            mSSGameDataManage.mGameData.SpawnNpc(2);
            mSSGameDataManage.mGameData.SpawnNpc(3);
            mUIController.SetActiveUIRoot(true);
        }
    }

    /// <summary>
    /// 有玩家链接到服务器(服务端消息).
    /// </summary>
    void OnPlayerConnectedEvent()
    {
        int movieNum = NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie;
        if (movieNum <= NetworkServerNet.GetInstance().LinkServerPlayerNum_Game)
        {
            //其他玩家全部链接到该服务器.
            Debug.Log("SSGameRoot::OnPlayerConnectedEvent -> creat server player...");
            mSSGameDataManage.mGameData.SpawnPlayer(0);
            //movieNum == 1 -- 2,3; movieNum == 2 -- 3; movieNum == 3 -- 不产生npc;
            for (int i = 0; i < 4; i++)
            {
                if (i > movieNum)
                {
                    mSSGameDataManage.mGameData.SpawnNpc(i);
                }
            }
            //mUIController.SetActiveUIRoot(true);

            if (PlayerController.GetInstance() != null)
            {
                PlayerController.GetInstance().NetSendPlayerAcitveUIPanel();
            }
        }
    }

    /// <summary>
    /// 玩家链接到服务器(客户端消息).
    /// </summary>
    void OnConnectedToServerEvent()
    {
        int indexVal = NetworkServerNet.GetInstance().IndexSpawnPlayer;
        Debug.Log("SSGameRoot::OnConnectedToServerEvent -> creat client player, indexVal == " + indexVal);
        mSSGameDataManage.mGameData.SpawnPlayer(indexVal);
        //mUIController.SetActiveUIRoot(true);
    }

    bool IsFailedToConnectMasterServer = false;
    void OnFailedToConnectToMasterServerEvent()
    {
        if (!IsFailedToConnectMasterServer && SSGameCtrl.GetInstance().eGameMode == NetworkRootMovie.GameMode.Link)
        {
            Debug.Log("SSGameRoot -> OnFailedToConnectToMasterServerEvent...");
            NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie = 0;
            NetworkServerNet.GetInstance().LinkServerPlayerNum_Game = 0;
            SSGameCtrl.GetInstance().eGameMode = NetworkRootMovie.GameMode.NoLink; //强制修改联机状态.
            //玩家链接主服务器失败.
            mSSGameDataManage.mGameData.SpawnPlayer(0);
            mSSGameDataManage.mGameData.SpawnNpc(1);
            mSSGameDataManage.mGameData.SpawnNpc(2);
            mSSGameDataManage.mGameData.SpawnNpc(3);
            mUIController.SetActiveUIRoot(true);
        }
    }
}