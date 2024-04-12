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
        InvokeReserveCommand(COMMAND_KEY.Charge);
    }

    public override void UpdateLogic()
    {
        if(Input.GetButton(COMMAND_KEY.Skill.ToString()))
        {
            _pressDuration += Time.deltaTime;
        }
        else
        {
            if (_pressDuration > _skillActiveThreshold)
            {
                //스킬 발동 상태로
                if(Input.GetButtonUp(COMMAND_KEY.Skill.ToString()))
                {
                    InvokeReserveCommand(COMMAND_KEY.Skill);
                    _sm.ChangeState(_sm.StateMap[PlayerState.Skill]);
                }
            }
            else
            {
                //Idle 상태로
                _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
            }
        }
    }

    public override void Exit()
    {
        InvokeReserveCommand(COMMAND_KEY.ResetCharge);
    }
}
