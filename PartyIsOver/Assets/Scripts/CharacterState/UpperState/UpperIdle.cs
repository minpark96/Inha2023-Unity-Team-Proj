using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UpperIdle : BodyState
{
    private UpperBodySM _sm;

    public UpperIdle(StateMachine stateMachine) : base(PlayerState.UpperIdle, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
    }

    public override void UpdateLogic()
    {
        if (_sm.Context.IsUpperActionProgress)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _sm.ChangeState(_sm.StateMap[PlayerState.PunchAndGrabReady]);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _sm.ChangeState(_sm.StateMap[PlayerState.SkillReady]);
        }
        if (_sm.InputHandler.InputCommnadKey(COMMAND_KEY.HeadButt, GetKeyType.Down))
        {
            _sm.ChangeState(_sm.StateMap[PlayerState.HeadButt]);
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
