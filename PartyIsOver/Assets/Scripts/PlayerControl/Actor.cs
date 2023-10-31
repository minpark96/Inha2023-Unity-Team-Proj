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
        Dead = 0x1,
        Unconscious = 0x2,
        Stand = 0x4,
        Run = 0x8,
        Jump = 0x10,
        Fall = 0x20,
        Climb = 0x40,
        Debuff = 0x80,
    }

    public enum DebuffState
    {
        Default =       0x00000000,  // X
        Balloon =       0x00000001,  // 풍선
        Unconscious =   0x00000010,  // 기절
        Drunk =         0x00000100,  // 취함
        ElectricShock = 0x00001000,  // 감전
        Ice =           0x00010000,  // 빙결
        Fire =          0x00100000,  // 화상
        Invisible =     0x01000000,  // 투명
        Strong =        0x10000000,  // 불끈
    }


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;

    public DebuffState debuffState = DebuffState.Default;

    private void Awake()
    {
        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerControll = GetComponent<PlayerController>();
    }
   

    private void FixedUpdate()
    {
        if (actorState != lastActorState)
        {
            PlayerControll.isStateChange = true;
            Debug.Log("stateChange");
        }
        else
        {
            PlayerControll.isStateChange = false;
        }



        switch (actorState)
        {
            case ActorState.Dead:
                break;
            case ActorState.Unconscious:
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
                break;
            case ActorState.Climb:
                break;
        }

        lastActorState = actorState;

        DebuffState debuffState = 

    }
}
