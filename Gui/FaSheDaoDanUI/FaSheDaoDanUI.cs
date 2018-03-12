using UnityEngine;

public class FaSheDaoDanUI : MonoBehaviour
{
    /// <summary>
    /// 动画事件回调.
    /// </summary>
    public void OnAnimationEnvent(int index)
    {
        if (PlayerController.GetInstance() != null && PlayerController.GetInstance().m_UIController != null)
        {
            PlayerController.GetInstance().m_UIController.RemoveFaSheDaoDanUI();
        }
    }
}