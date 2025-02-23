using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 스킬 차지 상태
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
        // COMMAND_KEY.Skill를 누르고 있는 동안 차지 시간 증가
        if(InputCommand(COMMAND_KEY.Skill, KeyType.Press))
        {
            _pressDuration += Time.deltaTime;
        }
        else
        {
            // 일정 시간 차지 했느냐에 따른 분기처리
            if (_pressDuration > _skillActiveThreshold)
            {
                //스킬 발동 상태로
                if(InputCommand(COMMAND_KEY.Skill, KeyType.Up))
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
