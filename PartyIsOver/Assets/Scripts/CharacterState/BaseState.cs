using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

/*
 * 플레이어 상태의 베이스가 되는 추상클래스
 */
public abstract class BaseState
{
    public Define.PlayerState Name { get; set; }

    protected StateMachine stateMachine;

    //생성시 상태별 이름을 enum으로 받고, 상태머신 저장
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

    //예약된 커맨드를 실행
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