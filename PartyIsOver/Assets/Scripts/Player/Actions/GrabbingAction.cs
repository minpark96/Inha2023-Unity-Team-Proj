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
    }

    Rigidbody _leftHandRigid;
    Rigidbody _rightHandRigid;

    bool _isGrounded;
    Vector3 _dir;
    Vector3 _leftTargetDir;
    Vector3 _rightTargetDir;

    public bool HandleGrabbingEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _isGrounded = data.IsGrounded;

        _leftTargetDir = data.LeftTargetDir;
        _rightTargetDir = data.RightTargetDir;
        _leftHandRigid = bodyHandler.LeftHand.PartRigidbody;
        _rightHandRigid = bodyHandler.RightHand.PartRigidbody;

        Gabbing();
        return true;
    }


    private void Gabbing()
    {
        if (_leftTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _dir = ((_leftTargetDir + Vector3.up * 2) - _leftHandRigid.transform.position).normalized;
            else
                _dir = (_leftTargetDir - _leftHandRigid.transform.position).normalized;

            _leftHandRigid.AddForce(_dir * 80f);
        }

        if (_rightTargetDir != Vector3.zero)
        {
            if (!_isGrounded)
                _dir = ((_rightTargetDir + Vector3.up * 2) - _rightHandRigid.transform.position).normalized;
            else
                _dir = (_rightTargetDir - _rightHandRigid.transform.position).normalized;

            _rightHandRigid.AddForce(_dir * 80f);
        }
    }

}
