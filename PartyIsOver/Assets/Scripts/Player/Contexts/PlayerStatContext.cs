using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatContext
{
    //공격력 방어력(100이 무적)
    private float _damageReduction = 0f;
    private float _attackPowerMultiplier = 1f;
    public float DamageReduction { get { return _damageReduction; } set { _damageReduction = value; } }
    public float AttackPowerMultiplier { get { return _attackPowerMultiplier; } set { _attackPowerMultiplier = value; } }


    // 체력
    private float _health;
    private float _maxHealth = 200f;
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }


    public float RecoveryTime = 0.1f;
    public float RecoveryStaminaValue = 1f;
    public float ExhaustedRecoveryTime = 0.2f;
    public float currentRecoveryTime;
    public float currentRecoveryStaminaValue;
    public float accumulatedTime = 0.0f;

    private float _stamina;
    private float _maxStamina = 100f;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _maxStamina; } }

    // 동사스택
    private int _magneticStack = 0;
    public int MagneticStack { get { return _magneticStack; } set { _magneticStack = value; } }
}
