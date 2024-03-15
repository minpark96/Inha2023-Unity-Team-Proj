using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChargeAction : PlayerAction
{
    public ResetChargeAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    AnimationData _animData;

    private ConfigurableJoint[] _childJoints;
    private ConfigurableJointMotion[] _originalYMotions;
    private ConfigurableJointMotion[] _originalZMotions;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _animData = animData;

        _childJoints = bodyHandler.ChildJoints;
        _originalYMotions = bodyHandler.OriginalYMotions;
        _originalZMotions = bodyHandler.OriginalZMotions;

        CoroutineHelper.StartCoroutine(ResetCharge());

        return true;
    }

    [PunRPC]
    IEnumerator ResetCharge()
    {
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
