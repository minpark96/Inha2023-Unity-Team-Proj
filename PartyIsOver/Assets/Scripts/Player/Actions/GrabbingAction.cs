using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GrabbingAction
{
    public GrabbingAction(ActionController actions)
    {
        actions.OnGrabbing -= HandleGrabbingEvent;
        actions.OnGrabbing += HandleGrabbingEvent;
    }
    PlayerActionContext _context;
    Rigidbody _leftHandRigid;
    Rigidbody _rightHandRigid;


    bool _isGrounded;
    Vector3 _leftAddForceDir;
    Vector3 _rightAddForceDir;
    Vector3 _leftTargetDir;
    Vector3 _rightTargetDir;

    public bool HandleGrabbingEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _isGrounded = data.IsGrounded;

        _leftTargetDir = data.LeftTargetDir;
        _rightTargetDir = data.RightTargetDir;
        _leftHandRigid = bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = bodyHandler.RightHand.PartRigidbody;

        if(_context.IsItemGrabbing && _context.RightSearchTarget != null && _context.RightSearchTarget.ItemObject !=null)
        {
            Debug.Log(_context.RightSearchTarget);
            ItemDirSetting(_context.RightSearchTarget.ItemObject);
        }
        else
            NonItemDirSetting();

        Grabbing();
        return true;
    }

    private void Grabbing()
    {
        if (_leftTargetDir != Vector3.zero)
            _leftHandRigid.AddForce(_leftAddForceDir.normalized * 150f);

        if (_rightTargetDir != Vector3.zero)
            _rightHandRigid.AddForce(_rightAddForceDir.normalized * 150f);

    }
    private void NonItemDirSetting()
    {
        if (_leftTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _leftAddForceDir = ((_leftTargetDir + Vector3.up * 2) - _leftHandRigid.transform.position).normalized;
            else
                _leftAddForceDir = (_leftTargetDir - _leftHandRigid.transform.position).normalized;
        }

        if (_rightTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _rightAddForceDir = ((_rightTargetDir + Vector3.up * 2) - _rightHandRigid.transform.position).normalized;
            else
                _rightAddForceDir = (_rightTargetDir - _rightHandRigid.transform.position).normalized;
        }
    }
    private void ItemDirSetting(Item item)
    {
        switch (item.ItemData.ItemType)
        {
            case ItemType.TwoHanded:
                    TwoHandedGrab(item);
                break;
            case ItemType.Ranged:
                    TwoHandedGrab(item);
                break;
            case ItemType.Consumable:
                {
                    _rightAddForceDir = item.OneHandedPos.position - _rightHandRigid.transform.position;
                    _leftTargetDir = Vector3.zero;
                    _rightTargetDir = Vector3.up;
                }
                break;
        }
    }


    void TwoHandedGrab(Item item)
    {
        Vector3 rightGripPos = item.TwoHandedPos.position;
        Vector3 leftGripPos = item.OneHandedPos.position;

        //아이템 방향따라 오른쪽 손잡이를 오른손으로 잡기 진행
        if (_context.ItemHandleSide == Side.Right)
        {
            rightGripPos = item.OneHandedPos.position;
            leftGripPos = item.TwoHandedPos.position;
        }

        _rightAddForceDir = rightGripPos - _rightHandRigid.transform.position;
        _leftAddForceDir = leftGripPos - _leftHandRigid.transform.position;
        _leftTargetDir = Vector3.up;
        _rightTargetDir = Vector3.up;
    }
}
