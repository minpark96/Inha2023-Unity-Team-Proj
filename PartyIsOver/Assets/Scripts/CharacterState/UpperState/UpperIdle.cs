using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UpperIdle : BaseState
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

        if(Input.GetButtonDown(COMMAND_KEY.LeftBtn.ToString()))
            _sm.ChangeState(_sm.StateMap[PlayerState.PunchAndGrabReady]);

        if (Input.GetButtonDown(COMMAND_KEY.Skill.ToString()))
            _sm.ChangeState(_sm.StateMap[PlayerState.SkillReady]);

        if (Input.GetButtonDown(COMMAND_KEY.HeadButt.ToString()))
        {
            InvokeReserveCommand(COMMAND_KEY.HeadButt);
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
