using UnityEngine;

public class InputEventCtrl : MonoBehaviour
{
    public enum ButtonState : int
    {
        UP = 1,
        DOWN = -1
    }
    /// <summary>
    /// 监听电脑按键响应.
    /// </summary>
    [HideInInspector]
    public ListenPcInputEvent mListenPcInputEvent;
    static InputEventCtrl _Instance = null;
    public static InputEventCtrl GetInstance()
    {
        if (_Instance == null)
        {
            GameObject obj = new GameObject("_InputEventCtrl");
            _Instance = obj.AddComponent<InputEventCtrl>();
            _Instance.mListenPcInputEvent = obj.AddComponent<ListenPcInputEvent>();
            pcvr.GetInstance();
        }
        return _Instance;
    }

    #region CaiPiaoJi Event
    /// <summary>
    /// 彩票机无票事件.
    /// </summary>
    public delegate void CaiPiaoJiWuPiaoEvent(pcvrTXManage.CaiPiaoJi val);
    public event CaiPiaoJiWuPiaoEvent OnCaiPiaJiWuPiaoEvent;
    public void OnCaiPiaJiWuPiao(pcvrTXManage.CaiPiaoJi val)
    {
        if (OnCaiPiaJiWuPiaoEvent != null)
        {
            OnCaiPiaJiWuPiaoEvent(val);
        }
    }

    /// <summary>
    /// 彩票机出票响应事件.
    /// </summary>
    public delegate void CaiPiaoJiChuPiaoEvent(pcvrTXManage.CaiPiaoJi val);
    public event CaiPiaoJiChuPiaoEvent OnCaiPiaJiChuPiaoEvent;
    public void OnCaiPiaJiChuPiao(pcvrTXManage.CaiPiaoJi val)
    {
        if (OnCaiPiaJiChuPiaoEvent != null)
        {
            OnCaiPiaJiChuPiaoEvent(val);
        }
    }
    #endregion

    #region Click Button Event
    /// <summary>
    /// 按键响应事件.
    /// </summary>
    public delegate void EventHandel(ButtonState val);
    public event EventHandel ClickPcvrBtEvent01;
    public void ClickPcvrBt01(ButtonState val)
    {
        if (ClickPcvrBtEvent01 != null)
        {
            ClickPcvrBtEvent01(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent02;
    public void ClickPcvrBt02(ButtonState val)
    {
        if (ClickPcvrBtEvent02 != null)
        {
            ClickPcvrBtEvent02(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent03;
    public void ClickPcvrBt03(ButtonState val)
    {
        if (ClickPcvrBtEvent03 != null)
        {
            ClickPcvrBtEvent03(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent04;
    public void ClickPcvrBt04(ButtonState val)
    {
        if (ClickPcvrBtEvent04 != null)
        {
            ClickPcvrBtEvent04(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent05;
    public void ClickPcvrBt05(ButtonState val)
    {
        if (ClickPcvrBtEvent05 != null)
        {
            ClickPcvrBtEvent05(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent06;
    public void ClickPcvrBt06(ButtonState val)
    {
        if (ClickPcvrBtEvent06 != null)
        {
            ClickPcvrBtEvent06(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent07;
    public void ClickPcvrBt07(ButtonState val)
    {
        if (ClickPcvrBtEvent07 != null)
        {
            ClickPcvrBtEvent07(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent08;
    public void ClickPcvrBt08(ButtonState val)
    {
        if (ClickPcvrBtEvent08 != null)
        {
            ClickPcvrBtEvent08(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent09;
    public void ClickPcvrBt09(ButtonState val)
    {
        if (ClickPcvrBtEvent09 != null)
        {
            ClickPcvrBtEvent09(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent10;
    public void ClickPcvrBt10(ButtonState val)
    {
        if (ClickPcvrBtEvent10 != null)
        {
            ClickPcvrBtEvent10(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent11;
    public void ClickPcvrBt11(ButtonState val)
    {
        if (ClickPcvrBtEvent11 != null)
        {
            ClickPcvrBtEvent11(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent12;
    public void ClickPcvrBt12(ButtonState val)
    {
        if (ClickPcvrBtEvent12 != null)
        {
            ClickPcvrBtEvent12(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent13;
    public void ClickPcvrBt13(ButtonState val)
    {
        if (ClickPcvrBtEvent13 != null)
        {
            ClickPcvrBtEvent13(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent14;
    public void ClickPcvrBt14(ButtonState val)
    {
        if (ClickPcvrBtEvent14 != null)
        {
            ClickPcvrBtEvent14(val);
        }
    }
    public event EventHandel ClickPcvrBtEvent15;
    public void ClickPcvrBt15(ButtonState val)
    {
        if (ClickPcvrBtEvent15 != null)
        {
            ClickPcvrBtEvent15(val);
        }
    }
    #endregion
}