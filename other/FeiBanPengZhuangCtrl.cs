using UnityEngine;

/// <summary>
/// 加速飞板碰撞控制.
/// </summary>
public class FeiBanPengZhuangCtrl : MonoBehaviour
{
    public Collider[] PengZhuangArray;
    public void SetIsEnablePengZhuang(bool isEnable)
    {
        for (int i = 0; i < PengZhuangArray.Length; i++)
        {
            PengZhuangArray[i].enabled = isEnable;
        }
    }
}