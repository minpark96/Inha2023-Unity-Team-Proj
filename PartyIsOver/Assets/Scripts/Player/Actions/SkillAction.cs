using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : PunchAction
{
    public SkillAction(ActionController actions):base(actions)
    {
    }

    Define.Side _readySide;
    int _punchCount = 5;

    protected override void Init()
    {
        actions.OnSkill -= InvokeSkillEvent;
        actions.OnSkill += InvokeSkillEvent;
    }

    bool InvokeSkillEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in Define.PlayerDynamicData data)
    {
        base.animData = animData;
        base.animPlayer = animPlayer;
        base.bodyHandler = bodyHandler;

        if(data.isMeowPunch)
        {
            isRSkillCheck = true;
            isMeowNyangPunch = true;
            CoroutineHelper.StartCoroutine(MeowNyangPunch());
        }
        else //ÇÙÆÝÄ¡ ½ºÅ³
        {
            isRSkillCheck = true;
            isMeowNyangPunch = false;
            CoroutineHelper.StartCoroutine(Punch(data.side, duration, readyTime, punchTime, resetTime));
        }

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
        actions.UpperActionEnd();
    }
}
