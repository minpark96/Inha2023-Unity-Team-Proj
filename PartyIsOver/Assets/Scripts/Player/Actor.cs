using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class Actor : MonoBehaviourPun, IPunObservable
{
    public delegate void ChangePlayerStatus(float HP,float Stamina, DebuffState debuffstate, int viewID);
    public event ChangePlayerStatus OnChangePlayerStatus;
    public delegate void KillPlayer(int viewID);
    public event KillPlayer OnKillPlayer;
    public delegate void ChangeStaminaBar();
    public event ChangeStaminaBar OnChangeStaminaBar;


    private PlayerContext _context = new PlayerContext
    {
        DirX = 0,
        DirY = 0,
        DirZ = 0f,
        IsRunState = false,
        IsGrounded = false,
        IsUpperActionProgress = false,
        IsLowerActionProgress = false,
        LimbPositions = new int[4],
        PunchSide = Side.Left,
        IsMeowPunch = false
    };

    public enum ActorFlag
    {
        None        = 0x0,
        StateChange = 0x1,
        Run         = 0x2,
        Fall    = 0x4,
    }


    public enum ActorState
    {
        Dead = 0x1,
        Stand = 0x4,
        Walk = 0x8,
        Run = 0x10,
        Roll = 0x20,
        Jump = 0x40,
        Fall = 0x80,
        Climb = 0x100,
        Debuff = 0x200,
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
    private int _magneticStack = 0;
    public int MagneticStack { get { return _magneticStack; } set { _magneticStack = value; } }
    


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    public static int LayerCnt = (int)Define.Layer.Player1;




    //

    private AnimationPlayer _animPlayer = new AnimationPlayer();
    private AnimationData _animData;

    public AudioListener _audioListener;
    public StatusHandler StatusHandler;
    private PlayerInputHandler _inputHandler;
    public BodyHandler BodyHandler;
    public CameraControl CameraControl;

    public ActionController ActionController;
    public LowerBodySM LowerSM;
    public UpperBodySM UpperSM;


    private COMMAND_KEY _activeCommand;
    private COMMAND_KEY[] _commandAry = (COMMAND_KEY[])Enum.GetValues(typeof(COMMAND_KEY));

    public PlayerController PlayerController;
    public Grab Grab;

    public void InvokeStatusChangeEvent()
    {
        if (OnChangePlayerStatus == null)
        {
            Debug.Log(photonView.ViewID + " OnChangePlayerStatus 이벤트 null");
            return;
        }

        OnChangePlayerStatus(_health, _stamina, debuffState, photonView.ViewID);
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

        _animData = new AnimationData(BodyHandler);
        ActionController = new ActionController(_animData,_animPlayer,BodyHandler);
        BindActionNotify();

        _inputHandler = GetComponent<PlayerInputHandler>();

        LowerSM = new LowerBodySM(_inputHandler, _context);
        UpperSM = new UpperBodySM(_inputHandler, _context,
           BodyHandler.LeftHand.GetComponent<HandChecker>(), BodyHandler.RightHand.GetComponent<HandChecker>());

        _inputHandler.InitCommnad(this);
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

        if(CameraControl == null || BodyHandler == null) return;
        CameraControl.LookAround(BodyHandler.Hip.transform.position);
        CameraControl.CursorControl();

        UpdateStateMachine();

        if (LowerSM._currentState != null && UpperSM._currentState != null)
            UpdateData(); //자리가 여기가 아닐수도

        if (Input.GetKeyDown(KeyCode.G))
            UpperSM.IsMeowPunch = !UpperSM.IsMeowPunch;
    }


    void UpdateData()
    {
        _context.Id = photonView.ViewID;
        _context.Position = BodyHandler.Chest.transform.position;
        _context.IsMine = photonView.IsMine? true: false;
        _context.Layer = gameObject.layer;

        Vector3 dir = _inputHandler.GetMoveInput(CameraControl.CameraArm.transform);
        _context.DirX = dir.x;
        _context.DirY = dir.y;
        _context.DirZ = dir.z;


        _context.IsRunState = LowerSM.IsRun;
        _context.IsGrounded = LowerSM.IsGrounded;
        _context.IsUpperActionProgress = UpperSM.IsUpperActionProgress;
        _context.IsLowerActionProgress = LowerSM.IsLowerActionProgress;
        _context.IsMeowPunch = UpperSM.IsMeowPunch;

        _context.PunchSide = UpperSM.ReadySide;

        int[] limbPositions = LowerSM.GetBodyPose();
        for (int i = 0; i < (int)BodyPose.End; i++)
        {
            _context.LimbPositions[i] = limbPositions[i];
        }
    }



    private void FixedUpdate()
    {
        if (actorState == ActorState.Dead) return;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (_stamina <= 0)
            {
                //벽타기 불가능
                //Grab.GrabResetTrigger();
                GrabState = GrabState.None;
            }

            //회복하는 수치값 변경
            RecoveryStamina();

            accumulatedTime += Time.fixedDeltaTime;
            //Time.fixedDeltaTime(0.02초 기준으로 계속 반복) >= 현제회복시간
            //0.02초 계속 더해서 >= 0.1,0.2초 보다 커지면 
            if (accumulatedTime >= currentRecoveryTime)
            {
                //뛰거나 잡기 상태에서는
                if (actorState == ActorState.Run || GrabState == GrabState.Climb)
                {
                    //뛰거나 잡기 상태일때 만약 특수 디버프 상태가 들어오면 계속 까이는 현상이 있는데 조건을 걸어서 방지
                    if ((debuffState & DebuffState.Ice) == DebuffState.Ice || (debuffState & DebuffState.Shock) == DebuffState.Shock)
                    {
                        _stamina -= 0;
                        //photonView.RPC("DecreaseStamina", RpcTarget.All, 0f);
                        //Grab.GrabResetTrigger();
                        GrabState = GrabState.None;
                        //PlayerController.isRun = false;
                    }
                    else if (_stamina == 0)
                    {
                        _stamina = -1f;
                        actorState = ActorState.Walk;
                    }
                    else
                        _stamina -= 1;
                    //photonView.RPC("DecreaseStamina", RpcTarget.All, 1f);
                }
                //else if (PlayerController._isRSkillCheck || PlayerController.isHeading || PlayerController._isCoroutineDrop)
                    //스킬 사용시 회복 불가능
                    //photonView.RPC("RecoverStamina",RpcTarget.All, 0f);
                    //_stamina += 0;
                else
                    //상태에 맞는 회복하기
                    //photonView.RPC("RecoverStamina", RpcTarget.All, currentRecoveryStaminaValue);
                    _stamina += currentRecoveryStaminaValue;
                accumulatedTime = 0f;
            }
            //스테미너가 최대치는 넘는거 방지
            if (_stamina > MaxStamina)
                _stamina = MaxStamina;

            OnChangePlayerStatus(_health, _stamina, debuffState, photonView.ViewID);
        }


        if (!photonView.IsMine) return;

        OnChangeStaminaBar();


        UpdatePhysicsSM(); //마스터에서만 해야될수도

        ExecuteCommand();
    }

    void BindActionNotify()
    {
        ActionController.OnUpperActionEnd -= UpperActionEnd;
        ActionController.OnUpperActionEnd += UpperActionEnd;
        ActionController.OnLowerActionEnd -= LowerActionEnd;
        ActionController.OnLowerActionEnd += LowerActionEnd;

        ActionController.OnUpperActionStart -= UpperActionStart;
        ActionController.OnUpperActionStart += UpperActionStart;
        ActionController.OnLowerActionStart -= LowerActionStart;
        ActionController.OnLowerActionStart += LowerActionStart;
    }
    void UpperActionEnd()
    {
        UpperSM.IsUpperActionProgress = false;
    }
    void LowerActionEnd()
    {
        LowerSM.IsLowerActionProgress = false;
    }
    void UpperActionStart()
    {
        UpperSM.IsUpperActionProgress = true;
    }
    void LowerActionStart()
    {
        LowerSM.IsLowerActionProgress = true;
    }

    void RecoveryStamina()
    {
        //회복해주는 수치
        if (!((debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
        {
            currentRecoveryTime = RecoveryTime;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
        else
        {
            //PlayerController.isRun = false;
            currentRecoveryTime = ExhaustedRecoveryTime;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
    }
    [PunRPC]
    void SetStemina(float amount)
    {
        Stamina = amount;
    }

    [PunRPC]
    void DecreaseStamina(float amount)
    {
        Stamina -= amount;
    }

    [PunRPC]
    void RecoverStamina(float amount)
    {
        Stamina += amount;
    }

   
    void UpdateStateMachine()
    {
        LowerSM.UpdateLogic();
        UpperSM.UpdateLogic();
    }

    void UpdatePhysicsSM()
    {
        LowerSM.UpdatePhysics();
        UpperSM.UpdatePhysics();
    }

    //Update에서 활성키들을 모아놨다가
    //프레임상 문제가 있음 Execute 할 떄 마다 _activeCommand를 0으로 해야함
    void ExecuteCommand()
    {
        _activeCommand = _inputHandler.GetActiveCmdFlag();
        //Master클라이어트에게 받은 커맨드로 해당하는 actor들을 컨트롤하는 방향으로 혹은 마스터에서 바로 actor.execute
        for (int i = 0; i < Enum.GetValues(typeof(COMMAND_KEY)).Length; i++)
        {
            if((_activeCommand & _commandAry[i]) == _commandAry[i])
            {
                if(!_inputHandler.GetCommand(_commandAry[i]).Execute(_context))
                    Debug.Log(_commandAry[i].ToString() + "커맨드 실행 실패");
            }
        }

        //커맨드 플래그 클리어
        _inputHandler.ClearCommand();
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(actorState);
        }
        else
        {
            if (this.actorState != ActorState.Dead)
                this.actorState = (ActorState)stream.ReceiveNext();
        }
    }

    private void OnGUI() //완성 후 삭제
    {
        string content = LowerSM._currentState != null ? LowerSM._currentState.Name : "(no current state)";
        string content2 = UpperSM._currentState != null ? UpperSM._currentState.Name : "(no current state)";

        Rect labelRect = new Rect(100, 0, 300, 40);
        Rect labelRect2 = new Rect(Screen.width - 300, 0, 300, 40);

        GUIStyle style1 = new GUIStyle(GUI.skin.label);
        style1.normal.textColor = Color.black;
        style1.fontSize = 40;

        GUIStyle style2 = new GUIStyle(GUI.skin.label);
        style2.normal.textColor = Color.black;
        style2.fontSize = 40;

        GUI.Label(labelRect, content, style1);
        GUI.Label(labelRect2, content2, style2);
    }
}
