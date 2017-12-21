using UnityEngine;
using System.Collections;

public class GlobalData
{
	public static GameTextType GameTextVal = GameTextType.Chinese;
	public static int CoinCur;
	private static  GlobalData Instance;
	public static GlobalData GetInstance()
	{
		if (Instance == null) {
			bool isChineseGame = false;
			if (!isChineseGame) {
				GameTextVal = GameTextType.English;
			}
			else {
				GameTextVal = GameTextType.Chinese;
			}
			Instance = new GlobalData();
		}
		return Instance;
	}

	public static GameTextType GetGameTextMode()
	{
		if (Instance == null) {
			GetInstance();
		}
		return GameTextVal;
	}
}

public enum GameTextType
{
	Chinese,
	English,
}