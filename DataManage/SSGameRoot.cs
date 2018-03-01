using UnityEngine;

/// <summary>
/// 游戏总控制.
/// </summary>
public class SSGameRoot : MonoBehaviour
{
    public SSGameDataManage mSSGameDataManage;
    void Awake()
    {
        SSGameCtrl.GetInstance().mSSGameRoot = this;
        mSSGameDataManage.mGameData.SpawnPlayer(0);
        mSSGameDataManage.mGameData.SpawnNpc(1);
        mSSGameDataManage.mGameData.SpawnNpc(2);
        mSSGameDataManage.mGameData.SpawnNpc(3);
    }
}