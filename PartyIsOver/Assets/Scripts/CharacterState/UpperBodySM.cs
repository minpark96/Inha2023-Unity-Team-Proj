using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class UpperBodySM : StateMachine
{
    public UpperBodySM(PlayerInputHandler inputHandler)
    {

        base.InputHandler = inputHandler;
        base.Init();
    }
}
