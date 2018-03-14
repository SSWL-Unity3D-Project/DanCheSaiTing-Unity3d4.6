using UnityEngine;

/// <summary>
/// 排行榜UI控制.
/// </summary>
public class RankListUICtrl : MonoBehaviour
{
    /// <summary>
    /// 头像列表.
    /// TouXiangImgArray[x] -> 0 猪猪侠, 1 波比, 2 超人强, 3 菲菲.
    /// </summary>
    //public Texture[] TouXiangImgArray;
    /// <summary>
    /// 排名列表头像.
    /// </summary>
    //public UITexture[] RankTouXiangArray;
    /// <summary>
    /// 排名数据UI列表控制.
    /// </summary>
    public RankDtUI[] mRankDtUIArray;
    /// <summary>
    /// 积分父级Tr列表.
    /// </summary>
    //public Transform[] mJiFenTrArray;
    /// <summary>
    /// 积分Tr.
    /// </summary>
    //public Transform mJiFenTr;
    //public UISprite[] JiFenSpriteArray;

    /// <summary>
    /// 显示排行榜UI.
    /// </summary>
    public void ShowRankListUI()
    {
        RankManage.RankData rankDt = null;
        SSGameCtrl.GetInstance().mSSGameRoot.mSSGameDataManage.mGameData.RankDtManage.SortRankDtList();

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
            mRankDtUIArray[i].ShowTimeUsedVal((int)rankDt.TimeUsedVal);
            mRankDtUIArray[i].ShowDisMoveInfo((int)rankDt.DisMoveVal);
        }
        gameObject.SetActive(true);
    }
}