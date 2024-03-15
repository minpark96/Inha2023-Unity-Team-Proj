using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class JointDestroyAction : PlayerAction
{
    public JointDestroyAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    PlayerActionContext _context;
    BodyHandler _bodyHandler;
    Item _equipItem;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _bodyHandler = bodyHandler;
        ResetGrab();
        return true;
    }


    void ResetGrab()
    {     
        if(_context.IsMine)
        {
            int PlayerID = PhotonNetwork.MasterClient.ActorNumber;
            if (_context.LeftGrabObject != null && _context.LeftGrabObject.PhotonView != null)
                _context.LeftGrabObject.PhotonView.TransferOwnership(PlayerID);
            if (_context.RightGrabObject != null && _context.RightGrabObject.PhotonView != null)
                _context.RightGrabObject.PhotonView.TransferOwnership(PlayerID);
        }
        if(_context.EquipItem != null)
        {
            _equipItem = _context.EquipItem.ItemObject;

            _context.EquipItem.gameObject.layer = (int)Define.Layer.Item;
            _equipItem.Body.gameObject.SetActive(true);

            _context.EquipItem.ItemObject.Owner = null;
            if (_equipItem.ItemData.ItemType == ItemType.OneHanded ||
                _equipItem.ItemData.ItemType == ItemType.TwoHanded)
                _context.EquipItem.damageModifier = InteractableObject.Damage.Default;
            _context.EquipItem.RigidbodyObject.mass = 10f;
            _context.EquipItem = null;
        }

        _bodyHandler.DestroyJoint(_context.RightGrabJoint, _context.LeftGrabJoint);
        _bodyHandler.UnlockArmPosition();

        _context.LeftGrabObject = null;
        _context.RightGrabObject = null;
        // _actor.GrabState = GrabState.None;
    }
}
