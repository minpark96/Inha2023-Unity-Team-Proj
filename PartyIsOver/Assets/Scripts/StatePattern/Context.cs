using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private IState _currentState;
    [SerializeField]
    private float _stunTime = 2;
    private float _currentTime = 0;

    public void Start()
    {
        Managers.DurationTime.DurationTimeExpired += OnDurationTime;
    }

    public void SetState(IState state)
    {
        state.MyActor = GetComponent<Actor>();

        Debug.Log("상태 변환");
        _currentState = state;
        _currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if(_currentState != null)
        {
            _currentState.UpdateState();
            Managers.DurationTime.UpdateDurationTimes(Time.fixedDeltaTime);
        }
    }

    public void ChangeState(IState newState)
    {
        Debug.Log("상태 바꾸라는 요청이 들어왔어요!!");

        if(_currentState != null)
        {
            Debug.Log("전 상태 끝내기");
            _currentState.ExitState();
        }
        //스턴 시간 추가 하기
        Managers.DurationTime.SetDurationTime(newState.ToString(), _stunTime);
        Debug.Log("상태 전환 신청");
        SetState(newState);
        
    }

    //이벤트는 종료를 하는 것
    private void OnDurationTime(string state)
    {
        if(_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = null;
    }


}
