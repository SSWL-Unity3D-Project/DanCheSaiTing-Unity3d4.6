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

    #region Click Button Event
    /// <summary>
    /// 关闭/打开动感按键.
    /// </summary>
    public event InputEventCtrl.EventHandel ClickCloseDongGanBtEvent;
    public void ClickCloseDongGanBt(InputEventCtrl.ButtonState val)
    {
        if (ClickCloseDongGanBtEvent != null)
        {
            ClickCloseDongGanBtEvent(val);
        }
    }

    /// <summary>
    /// 设置按键.
    /// </summary>
    public event InputEventCtrl.EventHandel ClickSetEnterBtEvent;
    public void ClickSetEnterBt(InputEventCtrl.ButtonState val)
    {
        if (ClickSetEnterBtEvent != null)
        {
            ClickSetEnterBtEvent(val);
        }
    }

    /// <summary>
    /// 移动按键.
    /// </summary>
    public event InputEventCtrl.EventHandel ClickSetMoveBtEvent;
    public void ClickSetMoveBt(InputEventCtrl.ButtonState val)
    {
        if (ClickSetMoveBtEvent != null)
        {
            ClickSetMoveBtEvent(val);
        }
    }
    #endregion

    void Update()
    {
        if (pcvr.bIsHardWare)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            ClickCloseDongGanBt(InputEventCtrl.ButtonState.UP);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ClickCloseDongGanBt(InputEventCtrl.ButtonState.DOWN);
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
            //FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            ClickSetMoveBt(InputEventCtrl.ButtonState.DOWN);
            //FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
        }
    }
}