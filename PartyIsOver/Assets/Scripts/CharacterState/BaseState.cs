using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public abstract class BaseState
{
    public Define.PlayerState Name { get; set; }

    protected StateMachine stateMachine;

    public BaseState(PlayerState name, StateMachine stateMachine)
    {
        this.Name = name;
        this.stateMachine = stateMachine;
    }
    public virtual void GetInput(){}
    public virtual void Enter(){}
    public virtual void UpdateLogic(){}
    public virtual void UpdatePhysics(){}
    public virtual void Exit(){}


    protected void InvokeReserveCommand(COMMAND_KEY cmd)
    {
        stateMachine.CommandReserveHandler.Invoke(cmd);
    }

    protected bool IsMoveKeyInput()
    {
        return Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f;
    }

    protected bool InputCommand(COMMAND_KEY key, KeyType type)
    {
        switch(type)
        {
            case KeyType.Up:
                return Input.GetButtonUp(key.ToString());
            case KeyType.Down: 
                return Input.GetButtonDown(key.ToString());
            case KeyType.Press:
                return Input.GetButton(key.ToString());

            default: return false;
        }
    }
}