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
        Default =   0x0,  // X
        // 버프
        PowerUp =   0x1,  // 불끈
        Invisible = 0x2,  // 투명     >> X
        // 디버프
        Burn =      0x4,  // 화상
        Exhausted = 0x8,  // 지침
        Slow =      0x10, // 둔화
        Freeze =    0x20, // 빙결
        Shock =     0x40, // 감전
        Stun =      0x80, // 기절
        // 상태변화
        Drunk =     0x100, // 취함
        Balloon =   0x200, // 풍선
        Ghost =     0x400, // 유령
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
    }
}
