using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MeowNyangPunch : BodyState
{
    protected UpperBodySM _sm;

    public MeowNyangPunch(StateMachine stateMachine) : base(PlayerState.Skill, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.Context.IsUpperActionProgress = true;
        //데미지 속성을 여기서 바꿔야 하는지 고민해야함
        //사운드, 이펙트를 여기서 관리해야 하는지 고민해야함
        //_sm에게 공격타입을 알려야함 그리고 DynamicData가 해당 타입을 저장
    }
    public override void UpdateLogic()
    {
        if (!_sm.Context.IsUpperActionProgress)
        {
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }
    public override void GetInput()
    {
    }

    public override void UpdatePhysics()
    {
    }

    public override void Exit()
    {
    }
}
