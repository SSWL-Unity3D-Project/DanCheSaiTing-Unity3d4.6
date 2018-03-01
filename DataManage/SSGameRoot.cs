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
    }
}