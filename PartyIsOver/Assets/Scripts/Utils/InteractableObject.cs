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
        DivingKick = 4,
        Headbutt = 5,
        Knockout = 6,
        Special= 7,

        Freeze = 8,
        Shock = 9,
        PowerUp = 10,
        Slow = 11,

    }

    public Damage damageModifier = Damage.Default;

}
