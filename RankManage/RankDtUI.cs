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
}