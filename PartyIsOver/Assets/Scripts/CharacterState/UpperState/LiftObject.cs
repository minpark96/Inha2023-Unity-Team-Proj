using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class LiftObject : BodyState
{
    private UpperBodySM _sm;

    public LiftObject(StateMachine stateMachine) : base(PlayerState.LiftObject, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _sm.InputHandler.ReserveCommand(COMMAND_KEY.FixJoint);
    }

    public override void UpdateLogic()
    {
    }

    public override void GetInput()
    {
        if (!_sm.ReserveInputCommand(COMMAND_KEY.LeftBtn, GetKeyType.Press))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);

        if (_sm.ReserveInputCommand(COMMAND_KEY.RightBtn, GetKeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
    }

    public override void Exit()
    {
        _sm.InputHandler.ReserveCommand(COMMAND_KEY.DestroyJoint);
    }
}
