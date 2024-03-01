using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PunchReady : BodyState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _punchGrabThreshold = 0.2f;

    public PunchReady(StateMachine stateMachine) : base(PlayerState.PunchAndGrabReady, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
    }

    public override void UpdateLogic()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _pressDuration += Time.deltaTime;

            if( _pressDuration > _punchGrabThreshold )
            {
                //그랩상태로
                _sm.ChangeState(_sm.StateMap[PlayerState.Grabbing]);
            }
        }
        else if (_pressDuration < _punchGrabThreshold)
        {
            //펀치
            if (_sm.InputHandler.InputCommnadKey(COMMAND_KEY.LeftBtn, GetKeyType.Up))
            {
                _sm.ChangeState(_sm.StateMap[PlayerState.Punch]);
            }
        }

        //행동트리나 리플레이에서 이 부분은 생략하고 바로 Punch나 Grabbing으로 넘어감
        //즉 PunchReady는 execute를 하지 않음
    }
    public override void GetInput()
    {
    }

    public override void Exit()
    {
    }
}
