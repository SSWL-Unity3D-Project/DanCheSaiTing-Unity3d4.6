using UnityEngine;
using System.Collections;

public class DongGanCtrl : MonoBehaviour {
	public GameObject DongGanOpen;
	public GameObject DongGanClose;
	GameObject DongGanObj;
	static DongGanCtrl Instance;
	public static DongGanCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Awake()
	{
		Instance = this;
		DongGanObj = gameObject;
		DongGanObj.SetActive(false);
		DongGanOpen.SetActive(false);
		DongGanClose.SetActive(false);
		//InputEventCtrl.GetInstance().ClickShaCheBtEvent += ClickShaCheBtEvent;
	}

//	void ClickShaCheBtEvent(ButtonState val)
//	{
//		if (val != ButtonState.DOWN) {
//			return;
//		}
//	}

	public void ShowDongGanOpen()
	{
		DongGanOpen.SetActive(true);
		DongGanClose.SetActive(false);
		DongGanObj.SetActive(true);
		CancelInvoke("CloseDongGanTiShi");
		Invoke("CloseDongGanTiShi", 3f);
	}

	void CloseDongGanTiShi()
	{
		DongGanObj.SetActive(false);
		DongGanOpen.SetActive(false);
		DongGanClose.SetActive(false);
	}

	public void ShowDongGanClose()
	{
		DongGanOpen.SetActive(false);
		DongGanClose.SetActive(true);
		DongGanObj.SetActive(true);
	}
}