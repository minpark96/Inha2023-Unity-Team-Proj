using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private IState _currentState;
    private float time = 2f;

    public void SetState(IState state)
    {
        if(_currentState != null)
        {
            Debug.Log("다른 상태가 들어왔어요 그래서 지금 있던 상태는 종료 할께요");
            _currentState.ExitState();
        }

        Debug.Log("상태 변환");
        _currentState = state;
        _currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if(_currentState != null)
            _currentState.UpdateState();
    }

    public void ChangeState(IState newState)
    {
        Debug.Log("상태 바꾸라는 요청이 들어왔어요!!");

        if(_currentState != null)
        {
            Debug.Log("전 상태 끝내기");
            _currentState.ExitState();
        }

        Debug.Log("상태 전환 신청");
        SetState(newState);
    }


}
