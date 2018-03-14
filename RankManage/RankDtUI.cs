using UnityEngine;

public class RankDtUI : MonoBehaviour
{
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
    /// 运动路程UI.
    /// mDisMoveUIArray
    /// </summary>
    public UISprite[] mDisMoveUIArray;
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
    /// 显示运动路程UI信息.
    /// </summary>
    public void ShowJiFenInfo(int disVal)
    {
        int jiFenTmp = 0;
        string jiFenStr = disVal.ToString();
        for (int i = 0; i < 6; i++)
        {
            if (jiFenStr.Length > i)
            {
                jiFenTmp = disVal % 10;
                mDisMoveUIArray[i].spriteName = jiFenTmp.ToString();
                disVal = (int)(disVal / 10f);
                mDisMoveUIArray[i].enabled = true;
            }
            else
            {
                mDisMoveUIArray[i].enabled = false;
            }
        }
    }
}