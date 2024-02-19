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

    //���ݷ� ����(100�� ����)
    [SerializeField]
    private float _damageReduction = 0f;
    public float DamageReduction { get { return _damageReduction; } set { _damageReduction = value; } }

    [SerializeField]
    private float _playerAttackPoint = 1f;

    public float PlayerAttackPoint { get { return _playerAttackPoint; } set { _damageReduction = value; } }


    // ü��
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

    // ���׹̳�
    [SerializeField]
    private float _stamina;
    [SerializeField]
    private float _maxStamina = 100f;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _maxStamina; } }

    // ���罺��
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
            Debug.Log(photonView.ViewID + " OnChangePlayerStatus �̺�Ʈ null");
            return;
        }

        OnChangePlayerStatus(_health, _stamina, debuffState, photonView.ViewID);
    }

    public void InvokeDeathEvent()
    {
        if (OnKillPlayer == null)
        {
            Debug.Log(photonView.ViewID + " OnKillPlayer �̺�Ʈ null");
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
                Debug.Log("ī�޶� ��Ʈ�� �ʱ�ȭ");
                CameraControl = GetComponent<CameraControl>();
            }
        }
        else
        {
            // ���� ����
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
            UpdateData(); //�ڸ��� ���Ⱑ �ƴҼ���

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
                //��Ÿ�� �Ұ���
                //Grab.GrabResetTrigger();
                GrabState = GrabState.None;
            }

            //ȸ���ϴ� ��ġ�� ����
            RecoveryStamina();

            accumulatedTime += Time.fixedDeltaTime;
            //Time.fixedDeltaTime(0.02�� �������� ��� �ݺ�) >= ����ȸ���ð�
            //0.02�� ��� ���ؼ� >= 0.1,0.2�� ���� Ŀ���� 
            if (accumulatedTime >= currentRecoveryTime)
            {
                //�ٰų� ��� ���¿�����
                if (actorState == ActorState.Run || GrabState == GrabState.Climb)
                {
                    //�ٰų� ��� �����϶� ���� Ư�� ����� ���°� ������ ��� ���̴� ������ �ִµ� ������ �ɾ ����
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
                    //��ų ���� ȸ�� �Ұ���
                    //photonView.RPC("RecoverStamina",RpcTarget.All, 0f);
                    //_stamina += 0;
                else
                    //���¿� �´� ȸ���ϱ�
                    //photonView.RPC("RecoverStamina", RpcTarget.All, currentRecoveryStaminaValue);
                    _stamina += currentRecoveryStaminaValue;
                accumulatedTime = 0f;
            }
            //���׹̳ʰ� �ִ�ġ�� �Ѵ°� ����
            if (_stamina > MaxStamina)
                _stamina = MaxStamina;

            OnChangePlayerStatus(_health, _stamina, debuffState, photonView.ViewID);
        }


        if (!photonView.IsMine) return;

        OnChangeStaminaBar();


        UpdatePhysicsSM(); //�����Ϳ����� �ؾߵɼ���

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
        //ȸ�����ִ� ��ġ
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

    //Update���� Ȱ��Ű���� ��Ƴ��ٰ�
    //�����ӻ� ������ ���� Execute �� �� ���� _activeCommand�� 0���� �ؾ���
    void ExecuteCommand()
    {
        _activeCommand = _inputHandler.GetActiveCmdFlag();
        //MasterŬ���̾�Ʈ���� ���� Ŀ�ǵ�� �ش��ϴ� actor���� ��Ʈ���ϴ� �������� Ȥ�� �����Ϳ��� �ٷ� actor.execute
        for (int i = 0; i < Enum.GetValues(typeof(COMMAND_KEY)).Length; i++)
        {
            if((_activeCommand & _commandAry[i]) == _commandAry[i])
            {
                if(!_inputHandler.GetCommand(_commandAry[i]).Execute(_context))
                    Debug.Log(_commandAry[i].ToString() + "Ŀ�ǵ� ���� ����");
            }
        }

        //Ŀ�ǵ� �÷��� Ŭ����
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

    private void OnGUI() //�ϼ� �� ����
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
