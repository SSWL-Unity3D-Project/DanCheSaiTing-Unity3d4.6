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
            if (value + _DiLeiNum > AmmoMax)
            {
                //子弹已经加满.
                return;
            }

            if (value > AmmoMax)
            {
                value = AmmoMax;
            }

            _DaoDanNum = value;
            //UpdateDaoDanInfo(_DaoDanNum);
            UpdateAmmoUI();

            if (_DaoDanNum <= 0 && _DiLeiNum <= 0)
            {
                if (PlayerController.GetInstance() != null && PlayerController.GetInstance().m_UIController != null)
                {
                    PlayerController.GetInstance().m_UIController.RemoveFaSheDaoDanUI();
                }
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
            if (value + _DaoDanNum > AmmoMax)
            {
                //子弹已经加满.
                return;
            }

            if (value > AmmoMax)
            {
                value = AmmoMax;
            }

            _DiLeiNum = value;
            //UpdateDiLeiInfo(_DiLeiNum);
            UpdateAmmoUI();

            if (_DaoDanNum <= 0 && _DiLeiNum <= 0)
            {
                if (PlayerController.GetInstance() != null && PlayerController.GetInstance().m_UIController != null)
                {
                    PlayerController.GetInstance().m_UIController.RemoveFaSheDaoDanUI();
                }
            }
        }
        get
        {
            return _DiLeiNum;
        }
    }

    /// <summary>
    /// 玩家获取子弹的UI提示.
    /// </summary>
    public GameObject[] AmmoUIArray;
    /// <summary>
    /// 子弹最大数量.
    /// </summary>
    int AmmoMax = 0;
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

        AmmoMax = AmmoUIArray.Length;
        for (int i = 0; i < AmmoMax; i++)
        {
            if (AmmoUIArray[i] != null)
            {
                AmmoUIArray[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新玩家子弹UI.
    /// </summary>
    void UpdateAmmoUI()
    {
        int ammoNum = DaoDanNum + DiLeiNum;
        for (int i = 0; i < AmmoMax; i++)
        {
            if (AmmoUIArray[i] != null)
            {
                AmmoUIArray[i].SetActive(i < ammoNum ? true : false);
            }
        }
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