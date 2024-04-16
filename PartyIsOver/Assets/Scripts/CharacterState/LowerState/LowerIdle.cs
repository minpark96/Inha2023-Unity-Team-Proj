using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Define;

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
        if (IsMoveKeyInput())
            _sm.ChangeState(_sm.MovingState);

        if (InputCommand(COMMAND_KEY.Jump, KeyType.Down))
        {
            InvokeReserveCommand(COMMAND_KEY.Jump);
            _sm.ChangeState(_sm.JumpingState);
        }
    }
}
