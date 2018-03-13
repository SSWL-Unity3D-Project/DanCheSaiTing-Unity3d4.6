using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    /// <summary>
    /// 开始按键.
    /// </summary>
    public GameObject StartBtObj;
    /// <summary>
    /// 选择关卡的动画.
    /// </summary>
    public Animator mAnimator;
    /// <summary>
    /// 场景图集.
    /// </summary>
    public UITexture[] LevelArray;
    /// <summary>
    /// 已完成图集.
    /// </summary>
    public GameObject[] YiWanChengUI = new GameObject[4];
    int _mSelectLevel = 1;
    /// <summary>
    /// 选择的游戏关卡[1, 4].
    /// </summary>
    public int mSelectLevel
    {
        set
        {
            _mSelectLevel = value;
            Debug.Log("LevelSelectUI -> Level " + _mSelectLevel);

            for (int i = 0; i < 4; i++)
            {
                LevelArray[i].depth = (i + 1) == _mSelectLevel ? 3 : 2;
            }
        }
        get
        {
            return _mSelectLevel;
        }
    }
    float TimeLastSelect = 0f;

    public void Init()
    {
        SetActiveStartBt(true);
        for (int i = 0; i < 4; i++)
        {
            if (YiWanChengUI[i] != null)
            {
                if (GlobalData.GetInstance().YiWanChengLvList.Contains(i + 1))
                {
                    YiWanChengUI[i].SetActive(true);
                }
                else
                {
                    YiWanChengUI[i].SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        float steerVal = pcvr.GetInstance().mGetSteer;
        if (steerVal > 0f && Time.time - TimeLastSelect > 0.5f)
        {
            TimeLastSelect = Time.time;
            int level = mSelectLevel;
            level++;
            if (level > 4)
            {
                level = 1;
            }
            mSelectLevel = level;

            //向右转动.
            string trigger = "right0" + mSelectLevel;
            mAnimator.SetTrigger(trigger);
            Debug.Log("aniName -> " + trigger);
        }

        if (steerVal < 0f && Time.time - TimeLastSelect > 0.5f)
        {
            TimeLastSelect = Time.time;
            int level = mSelectLevel;
            level--;
            if (level < 1)
            {
                level = 4;
            }
            mSelectLevel = level;

            //向左转动.
            string trigger = "left0" + mSelectLevel;
            mAnimator.SetTrigger(trigger);
            Debug.Log("aniName -> " + trigger);
        }
    }

    void SetActiveStartBt(bool isActive)
    {
        StartBtObj.SetActive(isActive);
    }

    public void HiddenSelf()
    {
        gameObject.SetActive(false);
    }
}