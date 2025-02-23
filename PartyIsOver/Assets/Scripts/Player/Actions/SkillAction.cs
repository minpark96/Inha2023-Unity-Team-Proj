using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬 사용 액션
// 펀치 액션을 상속받아 사용한다.
// 핵펀치 스킬은 펀치 함수에 매개변수를 다르게 줘서 구현하고
// 냥냥펀치는 이 클래스에서 구현
public class SkillAction : PunchAction
{
    public SkillAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    Define.Side _readySide;
    int _punchCount = 5;


    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        base.animData = animData;
        base.animPlayer = animPlayer;
        base.bodyHandler = bodyHandler;
        base.context = data;
        isMeowPunch = data.IsMeowPunch;
        isRSkillCheck = true;

        if (data.IsMeowPunch) // 냥냥펀치
        {
            isRSkillCheck = true;
            isMeowPunch = true;
            CoroutineHelper.StartCoroutine(MeowNyangPunch());
        }
        else // 핵펀치 스킬
        {
            isRSkillCheck = true;
            isMeowPunch = false;
            CoroutineHelper.StartCoroutine(Punch(Define.Side.Right, duration, readyTime, punchTime, resetTime));
            context.IsUpperActionProgress = false;
        }
        Debug.Log("skillEvent");
        return true;
    }

    // 냥냥펀치 스킬, 상속한 Punch 함수를 짧게 끊어서 여러번 실행하는 방식으로 구현
    IEnumerator MeowNyangPunch()
    {
        _readySide = Define.Side.Right;

        for (int i = 0; i < _punchCount; i++)
        {
            yield return Punch(_readySide, duration, readyTime, punchTime, resetTime);

            if (_readySide == Define.Side.Left)
                _readySide = Define.Side.Right;
            else
                _readySide = Define.Side.Left;
        }
        context.IsUpperActionProgress = false;
    }
}
