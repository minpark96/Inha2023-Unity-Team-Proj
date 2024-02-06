using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeReadyAction
{
    public ChargeReadyAction(ActionController actions)
    {
        actions.OnChargeReady -= HandleChargeEvent;
        actions.OnChargeReady += HandleChargeEvent;
    }

    AnimationPlayer _animPlayer;
    //BodyHandler _bodyHandler;
    AnimationData _animData;

    float _startChargeTime;
    float _endChargeTime = 0f;
    float _chargeAniHoldTime = 0.5f;

    ConfigurableJoint[] _childJoints;


    public bool HandleChargeEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in Define.PlayerDynamicData data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        //_bodyHandler = bodyhandler;
        _childJoints = bodyHandler.ChildJoints;

        CoroutineHelper.StartCoroutine(ChargeReady());

        return true;
    }


    IEnumerator ChargeReady()
    {
        for (int i = 0; i < _childJoints.Length; i++)
        {
            _childJoints[i].angularYMotion = ConfigurableJointMotion.Locked;
            _childJoints[i].angularZMotion = ConfigurableJointMotion.Locked;
        }

        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.RSkillAngleAniData].Length; i++)
        {
            _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.RSkillAngleAniData], i);
        }

        yield return (ForceRready(_chargeAniHoldTime));
        //yield return null;
    }

    IEnumerator ForceRready(float delay)
    {
        _startChargeTime = Time.time;
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData].Length; i++)
        {
            _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.RSkillAniData], i);
        }
        yield return new WaitForSeconds(delay);

        //물체의 모션을 고정
        Rigidbody _RPartRigidbody;
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData].Length; i++)
        {
            for (int j = 0; j < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].ActionRigidbodies[j];
                _RPartRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //키를 짧게 누르면 락 걸리는걸 방지 하기 위한 
                if (_endChargeTime - _startChargeTime > 0.0001f)
                {
                    _RPartRigidbody.constraints = RigidbodyConstraints.None;
                }
                _RPartRigidbody.velocity = Vector3.zero;
                _RPartRigidbody.angularVelocity = Vector3.zero;
            }
        }

        yield return null;
    }
}
