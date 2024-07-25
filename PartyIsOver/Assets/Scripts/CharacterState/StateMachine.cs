using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/*
 * ����ü ���¸ӽ��� ���̽��� �Ǵ� �߻�Ŭ����
 */
public abstract class StateMachine
{
    private BaseState _currentState;

    public delegate void CommandDelegate(COMMAND_KEY commandKey);
    public CommandDelegate CommandReserveHandler;

    protected void Init()
    {
        _currentState = GetInitialState();
        if (_currentState != null)
            _currentState.Enter();
    }
    public void ChangeState(BaseState newState)
    {
        _currentState.Exit();
        _currentState = newState;
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


    protected virtual BaseState GetInitialState()
    {
        return null;
    }
    public BaseState GetCurrentState()
    {
        return _currentState;
    }
}
