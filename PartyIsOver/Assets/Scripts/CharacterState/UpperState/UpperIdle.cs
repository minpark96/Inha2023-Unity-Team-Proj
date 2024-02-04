using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperIdle : BodyState
{
    private UpperBodySM _sm;

    public UpperIdle(StateMachine stateMachine) : base("UpperIdleState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
    }

    public override void UpdateLogic()
    {
        //if(_sm.InputHandler.InputGetDownKey(KeyCode.Mouse0, Define.GetKeyType.Down))
        //{
        //    _sm.ChangeState(_sm.PunchReadyState);
        //}
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _sm.ChangeState(_sm.PunchReadyState);
        }
    }
    public override void GetInput()
    {
    }


    public override void UpdatePhysics()
    {

    }

    public override void Exit()
    {
    }
}
