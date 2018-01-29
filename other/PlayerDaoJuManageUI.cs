using UnityEngine;

/// <summary>
/// 玩家道具(导弹/地雷)UI控制.
/// </summary>
public class PlayerDaoJuManageUI : MonoBehaviour
{
    int _DaoDanNum = 0;
    /// <summary>
    /// 导弹数量.
    /// </summary>
    public int DaoDanNum
    {
        set
        {
            if (value > 9)
            {
                value = 9;
            }

            _DaoDanNum = value;
            if (PlayerController.GetInstance() != null)
            {
                PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.UpdateDaoDanInfo(_DaoDanNum);
            }
        }
        get
        {
            return _DaoDanNum;
        }
    }
    int _DiLeiNum = 0;
    /// <summary>
    /// 地雷数量.
    /// </summary>
    public int DiLeiNum
    {
        set
        {
            if (value > 9)
            {
                value = 9;
            }

            _DiLeiNum = value;
            if (PlayerController.GetInstance() != null)
            {
                PlayerController.GetInstance().m_UIController.mPlayerDaoJuManageUI.UpdateDiLeiInfo(_DiLeiNum);
            }
        }
        get
        {
            return _DiLeiNum;
        }
    }

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
        UpdateDaoDanInfo(DaoDanNum);
        UpdateDiLeiInfo(DiLeiNum);
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