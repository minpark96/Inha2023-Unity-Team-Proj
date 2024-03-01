using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LowerIdle : BodyState
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
        {
            _sm.ChangeState(_sm.MovingState);
        }
        if (_sm.InputHandler.InputCommnadKey(Define.COMMAND_KEY.Jump, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.JumpingState);
        }
    }
}
