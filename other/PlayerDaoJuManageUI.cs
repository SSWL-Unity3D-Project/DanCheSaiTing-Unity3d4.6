using UnityEngine;

/// <summary>
/// 玩家道具(导弹/地雷)UI控制.
/// </summary>
public class PlayerDaoJuManageUI : MonoBehaviour
{
    /// <summary>
    /// 导弹数量.
    /// </summary>
    public UISprite DaoDanUiSprite;
    /// <summary>
    /// 地雷数量.
    /// </summary>
    public UISprite DiLeiUiSprite;
    void Start()
    {
        UpdateDaoDanInfo(pcvr.GetInstance().mPlayerDataManage.DaoDanNum);
        UpdateDiLeiInfo(pcvr.GetInstance().mPlayerDataManage.DiLeiNum);
    }

    /// <summary>
    /// 更新导弹UI数据.
    /// </summary>
    public void UpdateDaoDanInfo(int val)
    {
        DaoDanUiSprite.spriteName = val.ToString();
    }

    /// <summary>
    /// 更新地雷UI数据.
    /// </summary>
    public void UpdateDiLeiInfo(int val)
    {
        DiLeiUiSprite.spriteName = val.ToString();
    }
}