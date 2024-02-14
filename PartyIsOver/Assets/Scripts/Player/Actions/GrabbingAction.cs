using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrabbingAction
{
    public GrabbingAction(ActionController actions)
    {
        actions.OnGrabbing -= HandleGrabbingEvent;
        actions.OnGrabbing += HandleGrabbingEvent;
        this._actions = actions;
    }

    ActionController _actions;
    AnimationPlayer _animPlayer;
    //BodyHandler _bodyHandler;
    AnimationData _animData;

    InteractableObject _leftSearchTarget;
    InteractableObject _rightSearchTarget;

    Rigidbody _leftHandRigid;
    Rigidbody _rightHandRigid;


    Vector3 _moveDir = new Vector3();
    Vector3 dir;
    bool _isGrounded;
    bool _isLeftGrab;
    bool _isRightGrab;

    Vector3 _leftTargetDir;
    Vector3 _rightTargetDir;

    public bool HandleGrabbingEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        //_bodyHandler = bodyHandler;

        _moveDir.x = data.DirX;
        _moveDir.y = data.DirY;
        _moveDir.z = data.DirZ;
        _isGrounded = data.IsGrounded;
        //_leftSearchTarget = data.LeftSearchTarget;
        //_rightSearchTarget = data.RightSearchTarget;
        //_isLeftGrab = data.isLeftGrab;
        //_isRightGrab = data.isRightGrab;
        _leftTargetDir = data.LeftSearchTargeDir;
        _rightTargetDir = data.RightSearchTargeDir;

        _leftHandRigid = bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = bodyHandler.RightHand.PartRigidbody;


        return true;
    }


    private void Gabbing()
    {
        if (_leftTargetDir != Vector3.zero)
        {
            //손 뻗기, action으로 추출
            if (!_isGrounded)
                dir = ((_leftTargetDir + Vector3.up * 2) - _leftHandRigid.transform.position).normalized;
            else
                dir = (_leftTargetDir - _leftHandRigid.transform.position).normalized;

            _leftHandRigid.AddForce(dir * 80f);
        }

        if (_rightTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                dir = ((_rightTargetDir + Vector3.up * 2) - _rightHandRigid.transform.position).normalized;
            else
                dir = (_rightTargetDir - _rightHandRigid.transform.position).normalized;

            _rightHandRigid.AddForce(dir * 80f);
        }
    }

}
