using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
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
        for (int i = 0; i < _currentStateList.Count; i++)
        {
            var state = _currentStateList[i];

            if (state != null)
            {
                if (state.ToString().Contains("Exhausted") && state.MyActor.Stamina == 100)
                {
                    state.ExitState();
                    _currentStateList[i] = null;
                }

                if (state.CoolTime > 0f)
                    state.CoolTime -= Time.deltaTime;

                if (state.CoolTime <= 0f)
                {
                    if(!state.ToString().Contains("Exhausted"))
                    {
                        state.ExitState();
                        _currentStateList[i] = null;
                    }
                }
                state.UpdateState();
            }
        }
        _currentStateList.RemoveAll(state => state == null);
    }

    public void ChangeState(IDebuffState newState, float time = 0)
    {
        foreach(var state in _currentStateList)
        {
            //같은 상태가 중복되면 쿨을 늘리는 것보다 그냥 있던 것을 끝내는 것 같은 상태이면 return
            if (state != null)
            {
                if (state == newState)
                    return;
            }

            //새로 들어온 상태가 Ice 면은 나머지 상태들은 일단 다 종료
            if(newState.ToString().Contains("Ice"))
            {
                state.ExitState();
                _currentStateList.Remove(state);
            }

            //감전 상태인데 스턴이 들어오면 return;
            if(state.ToString().Contains("Shock"))
            {
                if (newState.ToString().Contains("Stun"))
                    return;
            }
        }
        newState.CoolTime = time;

        SetState(newState);
    }
}
