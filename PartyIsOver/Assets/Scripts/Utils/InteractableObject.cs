using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum Damage
    {
        Ignore = 0,
        Default = 1,
        Object = 2,
        Punch = 3,
        DropKick = 4,
        Headbutt = 5,
        Knockout = 6,
        Special= 7,

        PowerUp = 8,
        Burn = 9,
        Slow = 10,
        Ice = 11,
        Shock = 12,
        Stun = 13,
        Drunk = 14,
        Balloon = 15,
    }

    public Damage damageModifier = Damage.Default;

}
