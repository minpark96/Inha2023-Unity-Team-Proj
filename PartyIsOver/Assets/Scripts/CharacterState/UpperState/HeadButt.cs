using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeadButt : BodyState
{
    protected UpperBodySM _sm;

    public HeadButt(StateMachine stateMachine) : base(PlayerState.HeadButt, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.Context.IsUpperActionProgress = true;
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
