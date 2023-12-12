using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class DurationTimeManager : MonoBehaviour 
{
    private Dictionary<string, float> DurationTimes = new Dictionary<string, float>();

    //지속 시간 종료 이벤트 정의
    public event Action<string> DurationTimeExpired;

    //지속 시간 설정
    public void SetDurationTime(string state ,float duration)
    {
        if(DurationTimes.ContainsKey(state))
        {
            DurationTimes[state] = duration;
        }
        else
        {
            DurationTimes.Add(state, duration);
        }
    }

    //지속 시간 감소 및 종료 이벤트 발생
    public void UpdateDurationTimes(float deltaTime)
    {
        //예외 처리가 필요로 하다 같은게 들어오면 문제가 생기고 중복으로 들어오면 문제가
        foreach(var state in DurationTimes.Keys.ToList()) 
        {
            DurationTimes[state] = Mathf.Max(0f, DurationTimes[state] - deltaTime);
            Debug.Log("Time : " + DurationTimes[state]);
            if (DurationTimes[state] <= 0f)
            {
                Debug.Log("상태 이벤트 종료 시작");
                OnDurationTimesExpired(state);
                Debug.Log("상태 이벤트 종료 끝");
            }
        }
        Debug.Log("상태 이벤트 ++진짜++ 종료 끝");
    }

    //지속시간 종료 이벤트 발생
    protected virtual void OnDurationTimesExpired(string state)
    {
        //지금 상태의 이벤트 실행 지금은 Stun
        DurationTimeExpired?.Invoke(state);
    }


}
