using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReady : BodyState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _SkillActiveThreshold = 0.2f;

    public SkillReady(StateMachine stateMachine) : base("SkillReadyState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
    }

    public override void UpdateLogic()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _pressDuration += Time.deltaTime;

            if (_pressDuration > _SkillActiveThreshold)
            {
                //Idle 상태로
                _sm.ChangeState(_sm.IdleState);
            }
        }
        else if (_pressDuration < _SkillActiveThreshold)
        {
            //스킬 발동 상태로
            if (_sm.InputHandler.InputGetDownKey(KeyCode.R, Define.GetKeyType.Up))
            {
                _sm.ChangeState(_sm.SkillState);
                _sm.IsActionProgress = true;
            }
        }
    }

    public override void Exit()
    {
        //신체고정 해제
    }
}
