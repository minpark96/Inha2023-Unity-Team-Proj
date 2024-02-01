using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAction
{
    public PunchAction(ActionController actions)
    {
        actions.OnPunch -= InvokePunchEvent;
        actions.OnPunch += InvokePunchEvent;
    }

    AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;

    float _duration;
    float _readyTime;
    float _punchTime;
    float _resetTime;

    bool _isRSkillCheck;
    bool _isMeowNyangPunch;

    float _meowPunchPower;
    float _nuclearPunchPower;

    void InvokePunchEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler,in Define.PlayerDynamicData data)
    {
        CoroutineHelper.StartCoroutine(Punch(data.side,_duration,_readyTime,_punchTime,_resetTime));
        _animData = animData;
        _animPlayer = animPlayer;
        _bodyHandler = bodyHandler;
    }

    public IEnumerator Punch(Define.Side side, float duration, float readyTime, float punchTime, float resetTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ArmActionReadying(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ArmActionPunching(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ArmActionPunchResetting(side);
            yield return new WaitForSeconds(duration);
        }
    }
    
    public void ArmActionReadying(Define.Side side)
    {
        AniAngleData[] aniAngleDatas = (side == Define.Side.Right) ? _animData.AngleDataLists[Define.AniAngleData.RightPunchAniData.ToString()] : _animData.AngleDataLists[Define.AniAngleData.LeftPunchAniData.ToString()];
        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            _animPlayer.AniAngleForce(aniAngleDatas, i);
        }
    }

    public void ArmActionPunching(Define.Side side)
    {

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] aniFrameDatas;
        Transform transform2;

        if (side == Define.Side.Left)
        {
            aniFrameDatas = _animData.FrameDataLists[Define.AniFrameData.LeftPunchingAniData.ToString()];
            transform2 = _bodyHandler.LeftHand.transform;
            if (_isRSkillCheck)
            {
                if (_isMeowNyangPunch)
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, true);
        }
        else
        {
            aniFrameDatas = _animData.FrameDataLists[Define.AniFrameData.RightPunchingAniData.ToString()];
            transform2 = _bodyHandler.RightHand.transform;
            if (_isRSkillCheck)
            {
                if (_isMeowNyangPunch)
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, true);
        }

        for (int i = 0; i < aniFrameDatas.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);

            if (_isRSkillCheck)
            {
                if (_isMeowNyangPunch)
                    _animPlayer.AniForce(aniFrameDatas, i, dir, _meowPunchPower);
                else
                    _animPlayer.AniForce(aniFrameDatas, i, dir, _nuclearPunchPower);
            }
            else
                _animPlayer.AniForce(aniFrameDatas, i, dir);
        }
    }

    public void ArmActionPunchResetting(Define.Side side)
    {
        Transform partTransform = _bodyHandler.Chest.transform;

        AniAngleData[] aniAngleDatas = _animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()];

        if (side == Define.Side.Left)
        {
            _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, false);
        }
        else
        {
            aniAngleDatas = _animData.AngleDataLists[Define.AniAngleData.RightPunchResettingAniData.ToString()];
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, false);
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()], i, dir);
        }
    }
}
