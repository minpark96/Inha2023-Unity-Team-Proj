using Photon.Pun;
using UnityEngine;
using static Define;
using System;

public class Actor : MonoBehaviourPun, IPunObservable
{
    public delegate void ChangePlayerStatus(float HP, float Stamina, DebuffState debuffstate, int viewID);
    public event ChangePlayerStatus OnChangePlayerStatus;
    public delegate void KillPlayer(int viewID);
    public event KillPlayer OnKillPlayer;
    public delegate void ChangeStaminaBar();
    public event ChangeStaminaBar OnChangeStaminaBar;


    public static GameObject LocalPlayerInstance;
    public static int LayerCnt = (int)Define.Layer.Player1;

    private PlayerActionContext _actionContext;
    public PlayerStatContext StatContext;
    public DebuffState debuffState = DebuffState.Default;


    public enum DebuffState
    {
        Default = 0x0,
        PowerUp = 0x1,
        Burn = 0x2,
        Exhausted = 0x4,
        Slow = 0x8,
        Ice = 0x10,
        Shock = 0x20,
        Stun = 0x40,
        Drunk = 0x80,
        Ghost = 0x200,
    }



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

    AudioSource _audioSource;
    AudioClip _audioClip;



    public void InvokeStatusChangeEvent()
    {
        if (OnChangePlayerStatus == null)
        {
            Debug.Log(photonView.ViewID + " OnChangePlayerStatus 이벤트 null");
            return;
        }

        OnChangePlayerStatus(StatContext.Health, StatContext.Stamina, debuffState, photonView.ViewID);
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
        if (SoundListenerTransform != null)
            AudioListener = SoundListenerTransform.gameObject.AddComponent<AudioListener>();
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
            Destroy(AudioListener);
            //_audioListener.enabled = false;
        }

        //if (SceneManager.GetActiveScene().name != "[4]Room")
        //    DontDestroyOnLoad(this.gameObject);

        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        ChangeLayerRecursively(gameObject, LayerCnt++);
        StatContext = new PlayerStatContext();
        _actionContext = new PlayerActionContext();

        Init();
    }

    private void Init()
    {
        _animData = new AnimationData(BodyHandler);
        ActionController = new ActionController(_animData, _animPlayer, BodyHandler);

        _inputHandler = GetComponent<PlayerInputHandler>();

        LowerSM = new LowerBodySM(_inputHandler, _actionContext, _inputHandler.ReserveCommand);
        UpperSM = new UpperBodySM(_inputHandler, _actionContext, _inputHandler.ReserveCommand,
        BodyHandler.LeftHand.GetComponent<HandChecker>(), BodyHandler.RightHand.GetComponent<HandChecker>(),
        RangeWeaponSkin);

        _inputHandler.InitCommand(this);
#if UNITY_EDITOR
        //_inputHandler.SetupInputAxes();
#endif
        StatContext.SetupStat();
        _actionContext.SetupAction(photonView.ViewID, photonView.IsMine ? true : false);
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
        if (!StatContext.IsAlive) return;

        if (photonView.IsMine)
        {
            if (CameraControl == null || BodyHandler == null) return;
            CameraControl.LookAround(BodyHandler.Hip.transform.position);
            CameraControl.CursorControl();

            if (!IsActionable()) return;

            UpdateStateMachine();

            UpdateData();

            if (Input.GetKeyDown(KeyCode.G))
                _actionContext.IsMeowPunch = !_actionContext.IsMeowPunch;
        }
    }


    void UpdateData() 
    {
        if (LowerSM.GetCurrentState() == null || UpperSM.GetCurrentState() == null) return;

        _actionContext.Layer = gameObject.layer;
        _actionContext.Position = BodyHandler.Chest.transform.position;
        _actionContext.IsGrounded = LowerSM.IsGrounded;
        _actionContext.PunchSide = UpperSM.ReadySide;

        int[] limbPositions = LowerSM.GetBodyPose();
        for (int i = 0; i < (int)BodyPose.End; i++)
            _actionContext.LimbPositions[i] = limbPositions[i];

        _actionContext.IsRunState = LowerSM.IsRun;
        Vector3 dir = _inputHandler.GetMoveInput(CameraControl.CameraArm.transform);
        _actionContext.InputDirX = dir.x;
        _actionContext.InputDirY = dir.y;
        _actionContext.InputDirZ = dir.z;
    }



    private void FixedUpdate()
    {
        if (!StatContext.IsAlive) return;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (StatContext.Stamina <= 0)
            {
                //벽타기 불가능
                ResetGrab();
            }

            //회복하는 수치값 변경
            RecoveryStamina();

            StatContext.AccumulatedTime += Time.fixedDeltaTime;
            //Time.fixedDeltaTime(0.02초 기준으로 계속 반복) >= 현제회복시간
            //0.02초 계속 더해서 >= 0.1,0.2초 보다 커지면 
            if (StatContext.AccumulatedTime >= StatContext.CurrentRecoveryTime)
            {
                //뛰거나 잡기 상태에서는
                if (_actionContext.IsRunState || GetLowerState() == PlayerState.Climb)
                {
                    //뛰거나 잡기 상태일때 만약 특수 디버프 상태가 들어오면 계속 까이는 현상이 있는데 조건을 걸어서 방지
                    if ((debuffState & DebuffState.Ice) == DebuffState.Ice || (debuffState & DebuffState.Shock) == DebuffState.Shock)
                    {
                        StatContext.Stamina = 0;
                        //photonView.RPC("DecreaseStamina", RpcTarget.All, 0f);
                        ResetGrab();
                        _actionContext.IsRunState = false;
                    }
                    else if (StatContext.Stamina == 0)
                    {
                        StatContext.Stamina = -1f;
                        _actionContext.IsRunState = false;
                    }
                    else
                        StatContext.Stamina -= 1;
                    //photonView.RPC("DecreaseStamina", RpcTarget.All, 1f);
                }
                //else if (PlayerController._isRSkillCheck || PlayerController.isHeading || PlayerController._isCoroutineDrop)
                //스킬 사용시 회복 불가능
                //photonView.RPC("RecoverStamina",RpcTarget.All, 0f);
                //_stamina += 0;
                else
                    //상태에 맞는 회복하기
                    //photonView.RPC("RecoverStamina", RpcTarget.All, currentRecoveryStaminaValue);
                    StatContext.Stamina += StatContext.CurrentRecoveryStaminaValue;
                StatContext.AccumulatedTime = 0f;
            }
            //스테미너가 최대치는 넘는거 방지
            if (StatContext.Stamina > StatContext.MaxStamina)
                StatContext.Stamina = StatContext.MaxStamina;

            OnChangePlayerStatus(StatContext.Health, StatContext.Stamina, debuffState, photonView.ViewID);
        }

        if (photonView.IsMine)
        {
            if (IsActionable())
            {
                UpdatePhysicsSM();
                ExecuteCommand((int)_inputHandler.GetActiveCmdFlag());
                //photonView.RPC(nameof(ExecuteCommand), RpcTarget.All, (int)_inputHandler.GetActiveCmdFlag());
            }

            //커맨드 플래그 클리어
            _inputHandler.ClearCommand();

            OnChangeStaminaBar();  //isMine에서 하는게 맞나?
        }

    }

    public void ResetGrab() //수정해야함
    {
        _inputHandler.ReserveCommand(COMMAND_KEY.DestroyJoint);
        UpperSM.ChangeState(UpperSM.StateMap[PlayerState.UpperIdle]);
    }


    void RecoveryStamina()
    {
        //회복해주는 수치
        if (!((debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
        {
            StatContext.CurrentRecoveryTime = StatContext.RecoveryTime;
            StatContext.CurrentRecoveryStaminaValue = StatContext.RecoveryStaminaValue;
        }
        else
        {
            //PlayerController.isRun = false;
            StatContext.CurrentRecoveryTime = StatContext.ExhaustedRecoveryTime;
            StatContext.CurrentRecoveryStaminaValue = StatContext.RecoveryStaminaValue;
        }
    }
    [PunRPC]
    void SetStemina(float amount)
    {
        StatContext.Stamina = amount;
    }

    [PunRPC]
    void DecreaseStamina(float amount)
    {
        StatContext.Stamina -= amount;
    }

    [PunRPC]
    void RecoverStamina(float amount)
    {
        StatContext.Stamina += amount;
    }

    public Vector3 GetMoveDir()
    {
        return _inputHandler.GetMoveInput(CameraControl.CameraArm.transform);
    }

    public void SetSkill(bool IsMeowNyangPunch)
    {
        _actionContext.IsMeowPunch = IsMeowNyangPunch;
    }

    public void SetFlambe(bool value)
    {
        _actionContext.IsFlambe = value;
    }

    void ExecuteCommand(int commandKey)
    {
        _activeCommand = (COMMAND_KEY)commandKey;

        for (int i = 0; i < Enum.GetValues(typeof(COMMAND_KEY)).Length - 1; i++)
        {
            if ((_activeCommand & _commandAry[i]) == _commandAry[i])
            {
                if (_inputHandler.GetCommand(_commandAry[i]).Execute(_actionContext))
                {
                    //Debug.Log(_commandAry[i].ToString() + " + " + GetUpperState());
                }
                else
                    Debug.Log(gameObject.name + _commandAry[i].ToString() + "커맨드 실행 실패");
            }
        }
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

    bool IsActionable()
    {
        if ((debuffState & DebuffState.Ice) == DebuffState.Ice ||
            (debuffState & DebuffState.Shock) == DebuffState.Shock ||
            (debuffState & DebuffState.Stun) == DebuffState.Stun)
            return false;
        else
            return true;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(actorState);
        //}
        //else
        //{
        //    if (this.actorState != ActorState.Dead)
        //        this.actorState = (ActorState)stream.ReceiveNext();
        //}
    }

    private void OnGUI() //완성 후 삭제
    {
        if (photonView.IsMine)
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
}
