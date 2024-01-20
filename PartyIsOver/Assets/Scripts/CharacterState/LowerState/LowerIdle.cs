using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LowerIdle : BodyState
{
    protected LowerBodySM sm;

    public LowerIdle(StateMachine stateMachine) : base("LowerIdleState", stateMachine)
    {
        sm = (LowerBodySM)stateMachine;
    }

    public override void UpdateLogic()
    {
     
    }

    public override void GetInput()
    {
        if (sm.inputHandler.InputGetDownKey(KeyCode.Space, Define.GetKeyType.Down))
        {
            sm.ChangeState(sm.jumpingState);
        }
    }
}
