using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviourPun
{
    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerController;
    

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
        Roll,
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

    public float HeadMultiple = 1.5f;
    public float ArmMultiple = 0.8f;
    public float HandMultiple = 0.8f;
    public float LegMultiple = 0.8f;
    public float DamageReduction = 0f;
    public float PlayerAttackPoint = 1f;


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;

    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerController = GetComponent<PlayerController>();
    }
   

    private void FixedUpdate()
    {
        if (actorState != lastActorState)
        {
            PlayerController.isStateChange = true;
            Debug.Log("stateChange");
        }
        else
        {
            PlayerController.isStateChange = false;
        }



        switch (actorState)
        {
            case ActorState.Dead:
                break;
            case ActorState.Unconscious:
                break;
            case ActorState.Stand:
                PlayerController.Stand();
                break;
            case ActorState.Run:
                PlayerController.Move();
                break;
            case ActorState.Jump:
                PlayerController.Jump();
                break;
            case ActorState.Fall:
                break;
            case ActorState.Climb:
                break;
            case ActorState.Roll:
                break;
        }

        lastActorState = actorState;

        //DebuffState debuffState = 

    }
}
