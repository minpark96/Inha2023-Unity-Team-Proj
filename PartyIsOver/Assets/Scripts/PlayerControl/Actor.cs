using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerControll;

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
    public ActorState lastActorState = ActorState.Run;



    // Start is called before the first frame update

    private void Awake()
    {
        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerControll = GetComponent<PlayerController>();
    }
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {



        if (actorState != lastActorState)
        {
            PlayerControll.isStateChange = true;
        }
        else
        {
            PlayerControll.isStateChange = false;
        }



        switch (actorState)
        {
            case ActorState.Dead:
                //_playerControll.Dead();
                break;
            case ActorState.Unconscious:
                //PlayerControll.Unconscious();
                break;
            case ActorState.Stand:
                PlayerControll.Stand();
                break;
            case ActorState.Run:
                PlayerControll.Move();
                break;
            case ActorState.Jump:
                PlayerControll.Jump();
                break;
            case ActorState.Fall:
                //_playerControll.Fall();
                break;
            case ActorState.Climb:
                //_playerControll.Climb();
                break;

        }

        lastActorState = actorState;
    }
}
