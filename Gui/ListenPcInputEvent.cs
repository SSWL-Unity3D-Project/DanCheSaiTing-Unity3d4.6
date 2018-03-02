using UnityEngine;

public class ListenPcInputEvent : MonoBehaviour
{
    void Start()
    {
        //响应pcvr的按键消息.
        //InputEventCtrl.GetInstance().ClickPcvrBtEvent07 += ClickSetEnterBt;
        //InputEventCtrl.GetInstance().ClickPcvrBtEvent08 += ClickSetMoveBt;
        //InputEventCtrl.GetInstance().ClickPcvrBtEvent10 += ClickCloseDongGanBt;
    }

    #region Click Button Envent
    public event InputEventCtrl.EventHandel ClickStartBtOneEvent;
    public void ClickStartBtOne(InputEventCtrl.ButtonState val)
    {
        if (ClickStartBtOneEvent != null)
        {
            ClickStartBtOneEvent(val);
            //pcvr.StartBtLight = StartLightState.Mie;
        }
    }

    public event InputEventCtrl.EventHandel ClickCloseDongGanBtEvent;
    public void ClickCloseDongGanBt(InputEventCtrl.ButtonState val)
    {
        if (ClickCloseDongGanBtEvent != null)
        {
            ClickCloseDongGanBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickPlayerYouMenBtEvent;
    public void ClickPlayerYouMenBt(InputEventCtrl.ButtonState val)
    {
        if (ClickPlayerYouMenBtEvent != null)
        {
            ClickPlayerYouMenBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickSetEnterBtEvent;
    public void ClickSetEnterBt(InputEventCtrl.ButtonState val)
    {
        if (ClickSetEnterBtEvent != null)
        {
            ClickSetEnterBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickSetMoveBtEvent;
    public void ClickSetMoveBt(InputEventCtrl.ButtonState val)
    {
        if (ClickSetMoveBtEvent != null)
        {
            ClickSetMoveBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickFireBtEvent;
    public void ClickFireBt(InputEventCtrl.ButtonState val)
    {
        if (ClickFireBtEvent != null)
        {
            ClickFireBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickShaCheBtEvent;
    public void ClickShaCheBt(InputEventCtrl.ButtonState val)
    {
        if (ClickShaCheBtEvent != null)
        {
            ClickShaCheBtEvent(val);
        }
    }

    public event InputEventCtrl.EventHandel ClickLaBaBtEvent;
    public void ClickLaBaBt(InputEventCtrl.ButtonState val)
    {
        if (ClickLaBaBtEvent != null)
        {
            ClickLaBaBtEvent(val);
        }
    }
    #endregion

    void Update()
    {
        if (pcvr.bIsHardWare)
        {
            return;
        }

        //StartBt PlayerOne
        if (Input.GetKeyUp(KeyCode.K))
        {
            ClickStartBtOne(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ClickStartBtOne(InputEventCtrl.ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            ClickCloseDongGanBt(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ClickCloseDongGanBt(InputEventCtrl.ButtonState.DOWN);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            ClickPlayerYouMenBt(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ClickPlayerYouMenBt(InputEventCtrl.ButtonState.DOWN);
        }

        //setPanel enter button
        if (Input.GetKeyUp(KeyCode.F4))
        {
            ClickSetEnterBt(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            ClickSetEnterBt(InputEventCtrl.ButtonState.DOWN);
        }

        //setPanel move button
        if (Input.GetKeyUp(KeyCode.F5))
        {
            ClickSetMoveBt(InputEventCtrl.ButtonState.UP);
            //FramesPerSecond.GetInstance().ClickSetMoveBtEvent( InputEventCtrl.ButtonState.UP );
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            ClickSetMoveBt(InputEventCtrl.ButtonState.DOWN);
            //FramesPerSecond.GetInstance().ClickSetMoveBtEvent( InputEventCtrl.ButtonState.DOWN );
        }

        //Fire button
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //IsClickFireBtDown = false;
            ClickFireBt(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //IsClickFireBtDown = true;
            ClickFireBt(InputEventCtrl.ButtonState.DOWN);
        }
    }
}