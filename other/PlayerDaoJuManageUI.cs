using UnityEngine;

/// <summary>
/// 玩家道具(导弹/地雷)UI控制.
/// </summary>
public class PlayerDaoJuManageUI : MonoBehaviour
{
    float _DianLiangVal = 1f;
    /// <summary>
    /// 玩家电池电量.
    /// </summary>
    public float DianLiangVal
    {
        set
        {
            if (value > 1f)
            {
                value = 1f;
            }

            if (_DianLiangVal != value)
            {
                _DianLiangVal = value;
                if (PlayerController.GetInstance() != null)
                {
                    PlayerController.GetInstance().m_UIController.UpdateDianLiangUI(_DianLiangVal);
                }
            }
        }
        get
        {
            return _DianLiangVal;
        }
    }

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
            UpdateDaoDanInfo(_DaoDanNum);
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
            UpdateDiLeiInfo(_DiLeiNum);
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
        DianLiangVal = 1f;
        DaoDanNum = 0;
        DiLeiNum = 0;
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