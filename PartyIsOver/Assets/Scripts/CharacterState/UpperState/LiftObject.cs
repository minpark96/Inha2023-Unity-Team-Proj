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
        if(!Input.GetButton(COMMAND_KEY.LeftBtn.ToString()))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        
        if(Input.GetButtonDown(COMMAND_KEY.RightBtn.ToString()))
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
