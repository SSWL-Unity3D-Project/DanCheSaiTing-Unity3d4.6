using UnityEngine;

/// <summary>
/// 游戏总控制.
/// </summary>
public class SSGameRoot : MonoBehaviour
{
    public NetworkRootGame mNetworkRootGame;
    public SSGameDataManage mSSGameDataManage;
    /// <summary>
    /// 游戏UI总控.
    /// </summary>
    public UIController mUIController;
    /// <summary>
    /// 是否激活游戏UIRoot.
    /// </summary>
    bool IsActiveGameUIRoot = false;
    void Awake()
    {
        SSGameCtrl.GetInstance().mSSGameRoot = this;
        Debug.Log("SSGameRoot::Awake -> peerType " + Network.peerType + ", eGameMode " + SSGameCtrl.GetInstance().eGameMode);
        switch (SSGameCtrl.GetInstance().eGameMode)
        {
            case GameModeSelect.GameMode.NoLink:
                {
                    mSSGameDataManage.mGameData.SpawnPlayer(0);
                    mSSGameDataManage.mGameData.SpawnNpc(1);
                    mSSGameDataManage.mGameData.SpawnNpc(2);
                    mSSGameDataManage.mGameData.SpawnNpc(3);
                    break;
                }
            case GameModeSelect.GameMode.Link:
                {
                    mUIController.SetActiveUIRoot(false);
                    IsActiveGameUIRoot = false;
                    break;
                }
        }
        NetworkEvent.GetInstance().OnServerInitializedEvent += OnServerInitializedEvent;
    }

    void Update()
    {
        //if (SSGameCtrl.GetInstance().eGameMode == GameModeSelect.GameMode.Link)
        //{
        //    if (Time.frameCount % 30 == 0)
        //    {
        //        if (Network.peerType == NetworkPeerType.Server || Network.peerType == NetworkPeerType.Client)
        //        {
        //            if (!IsActiveGameUIRoot)
        //            {
        //                mUIController.SetActiveUIRoot(true);
        //                IsActiveGameUIRoot = true;
        //            }
        //        }
        //    }
        //}
    }
    
    /// <summary>
    /// 游戏场景里服务器被初始化.
    /// </summary>
    void OnServerInitializedEvent()
    {
        Debug.Log("SSGameRoot::OnServerInitializedEvent -> creat server player...");
        if (NetworkServerNet.GetInstance().LinkServerCount <= 0)
        {
            //没有其他玩家链接服务器.
            mSSGameDataManage.mGameData.SpawnPlayer(0);
            mSSGameDataManage.mGameData.SpawnNpc(1);
            mSSGameDataManage.mGameData.SpawnNpc(2);
            mSSGameDataManage.mGameData.SpawnNpc(3);
            mUIController.SetActiveUIRoot(true);
        }
    }
}