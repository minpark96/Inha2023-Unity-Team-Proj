using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private List<IDebuffState> _currentStateList = new List<IDebuffState>();

    public void SetState(IDebuffState state)
    {
        state.MyActor = GetComponent<Actor>();

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

    public void ChangeState(IDebuffState newState, float time)
    {        
        //같은 상태가 중복되면 쿨을 늘리는 것보다 그냥 있던 것을 끝내는 것 같은 상태이면 return
        foreach(var state in _currentStateList)
        {
            if (state == newState && state != null)
                return;
        }

        newState.CoolTime = time;

        SetState(newState);
    }
}
