using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbing : BodyState
{
    private UpperBodySM _sm;


    public Grabbing(StateMachine stateMachine) : base("GrabbingState", stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
    }

    public override void UpdateLogic()
    {
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
