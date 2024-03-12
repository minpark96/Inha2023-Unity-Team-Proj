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


    private PlayerActionContext _actionContext = new PlayerActionContext
    {
        InputDirX = 0,
        InputDirY = 0,
        InputDirZ = 0f,
        IsRunState = false,
        IsGrounded = false,
        IsUpperActionProgress = false,
        IsLowerActionProgress = false,
        LimbPositions = new int[4],
        PunchSide = Side.Left,
        IsMeowPunch = false
    };

    private PlayerStatContext _statContext = new PlayerStatContext
    {
        DamageReduction = 0f,
        AttackPowerMultiplier = 1f,

        Health = 0f,
        MaxHealth = 200f,


    };

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
    private float _attackPowerMultiplier = 1f;

    public float AttackPowerMultiplier { get { return _attackPowerMultiplier; } set { _attackPowerMultiplier = value; } }


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
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    public static int LayerCnt = (int)Define.Layer.Player1;




    //
    public AudioListener AudioListener;
    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public CameraControl CameraControl;

    public ActionController ActionController;
    public LowerBodySM LowerSM;
    public UpperBodySM UpperSM;
    public Transform RangeWeaponSkin;

    private PlayerInputHandler _inputHandler;
    private AnimationPlayer _animPlayer = new AnimationPlayer();
    private AnimationData _animData;

    private COMMAND_KEY _activeCommand;
    private COMMAND_KEY[] _commandAry = (COMMAND_KEY[])Enum.GetValues(typeof(COMMAND_KEY));

    //
    public PlayerController PlayerController;
    AudioSource _audioSource;
    AudioClip _audioClip;

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

    [PunRPC]
    public void PlayerEffectSound(string path)
    {
        _audioClip = Managers.Sound.GetOrAddAudioClip(path, Define.Sound.PlayerEffect);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);

    }

    private void Awake()
    {
        Transform SoundListenerTransform = transform.Find("GreenHead");
        if(SoundListenerTransform != null)
            AudioListener = SoundListenerTransform.gameObject.AddComponent<AudioListener>();
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
            Destroy(AudioListener);
            //_audioListener.enabled = false;
        }

        //if (SceneManager.GetActiveScene().name != "[4]Room")
        //    DontDestroyOnLoad(this.gameObject);

        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerController = GetComponent<PlayerController>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
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
        _attackPowerMultiplier = statData.AttackPowerMultiplier;

        _animData = new AnimationData(BodyHandler);
        ActionController = new ActionController(_animData,_animPlayer,BodyHandler);

        _inputHandler = GetComponent<PlayerInputHandler>();

        LowerSM = new LowerBodySM(_inputHandler, _actionContext);
        UpperSM = new UpperBodySM(_inputHandler, _actionContext,
           BodyHandler.LeftHand.GetComponent<HandChecker>(), BodyHandler.RightHand.GetComponent<HandChecker>(),
           RangeWeaponSkin);

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

        if (LowerSM.GetCurrentState() != null && UpperSM.GetCurrentState() != null)
            UpdateData(); //�ڸ��� ���Ⱑ �ƴҼ���

        if (Input.GetKeyDown(KeyCode.G))
            _actionContext.IsMeowPunch = !_actionContext.IsMeowPunch;
    }


    void UpdateData()
    {
        _actionContext.Id = photonView.ViewID;
        _actionContext.Position = BodyHandler.Chest.transform.position;
        _actionContext.IsMine = photonView.IsMine? true: false;
        _actionContext.Layer = gameObject.layer;

        Vector3 dir = _inputHandler.GetMoveInput(CameraControl.CameraArm.transform);
        _actionContext.InputDirX = dir.x;
        _actionContext.InputDirY = dir.y;
        _actionContext.InputDirZ = dir.z;


        _actionContext.IsRunState = LowerSM.IsRun;
        _actionContext.IsGrounded = LowerSM.IsGrounded;

        _actionContext.PunchSide = UpperSM.ReadySide;

        int[] limbPositions = LowerSM.GetBodyPose();
        for (int i = 0; i < (int)BodyPose.End; i++)
            _actionContext.LimbPositions[i] = limbPositions[i];
       
    }



    private void FixedUpdate()
    {
        if (actorState == ActorState.Dead) return;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (_stamina <= 0)
            {
                //��Ÿ�� �Ұ���
                ResetGrab();
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
                        ResetGrab();
                        _actionContext.IsRunState = false;
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


        ExecuteCommand();
        UpdatePhysicsSM(); //�����Ϳ����� �ؾߵɼ���

    }

    public void ResetGrab()
    {
        _inputHandler.ReserveCommand(COMMAND_KEY.DestroyJoint);
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

   


    //Update���� Ȱ��Ű���� ��Ƴ��ٰ�
    //�����ӻ� ������ ���� Execute �� �� ���� _activeCommand�� 0���� �ؾ���
    void ExecuteCommand()
    {
        _activeCommand = _inputHandler.GetActiveCmdFlag();
        //MasterŬ���̾�Ʈ���� ���� Ŀ�ǵ�� �ش��ϴ� actor���� ��Ʈ���ϴ� �������� Ȥ�� �����Ϳ��� �ٷ� actor.execute
        for (int i = 0; i < Enum.GetValues(typeof(COMMAND_KEY)).Length -1; i++)
        {
            if((_activeCommand & _commandAry[i]) == _commandAry[i])
            {
                if (_inputHandler.GetCommand(_commandAry[i]).Execute(_actionContext))
                    Debug.Log(_commandAry[i].ToString() +" + "+ GetUpperState());
                else
                    Debug.Log(_commandAry[i].ToString() + "Ŀ�ǵ� ���� ����");
            }
        }

        //Ŀ�ǵ� �÷��� Ŭ����
        _inputHandler.ClearCommand();
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

    public Define.PlayerState GetUpperState()
    {
        return UpperSM.GetCurrentState().Name;
    }
    public Define.PlayerState GetLowerState()
    {
        return LowerSM.GetCurrentState().Name;
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
        string content = LowerSM.GetCurrentState() != null ? LowerSM.GetCurrentState().Name.ToString() : "(no current state)";
        string content2 = UpperSM.GetCurrentState() != null ? UpperSM.GetCurrentState().Name.ToString() : "(no current state)";

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
