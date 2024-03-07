using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Scriptable Object/Stat Data", order = int.MaxValue)]
public class PlayerStatData : ScriptableObject
{
    [Header("PlayerStat")]
    [SerializeField]
    private float _runSpeed;
    public float RunSpeed { get { return _runSpeed; } }

    [SerializeField]
    private float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } }

    [SerializeField]
    private float _attackPowerMultiplier;
    public float AttackPowerMultiplier { get { return _attackPowerMultiplier; } }


    [SerializeField]
    private float _health;
    public float Health { get { return _health; } }

    [SerializeField]
    private float _maxHealth;
    public float MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    private float _stamina;
    public float Stamina { get { return _stamina; } }

    [SerializeField]
    private float _maxStamina;
    public float MaxStamina { get { return _maxStamina; } }

    [SerializeField]
    private float _damageReduction;
    public float DamageReduction { get { return _damageReduction; } }


    [SerializeField]
    private float _throwingForce;
    public float ThrowingForce { get { return _throwingForce; } }

    [SerializeField]
    private float _itemSwingPower;
    public float ItemSwingPower { get { return _itemSwingPower; } }

    [Header("HitDamageMultiple")]
    [SerializeField]
    private float _headMultiple;
    public float HeadMultiple { get { return _headMultiple; } }

    [SerializeField]
    private float _armMultiple;
    public float ArmMultiple { get { return _armMultiple; } }

    [SerializeField]
    private float _handMultiple;
    public float HandMultiple { get { return _handMultiple; } }

    [SerializeField]
    private float _legMultiple;
    public float LegMultiple { get { return _legMultiple; } }


  
}
