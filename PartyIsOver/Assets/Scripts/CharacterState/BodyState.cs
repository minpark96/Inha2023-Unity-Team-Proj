using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public abstract class BodyState:IBaseState
{
    public Define.PlayerState Name { get; set; }
    
    protected StateMachine stateMachine;

    public BodyState(PlayerState name, StateMachine stateMachine)
    {
        this.Name = name;
        this.stateMachine = stateMachine;
    }
    public virtual void GetInput()
    {

    }
    public virtual void Enter()
    {
    }

    public virtual void UpdateLogic()
    {
    }

    public virtual void UpdatePhysics()
    {
    }

    public virtual void Exit()
    {
    }

    protected bool IsMoveKeyInput()
    {
        if (stateMachine.ReserveInputCommand(COMMAND_KEY.Move, Define.GetKeyType.Press))
            return true;
        else
            return false;
    }

}