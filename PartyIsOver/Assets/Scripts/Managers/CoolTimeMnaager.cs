using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoolTimeMnaager
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    public event Action<string> CooldownExpired;

    //상태에 따른 쿨타임 설정
    public void SetCooldown(string state, float duration)
    {
        if(cooldowns.ContainsKey(state))
        {
            cooldowns[state] = duration;
        }
        else
        {
            cooldowns.Add(state, duration);
        }
    }

    //쿨타임 감소
    public void UpdateCooldowns(float deltaTime)
    {
        foreach(var state in cooldowns.Keys.ToList())
        {
            cooldowns[state] = Mathf.Max(0f, cooldowns[state] - deltaTime);

            if (cooldowns[state] <= 0f)
            {
                OnCooldownExpired(state);
            }
        }
    }

    protected virtual void OnCooldownExpired(string state)
    {
        CooldownExpired?.Invoke(state);
    }

    //쿨타임 확인
    public bool IsCooldownActive(string state)
    {
        return cooldowns.ContainsKey(state) && cooldowns[state] > 0f;
    }

}
