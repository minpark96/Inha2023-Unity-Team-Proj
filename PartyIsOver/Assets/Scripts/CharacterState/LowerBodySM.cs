using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class LowerBodySM : StateMachine
{
    public PlayerInputHandler inputHandler;
    public bool isGrounded=false;

    //[HideInInspector]
    //public Moving movingState;
    [HideInInspector]
    public Jumping jumpingState;
    [HideInInspector]
    public LowerIdle idleState;

    private void Awake()
    {
        idleState = new LowerIdle(this);
        jumpingState = new Jumping(this);
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    protected override IBaseState GetInitialState()
    {
        return idleState;
    }
}
