using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReady : BodyState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _skillActiveThreshold = 0.2f;

    public SkillReady(StateMachine stateMachine) : base("SkillReadyState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
        _sm.InputHandler.EnqueueCommand(Define.COMMAND_KEY.Charge);
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
                if (_sm.InputHandler.InputCommnadKey(KeyCode.R, Define.GetKeyType.Up))
                {
                    _sm.InputHandler.EnqueueCommand(Define.COMMAND_KEY.ResetCharge);
                    _sm.ChangeState(_sm.SkillState);
                    _sm.IsActionProgress = true;
                }
            }
            else
            {
                //Idle 상태로
                _sm.ChangeState(_sm.IdleState);
                _sm.InputHandler.EnqueueCommand(Define.COMMAND_KEY.ResetCharge);
            }
        }
    }

    public override void Exit()
    {
    }
}
