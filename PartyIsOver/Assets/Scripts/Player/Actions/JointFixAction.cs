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
    PlayerContext _context;

    bool HandleJointFixEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _bodyHandler = bodyHandler;
        _context = data;

        if (_context.LeftSearchTarget != null && _context.LeftGrabJoint == null)
            JointFix((int)Define.Side.Left);

        if (_context.RightSearchTarget != null && _context.RightGrabJoint == null)
            JointFix((int)Define.Side.Right);

        return true;
    }

    void JointFix(int side)
    {
        ItemType type = ItemType.None;

        //아이템 장착시
        if (_context.EquipItem != null)
        {
            //레이어 변경
            type = _context.EquipItem.ItemObject.ItemData.ItemType;
            _context.EquipItem.gameObject.layer = _bodyHandler.gameObject.layer;

            _context.EquipItem.ItemObject.Owner = _bodyHandler.GetComponent<Actor>();
            _context.EquipItem.RigidbodyObject.mass = 0.3f;

            //owner변경
            if (_context.IsMine && _context.EquipItem.PhotonView != null)
            {
                int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                _context.EquipItem.PhotonView.TransferOwnership(playerID);
            }
        }

        //관절 생성 및 연결
        if ((Define.Side)side == Define.Side.Left)
        {
            _context.LeftGrabObject = _context.LeftSearchTarget;
            _context.LeftGrabJoint = _bodyHandler.LeftHand.gameObject.AddComponent<FixedJoint>();
            _context.LeftGrabJoint.connectedBody = _context.LeftSearchTarget.RigidbodyObject;
        }
        else if ((Define.Side)side == Define.Side.Right)
        {
            _context.LeftGrabObject = _context.LeftSearchTarget;
            _context.RightGrabJoint = _bodyHandler.RightHand.gameObject.AddComponent<FixedJoint>();
            _context.RightGrabJoint.connectedBody = _context.RightSearchTarget.RigidbodyObject;
        }

        //관절 고정
        if (_context.EquipItem != null && (type == ItemType.TwoHanded || type == ItemType.Ranged))
            _bodyHandler.JointLock((Define.Side)side);
    }
}
