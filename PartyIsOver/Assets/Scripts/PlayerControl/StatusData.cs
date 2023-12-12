using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Data", menuName = "Scriptable Object/Status Data", order = int.MaxValue)]
public class StatusData : ScriptableObject
{
    [Header("DebuffTime")]
    [SerializeField] private float _stunTime;
    public float StunTime { get { return _stunTime; } }
    [SerializeField] private float _burnTime;
    public float BurnTime { get { return _burnTime; } }
    [SerializeField] private float _iceTime;
    public float IceTime { get { return _iceTime; } }
    [SerializeField] private float _powerUpTime;
    public float PowerUpTime { get { return _powerUpTime; } }
    [SerializeField] private float _drunktime;
    public float DrunkTime { get { return _drunktime; } }
    [SerializeField] private float _slowTime;
    public float SlowTime { get { return _slowTime; } }
    [SerializeField] private float _shockTime;
    public float ShockTime { get { return _shockTime; } }
}
