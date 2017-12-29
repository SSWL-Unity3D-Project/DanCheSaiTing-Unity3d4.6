using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour
{
    /// <summary>
    /// 主角动画.
    /// </summary>
    //public Animator PlayerAni;
    /// <summary>
    /// 喷气动画列表.
    /// </summary>
    public Animator[] PenQiAniAy;
    /// <summary>
    /// 飞机翅膀动画列表.
    /// </summary>
    public Animator[] FiXingYiAniAy;
    /// <summary>
    /// 风框动画.
    /// </summary>
    public Animator FengKuangAni;
    /// <summary>
    /// 道具风框转动脚本.
    /// </summary>
	public TweenRotation FengKuangTwRot;
	/// <summary>
	/// 积分产生点.
	/// </summary>
	public Transform SpawnJiFenTr;
}