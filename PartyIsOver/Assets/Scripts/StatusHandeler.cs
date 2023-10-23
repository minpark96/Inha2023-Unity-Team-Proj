using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static InteractableObject;

public class StatusHandeler : MonoBehaviour
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;

    private float _health;
    public float Health { get { return _health; } set { _health = value; } }

    // Start is called before the first frame update
    void Start()
    {
        actor = transform.GetComponent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        damage *= _damageModifer;
        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            //if (numOfDamageTypes < damageTypes.Length)
            //{
            //    damageTypes[numOfDamageTypes] = new Damage(type, damage, causer);
            //    numOfDamageTypes++;
            //    hasDamageTypes = true;
            //}
            //healthDamage += damage * GameplayModifiers.damageTakenMul;
        }
    }
}
