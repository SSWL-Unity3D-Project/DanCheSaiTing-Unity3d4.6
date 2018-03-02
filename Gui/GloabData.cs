
public enum GameMode
{
    None,
    SoloMode,
    OnlineMode
}

public enum GameLeve : int
{
    None = -1,
    Movie = 0,
    Waterwheel = 1,
    WaterwheelNet = 2,
    SetPanel = 3
}

public class GlobalData
{
	public static GameTextType GameTextVal = GameTextType.Chinese;
	public int CoinCur;
    public GameMode gameMode;
    public GameLeve gameLeve;
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