using UnityEngine;

/// <summary>
/// 排行榜UI控制.
/// </summary>
public class RankListUICtrl : MonoBehaviour
{
    /// <summary>
    /// 玩家排行信息背景.
    /// </summary>
    public Texture mPlayerBJTexture;
    /// <summary>
    /// 排名数据UI列表控制.
    /// </summary>
    public RankDtUI[] mRankDtUIArray;

    /// <summary>
    /// 显示排行榜UI.
    /// </summary>
    public void ShowRankListUI()
    {
        RankManage.RankData rankDt = null;
        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.SortRankDtList();
        float pathDis = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.DistancePath;
        float wangChengDuTmp = 1000f;
        int disValTmp = 0;

        for (int i = 0; i < 4; i++)
		{
			mRankDtUIArray[i].gameObject.SetActive(true);
            if (SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.RankDtList.Count > i)
            {
                rankDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.RankDtList[i];
                if (rankDt.IsPlayerData)
                {
                    mRankDtUIArray[i].ShowPlayerRankBeiJingUI(mPlayerBJTexture);
                    mRankDtUIArray[i].transform.localScale += new Vector3(0.1f, 0.1f, 0f);
#if UNITY_EDITOR
                    Debug.Log("ShowRankListUI -> PlayerTimeUsed " + rankDt.TimeUsedVal.ToString("f2")
                        + ", disMove " + rankDt.DisMoveVal.ToString("f2"));
#endif
                }

                //#if UNITY_EDITOR
                Debug.Log("ShowRankListUI -> index " + i + ", RankType " + rankDt.RankType);
                //#endif

                int disVal = (int)(pathDis - rankDt.DisMoveVal);
                float wanChengDu = rankDt.DisMoveVal / pathDis;
                if (rankDt.IsMoveToFinishPoint)
                {
                    wanChengDu = 1f;
                }
                else
                {
                    wanChengDu = wanChengDu > 1f ? 1f : wanChengDu;
                }

                if (disVal < disValTmp)
                {
                    disVal = disValTmp;
                }
                disValTmp = disVal;

                if (wanChengDu > wangChengDuTmp)
                {
                    wanChengDu = wangChengDuTmp;
                }
                wangChengDuTmp = wanChengDu;
                mRankDtUIArray[i].ShowWanChengDuInfo((int)(wanChengDu * 100f));

                if (rankDt.IsMoveToFinishPoint || wanChengDu >= 1f)
                {
                    mRankDtUIArray[i].ShowTimeUsedVal((int)rankDt.TimeUsedVal);
                }
                else
                {
                    mRankDtUIArray[i].HiddenTimeUI();
                }

                if (rankDt.IsMoveToFinishPoint || wanChengDu >= 1f)
                {
                    mRankDtUIArray[i].ShowShengYuDisMoveInfo(0);
                }
                else
                {
                    mRankDtUIArray[i].ShowShengYuDisMoveInfo(disVal < 0 ? 0 : disVal);
                }
            }
            else
            {
                Debug.Log("rankDataList was wrong! i == " + i);
                mRankDtUIArray[i].ShowWanChengDuInfo(2);
                mRankDtUIArray[i].ShowShengYuDisMoveInfo((int)pathDis - 60);
            }
        }
        gameObject.SetActive(true);
    }
}