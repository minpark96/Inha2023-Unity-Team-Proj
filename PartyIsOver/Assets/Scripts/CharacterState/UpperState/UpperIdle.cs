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

        if (_sm.InputHandler.CheckInput(COMMAND_KEY.LeftBtn, GetKeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.PunchAndGrabReady]);

        if (_sm.InputHandler.CheckInput(COMMAND_KEY.Skill, GetKeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.SkillReady]);

        if (_sm.ReserveInputCommand(COMMAND_KEY.HeadButt, GetKeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.HeadButt]);
        
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
