﻿using UnityEngine;
using System;

public class SetPanel : MonoBehaviour
{
	//zhujiemian
//	private bool m_IsZhujiemian = true;
	public GameObject m_ZhujiemianObject;
	public Transform m_ZhujiemianXingXing;
	private int m_IndexZhujiemian = 0;
	public UILabel m_CoinForStar;
	public UILabel PlayerMinSpeed;
	public UITexture m_GameModeDuigou1;
	public UITexture m_GameModeDuigou2;
	public UITexture[] GameGradeDuiGou;
	public UILabel InsertCoinNumLabel;
	public UILabel BtInfoLabel;
	public UILabel YouMenInfoLabel;
	public UILabel FangXiangInfoLabel;
	private int m_InserNum = 0;
	public UITexture JiaoZhunTexture;
	public Texture[] JiaoZhunTextureArray;
	bool IsInitJiaoZhunPcvr;
	int JiaoZhunCount;
	GameObject JiaoZhunObj;
	public static bool IsOpenSetPanel = false;
	int GameAudioVolume;
	void Start () 
	{
		//XkGameCtrl.IsLoadingLevel = false;
		GameAudioVolume = ReadGameInfo.GetInstance().ReadGameAudioVolume();
		GameAudioVolumeLB.text = GameAudioVolume.ToString();

		IsOpenSetPanel = true;
		CloseAllQiNang();
		//pcvr.ShaCheBtLight = StartLightState.Mie;
		//pcvr.CloseFangXiangPanPower();
		//pcvr.IsSlowLoopCom = false;
		m_InserNum = Convert.ToInt32(ReadGameInfo.GetInstance().ReadInsertCoinNum());
		UpdateInsertCoin();

		BtInfoLabel.text = "";
		m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f,212.0f,0.0f);
		string GameMode = ReadGameInfo.GetInstance().ReadGameStarMode();
		if (GameMode == "" || GameMode == null) {
			GameMode = ReadGameInfo.GameMode.Oper.ToString();
		}

		m_CoinForStar.text = ReadGameInfo.GetInstance().ReadStarCoinNumSet();
		int minSpeedPlayer = (int)ReadGameInfo.GetInstance().ReadPlayerMinSpeedVal();
		PlayerMinSpeed.text = minSpeedPlayer.ToString();
		if(GameMode == ReadGameInfo.GameMode.Oper.ToString())
		{
			m_GameModeDuigou1.enabled = true;
			m_GameModeDuigou2.enabled = false;
		}
		else
		{
			m_GameModeDuigou1.enabled = false;
			m_GameModeDuigou2.enabled = true;
		}

		if (JiaoZhunTexture != null) {
			JiaoZhunObj = JiaoZhunTexture.gameObject;
			JiaoZhunObj.SetActive(false);
		}
		
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickStartBtOneEvent += ClickStartBtOneEvent;
		//InputEventCtrl.GetInstance().ClickShaCheBtEvent += ClickShaCheBtEvent;
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickLaBaBtEvent += ClickLaBaBtEvent;
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickCloseDongGanBtEvent += ClickCloseDongGanBtEvent;

        if (PlayerPrefs.GetInt("Grade") == 0)
        {
            PlayerPrefs.SetInt("Grade", 2);
        }

        switch (PlayerPrefs.GetInt("Grade"))
        {
            case 1:
				GameGradeDuiGou[0].enabled = true;
				GameGradeDuiGou[1].enabled = false;
				GameGradeDuiGou[2].enabled = false;
                break;
			case 2:
				GameGradeDuiGou[0].enabled = false;
				GameGradeDuiGou[1].enabled = true;
				GameGradeDuiGou[2].enabled = false;
                break;
			case 3:
				GameGradeDuiGou[0].enabled = false;
				GameGradeDuiGou[1].enabled = false;
				GameGradeDuiGou[2].enabled = true;
                break;
            default:
                break;
        }
    }

	void Update () 
	{
		if (pcvr.bIsHardWare) {
			if (GlobalData.GetInstance().CoinCur > m_InserNum)
            {
				m_InserNum = GlobalData.GetInstance().CoinCur - 1;
				OnClickInsertBt();
			}

			//YouMenInfoLabel.text = pcvr.BikePowerCur.ToString();
			//FangXiangInfoLabel.text = pcvr.SteerValCur.ToString();

			if (!IsInitJiaoZhunPcvr) {
                if (pcvr.GetInstance().mGetPower > 0.1f)
                {
                    YouMenInfoLabel.text += ", Throttle Response";
                }

                float offsetSteer = 0.05f;
                if (pcvr.GetInstance().mGetSteer < -offsetSteer)
                {
                    FangXiangInfoLabel.text += ", Turn Left";
                }
                else if (pcvr.GetInstance().mGetSteer > offsetSteer)
                {
                    FangXiangInfoLabel.text += ", Turn Right";
                }
                else
                {
                    FangXiangInfoLabel.text += ", Turn Middle";
                }
            }
		}
		else {
			if (Input.GetKeyDown(KeyCode.T)) {
				OnClickInsertBt();
			}
            
            int val = (int)(pcvr.GetInstance().mGetSteer * 100);
            FangXiangInfoLabel.text = val.ToString();
			if (!IsInitJiaoZhunPcvr) {
				if (val < 0) {
					FangXiangInfoLabel.text += ", Turn Left";
				}
				else if (val > 0) {
					FangXiangInfoLabel.text += ", Turn Right";
				}
				else {
					FangXiangInfoLabel.text += ", Turn Middle";
				}
			}

            val = (int)(pcvr.GetInstance().mGetPower * 100);
            YouMenInfoLabel.text = val.ToString();
			if (!IsInitJiaoZhunPcvr) {
				if (val > 0) {				
					YouMenInfoLabel.text += ", Throttle Response";
				}
			}
		}
	}

	void OnClickInsertBt()
	{
		m_InserNum++;
		ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
		UpdateInsertCoin();
	}
	
	void UpdateInsertCoin()
	{
		//Debug.Log("m_InserNum "+m_InserNum);
		InsertCoinNumLabel.text = m_InserNum.ToString();
	}

	void ClickSetMoveBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.UP) {
			return;
		}
		OnClickMoveBtInZhujiemian();
	}

	void ClickSetEnterBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.UP) {
			return;
		}
		OnClickSelectBtInZhujiemian();
	}

	void ClickStartBtOneEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN)
        {
			BtInfoLabel.text = "StartBtDown";
            if (IsInitJiaoZhunPcvr)
            {
                UpdataJiaoZhunTexture();
            }
        }
		else
        {
			BtInfoLabel.text = "StartBtUp";
		}
	}

	void ClickCloseDongGanBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN) {
			BtInfoLabel.text = "DongGanBtDown";
		}
		else {
			BtInfoLabel.text = "DongGanBtUp";
		}
	}

	void ResetJiaoZhunPcvr()
	{
		if (!IsInitJiaoZhunPcvr) {
			return;
		}
		m_ZhujiemianXingXing.gameObject.SetActive(true);
		IsInitJiaoZhunPcvr = false;
		JiaoZhunObj.SetActive(false);
	}

	void InitJiaoZhunPcvr()
	{
		if (IsInitJiaoZhunPcvr) {
			return;
		}

        if (pcvr.bIsHardWare)
        {
            pcvr.GetInstance().InitJiaoZhunSteer();
            pcvr.GetInstance().InitJiaoZhunYouMenValInfo();
            pcvr.GetInstance().InitJiaoZhunJiaoTaBan();
        }
		m_ZhujiemianXingXing.gameObject.SetActive(false);
		IsInitJiaoZhunPcvr = true;

		JiaoZhunCount = 0;
		JiaoZhunTexture.mainTexture = JiaoZhunTextureArray[0];
		JiaoZhunObj.SetActive(true);
	}

    enum JiaoZhunEnum
    {
        SteerMin,   //方向左极限.
        SteerCen,   //方向中间值.
        SteerMax,   //方向右极限.
        YouMenMax,  //油门极限值.
        JiaoTaBan,  //脚踏板(编码器)信息.
    }

	void UpdataJiaoZhunTexture()
	{
        if (pcvr.bIsHardWare)
        {
            JiaoZhunEnum jiaoZhunSt = (JiaoZhunEnum)JiaoZhunCount;
            switch (jiaoZhunSt)
            {
                case JiaoZhunEnum.SteerMin:
                    {
                        pcvr.GetInstance().SaveSteerVal(pcvr.PcvrValState.ValMin);
                        break;
                    }
                case JiaoZhunEnum.SteerCen:
                    {
                        pcvr.GetInstance().SaveSteerVal(pcvr.PcvrValState.ValCenter);
                        break;
                    }
                case JiaoZhunEnum.SteerMax:
                    {
                        pcvr.GetInstance().SaveSteerVal(pcvr.PcvrValState.ValMax);
                        break;
                    }
                case JiaoZhunEnum.YouMenMax:
                    {
                        pcvr.GetInstance().SaveYouMenVal();
                        break;
                    }
                case JiaoZhunEnum.JiaoTaBan:
                    {
                        pcvr.GetInstance().SaveJiaoTaBanVal();
                        pcvr.GetInstance().EndJiaoZhunSteer();
                        pcvr.GetInstance().EndJiaoZhunYouMen();
                        pcvr.GetInstance().EndJiaoZhunJiaoTaBan();
                        break;
                    }
            }
        }

        if (JiaoZhunCount < (int)JiaoZhunEnum.JiaoTaBan)
        {
            JiaoZhunCount++;
            JiaoZhunTexture.mainTexture = JiaoZhunTextureArray[JiaoZhunCount];
        }
        else
        {
            ResetJiaoZhunPcvr();
        }
	}

    void OnClickMoveBtInZhujiemian()
	{
        if (IsInitJiaoZhunPcvr)
        {
            return;
        }

        m_IndexZhujiemian++;
        if (m_IndexZhujiemian > 18)
        {
            m_IndexZhujiemian = 0;
        }
        switch (m_IndexZhujiemian)
        {
            case 0:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, 212.0f, 0.0f);
                    break;
                }
            case 1:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-640.0f, 139.0f, 0.0f);
                    break;
                }
            case 2:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-278.0f, 139.0f, 0.0f);
                    break;
                }
            case 3:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, 66.0f, 0.0f);
                    break;
                }
            case 4:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, -4.5f, 0.0f);
                    break;
                }
            case 5:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-385.0f, -4.5f, 0.0f);
                    break;
                }
            case 6:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-253.0f, -4.5f, 0.0f);
                    break;
                }
            case 7:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -93.0f, 0.0f);
                    break;
                }
            case 8:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -125.0f, 0.0f);
                    break;
                }
            case 9:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -165.0f, 0.0f);
                    break;
                }
            case 10:
				{
					CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-620.0f, -200.0f, 0.0f);
                    break;
                }
            case 11:
				{
                    CloseAllQiNang();
					m_ZhujiemianXingXing.localPosition = new Vector3(-575.0f, -285.0f, 0.0f);
                    break;
                }
            case 12:
                {
					m_ZhujiemianXingXing.localPosition = new Vector3(-270.0f, -285.0f, 0.0f);
                    break;
                }
            case 13:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(-510.0f, -358.0f, 0.0f);
                    break;
                }
            case 14:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(56.0f, -81.0f, 0.0f);
                    break;
                }
            case 15:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(35.0f, -285.0f, 0.0f);
                    break;
                }
            case 16:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(235.0f, -285.0f, 0.0f);
                    break;
                }
            case 17:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(450.0f, -285.0f, 0.0f);
                    break;
                }
            case 18:
                {
                    m_ZhujiemianXingXing.localPosition = new Vector3(56.0f, -358.0f, 0.0f);
                    break;
                }
        }
    }

    void OnClickSelectBtInZhujiemian()
    {
        switch (m_IndexZhujiemian)
        {
            case 0:
                {
                    int CoinNum = Convert.ToInt32(m_CoinForStar.text);
                    CoinNum++;
                    if (CoinNum > 9)
                    {
                        CoinNum = 1;
                    }
                    m_CoinForStar.text = CoinNum.ToString();
                    ReadGameInfo.GetInstance().WriteStarCoinNumSet(CoinNum.ToString());
                    break;
                }
            case 1:
                {
                    m_GameModeDuigou1.enabled = true;
                    m_GameModeDuigou2.enabled = false;
                    ReadGameInfo.GetInstance().WriteGameStarMode(ReadGameInfo.GameMode.Oper.ToString());
                    break;
                }
            case 2:
                {
                    m_GameModeDuigou1.enabled = false;
                    m_GameModeDuigou2.enabled = true;
                    ReadGameInfo.GetInstance().WriteGameStarMode(ReadGameInfo.GameMode.Free.ToString());
                    break;
                }
            case 3:
                {
                    ResetFactory();
                    break;
                }
            case 4:
                {
                    //pcvr.StartBtLight = StartLightState.Liang;
                    break;
                }
            case 5:
                {
                    //pcvr.StartBtLight = StartLightState.Shan;
                    break;
                }
            case 6:
                {
                    //pcvr.StartBtLight = StartLightState.Mie;
                    break;
                }
            case 7:
                {
                    //pcvr.m_IsOpneForwardQinang = true;
                    //pcvr.m_IsOpneBehindQinang = false;
                    //pcvr.m_IsOpneLeftQinang = false;
                    //pcvr.m_IsOpneRightQinang = false;
                    break;
                }
            case 8:
                {
                    //pcvr.m_IsOpneForwardQinang = false;
                    //pcvr.m_IsOpneBehindQinang = true;
                    //pcvr.m_IsOpneLeftQinang = false;
                    //pcvr.m_IsOpneRightQinang = false;
                    break;
                }
            case 9:
                {
                    //pcvr.m_IsOpneForwardQinang = false;
                    //pcvr.m_IsOpneBehindQinang = false;
                    //pcvr.m_IsOpneLeftQinang = true;
                    //pcvr.m_IsOpneRightQinang = false;
                    break;
                }
            case 10:
                {
                    //pcvr.m_IsOpneForwardQinang = false;
                    //pcvr.m_IsOpneBehindQinang = false;
                    //pcvr.m_IsOpneLeftQinang = false;
                    //pcvr.m_IsOpneRightQinang = true;
                    break;
                }
			case 11:
				{
					GameAudioVolume++;
					if (GameAudioVolume > 10) {
						GameAudioVolume = 0;
					}
					GameAudioVolumeLB.text = GameAudioVolume.ToString();
					ReadGameInfo.GetInstance().WriteGameAudioVolume(GameAudioVolume);
					break;
				}
			case 12:
				{
					GameAudioVolume = 7;
					GameAudioVolumeLB.text = GameAudioVolume.ToString();
					ReadGameInfo.GetInstance().WriteGameAudioVolume(GameAudioVolume);
					break;
				}
            case 13:
                {
                    //int speedVal = Convert.ToInt32(PlayerMinSpeed.text);
                    //speedVal += 10;
                    //if (speedVal > 80)
                    //{
                    //    speedVal = 0;
                    //}
                    //PlayerMinSpeed.text = speedVal.ToString();
                    //ReadGameInfo.GetInstance().WritePlayerMinSpeedVal(speedVal);
                    break;
                }
            case 14:
                {
                    InitJiaoZhunPcvr();
                    break;
                }

            case 15:
				{
					GameGradeDuiGou[0].enabled = true;
					GameGradeDuiGou[1].enabled = false;
					GameGradeDuiGou[2].enabled = false;
                    PlayerPrefs.SetInt("Grade", 1);
                    break;
                }

            case 16:
				{
					GameGradeDuiGou[0].enabled = false;
					GameGradeDuiGou[1].enabled = true;
					GameGradeDuiGou[2].enabled = false;
                    PlayerPrefs.SetInt("Grade", 2);
                    break;
                }

            case 17:                
				{
					GameGradeDuiGou[0].enabled = false;
					GameGradeDuiGou[1].enabled = false;
					GameGradeDuiGou[2].enabled = true;
                    PlayerPrefs.SetInt("Grade", 3);
                    break;
                }

            case 18:
                {
                    CloseAllQiNang();
					//pcvr.StartBtLight = StartLightState.Mie;
					//XkGameCtrl.IsLoadingLevel = true;
                    IsOpenSetPanel = false;
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    Application.LoadLevel(0);
                    break;
                }
        }
    }

	public UILabel GameAudioVolumeLB;
	void CloseAllQiNang()
	{
		//pcvr.m_IsOpneForwardQinang = false;
		//pcvr.m_IsOpneBehindQinang = false;
		//pcvr.m_IsOpneLeftQinang = false;
		//pcvr.m_IsOpneRightQinang = false;
	}

	void ResetFactory()
	{
		ReadGameInfo.GetInstance().FactoryReset();
		PlayerMinSpeed.text = "0";
		m_CoinForStar.text = "1";
		m_GameModeDuigou1.enabled = false;
		m_GameModeDuigou2.enabled = true;
		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();

		if (pcvr.bIsHardWare)
        {
            pcvr.GetInstance().mPcvrTXManage.SubPlayerCoin(m_InserNum, pcvrTXManage.PlayerCoinEnum.player01);
        }
		m_InserNum = 0;
		UpdateInsertCoin();
	}

	void ClickShaCheBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN) {
			BtInfoLabel.text = "BrakeBtDown";
		}
		else {
			BtInfoLabel.text = "BrakeBtUp";
		}
	}

	void ClickLaBaBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.DOWN) {
			BtInfoLabel.text = "SpeakerBtDown";
		}
		else {
			BtInfoLabel.text = "SpeakerBtUp";
		}
	}

//    void OnGUI()
//    {
//        GUI.Box(new Rect(Screen.width / 1.8f, Screen.height / 1.5f, 400f, 30f), "难度(GRADE)");
//        GUI.Box(new Rect(Screen.width / 1.8f, Screen.height / 1.4f, 100f, 30f), "简单(EASY)");
//        GUI.Box(new Rect(Screen.width / 1.5f, Screen.height / 1.4f, 100f, 30f), "正常(NORMAL)");
//        GUI.Box(new Rect(Screen.width / 1.285f, Screen.height / 1.4f, 100f, 30f), "困难(HARD)");
//    }
}