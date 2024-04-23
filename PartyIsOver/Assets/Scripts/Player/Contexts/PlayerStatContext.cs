using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatContext
{
    public bool IsAlive { get; set; }
    public float RunSpeed { get; set; }
    public float MaxSpeed { get; set; }

    //공격력, 방어력(100이 무적)
    public float DamageReduction { get; set; }
    public float AttackPowerMultiplier { get; set; }


    // 체력
    public float Health { get; set; }
    public float MaxHealth { get; set; }


    public float RecoveryTime = 0.1f;
    public float RecoveryStaminaValue = 1f;
    public float ExhaustedRecoveryTime = 0.2f;
    public float CurrentRecoveryTime;
    public float CurrentRecoveryStaminaValue;
    public float AccumulatedTime = 0.0f;
    public float Stamina { get; set; }
    public float MaxStamina { get; set; }

    // 동사스택
    public int MagneticStack { get; set; }

    public void SetupStat()
    {
        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");

        IsAlive = true;
        RunSpeed = statData.RunSpeed;
        MaxSpeed = statData.MaxSpeed;
        Health = statData.Health;
        Stamina = statData.Stamina;
        MaxHealth = statData.MaxHealth;
        MaxStamina = statData.MaxStamina;
        DamageReduction = statData.DamageReduction;
        AttackPowerMultiplier = statData.AttackPowerMultiplier;
    }
}
