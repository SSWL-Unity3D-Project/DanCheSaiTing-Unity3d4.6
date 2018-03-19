using UnityEngine;

/// <summary>
/// 排行榜UI控制.
/// </summary>
public class RankListUICtrl : MonoBehaviour
{
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

        for (int i = 0; i < 4; i++)
		{
			mRankDtUIArray[i].gameObject.SetActive(true);
            rankDt = SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.RankDtList[i];
            if (rankDt.IsPlayerData)
            {
                mRankDtUIArray[i].transform.localScale += new Vector3(0.5f, 0.5f, 0f);
#if UNITY_EDITOR
                Debug.Log("ShowRankListUI -> PlayerTimeUsed " + rankDt.TimeUsedVal.ToString("f2")
                    + ", disMove " + rankDt.DisMoveVal.ToString("f2"));
#endif
            }

#if UNITY_EDITOR
            Debug.Log("ShowRankListUI -> index " + i + ", RankType " + rankDt.RankType);
#endif
            if (rankDt.IsMoveToFinishPoint)
            {
                mRankDtUIArray[i].ShowTimeUsedVal((int)rankDt.TimeUsedVal);
            }
            else
            {
                mRankDtUIArray[i].HiddenTimeUI();
            }

            int disVal = (int)(pathDis - rankDt.DisMoveVal);
            mRankDtUIArray[i].ShowShengYuDisMoveInfo(disVal < 0 ? 0 : disVal);

            float wanChengDu = rankDt.DisMoveVal / pathDis;
            wanChengDu = wanChengDu > 1f ? 1f : wanChengDu;
            mRankDtUIArray[i].ShowWanChengDuInfo((int)(wanChengDu * 100f));
        }
        gameObject.SetActive(true);
    }
}