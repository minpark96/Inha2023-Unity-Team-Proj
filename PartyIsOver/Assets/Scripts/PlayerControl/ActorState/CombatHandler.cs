using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public Context      Context;
    public Stun         StunInstance;
    public Burn         BurnInstance;
    public Ice          IceInstance;
    public PowerUp      PowerUpInstance;
    public Drunk        DrunkInstance;
    public Shock        ShockInstance;
    public Exhausted    ExhaustedInstance;

    private void Awake()
    {
        
    }

    private void Init()
    {
        Context = GetComponent<Context>();
        StunInstance = gameObject.AddComponent<Stun>();
    }
}
