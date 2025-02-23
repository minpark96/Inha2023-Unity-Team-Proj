using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 드롭킥 액션
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
        
        // 아이템이 장착중이거나 땅에 있을 경우 동작을 진행하지 않음
        if (data.EquipItem != null || data.IsGrounded)
            return false;
        
        // 동작 코루틴 실행
        CoroutineHelper.StartCoroutine(DropKick());
        
        // 동작을 진행하는 동안 다른 상태로 변경되지 않도록 처리, 코루틴이 다 끝나면 false로 변경
        _context.IsUpperActionProgress = true;


        return true;
    }

    IEnumerator DropKick()
    {
        Transform partTransform = _bodyHandler.Hip.transform;

        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.DropAniData].Length; i++)
        {
            // 플레이어의 발이 앞으로 날아갈 수 있도록 관절의 스프링을 0으로 하여 몸에 힘을 뺌 
            _bodyHandler.StartCoroutine("ResetBodySpring");

            // 애니메이션 데이터에 따라 발 등에다 정면방향으로 힘을 줌과 동시에 양쪽 발의 데미지 타입을 isAttack으로 변경
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
        
        // 드롭킥 후에 캐릭터가 넘어져 있다가 약간의 시간이 지나면 관절 Spring을 회복하여 서서히 일어남
        yield return new WaitForSeconds(_DropKickCoolTime);
        _bodyHandler.StartCoroutine("RestoreBodySpring", _springLerpTime);
        
        // 다리의 데미지 타입을 false로 변경하여 다리의 충돌이 공격으로 적용되지 않도록 변경
        _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerR, false);
        _bodyHandler.ChangeDamageModifier(Define.BodyPart.LegLowerL, false);
        
        //동작을 종료
        _context.IsUpperActionProgress = false;
        _context.IsLowerActionProgress = false;
    }
}
