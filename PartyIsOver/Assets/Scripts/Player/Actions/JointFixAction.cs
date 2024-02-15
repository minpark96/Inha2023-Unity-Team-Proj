using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class JointFixAction
{
    public JointFixAction(ActionController actions)
    {
        actions.OnJointFix -= HandleJointFixEvent;
        actions.OnJointFix += HandleJointFixEvent;
    }

    BodyHandler _bodyHandler;
    PlayerContext context;

    bool HandleJointFixEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _bodyHandler = bodyHandler;
        context = data;

        if (context.LeftGrabObject != null && context.LeftGrabJoint == null)
            JointFix((int)Define.Side.Left);

        if (context.RightGrabObject != null && context.RightGrabJoint == null)
            JointFix((int)Define.Side.Right);

        return true;
    }

    void JointFix(int side)
    {
        ItemType type = ItemType.None;

        //아이템 장착시
        if (context.EquipItem != null)
        {
            //레이어 변경
            type = context.EquipItem.ItemObject.ItemData.ItemType;
            context.EquipItem.gameObject.layer = _bodyHandler.gameObject.layer;

            //owner변경
            if (context.IsMine && context.EquipItem.PhotonView != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                context.EquipItem.PhotonView.TransferOwnership(playerID);
            }
        }
   

        //관절 생성 및 연결
        if ((Define.Side)side == Define.Side.Left)
        {
            if (context.LeftSearchTarget == null)
                Debug.Log("Fail LeftJointFix");

            context.LeftGrabJoint = _bodyHandler.LeftHand.gameObject.AddComponent<FixedJoint>();
            context.LeftGrabJoint.connectedBody = context.LeftSearchTarget.RigidbodyObject;
        }
        else if ((Define.Side)side == Define.Side.Right)
        {
            if (context.RightSearchTarget == null)
                Debug.Log("Fail RightJointFix");

            context.RightGrabJoint = _bodyHandler.RightHand.gameObject.AddComponent<FixedJoint>();
            context.RightGrabJoint.connectedBody = context.RightSearchTarget.RigidbodyObject;
        }

        //관절 고정
        if (context.EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            _bodyHandler.JointLock((Define.Side)side);
    }
}
