using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButt : BodyState
{
    protected UpperBodySM _sm;

    public HeadButt(StateMachine stateMachine) : base("HeadButtState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }
    public override void Enter()
    {
        _sm.IsUpperActionProgress = true;
    }
    public override void UpdateLogic()
    {
        if (!_sm.IsUpperActionProgress)
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
    }
}
