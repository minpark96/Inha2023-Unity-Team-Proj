using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Collision Data", menuName = "Scriptable Object/Collision Data", order = int.MaxValue)]
public class CollisionData : ScriptableObject
{
    [Header("DamageMultiple")]
    [SerializeField] private float _objectDamage;
    public float _ObjectDamage { get { return _objectDamage; } }
    [SerializeField] private float _punchDamage;
    public float PunchDamage { get { return _punchDamage; } }
    [SerializeField] private float _dropkickDamage;
    public float DropkickDamage { get { return _dropkickDamage; } }
    [SerializeField] private float _headbuttDamage;
    public float HeadbuttDamage { get { return _headbuttDamage; } }

    [Header("NormalForce")]
    [SerializeField] private float _objectForceNormal;
    public float ObjectForceNormal { get { return _objectForceNormal; } }
    [SerializeField] private float _punchForceNormal;
    public float PunchForceNormal { get { return _punchForceNormal; } }
    [SerializeField] private float _dropkickForceNormal;
    public float DropkickForceNormal { get { return _dropkickForceNormal; } }
    [SerializeField] private float _headbuttForceNormal;
    public float HeadbuttForceNormal { get { return _headbuttForceNormal; } }

    [Header("UpForce")]
    [SerializeField] private float _objectForceUp;
    public float ObjectForceUp { get { return _objectForceUp; } }
    [SerializeField] private float _punchForceUp;
    public float PunchForceUp { get { return _punchForceUp; } }
    [SerializeField] private float _headbuttForceUp;
    public float HeadbuttForceUp { get { return _headbuttForceUp; } }
    [SerializeField] private float _dropkickForceUp;
    public float DropkickForceUp { get { return _dropkickForceUp; } }
}
