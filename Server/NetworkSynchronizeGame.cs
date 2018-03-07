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

    void FixedUpdate()
    {
        if (!IsNetControlPort)
        {
            //非主控制端.
            SynNetPosition();
            SynNetRotation();
            return;
        }

        //主控制端同步坐标.
        if (mNetPos != transform.position)
        {
            mNetPos = transform.position;
            if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
            {
                mNetworkView.RPC("RpcNetSynPosition", RPCMode.OthersBuffered, mNetPos);
            }
        }

        //主控制端同步转向.
        if (mNetRot != transform.forward)
        {
            mNetRot = transform.forward;
            if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
            {
                mNetworkView.RPC("RpcNetSynRotation", RPCMode.OthersBuffered, mNetRot);
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
}