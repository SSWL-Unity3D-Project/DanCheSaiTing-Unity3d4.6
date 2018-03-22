using UnityEngine;

public class RankDtUI : MonoBehaviour
{
    /// <summary>
    /// 玩家排行信息背景.
    /// </summary>
    public UITexture mPlayerBJUITexture;
    /// <summary>
    /// 时间UI总控.
    /// </summary>
    public GameObject TimeUIObj;
    /// <summary>
    /// 分个位.
    /// </summary>
    public UISprite FenGewei;
    /// <summary>
    /// 秒十位.
    /// </summary>
    public UISprite MiaoShiwei;
    /// <summary>
    /// 秒个位.
    /// </summary>
    public UISprite MiaoGewei;
    /// <summary>
    /// 剩余运动路程UI.
    /// mDisMoveUIArray
    /// </summary>
    public UISprite[] mDisMoveUIArray = new UISprite[6];
    /// <summary>
    /// 完成度UI.
    /// mDisMoveUIArray
    /// </summary>
    public UISprite[] mWanChengDuUIArray = new UISprite[3];
    /// <summary>
    /// 显示时间.
    /// </summary>
    public void ShowTimeUsedVal(int timeVal)
    {
        int fen = timeVal / 60;
        FenGewei.spriteName = fen.ToString();

        int miao = timeVal - (fen * 60);
        int miaoshiwei = miao / 10;
        int miaogewei = miao - (miaoshiwei * 10);
        MiaoShiwei.spriteName = miaoshiwei.ToString();
        MiaoGewei.spriteName = miaogewei.ToString();
    }

    /// <summary>
    /// 隐藏时间UI.
    /// </summary>
    public void HiddenTimeUI()
    {
        if (TimeUIObj != null)
        {
            TimeUIObj.SetActive(false);
        }
    }
    
    /// <summary>
    /// 显示剩余运动路程的UI信息.
    /// </summary>
    public void ShowShengYuDisMoveInfo(int disVal)
    {
        int tmpVal = 0;
        string valStr = disVal.ToString();
        for (int i = 0; i < 6; i++)
        {
            if (valStr.Length > i)
            {
                tmpVal = disVal % 10;
                mDisMoveUIArray[i].spriteName = tmpVal.ToString();
                disVal = (int)(disVal / 10f);
                mDisMoveUIArray[i].enabled = true;
            }
            else
            {
                mDisMoveUIArray[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 显示完成度的UI信息.
    /// </summary>
    public void ShowWanChengDuInfo(int val)
    {
        int tmpVal = 0;
        string valStr = val.ToString();
        for (int i = 0; i < 3; i++)
        {
            if (valStr.Length > i)
            {
                tmpVal = val % 10;
                mWanChengDuUIArray[i].spriteName = tmpVal.ToString();
                val = (int)(val / 10f);
                mWanChengDuUIArray[i].enabled = true;
            }
            else
            {
                mWanChengDuUIArray[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 显示玩家排行背景.
    /// </summary>
    public void ShowPlayerRankBeiJingUI(Texture textureVal)
    {
        if (textureVal != null && mPlayerBJUITexture != null)
        {
            mPlayerBJUITexture.mainTexture = textureVal;
        }
    }
}