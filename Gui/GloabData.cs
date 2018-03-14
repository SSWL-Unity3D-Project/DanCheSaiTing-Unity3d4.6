using System.Collections.Generic;

public class GlobalData
{
    public enum GameTextType
    {
        Chinese,
        English,
    }
    public GameTextType GameTextVal = GameTextType.Chinese;
	public int CoinCur;
    /// <summary>
    /// 已完成的关卡.
    /// </summary>
    public List<int> YiWanChengLvList = new List<int>();
    private static  GlobalData Instance;
	public static GlobalData GetInstance()
	{
		if (Instance == null)
        {
			Instance = new GlobalData();
            bool isChineseGame = false;
            if (!isChineseGame)
            {
                Instance.GameTextVal = GameTextType.English;
            }
            else
            {
                Instance.GameTextVal = GameTextType.Chinese;
            }
        }
		return Instance;
	}

    /// <summary>
    /// 添加已完成场景.
    /// </summary>
    public void AddYiWanChengLevel(int level)
    {
        if (!YiWanChengLvList.Contains(level))
        {
            UnityEngine.Debug.Log("AddYiWanChengLevel -> level == " + level);
            YiWanChengLvList.Add(level);
        }
    }

    /// <summary>
    /// 清除已完成场景.
    /// </summary>
    public void ClearYiWanChengLevel()
    {
        UnityEngine.Debug.Log("ClearYiWanChengLevel...");
        YiWanChengLvList.Clear();
    }
}