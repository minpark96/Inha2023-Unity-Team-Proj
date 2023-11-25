using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private string _itemName;
    public string ItemName { get { return _itemName; } }

    [SerializeField]
    private ItemType _itemType;
    public ItemType ItemType { get { return _itemType; } }

    [SerializeField]
    private ProjectileBase _projectile;
    public ProjectileBase Projectile { get { return _projectile; } }


    [SerializeField]
    private InteractableObject.Damage _useDamageType;
    public InteractableObject.Damage UseDamageType { get { return _useDamageType; } }

    [SerializeField]
    private float _damage;
    public float Damage { get {return  _damage; } }

    [SerializeField]
    private float _coolTime;
    public float CoolTime { get { return _coolTime; } }
}
