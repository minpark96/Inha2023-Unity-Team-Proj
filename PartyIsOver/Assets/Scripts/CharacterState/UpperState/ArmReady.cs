using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 펀치나 잡기등이 나가기 전의 준비동작을 담당하는 상태
// 입력에 따라 펀치나 잡기 상태로 변경한다.
public class ArmReady : BaseState
{
    private UpperBodySM _sm;
    private float _pressDuration;
    private float _punchGrabThreshold = 0.2f;

    public ArmReady(StateMachine stateMachine) : base(PlayerState.PunchAndGrabReady, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        _pressDuration = 0f;
    }

    public override void UpdateLogic()
    {
        // 왼쪽 버튼을 누르고 있는 동안 차지 시간 증가
        if (InputCommand(COMMAND_KEY.LeftBtn, KeyType.Press))
        {
            _pressDuration += Time.deltaTime;
            
            // 일정 시간 차지 했으면 잡기로 변경
            if (_pressDuration > _punchGrabThreshold)
            {
                // 그랩상태로 변경
                _sm.ChangeState(_sm.StateMap[PlayerState.Grabbing]);
            }
        }
        else if (_pressDuration < _punchGrabThreshold) // 일정 시간 내에 왼쪽 버튼을 떼면 펀치
        {
            // 펀치 상태로 변경
            if (InputCommand(COMMAND_KEY.LeftBtn, KeyType.Up))
            {
                // LeftBtn에 해당하는 커맨드(펀치)를 예약
                InvokeReserveCommand(COMMAND_KEY.LeftBtn);
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
