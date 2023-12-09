using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private List<IState> _currentStateList = new List<IState>();

    public void SetState(IState state)
    {
        state.MyActor = GetComponent<Actor>();

        Debug.Log("상태 변환");
        _currentStateList.Add(state);
        state.EnterState();
    }

    private void FixedUpdate()
    {
        for(int i = 0; i < _currentStateList.Count; i++)
        {
            var state = _currentStateList[i];

            if(state != null)
            {
                if(state.CoolTime > 0f)
                    state.CoolTime -= Time.deltaTime;

                if(state.CoolTime <= 0f)
                {
                    state.ExitState();
                    _currentStateList[i] = null;
                }

                state.UpdateState();
            }
        }
        _currentStateList.RemoveAll(state => state == null);
    }

    public void ChangeState(IState newState, float time)
    {
        Debug.Log("상태 바꾸라는 요청이 들어왔어요!!");
        
        //같은 상태가 중복되면 쿨을 늘리는 것보다 그냥 있던 것을 끝내는 것 같은 상태이면 return
        foreach(var state in _currentStateList)
        {
            if (state == newState && state != null)
            {
                Debug.Log("같은 상태여서 그냥 나가");
                return;
            }
        }

        newState.CoolTime = time;

        Debug.Log("상태 전환 신청");
        SetState(newState);
    }
}
