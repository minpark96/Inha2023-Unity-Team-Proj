using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LowerIdle : BodyState
{
    private LowerBodySM _sm;

    public LowerIdle(StateMachine stateMachine) : base("LowerIdleState", stateMachine)
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
            _sm.ChangeState(_sm.movingState);
        }
        if (_sm.InputHandler.InputGetDownKey(KeyCode.Space, Define.GetKeyType.Down))
        {
            _sm.ChangeState(_sm.jumpingState);
        }
    }
}
