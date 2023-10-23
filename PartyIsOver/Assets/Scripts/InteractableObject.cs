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
        DamageVolume = 8
    }

    public Damage damageModifier = Damage.Default;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(damageModifier == Damage.Punch) {
            Debug.Log("punch");
        }
    }
}
