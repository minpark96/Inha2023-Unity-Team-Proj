using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillReady : BaseState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _skillActiveThreshold = 0.2f;

    public SkillReady(StateMachine stateMachine) : base(Define.PlayerState.SkillReady, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
        _sm.InputHandler.ReserveCommand(COMMAND_KEY.Charge);
    }

    public override void UpdateLogic()
    {
        if (_sm.InputHandler.CheckInput(COMMAND_KEY.Skill,GetKeyType.Press))
        {
            _pressDuration += Time.deltaTime;
        }
        else
        {
            if (_pressDuration > _skillActiveThreshold)
            {
                //스킬 발동 상태로
                if (_sm.ReserveInputCommand(COMMAND_KEY.Skill, GetKeyType.Up))
                {
                    _sm.InputHandler.ReserveCommand(COMMAND_KEY.ResetCharge);
                    _sm.ChangeState(_sm.StateMap[PlayerState.Skill]);
                }
            }
            else
            {
                //Idle 상태로
                _sm.InputHandler.ReserveCommand(COMMAND_KEY.ResetCharge);
                _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
            }
        }
    }

    public override void Exit()
    {
    }
}
