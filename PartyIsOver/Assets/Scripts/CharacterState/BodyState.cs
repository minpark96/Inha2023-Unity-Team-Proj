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
        KeyCode[] keyCodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (stateMachine.InputHandler.InputCommnadKey(keyCodes[i], Define.GetKeyType.Press))
                return true;
        }
        return false;
    }

}