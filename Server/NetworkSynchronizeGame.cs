using UnityEngine;

/// <summary>
/// 用于网络游戏中主角或npc的坐标动画等信息同步.
/// </summary>
public class NetworkSynchronizeGame : MonoBehaviour
{
    /// <summary>
    /// 是否为网络主控端.
    /// </summary>
    bool IsNetControlPort = false;
    /// <summary>
    /// Rpc消息控制组件.
    /// </summary>
    NetworkView mNetworkView;
    Vector3 mNetPos;
    /// <summary>
    /// 是否同步坐标.
    /// </summary>
    bool IsSynNetPos = false;
    Vector3 mNetRot;
    /// <summary>
    /// 是否同步转向.
    /// </summary>
    bool IsSynNetRot = false;
    /// <summary>
    /// 动画控制组件.
    /// </summary>
    Animator mAnimator;
    public enum AnimatorType
    {
        Trigger,
        Bool,
    }

    /// <summary>
    /// 只在主控制端初始化.
    /// </summary>
    public void Init(NetworkView netView)
    {
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (NetworkServerNet.GetInstance().LinkServerPlayerNum_Movie <= 0
                && NetworkServerNet.GetInstance().ePlayerPortState == NetworkServerNet.PlayerPortType.Server)
            {
                //没有玩家链接服务器.
                enabled = false;
            }
            else
            {
                //多人网络游戏.
                mNetworkView = netView;
                IsNetControlPort = true;
            }
        }
        else
        {
            //单机游戏.
            enabled = false;
        }
    }

    /// <summary>
    /// 初始化数据.
    /// </summary>
    public void InitData(Animator ani)
    {
        mAnimator = ani;
    }

    void FixedUpdate()
    {
        if (!IsNetControlPort)
        {
            //非主控制端.
            SynNetPosition();
            SynNetRotation();
            return;
        }

        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (NetworkServerNet.GetInstance().mNetworkRootGame.ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.GameBackMovie)
            {
                //游戏准备返回循环动画场景,无需继续同步信息!
            }
            else
            {
                //主控制端同步坐标.
                if (mNetPos != transform.position)
                {
                    mNetPos = transform.position;
                    mNetworkView.RPC("RpcNetSynPosition", RPCMode.Others, mNetPos);
                }

                //主控制端同步转向.
                if (mNetRot != transform.forward)
                {
                    mNetRot = transform.forward;
                    mNetworkView.RPC("RpcNetSynRotation", RPCMode.Others, mNetRot);
                }
            }
        }
    }

    [RPC]
    void RpcNetSynPosition(Vector3 pos)
    {
        mNetPos = pos;
        IsSynNetPos = true;
    }

    /// <summary>
    /// 同步坐标(非主控制端).
    /// </summary>
    void SynNetPosition()
    {
        if (IsSynNetPos)
        {
            if (Vector3.Distance(transform.position, mNetPos) > 0.25f)
            {
                transform.position = Vector3.Lerp(transform.position, mNetPos, 0.1f);
            }
        }
    }

    [RPC]
    void RpcNetSynRotation(Vector3 rot)
    {
        mNetRot = rot;
        IsSynNetRot = true;
    }

    /// <summary>
    /// 同步转向(非主控制端).
    /// </summary>
    void SynNetRotation()
    {
        if (IsSynNetRot)
        {
            if (Vector3.Dot(transform.forward, mNetRot) < Mathf.Cos(Mathf.PI / 36))
            {
                transform.forward = Vector3.Lerp(transform.forward, mNetRot, 0.1f);
            }
        }
    }

    /// <summary>
    /// 同步网络动画.
    /// </summary>
    public void SynNetAnimator(string aniName, AnimatorType aniType, bool isPlay = false)
    {
        if (!IsNetControlPort || !enabled)
        {
            return;
        }

        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            if (NetworkServerNet.GetInstance().mNetworkRootGame.ePlayerGameNetState == NetworkServerNet.PlayerGameNetType.GameBackMovie)
            {
                //游戏准备返回循环动画场景,无需继续同步信息!
            }
            else
            {
                mNetworkView.RPC("RpcNetSynAnimator", RPCMode.Others, aniName, (int)aniType, isPlay == false ? 0 : 1);
            }
        }
    }

    [RPC]
    void RpcNetSynAnimator(string aniName, int aniType, int isPlay)
    {
        AnimatorType aniState = (AnimatorType)aniType;
        switch (aniState)
        {
            case AnimatorType.Trigger:
                {
                    mAnimator.SetTrigger(aniName);
                    break;
                }
            case AnimatorType.Bool:
                {
                    mAnimator.SetBool(aniName, isPlay == 0 ? false : true);
                    break;
                }
        }
    }
}