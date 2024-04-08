using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class StateMachine
{
    private BodyState _currentState; //나중에 OnGUI 필요없을때 private으로
    public PlayerInputHandler InputHandler;

    protected void Init()
    {
        _currentState = GetInitialState();
        if (_currentState != null)
            _currentState.Enter();
    }
    public void ChangeState(BodyState newState)
    {
        _currentState.Exit();

        _currentState = newState;
        newState.Enter();
    }
    public void UpdateLogic()
    {
        if (_currentState != null)
        {
            _currentState.UpdateLogic();

            if (Input.anyKey)
                _currentState.GetInput();
        }
    }
    public void UpdatePhysics()
    {
        if (_currentState != null)
            _currentState.UpdatePhysics();
    }


    protected virtual BodyState GetInitialState()
    {
        return null;
    }
    public BodyState GetCurrentState()
    {
        return _currentState;
    }

    public bool ReserveInputCommand(COMMAND_KEY commandKey, GetKeyType keyType)
    {
        if (InputHandler.CheckInput(commandKey, keyType))
        {
            InputHandler.ReserveCommand(commandKey);
            return true;
        }
        else return false;
    }
}
