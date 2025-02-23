using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/*
 * 상하체 상태머신의 베이스가 되는 추상클래스
 */
public abstract class StateMachine
{
    private BaseState _currentState;

    // 커맨드 델리게이트 선언
    public delegate void CommandDelegate(COMMAND_KEY commandKey);
    public CommandDelegate CommandReserveHandler;

    // 초기 State를 받아서 해당 State의 Enter함수 실행
    protected void Init()
    {
        _currentState = GetInitialState();
        if (_currentState != null)
            _currentState.Enter();
    }
    
    // 현재 상태 변경
    public void ChangeState(BaseState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    
    // 현재 State의 Input을 받고 Update로직 실행
    public void UpdateLogic()
    {
        if (_currentState != null)
        {
            if (Input.anyKey)
                _currentState.GetInput();

            _currentState.UpdateLogic();
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
