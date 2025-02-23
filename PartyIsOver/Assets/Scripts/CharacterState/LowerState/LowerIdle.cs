using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Define;

// 하체의 기본 상태
public class LowerIdle : BaseState
{
    private LowerBodySM _sm;

    public LowerIdle(StateMachine stateMachine) : base(Define.PlayerState.LowerIdle, stateMachine)
    {
        _sm = (LowerBodySM)stateMachine;
    }

    public override void UpdateLogic()
    {
    }

    public override void GetInput()
    {
        // 이동 키 입력이 들어오면 이동 상태로 변경
        if (IsMoveKeyInput())
            _sm.ChangeState(_sm.MovingState);
        
        // 점프 키 입력이 들어오면 점프 상태로 변경
        if (InputCommand(COMMAND_KEY.Jump, KeyType.Down))
        {
            InvokeReserveCommand(COMMAND_KEY.Jump);
            _sm.ChangeState(_sm.JumpingState);
        }
    }
}
