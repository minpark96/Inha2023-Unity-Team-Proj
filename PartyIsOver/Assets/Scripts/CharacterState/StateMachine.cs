using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IBaseState _currentState; //나중에 OnGUI 필요없을때 private으로
    public PlayerInputHandler InputHandler;

    public StateMachine()
    {
    }

    protected void Init()
    {
        _currentState = GetInitialState();
        if (_currentState != null)
            _currentState.Enter();
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


    protected virtual IBaseState GetInitialState()
    {
        return null;
    }

    public void ChangeState(IBaseState newState)
    {
        _currentState.Exit();

        _currentState = newState;
        newState.Enter();
    }

    public IBaseState GetCurrentState()
    {
        return _currentState;
    }

}
