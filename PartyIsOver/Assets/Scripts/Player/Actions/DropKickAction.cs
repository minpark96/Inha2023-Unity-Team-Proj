using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKickAction
{
    public DropKickAction(ActionController actions)
    {
        actions.OnDropKick -= HandleDropKickEvent;
        actions.OnDropKick += HandleDropKickEvent;
        this._actions = actions;
    }

    ActionController _actions;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;
    AnimationData _animData;
    float _DropKickCoolTime = 2f;
    float _springLerpTime =1f;


    public bool HandleDropKickEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _bodyHandler = bodyHandler;

        if (data.EquipItem != null && data.IsGrounded)
            return false;
        CoroutineHelper.StartCoroutine(DropKick());
        _actions.UpperActionStart();

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
                _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //������
                Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i, dir);
                //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerR, true);
            }
            else if (i == 1)
            {
                Transform transform2 = _bodyHandler.LeftFoot.transform;
                //_bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                //_bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //������
                Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i, dir);
                //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerL, true);
            }
            else
            {
                _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.DropAniData], i);
            }
        }

        yield return new WaitForSeconds(_DropKickCoolTime);
        _bodyHandler.StartCoroutine("RestoreBodySpring", _springLerpTime);
        //_bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        //_bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerL, false);
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerR, false);

        _actions.UpperActionEnd();
        _actions.LowerActionEnd();
    }
}