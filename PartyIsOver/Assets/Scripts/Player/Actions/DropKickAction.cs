using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKickAction:BaseAction
{
    public DropKickAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
        this._actions = actions;
    }

    ActionController _actions;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;
    AnimationData _animData;
    PlayerActionContext _context;
    float _DropKickCoolTime = 2f;
    float _springLerpTime =1f;


    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _bodyHandler = bodyHandler;
        _context = data;

        if (data.EquipItem != null && data.IsGrounded)
            return false;
        CoroutineHelper.StartCoroutine(DropKick());
        _context.IsUpperActionProgress = true;


        return true;
    }

    IEnumerator DropKick()
    {
        Transform partTransform = _bodyHandler.Hip.transform;

        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.DropAniData].Length; i++)
        {
            _bodyHandler.StartCoroutine("ResetBodySpring");

            if (i == 0)
            {
                Transform transform2 = _bodyHandler.RightFoot.transform;
                //_bodyHandler.RightFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                //_bodyHandler.RightThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i, dir);
                _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerR, true);
            }
            else if (i == 1)
            {
                Transform transform2 = _bodyHandler.LeftFoot.transform;
                //_bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                //_bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i, dir);
                _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerL, true);
            }
            else
            {
                _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i);
            }
        }

        yield return new WaitForSeconds(_DropKickCoolTime);
        _bodyHandler.StartCoroutine("RestoreBodySpring", _springLerpTime);
        _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerR, false);
        _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerL, false);

        _context.IsUpperActionProgress = false;
        _context.IsLowerActionProgress = false;
    }
}
