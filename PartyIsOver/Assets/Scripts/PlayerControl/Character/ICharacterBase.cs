using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterBase 
{
    float Defense { get; set; }
    float AttackDamage { get; set; }
    float CurrentHealth { get; set; }
    float MaxHealth { get; set; }
    float CurrentStamina { get; set; }
    float MaxStamina { get; set; }
}
