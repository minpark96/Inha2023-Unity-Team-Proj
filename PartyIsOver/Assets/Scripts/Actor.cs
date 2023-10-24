using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public StatusHandler statusHandler;
    private Actor _actor;

    public enum ActorState
    {
        Dead = 1,
        Unconscious = 2,
        Stand = 4,
        Run = 8,
        Jump = 0x10,
        Fall = 0x20,
        Climb = 0x40,
    }

    public ActorState actorState = ActorState.Stand;


    // Start is called before the first frame update
    void Start()
    {
       statusHandler = GetComponent<StatusHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
