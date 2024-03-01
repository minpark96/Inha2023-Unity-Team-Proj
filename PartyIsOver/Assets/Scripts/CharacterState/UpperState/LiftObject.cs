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
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.FixJoint);
    }

    public override void UpdateLogic()
    {
    }

    public override void GetInput()
    {
        if (!_sm.InputHandler.InputCommnadKey(COMMAND_KEY.LeftBtn, GetKeyType.Press))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);

        if (_sm.InputHandler.InputCommnadKey(COMMAND_KEY.RightBtn, GetKeyType.Up))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
    }

    public override void Exit()
    {
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.DestroyJoint);
    }
}
