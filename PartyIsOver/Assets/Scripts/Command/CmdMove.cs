using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdMove : PlayerCommand
{
    bool isInAir = false;

    public CmdMove(Actor actor)
    {
        bodyHandler = actor.BodyHandler;
       
    }

    public override void Execute(Vector3 moveDir = default)
    {
        
    }
}
