using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public StatusHandeler statusHandeler;
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
        Swim = 0x80
    }

    public ActorState actorState = ActorState.Stand;


    // Start is called before the first frame update
    void Start()
    {
       statusHandeler = GetComponent<StatusHandeler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
