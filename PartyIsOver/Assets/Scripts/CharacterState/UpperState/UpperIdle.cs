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

        if(InputCommand(COMMAND_KEY.LeftBtn, KeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.PunchAndGrabReady]);

        if (InputCommand(COMMAND_KEY.Skill, KeyType.Down))
            _sm.ChangeState(_sm.StateMap[PlayerState.SkillReady]);

        if (InputCommand(COMMAND_KEY.HeadButt, KeyType.Down))
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
