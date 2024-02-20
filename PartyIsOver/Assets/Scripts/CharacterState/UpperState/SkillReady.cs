using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillReady : BodyState
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
        _sm.InputHandler.EnqueueCommand(COMMAND_KEY.Charge);
    }

    public override void UpdateLogic()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _pressDuration += Time.deltaTime;
        }
        else
        {
            if (_pressDuration > _skillActiveThreshold)
            {
                //스킬 발동 상태로
                if (_sm.InputHandler.InputCommnadKey(KeyCode.R, GetKeyType.Up))
                {
                    _sm.InputHandler.EnqueueCommand(COMMAND_KEY.ResetCharge);
                    _sm.ChangeState(_sm.StateMap[PlayerState.Skill]);
                }
            }
            else
            {
                //Idle 상태로
                _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
                _sm.InputHandler.EnqueueCommand(COMMAND_KEY.ResetCharge);
            }
        }
    }

    public override void Exit()
    {
    }
}
