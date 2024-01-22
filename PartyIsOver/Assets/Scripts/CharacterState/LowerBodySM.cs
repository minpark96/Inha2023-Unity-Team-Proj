using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

public class LowerBodySM : StateMachine
{
    public bool isGrounded=false;
    public bool isRun = false;




    //[HideInInspector]
    //public Moving movingState;
    [HideInInspector]
    public Jumping jumpingState;
    [HideInInspector]
    public LowerIdle idleState;
    [HideInInspector]
    public Moving movingState;

    private void Awake()
    {
        idleState = new LowerIdle(this);
        jumpingState = new Jumping(this);
        movingState = new Moving(this);
        inputHandler = GetComponent<PlayerInputHandler>();
    }


    protected override IBaseState GetInitialState()
    {
        return idleState;
    }
}
