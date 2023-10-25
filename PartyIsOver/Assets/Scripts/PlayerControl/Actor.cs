using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviourPunCallbacks
{
    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    private PlayerController _playerControll;

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

    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        BodyHandler = GetComponent<BodyHandler>();  
       StatusHandler = GetComponent<StatusHandler>();
        _playerControll = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {



        if (actorState != lastActorState)
        {
            _playerControll.isStateChange = true;
            Debug.Log("stateChange");
        }
        else
        {
            _playerControll.isStateChange = false;
        }



        switch (actorState)
        {
            case ActorState.Dead:
                //_playerControll.Dead();
                break;
            case ActorState.Unconscious:
                _playerControll.Unconscious();
                break;
            case ActorState.Stand:
                _playerControll.Stand();
                break;
            case ActorState.Run:
                _playerControll.Move();
                break;
            case ActorState.Jump:
                _playerControll.Jump();
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
