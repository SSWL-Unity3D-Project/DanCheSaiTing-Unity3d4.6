using UnityEngine;
using System.Collections;

public class InputEventCtrl : MonoBehaviour {

	public static bool IsClickFireBtDown;
	public static uint SteerValCur;
	public static uint TaBanValCur;
	static private InputEventCtrl Instance = null;
	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_InputEventCtrl");
			Instance = obj.AddComponent<InputEventCtrl>();
			pcvr.GetInstance();
		}
		return Instance;
	}

	#region Click Button Envent
	public delegate void EventHandel(ButtonState val);
	public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			pcvr.StartBtLight = StartLightState.Mie;
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickCloseDongGanBtEvent;
	public void ClickCloseDongGanBt(ButtonState val)
	{
		if(ClickCloseDongGanBtEvent != null)
		{
			ClickCloseDongGanBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickPlayerYouMenBtEvent;
	public void ClickPlayerYouMenBt(ButtonState val)
	{
		if(ClickPlayerYouMenBtEvent != null)
		{
			ClickPlayerYouMenBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickFireBtEvent;
	public void ClickFireBt(ButtonState val)
	{
		if(ClickFireBtEvent != null)
		{
			ClickFireBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickShaCheBtEvent;
	public void ClickShaCheBt(ButtonState val)
	{
		if(ClickShaCheBtEvent != null)
		{
			ClickShaCheBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickLaBaBtEvent;
	public void ClickLaBaBt(ButtonState val)
	{
		if(ClickLaBaBtEvent != null)
		{
			ClickLaBaBtEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	#endregion

	void Update()
	{
		if (pcvr.bIsHardWare) {
			return;
		}

		//StartBt PlayerOne
		if(Input.GetKeyUp(KeyCode.K))
		{
			ClickStartBtOne( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.K))
		{
			ClickStartBtOne( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.P))
		{
			ClickCloseDongGanBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.P))
		{
			ClickCloseDongGanBt( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.W))
		{
			ClickPlayerYouMenBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			ClickPlayerYouMenBt( ButtonState.DOWN );
		}

		//setPanel enter button
		if(Input.GetKeyUp(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.DOWN );
		}

		//setPanel move button
		if(Input.GetKeyUp(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.UP );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.DOWN );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
		}

		//Fire button
		if(Input.GetKeyUp(KeyCode.Mouse0))
		{
			IsClickFireBtDown = false;
			ClickFireBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			IsClickFireBtDown = true;
			ClickFireBt( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.Space))
		{
			ClickShaCheBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ClickShaCheBt( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.G))
		{
			ClickLaBaBt( ButtonState.UP );
		}

		if(Input.GetKeyDown(KeyCode.G))
		{
			ClickLaBaBt( ButtonState.DOWN );
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}