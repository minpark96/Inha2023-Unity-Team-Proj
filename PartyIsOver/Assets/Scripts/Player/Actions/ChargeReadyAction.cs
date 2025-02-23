using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬 차지 액션
// 팔을 뒤로 젖힌 상태로 모션을 고정
public class ChargeReadyAction : BaseAction
{
    public ChargeReadyAction(ActionController actions,Define.ActionEventName name):base(actions,name)
    {
    }

    AnimationPlayer _animPlayer;
    AnimationData _animData;
    float _startChargeTime;
    float _endChargeTime = 0f;
    float _chargeAniHoldTime = 0.5f;

    ConfigurableJoint[] _childJoints;

    // 액션이 실행되면 실행되는 함수 (ChargeReady 코루틴을 실행)
    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _childJoints = bodyHandler.ChildJoints;

        CoroutineHelper.StartCoroutine(ChargeReady());

        return true;
    }


    IEnumerator ChargeReady()
    {
        // 차지 모션을 구현하기 위해 관절의 Y,Z 모션에 Lock을 걸어 고정
        for (int i = 0; i < _childJoints.Length; i++)
        {
            _childJoints[i].angularYMotion = ConfigurableJointMotion.Locked;
            _childJoints[i].angularZMotion = ConfigurableJointMotion.Locked;
        }
        // 이하는 관절의 X 모션에만 힘을 줘서 팔을 뒤로 젖히는 효과를 줌
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.RSkillAngleAniData].Length; i++)
            _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.RSkillAngleAniData], i);


        yield return (ForceRready(_chargeAniHoldTime));
    }

    IEnumerator ForceRready(float delay)
    {
        _startChargeTime = Time.time;
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData].Length; i++)
        {
            _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.RSkillAniData], i);
        }
        yield return new WaitForSeconds(delay);

        // 팔을 뒤로 젖힌 상태로 모션을 고정
        Rigidbody _RPartRigidbody;
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData].Length; i++)
        {
            for (int j = 0; j < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].ActionRigidbodies[j];
                _RPartRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                // 키를 짧게 누르면 락 걸리는걸 방지 하기 위함
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
