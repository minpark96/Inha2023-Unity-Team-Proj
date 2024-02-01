using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : BodyState
{
    private UpperBodySM _sm;

    public Punch(StateMachine stateMachine) : base("PunchState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        //데미지 속성을 여기서 바꿔야 하는지 고민해야함
        //사운드, 이펙트를 여기서 관리해야 하는지 고민해야함
        //_sm에게 공격타입을 알려야함 그리고 DynamicData가 해당 타입을 저장
    }

    public override void UpdateLogic()
    {
        //action 코루틴에서 펀치 액션이 끝나면 _sm에게 이벤트로 액션이 끝났음을 알려야함
        //_sm의 attacking이 false가 되면 Idle로 돌림
        if(!_sm.IsAttacking)
        {
            _sm.ChangeState(_sm.IdleState);
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
        if (_sm.ReadySide == Define.Side.Left)
            _sm.ReadySide = Define.Side.Right;
        else
            _sm.ReadySide = Define.Side.Left;
    }
}
