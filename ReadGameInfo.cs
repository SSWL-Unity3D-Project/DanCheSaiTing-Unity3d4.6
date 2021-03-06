﻿using UnityEngine;
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
	HandleJson mHandleJson;
    [HideInInspector]
    public string mFileName = "GameConfig.xml";

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
        mHandleJson = HandleJson.GetInstance();
        m_pInsertCoinNum = "0";
		//int value = PlayerPrefs.GetInt("PlayerMinSpeedVal");
		//if (value < 0) {
		//	value = 0;
		//}
		//PlayerMinSpeedVal = value;
        
		string readInfo = mHandleJson.ReadFromFileXml(mFileName, "GameAudioVolume");
		if (readInfo == null || readInfo == "") {
			readInfo = "7";
			mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", readInfo);
		}

        int value = Convert.ToInt32(readInfo);
		if (value < 0 || value > 10) {
			value = 7;
			mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", value.ToString());
		}
		GameAudioVolume = value;

        //GAME_MODE
        readInfo = mHandleJson.ReadFromFileXml(mFileName, "GAME_MODE");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "1";
            mHandleJson.WriteToFileXml(mFileName, "GAME_MODE", readInfo);
        }

        value = Convert.ToInt32(readInfo);
        if (value != 0 && value != 1)
        {
            value = 1;
            mHandleJson.WriteToFileXml(mFileName, "GAME_MODE", value.ToString());
        }
        int gameModeSt = value; //0->运营模式, 1->免费模式.
        m_pGameMode = gameModeSt == 0 ? ReadGameInfo.GameMode.Oper.ToString() : ReadGameInfo.GameMode.Free.ToString();

        //START_COIN
        readInfo = mHandleJson.ReadFromFileXml(mFileName, "START_COIN");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "1";
            mHandleJson.WriteToFileXml(mFileName, "START_COIN", readInfo);
        }

        int coinStart = Convert.ToInt32(readInfo);
        if (coinStart <= 0 || coinStart > 99)
        {
            coinStart = 1;
            mHandleJson.WriteToFileXml(mFileName, "START_COIN", coinStart.ToString());
        }
        m_pStarCoinNum = coinStart.ToString();

        //GAME_RECORD
        readInfo = mHandleJson.ReadFromFileXml(mFileName, "GAME_RECORD");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "180";
            mHandleJson.WriteToFileXml(mFileName, "GAME_RECORD", readInfo);
        }
        GameRecordVal = Convert.ToInt32(readInfo);
    }
	public void FactoryReset()
	{
		WriteStarCoinNumSet("1");
		WriteGameStarMode(GameMode.Free.ToString());
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
		mHandleJson.WriteToFileXml(mFileName, "GameAudioVolume", value.ToString());
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
        mHandleJson.WriteToFileXml(mFileName, "START_COIN", coinStart.ToString());
        m_pStarCoinNum = coinStart.ToString();
	}
	public void WriteGameStarMode(string value)
	{
		int gameModeSt = value == GameMode.Oper.ToString() ? 0 : 1;
        mHandleJson.WriteToFileXml(mFileName, "GAME_MODE", gameModeSt.ToString());
        m_pGameMode = value;
	}
	public void WriteInsertCoinNum(string value)
	{
		m_pInsertCoinNum = value;
	}
	public void WriteGameRecord(int value)
	{
        mHandleJson.WriteToFileXml(mFileName, "GAME_RECORD", value.ToString());
        GameRecordVal = value;
	}
	public int ReadPlayerMinSpeedVal()
	{
		return PlayerMinSpeedVal;
	}
	public void WritePlayerMinSpeedVal(int value)
	{
		//PlayerPrefs.SetInt("PlayerMinSpeedVal", value);
		PlayerMinSpeedVal = value;
	}
//	void OnGUI()
//	{
//		string strA = "StarCoinNum "+m_pStarCoinNum
//						+", GameMode "+m_pGameMode;
//		GUI.Box(new Rect(0f, 0f, Screen.width, 30f), strA);
//	}
}