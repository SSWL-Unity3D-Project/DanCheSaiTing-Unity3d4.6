using System.Collections.Generic;

/// <summary>
/// 玩家排名信息管理.
/// </summary>
public class RankManage
{
    /// <summary>
    /// 服务端比赛开始时间.
    /// </summary>
    float TimeServerStartVal = 0f;
    /// <summary>
    /// 比赛开始时间.
    /// </summary>
    float TimeStartVal = 0f;
    /// <summary>
    /// 排名标记.
    /// </summary>
    public enum RankEnum
    {
        Null = -1,
        ZhuZhuXia = 0,          //猪猪侠
        BoBi = 1,               //波比
        ChaoRenQiang = 2,       //超人强
        FeiFei = 3,             //菲菲
    }

    /// <summary>
    /// 排名数据.
    /// </summary>
    public class RankData
    {
        /// <summary>
        /// 排名标记.
        /// </summary>
        public RankEnum RankType = RankEnum.Null;
        /// <summary>
        /// 是否是玩家数据.
        /// </summary>
        public bool IsPlayerData = false;
        /// <summary>
        /// 是否到达终点.
        /// </summary>
        public bool IsMoveToFinishPoint = false;
        /// <summary>
        /// 到终点时的时间.
        /// </summary>
        public float TimeFinishPoint = 0f;
        /// <summary>
        /// 当前走过主角路径点的索引.
        /// </summary>
        public int PathNodeCur = 0;
        /// <summary>
        /// 走过主角路径点的时间记录信息.
        /// </summary>
        public float TimePathNodeCur = 0f;
        /// <summary>
        /// 比赛使用时间.
        /// </summary>
        public float TimeUsedVal = 0f;
        public RankData(RankEnum rankType, bool isPlayer)
        {
            RankType = rankType;
			IsPlayerData = isPlayer;
			UnityEngine.Debug.Log("SortRankDtList -> isPlayer " + isPlayer + ", RankType " + RankType);
        }

        /// <summary>
        /// 更新排名数据到达终点时间.
        /// </summary>
        public void UpdateRankDtTimeFinish(float timeVal)
        {
            if (PlayerController.GetInstance().m_IsFinished || PlayerController.GetInstance().m_UIController.m_IsGameOver)
            {
                return;
            }

            if (IsMoveToFinishPoint)
            {
                return;
            }
            IsMoveToFinishPoint = true;
#if UNITY_EDITOR
            UnityEngine.Debug.Log("UpdateRankDtTimeFinish -> TimeFinishPoint " + TimeFinishPoint + ", RankType " + RankType);
#endif
        }

        /// <summary>
        /// 更新排名数据到达主角路径点信息.
        /// </summary>
        public void UpdateRankDtPathPoint(int node, float timeVal)
        {
            if (PlayerController.GetInstance().m_IsFinished || PlayerController.GetInstance().m_UIController.m_IsGameOver)
            {
                return;
            }

            if (IsMoveToFinishPoint)
            {
                return;
            }
            PathNodeCur = node;
            TimePathNodeCur = timeVal;
#if UNITY_EDITOR
            //UnityEngine.Debug.Log("UpdateRankDtTimeFinish -> TimePathNodeCur " + TimePathNodeCur + ", node " + node + ", RankType " + RankType);
#endif
        }
    }

    /// <summary>
    /// 排名数据列表.
    /// </summary>
    public List<RankData> RankDtList = new List<RankData>();
    /// <summary>
    /// 添加排名数据.
    /// </summary>
    public RankData AddRankDt(RankEnum rankType, bool isPlayer)
    {
        if (rankType == RankEnum.Null)
        {
            UnityEngine.Debug.LogError("AddRankDt -> rankType was wrong!");
            return null;
        }
        RankData rankDt = new RankData(rankType, isPlayer);
        RankDtList.Add(rankDt);
        return rankDt;
    }

    public int CompareRankDt(RankData x, RankData y)//排序器  
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            return 1;
        }

        if (y == null)
        {
            return -1;
        }

        int retval = 0;
        //修正服务端和客户端的时间,使所有端口的时间轴和服务器保持一致.
        float dTime = TimeStartVal - TimeServerStartVal;
        x.TimeFinishPoint -= dTime;
        x.TimePathNodeCur -= dTime;

        if (x.IsMoveToFinishPoint && y.IsMoveToFinishPoint)
        {
            //x和y都到达终点.
            retval = x.TimeFinishPoint.CompareTo(y.TimeFinishPoint);
            x.TimeUsedVal = x.TimeFinishPoint - TimeStartVal;
            y.TimeUsedVal = y.TimeFinishPoint - TimeStartVal;
        }
        else if (x.IsMoveToFinishPoint)
        {
            //x到达终点.
            retval = -1;
            x.TimeUsedVal = x.TimeFinishPoint - TimeStartVal;
        }
        else if (y.IsMoveToFinishPoint)
        {
            //y到达终点.
            retval = 1;
            y.TimeUsedVal = y.TimeFinishPoint - TimeStartVal;
        }
        else
        {
            //x和y都没有到达终点.
            retval = y.PathNodeCur.CompareTo(x.PathNodeCur);
            if (retval == 0)
            {
                //x和y在相同路径点上.
                retval = x.TimePathNodeCur.CompareTo(y.TimePathNodeCur);
            }

            float timeUsed = UnityEngine.Time.time - TimeStartVal;
            x.TimeUsedVal = y.TimeUsedVal = timeUsed;
        }
        return retval;
    }

    /// <summary>
    /// 对排名数据进行排序.
    /// </summary>
    public void SortRankDtList()
    {
        RankDtList.Sort(CompareRankDt);
#if UNITY_EDITOR
        for (int i = 0; i < RankDtList.Count; i++)
        {
            UnityEngine.Debug.Log("SortRankDtList -> index " + i + ", node " + RankDtList[i].PathNodeCur + ", timeNode " + RankDtList[i].TimePathNodeCur + ", RankType " + RankDtList[i].RankType);
            UnityEngine.Debug.Log("SortRankDtList -> TimeFinishPoint " + RankDtList[i].TimeFinishPoint + ", IsMoveToFinishPoint " + RankDtList[i].IsMoveToFinishPoint);
        }
#endif
    }

    public void SetTimeStartVal(float timeVal)
    {
        UnityEngine.Debug.Log("SetTimeStartVal -> time " + timeVal.ToString("f2"));
        TimeStartVal = timeVal;
    }

    /// <summary>
    /// 设置服务端的比赛开始时间.
    /// </summary>
    public void SetTimeServerStartVal(float timeVal)
    {
        UnityEngine.Debug.Log("SetTimeServerStartVal -> time " + timeVal.ToString("f2"));
        TimeServerStartVal = timeVal;
    }
}