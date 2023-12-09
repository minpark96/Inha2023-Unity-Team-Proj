using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.SceneManagement;
using System.Numerics;


public class Actor : MonoBehaviourPun, IPunObservable
{
    public delegate void ChangePlayerStatus(float HP, ActorState actorState, DebuffState debuffstate, int viewID);
    public event ChangePlayerStatus OnChangePlayerStatus;
    public delegate void KillPlayer(int viewID);
    public event KillPlayer OnKillPlayer;
    public delegate void ChangeStaminaBar();
    public event ChangeStaminaBar OnChangeStaminaBar;

    public AudioListener _audioListener;

    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerController;
    public Grab Grab;
    public BalloonState BalloonState;
    public CameraControl CameraControl;

    public enum ActorState
    {
        Dead = 0x1,
        Unconscious = 0x2,
        Stand = 0x4,
        Walk = 0x8,
        Run = 0x10,
        Roll = 0x20,
        Jump = 0x40,
        Fall = 0x80,
        Climb = 0x100,
        Debuff = 0x200,
        BalloonWalk = 0x400,
     
    }

    public enum DebuffState
    {
        Default =   0x0,
        PowerUp =   0x1,
        Burn =      0x2,
        Exhausted = 0x4,
        Slow =      0x8,
        Ice =       0x10,
        Shock =     0x20, 
        Stun =      0x40, 
        Drunk =     0x80,  
        Balloon =   0x100, 
        Ghost =     0x200,
    }

    public GrabState GrabState = GrabState.None;

    //공격력 방어력(100이 무적)
    [SerializeField]
    private float _damageReduction = 0f;
    public float DamageReduction { get { return _damageReduction; } set { _damageReduction = value; } }

    [SerializeField]
    private float _playerAttackPoint = 1f;

    public float PlayerAttackPoint { get { return _playerAttackPoint; } set { _damageReduction = value; } }


    // 체력
    [SerializeField]
    private float _health;
    [SerializeField]
    private float _maxHealth = 200f;
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _maxHealth; } }


    [Header("Stamina Recovery")]
    public float RecoveryTime = 0.1f;
    public float RecoveryStaminaValue = 1f;
    public float ExhaustedRecoveryTime = 0.2f;
    float currentRecoveryTime;
    float currentRecoveryStaminaValue; 
    float accumulatedTime = 0.0f;

    // 스테미나
    [SerializeField]
    private float _stamina;
    [SerializeField]
    private float _maxStamina = 100f;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _maxStamina; } }

    // 동사스택
    [SerializeField]
    private float _magneticStack = 0f;
    public float MagneticStack { get { return _magneticStack; } set { _magneticStack = value; } }
    


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    public static int LayerCnt = (int)Define.Layer.Player1;

    public void InvokeStatusChangeEvent()
    {
        if (OnChangePlayerStatus == null)
        {
            Debug.Log(photonView.ViewID + " OnChangePlayerStatus 이벤트 null");
            return;
        }

        OnChangePlayerStatus(_health, actorState, debuffState, photonView.ViewID);
    }

    public void InvokeDeathEvent()
    {
        if (OnKillPlayer == null)
        {
            Debug.Log(photonView.ViewID + " OnKillPlayer 이벤트 null");
            return;
        }

        OnKillPlayer(photonView.ViewID);
    }

    private void Awake()
    {
        Transform SoundListenerTransform = transform.Find("GreenHead");
        if(SoundListenerTransform != null)
            _audioListener = SoundListenerTransform.gameObject.AddComponent<AudioListener>();
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;

            if (CameraControl == null)
            {
                Debug.Log("카메라 컨트롤 초기화");
                CameraControl = GetComponent<CameraControl>();
            }
        }
        else
        {
            // 사운드 끄기
            Destroy(_audioListener);
            //_audioListener.enabled = false;
        }

        //if (SceneManager.GetActiveScene().name != "[4]Room")
        //    DontDestroyOnLoad(this.gameObject);

        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerController = GetComponent<PlayerController>();
        Grab = GetComponent<Grab>();
        BalloonState = GetComponent<BalloonState>();

        ChangeLayerRecursively(gameObject, LayerCnt++);

        Init();
    }

    private void Init()
    {
        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");

        _health = statData.Health;
        _stamina = statData.Stamina;
        _maxHealth = statData.MaxHealth;
        _maxStamina = statData.MaxStamina;
        _damageReduction = statData.DamageReduction;
        _playerAttackPoint = statData.PlayerAttackPoint;
    }

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    } 

    private void Update()
    {
        if (!photonView.IsMine || actorState == ActorState.Dead) return;

        CameraControl.LookAround(BodyHandler.Hip.transform.position);
        CameraControl.CursorControl();

    }

    void RecoveryStamina()
    {
        if (debuffState != Actor.DebuffState.Exhausted)
        {
            currentRecoveryTime = RecoveryTime;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
        else
        {
            PlayerController.isRun = false;
            currentRecoveryTime = 0.2f;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || actorState == ActorState.Dead) return;

        RecoveryStamina();

        accumulatedTime += Time.fixedDeltaTime;
        if (accumulatedTime >= currentRecoveryTime)
        {
            if (PlayerController.isRun || GrabState == GrabState.Climb)
            {
                if(debuffState == Actor.DebuffState.Ice || debuffState == Actor.DebuffState.Shock)
                {
                    Stamina -= 0;
                    GrabState = GrabState.None;
                    PlayerController.isRun = false;
                }
                else
                    Stamina -= 1;

            }
            else if(PlayerController._isRSkillCheck || PlayerController.isHeading || PlayerController._isCoroutineDrop)
                Stamina += 0;
            else
                Stamina += currentRecoveryStaminaValue;

            if (Stamina > MaxStamina)
                Stamina = MaxStamina;

            accumulatedTime = 0f;
        }

        OnChangeStaminaBar();

        if (actorState != lastActorState)
        {
            PlayerController.isStateChange = true;
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
                //PlayerController.Stun();
                break;
            case ActorState.Stand:
                PlayerController.Stand();
                break;
            case ActorState.Walk:
                PlayerController.Move();
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
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(actorState);
        }
        else
        {
            this.actorState = (ActorState)stream.ReceiveNext();
        }
    }
}
