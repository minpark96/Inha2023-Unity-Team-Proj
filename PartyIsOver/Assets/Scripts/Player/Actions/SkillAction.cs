using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (data.IsMeowPunch)
        {
            isRSkillCheck = true;
            isMeowPunch = true;
            CoroutineHelper.StartCoroutine(MeowNyangPunch());
        }
        else //����ġ ��ų
        {
            isRSkillCheck = true;
            isMeowPunch = false;
            CoroutineHelper.StartCoroutine(Punch(Define.Side.Right, duration, readyTime, punchTime, resetTime));
            context.IsUpperActionProgress = false;
        }
        Debug.Log("skillEvent");
        return true;
    }

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
