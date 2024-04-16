using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class LiftObject : BaseState
{
    private UpperBodySM _sm;

    public LiftObject(StateMachine stateMachine) : base(PlayerState.LiftObject, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        InvokeReserveCommand(COMMAND_KEY.FixJoint);
    }

    public override void UpdateLogic()
    {
        InvokeReserveCommand(COMMAND_KEY.LeftBtn);
    }

    public override void GetInput()
    {
        if(!InputCommand(COMMAND_KEY.LeftBtn, KeyType.Press))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        
        if(InputCommand(COMMAND_KEY.RightBtn, KeyType.Down))
        {
            InvokeReserveCommand(COMMAND_KEY.RightBtn);
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        }
    }

    public override void Exit()
    {
        InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
    }
}
