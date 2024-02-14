using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChargeAction
{
    public ResetChargeAction(ActionController actions)
    {
        actions.OnResetCharge -= HandleResetChargeEvent;
        actions.OnResetCharge += HandleResetChargeEvent;
    }

    AnimationPlayer _animPlayer;
    //BodyHandler _bodyHandler;
    AnimationData _animData;

    float _endChargeTime = 0f;
    int _checkHoldTimeCount = 0;

    private ConfigurableJoint[] _childJoints;
    private ConfigurableJointMotion[] _originalYMotions;
    private ConfigurableJointMotion[] _originalZMotions;

    public bool HandleResetChargeEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        //_bodyHandler = bodyhandler;

        _childJoints = bodyHandler.ChildJoints;
        _originalYMotions = bodyHandler.OriginalYMotions;
        _originalZMotions = bodyHandler.OriginalZMotions;

        CoroutineHelper.StartCoroutine(ResetCharge());

        return true;
    }

    [PunRPC]
    IEnumerator ResetCharge()
    {
        _checkHoldTimeCount = 0;
        _endChargeTime = Time.time;
        Rigidbody _RPartRigidbody;

        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData].Length; i++)
        {
            for (int j = 0; j < _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = _animData.FrameDataLists[Define.AniFrameData.RSkillAniData][i].ActionRigidbodies[j];
                //Debug.Log("Freeze풀기 : "+ _RPartRigidbody);
                _RPartRigidbody.constraints = RigidbodyConstraints.None;
                _RPartRigidbody.velocity = Vector3.zero;
                _RPartRigidbody.angularVelocity = Vector3.zero;
            }
        }
        RestoreOriginalMotions();
        yield return null;
    }

    [PunRPC]
    void RestoreOriginalMotions()
    {
        //y z 초기값 대입
        for (int i = 0; i < _childJoints.Length; i++)
        {
            _childJoints[i].angularYMotion = _originalYMotions[i];
            _childJoints[i].angularZMotion = _originalZMotions[i];
        }
    }
}
