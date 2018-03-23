﻿#define USE_HANDLE_JSON
using UnityEngine;
using System.Collections;
using System;

public class ReadGameInfo : MonoBehaviour 
{
	static private ReadGameInfo Instance = null;
    public enum GameMode
    {
        Oper, //运营.
        Free, //免费.
    }
    private HandleJson handleJsonObj;
	public string m_pStarCoinNum = "";
	public string m_pGameMode = "";
	public string m_pInsertCoinNum = "0";
	int GameRecordVal;
	int PlayerMinSpeedVal = 0;
	/**
	 * 游戏音量(0-10).
	 */
	int GameAudioVolume;
	#if USE_HANDLE_JSON
	HandleJson mHandleJson;
    [HideInInspector]
    public string mFileName = "GameConfig.xml";
	#endif
	static public ReadGameInfo GetInstance()
	{
		if (Instance == null)
        {
			GameObject obj = new GameObject("_ReadGameInfo");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<ReadGameInfo>();
			Instance.InitGameInfo();
        }
        return Instance;
	}

    /// <summary>
    /// 初始化游戏配置信息.
    /// </summary>
	void InitGameInfo()
    {
#if USE_HANDLE_JSON
        mHandleJson = HandleJson.GetInstance();
#endif

        m_pInsertCoinNum = "0";		
		int gameModeSt = PlayerPrefs.GetInt("GAME_MODE");
		if (gameModeSt != 0 && gameModeSt != 1) {
			gameModeSt = 1; //0->运营模式, 1->免费模式.
			PlayerPrefs.SetInt("GAME_MODE", gameModeSt);
		}
		m_pGameMode = gameModeSt == 0 ? ReadGameInfo.GameMode.Oper.ToString() : ReadGameInfo.GameMode.Free.ToString();

		int coinStart = PlayerPrefs.GetInt("START_COIN");
		if (coinStart == 0) {
			coinStart = 1;
			PlayerPrefs.SetInt("START_COIN", coinStart);
		}
		m_pStarCoinNum = coinStart.ToString();

		GameRecordVal = PlayerPrefs.GetInt("GAME_RECORD");
		
		int value = PlayerPrefs.GetInt("PlayerMinSpeedVal");
		if (value < 0) {
			value = 0;
		}
		PlayerMinSpeedVal = value;

		if (!PlayerPrefs.HasKey("GameAudioVolume")) {
			PlayerPrefs.SetInt("GameAudioVolume", 7);
			PlayerPrefs.Save();
		}

		
		#if USE_HANDLE_JSON
		string readInfo = mHandleJson.ReadFromFileXml(mFileName, "GameAudioVolume");
		if (readInfo == null || readInfo == "") {
			readInfo = "7";
			mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", readInfo);
		}

		value = Convert.ToInt32(readInfo);
		if (value < 0 || value > 10) {
			value = 7;
			mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", value.ToString());
		}
		#else
		value = PlayerPrefs.GetInt("GameAudioVolume");
		if (value < 0 || value > 10) {
			value = 7;
			PlayerPrefs.SetInt("GameAudioVolume", value);
			PlayerPrefs.Save();
		}
		#endif
		GameAudioVolume = value;
	}
	public void FactoryReset()
	{
		WriteStarCoinNumSet("1");
		WriteGameStarMode(GameMode.Oper.ToString());
		WriteInsertCoinNum("0");
		WriteGameRecord(180);
		WritePlayerMinSpeedVal(0);
		WriteGameAudioVolume(7);
	}
	public int ReadGameAudioVolume()
	{
		return GameAudioVolume;
	}
	public void WriteGameAudioVolume(int value)
	{
		#if USE_HANDLE_JSON
		mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", value.ToString());
		#else
		PlayerPrefs.SetInt("GameAudioVolume", value);
		PlayerPrefs.Save();
		#endif
		GameAudioVolume = value;
		AudioListener.volume = (float)value / 10f;
	}
	public string ReadStarCoinNumSet()
	{
		return m_pStarCoinNum;
	}
	public string ReadGameStarMode()
	{
		return m_pGameMode;
	}
	public string ReadInsertCoinNum()
	{
		return m_pInsertCoinNum;
	}
	public int ReadGameRecord()
	{
		return GameRecordVal;
	}
	public void WriteStarCoinNumSet(string value)
	{
		int coinStart = Convert.ToInt32(value);
		PlayerPrefs.SetInt("START_COIN", coinStart);
		m_pStarCoinNum = coinStart.ToString();
	}
	public void WriteGameStarMode(string value)
	{
		int gameModeSt = value == GameMode.Oper.ToString() ? 0 : 1;
		PlayerPrefs.SetInt("GAME_MODE", gameModeSt);
		m_pGameMode = value;
	}
	public void WriteInsertCoinNum(string value)
	{
		m_pInsertCoinNum = value;
	}
	public void WriteGameRecord(int value)
	{
		PlayerPrefs.SetInt("GAME_RECORD", value);
		GameRecordVal = value;
	}
	public int ReadPlayerMinSpeedVal()
	{
		return PlayerMinSpeedVal;
	}
	public void WritePlayerMinSpeedVal(int value)
	{
		PlayerPrefs.SetInt("PlayerMinSpeedVal", value);
		PlayerMinSpeedVal = value;
	}
//	void OnGUI()
//	{
//		string strA = "StarCoinNum "+m_pStarCoinNum
//						+", GameMode "+m_pGameMode;
//		GUI.Box(new Rect(0f, 0f, Screen.width, 30f), strA);
//	}
}