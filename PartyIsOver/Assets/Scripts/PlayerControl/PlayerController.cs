using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static Actor;
using static AniFrameData;
using static AniAngleData;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

[System.Serializable]
public class AniFrameData
{
    public enum RollForce
    {
        Zero,
        ZeroReverse,
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left,
    }
    public Rigidbody[] StandardRigidbodies;
    public Rigidbody[] ActionRigidbodies;
    public RollForce[] ForceDirections;
    public float[] ForcePowerValues;
}

[System.Serializable]
public class AniAngleData
{
    public enum AniAngle
    {
        Zero,
        Forward,
        Backward,
        Up,
        Down,
        Right,
        Left,
    }
    public Rigidbody[] StandardRigidbodies;
    public Transform[] ActionDirection;
    public Transform[] TargetDirection;
    public AniAngle[] ActionAngleDirections;
    public AniAngle[] TargetAngleDirections;
    public float[] AngleStability;
    public float[] AnglePowerValues;

}

public class PlayerController : MonoBehaviourPun
{
    [Header("AnimationControll")]
    [SerializeField]
    public AniFrameData[] RollAniData;

    [SerializeField]
    public AniAngleData[] RollAngleAniData;

    [SerializeField]
    public AniFrameData[] DropAniData;

    [SerializeField]
    public AniAngleData[] RightPunchAniData;

    [SerializeField]
    public AniAngleData[] LeftPunchAniData;

    [SerializeField]
    public AniFrameData[] RightPunchingAniData;

    [SerializeField]
    public AniFrameData[] LeftPunchingAniData;

    [SerializeField]
    public AniAngleData[] RightPunchResettingAniData;

    [SerializeField]
    public AniAngleData[] LeftPunchResettingAniData;

    [SerializeField]
    public AniFrameData[] MoveForceJumpAniData;

    [SerializeField]
    public AniAngleData[] MoveAngleJumpAniData;

    [SerializeField]
    public AniFrameData[] HeadingAniData;

    [SerializeField]
    public AniAngleData[] HeadingAngleAniData;

    [SerializeField]
    public AniFrameData[] RSkillAniData;

    [SerializeField]
    public AniAngleData[] RSkillAngleAniData;

    [SerializeField]
    public AniFrameData[] ItemTwoHandAniData;

    [SerializeField]
    public AniAngleData[] ItemTwoHandAngleData;

    [SerializeField]
    public AniAngleData[] ItemOneHandAngleAniData;

    [SerializeField]
    public AniFrameData[] ItemOneHandReadyAniData;

    [SerializeField]
    public AniFrameData[] ItemOneHandSwingAniData;

    [SerializeField]
    public AniFrameData[] PotionReadyAniData;

    [SerializeField]
    public AniFrameData[] PotionDrinkingAniData;

    [SerializeField]
    public AniAngleData[] PotionAngleAniData;

    [SerializeField]
    public AniFrameData[] PickUpRipAniData;

    [SerializeField]
    public AniFrameData[] ShootDownRipAniData;

    [SerializeField]
    public AniAngleData[] RipAngleAniData;

    [SerializeField]
    public AniFrameData[] ItemTwoHandLeftAniData;

    [SerializeField]
    public AniAngleData[] ItemTwoHandLeftAngleData;

    [SerializeField]
    public AniFrameData[] PotionThrowAniData;

    [SerializeField]
    public AniAngleData[] PotionThrowAngleData;

    [SerializeField]
    public AniFrameData[] TestRready1;

    [SerializeField]
    public AniFrameData[] TestDrinking;

    [SerializeField]
    public AniAngleData[] TestRready2;



    [Header("Speed")]
    public float RunSpeed;
    private float MaxSpeed;

    [SerializeField]
    private Rigidbody _hips;

    private Transform _cameraArm;

    [SerializeField]
    private BodyHandler _bodyHandler;


    private Grab _grab;
    private Actor _actor;

    [Header("PunchControll")]
    public float ReadyPunch = 0.1f;
    public float Punching = 0.1f;
    public float ResetPunch = 0.3f;

    [Header("StatusControll")]
    public bool isGrounded;
    public bool isRun;
    public bool isMove;
    public bool isDuck;
    public bool isKickDuck;
    public bool leftGrab;
    public bool rightGrab;
    public bool leftKick;
    public bool rightKick;
    public bool isStateChange;
    public bool isMeowNyangPunch = false;
    public bool _isRSkillCheck;
    public bool isBalloon;
    public bool isDrunk;
    public bool isHeading;
    public bool isDropkick;

    [Header("SkillControll")]
    public float RSkillCoolTime = 10;
    //잠깐 딜레이를 줘야 자세를 잡음
    private float ChargeAniHoldTime = 0.5f;
    public float MeowPunchPower = 1f;
    //펀치 3개
    public float MeowPunchReadyPunch = 0.1f;
    public float MeowPunchPunching = 0.1f;
    public float MeowPunchResetPunch = 0.3f;

    public float NuclearPunchPower = 1f;
    public float NuclearPunchReadyPunch = 0.1f;
    public float NuclearPunching = 0.1f;
    public float NuclearPunchResetPunch = 0.3f;

    //차지 시간
    public float ChargeTime = 1.3f;

    public bool BalloonJump;
    public bool BalloonDrop;

    public bool IsFlambe;


    private float _itemSwingPower;

    [Header("MoveControll")]
    private float _runSpeedOffset = 350f;
    public Vector3 MoveInput;
    private Vector3 _moveDir;
    private bool _isCoroutineRunning;
    public bool _isCoroutineDrop;
    private bool _isCoroutineRoll;
    private float _idleTimer = 0;
    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 800f;

    private Vector3 _runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    public List<Quaternion> RotationsForBalloon = new List<Quaternion>();
    private BalloonState _balloonState;
    private DrunkState _drunkState;

    public static PlayerController Instance;
    public AudioSource _audioSource;
    AudioClip _audioClip;

    [Header("Dummy")]
    public bool isAI = false;

    Rigidbody _hipRB;

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;

    Side _readySide = Side.Left;

    InteractableObject target;
    Rigidbody _childRigidbody;
    Transform[] _children;
    private Dictionary<Transform, Quaternion> _initialRotations = new Dictionary<Transform, Quaternion>();

    float startChargeTime;
    float endChargeTime = 0f;
    int _checkHoldTimeCount = 0;
    public enum Side
    {
        Left = 0,
        Right = 1
    }

    public enum Pose
    {
        Bent = 0,
        Forward = 1,
        Straight = 2,
        Behind = 3
    }

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        _bodyHandler.BodySetup();
        for (int i = 0; i < _bodyHandler.BodyParts.Count - 1; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                RotationsForBalloon.Add(Quaternion.Euler(-90, 0, 0));
            else
                RotationsForBalloon.Add(_bodyHandler.BodyParts[i].PartTransform.localRotation);

        }

        if (photonView.IsMine)
            _cameraArm = _actor.CameraControl.CameraArm;
        

    }



    private ConfigurableJoint[] childJoints;
    private ConfigurableJointMotion[] originalYMotions;
    private ConfigurableJointMotion[] originalZMotions;
    void Init()
    {
        _bodyHandler = GetComponent<BodyHandler>();
        _actor = GetComponent<Actor>();
        _hipRB = transform.Find("GreenHip").GetComponent<Rigidbody>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
         
        childJoints = GetComponentsInChildren<ConfigurableJoint>();
        originalYMotions = new ConfigurableJointMotion[childJoints.Length];
        originalZMotions = new ConfigurableJointMotion[childJoints.Length];

        _children = GetComponentsInChildren<Transform>();

        // 원래의 angularXMotion 값을 저장
        for (int i = 0; i < childJoints.Length; i++)
        {
            originalYMotions[i] = childJoints[i].angularYMotion;
            originalZMotions[i] = childJoints[i].angularZMotion;
        }
        _grab = GetComponent<Grab>();

        _balloonState = GetComponent<BalloonState>();
        _drunkState = GetComponent<DrunkState>();

        Instance = this;

        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");
        MaxSpeed = statData.MaxSpeed;
        RunSpeed = statData.RunSpeed;
        _itemSwingPower = statData.ItemSwingPower;
    }

    [PunRPC]
    void RestoreOriginalMotions()
    {

        //y z 초기값 대입
        for (int i = 0; i < childJoints.Length; i++)
        {
            childJoints[i].angularYMotion = originalYMotions[i];
            childJoints[i].angularZMotion = originalZMotions[i];
        }
    }

    [PunRPC]
    public void PlayerEffectSound(string path)
    {
        _audioClip = Managers.Sound.GetOrAddAudioClip(path, Define.Sound.PlayerEffect);
        _audioSource.clip = _audioClip;
        _audioSource.volume = 0.2f;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);

    }

    #region OnMouseEvent_Grab
    public void OnMouseEvent_Grab(Define.MouseEvent evt)
    {

        switch (evt)
        {
            case Define.MouseEvent.Press:
                {
                    if (Input.GetMouseButton(0))
                        _grab.Grabbing();
                }
                break;
            case Define.MouseEvent.PointerUp:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        _grab.GrabResetTrigger();
                    }
                }
                break;
        }
    }
    #endregion

    #region OnMouseEvent_Skill

    public void OnMouseEvent_Skill(Define.MouseEvent evt)
    {


        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (Input.GetMouseButtonDown(1) && _actor.Stamina >= 0)
                    {
                        if (_actor.debuffState == DebuffState.Exhausted)
                            return;
                        _actor.Stamina -= 5;

                        if (_actor.Stamina <= 0)
                            _actor.Stamina = 0;

                        if (_actor.debuffState != DebuffState.Balloon && !isGrounded)
                        {
                            isDropkick = true;
                            DropKickTrigger();
                        }
                    }
                }
                break;
            case Define.MouseEvent.Click:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if(_actor.debuffState == DebuffState.Balloon)
                        {
                            BalloonDrop = false;

                            if (BalloonJump == true && BalloonDrop == false)
                            {
                                BalloonJump = false;
                                BalloonDrop = true;
                                _bodyHandler.Hip.PartRigidbody.AddForce(Vector3.down * 200000f);
                            }
                        }
                        else
                            PunchAndGrab();
                    }

                    if (Input.GetMouseButtonUp(1) && _actor.Stamina >= 0)
                    {
                        if (_actor.debuffState == DebuffState.Exhausted)
                            return;
                        _actor.Stamina -= 5;

                        if(_actor.Stamina <= 0)
                            _actor.Stamina = 0;

                        if (_actor.debuffState == DebuffState.Balloon)
                        {
                            //StartCoroutine(_balloonState.BalloonSpin());
                        }

                    }

                    if (_actor.debuffState == DebuffState.Balloon)
                        return;

                    if (Input.GetMouseButtonUp(2) && _actor.Stamina >= 0)
                    {
                        if (_actor.debuffState == DebuffState.Exhausted)
                            return;
                        _actor.Stamina -= 5;

                        if (_actor.Stamina <= 0)
                            _actor.Stamina = 0;
                        isHeading = true;
                        StartCoroutine(Heading());
                    }
                }
                break;
        }
    }
    #endregion

    #region OnKeyboardEvent_Move

    public void OnKeyboardEvent_Move(Define.KeyboardEvent evt)
    {
        if (IsFlambe)
            return;

        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (_actor.GrabState == Define.GrabState.Climb)
                            _actor.Grab.Climb();
                        _actor.actorState = Actor.ActorState.Jump;

                        if (_actor.debuffState == Actor.DebuffState.Balloon)
                        {
                            BalloonJump = true;
                        }
                    }

                    if (_actor.debuffState == Actor.DebuffState.Balloon)
                    {
                        if (isBalloon)
                        {
                            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                            {
                                MoveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                            }
                        }
                    }
                    else if(_actor.debuffState == Actor.DebuffState.Drunk)
                    {
                        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                        {
                            MoveInput = new Vector3(-Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical"));
                        }
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                        {
                            MoveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                        }
                    }
                }
                break;
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    {
                        MoveInput = new Vector3(0, 0, 0);
                    }
                }
                break;
            
        }
    }
    #endregion

    #region OnKeyboardEvent_Skill

    public void OnKeyboardEvent_Skill(Define.KeyboardEvent evt)
    {


        switch (evt)
        {
            case Define.KeyboardEvent.PointerDown:
                {
                    if (Input.GetKeyDown(KeyCode.R) && _actor.Stamina >= 0)
                    {
                        _actor.Stamina -= 30;

                        if (_actor.Stamina <= 0)
                        {
                            _actor.Stamina = 0;
                            _actor.debuffState = DebuffState.Exhausted;
                        }

                        if (_actor.debuffState == DebuffState.Exhausted)
                            return;
                        else
                        {
                            if (_actor.debuffState != DebuffState.Drunk)
                            {
                                if (!_isRSkillCheck)
                                {
                                    photonView.RPC("EffectCreate", RpcTarget.All, "Effects/Love_aura");
                                    photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/ACTION_Changing_Smoke");
                                    _isRSkillCheck = true;
                                    photonView.RPC("ChargeReady", RpcTarget.All);
                                }
                            }
                        }
                    }
                }
                break;
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKey(KeyCode.LeftShift) && _actor.actorState!=ActorState.Jump)
                    {
                        _actor.actorState = Actor.ActorState.Run;
                        isRun = true;
                    }
                }
                break;
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.LeftShift) && isRun == true)
                    {
                        _actor.actorState = Actor.ActorState.Stand;
                        isRun = false;
                    }

                    if (Input.GetKeyUp(KeyCode.R) && Managers.Input._checkHoldTime)
                    {
                        _isRSkillCheck = false;
                        photonView.RPC("ResetCharge", RpcTarget.All);
                        photonView.RPC("RSkillDestroyEffect", RpcTarget.All, "Love_aura");
                    }
                }
                break;
            case Define.KeyboardEvent.Charge:
                {
                    if (Input.GetKeyUp(KeyCode.R) && _actor.debuffState == DebuffState.Drunk)
                    {
                        IsFlambe = true;
                        photonView.RPC("DrunkAction", RpcTarget.All);
                    }
                    else
                    {
                        photonView.RPC("RestoreOriginalMotions", RpcTarget.All);
                        if (Input.GetKeyUp(KeyCode.R) && isMeowNyangPunch)
                            MeowNyangPunch();
                        else
                            NuclearPunch();

                        photonView.RPC("DestroyEffect", RpcTarget.All, "Love_aura");
                    }
                }
                break;
            case Define.KeyboardEvent.Hold:
                {
                    if (Managers.Input._checkHoldTime == false && _checkHoldTimeCount == 0)
                    {
                        photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/Item_UI_029");
                        _checkHoldTimeCount++;
                    }

                    if (Input.GetKey(KeyCode.R) && _actor.Stamina >= 0)
                    {
                        if (_actor.debuffState == DebuffState.Drunk)
                        {
                            StartCoroutine(_drunkState.DrunkActionReady());
                        }
                    }
                    //중일때 확인 ex 이펙트 출현하는 코드를 넣어주면 기모아지는 것 첨 될듯

                }
                break;
        }
    }
    [PunRPC]
    void EffectCreate(string path)
    {
        _actor.StatusHandler.EffectObjectCreate($"{path}");
    }

    [PunRPC]
    void RSkillDestroyEffect(string name)
    {
        _actor.StatusHandler.DestroyEffect(name);
    }

    [PunRPC]
    void DrunkAction()
    {
        StartCoroutine(_drunkState.DrunkAction());
    }

    #endregion

    #region ChargeSkill
    [PunRPC]
    IEnumerator ChargeReady()
    {
        for (int i = 0; i < childJoints.Length; i++)
        {
            childJoints[i].angularYMotion = ConfigurableJointMotion.Locked;
            childJoints[i].angularZMotion = ConfigurableJointMotion.Locked;
        }

        for (int i = 0; i < RSkillAngleAniData.Length; i++)
        {
            AniAngleForce(RSkillAngleAniData, i);
        }
        StartCoroutine(ForceRready(ChargeAniHoldTime));
        yield return null;
    }

    IEnumerator ForceRready(float _delay)
    {
        startChargeTime = Time.time;
        for (int i = 0; i < RSkillAniData.Length; i++)
        {
            AniForce(RSkillAniData, i);
        }
        yield return new WaitForSeconds(_delay);
        //물체의 모션을 고정
        Rigidbody _RPartRigidbody;
        for (int i = 0; i < RSkillAniData.Length; i++)
        {
            for (int j = 0; j < RSkillAniData[i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = RSkillAniData[i].ActionRigidbodies[j];
                _RPartRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                //키를 짧게 누르면 락 걸리는걸 방지 하기 위한 
                if (endChargeTime - startChargeTime > 0.0001f)
                {
                    _RPartRigidbody.constraints = RigidbodyConstraints.None;
                }
                _RPartRigidbody.velocity = Vector3.zero;
                _RPartRigidbody.angularVelocity = Vector3.zero;
            }
        }

        yield return null;
    }

    [PunRPC]
    IEnumerator ResetCharge()
    {
        _checkHoldTimeCount = 0;
        endChargeTime = Time.time;
        Rigidbody _RPartRigidbody;

        for (int i = 0; i < RSkillAniData.Length; i++)
        {
            for (int j = 0; j < RSkillAniData[i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = RSkillAniData[i].ActionRigidbodies[j];
                //Debug.Log("Freeze풀기 : "+ _RPartRigidbody);
                _RPartRigidbody.constraints = RigidbodyConstraints.None;
                _RPartRigidbody.velocity = Vector3.zero;
                _RPartRigidbody.angularVelocity = Vector3.zero;
            }
        }
        RestoreOriginalMotions();
        yield return new WaitForSeconds(0.5f);
    }
    #endregion

    #region ChargeSkillAnimation

    private void NuclearPunch()
    {
        StartCoroutine(NuclearPunchDelay());
        photonView.RPC("ResetCharge", RpcTarget.All);
    }

    IEnumerator NuclearPunchDelay()
    {
        photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/SUPERMODE_Punch_Hit_03");
        yield return MeowPunch(Side.Right, 0.07f, NuclearPunchReadyPunch, NuclearPunching, NuclearPunchResetPunch);
        yield return RSkillCoolTimer();
    }

    private void MeowNyangPunch()
    {
        StartCoroutine(MeowNyangPunchDelay());
        photonView.RPC("ResetCharge", RpcTarget.All);
    }

    IEnumerator MeowNyangPunchDelay()
    {
        int _punchcount = 0;
        _readySide = Side.Right;
        while (_punchcount < 5)
        {
            photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/Effect/SFX_ArrowShot_Hit");
            if (_readySide == Side.Left)
            {
                yield return MeowPunch(Side.Left, 0.07f, MeowPunchReadyPunch, MeowPunchPunching, MeowPunchResetPunch);
                _readySide = Side.Right;
            }
            else
            {
                yield return MeowPunch(Side.Right, 0.07f, MeowPunchReadyPunch, MeowPunchPunching, MeowPunchResetPunch);
                _readySide = Side.Left;
            }
            _punchcount++;
        }
        yield return RSkillCoolTimer();

    }
    IEnumerator RSkillCoolTimer()
    {
        _isRSkillCheck = false;
        yield return new WaitForSeconds(RSkillCoolTime);
    }

    IEnumerator MeowPunch(Side side, float duration, float readyTime, float punchTime, float resettingTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ArmActionReadying(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ArmActionPunching(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resettingTime)
        {
            ArmActionPunchResetting(side);
            yield return new WaitForSeconds(duration);
        }
    }

    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        if(_isRSkillCheck == true)
        {
            if (_actor.debuffState == DebuffState.Stun || _actor.debuffState == DebuffState.Ice || _actor.debuffState == DebuffState.Shock || _actor.debuffState == DebuffState.Drunk)
            {
                photonView.RPC("ResetCharge", RpcTarget.All);
                _isRSkillCheck = false;
            }
        }
        

        if (!photonView.IsMine || _actor.actorState == ActorState.Dead) return;

        if (isAI)
            return;

        if (_actor.debuffState == Actor.DebuffState.Balloon && isBalloon == false)
        {
            photonView.RPC("_balloonState.BalloonShapeOn", RpcTarget.All);
            //StartCoroutine(_balloonState.BalloonShapeOn());
        }

        if (_actor.debuffState == Actor.DebuffState.Drunk && isDrunk == false)
        {
            isDrunk = true;
            StartCoroutine(_drunkState.DrunkOff());
        }


        if (_actor.actorState != Actor.ActorState.Jump && _actor.actorState != Actor.ActorState.Roll 
            && _actor.actorState != Actor.ActorState.Run && _actor.actorState != ActorState.Unconscious)
        {
            if (MoveInput.magnitude == 0f)
            {
                _actor.actorState = Actor.ActorState.Stand;
            }
            else
            {
                if (_actor.debuffState == Actor.DebuffState.Balloon)
                    _actor.actorState = Actor.ActorState.BalloonWalk;
                else
                    _actor.actorState = Actor.ActorState.Walk;

                //Stand();
            }
        }
    }

    #endregion

    #region ForwardRoll
    private void ForwardRollTrigger()
    {
        if (!_isCoroutineRoll)
        {
            //자식들의 오브젝터 rotation을 뛰기 전에 저장
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms)
            {
                _initialRotations[childTransform] = childTransform.localRotation;
            }
            //점프를 해야 잘 굴러서 점프를 한번 한 다음에 구르기 시작
            _actor.actorState = Actor.ActorState.Jump;
            StartCoroutine(ForwardRollDelay(3f));
        }
    }

    IEnumerator ForwardRollDelay(float delay)
    {
        _isCoroutineRoll = true;
        yield return ForwardRoll(0.07f, 1.5f);
        yield return new WaitForSeconds(delay);
        _isCoroutineRoll = false;
    }

    IEnumerator ForwardRoll(float duration, float readyRoll)
    {
        //최고 속도를 구를 때 마다 일정하게 값을 넣을려고
        _hips.velocity = -_hips.transform.up.normalized * MaxSpeed * 1.5f;
        //연산이 너무 빨라서 잠깐 멈춰줘야함
        yield return new WaitForSeconds(0.08f);
        //상태를 Roll 상태로 전환
        _actor.actorState = ActorState.Roll;

        //spring을 풀어서 구르기가 자연스럽게 할 수 있게 한다.
        _actor.StatusHandler.StartCoroutine("ResetBodySpring");
        //hip의 잠겨 있는 FreezeRotationX 축을 풀음
        _hipRB.constraints &= ~RigidbodyConstraints.FreezeRotationX;

        float rollTime = Time.time;

        //실제로 회전 하는 것
        while (Time.time - rollTime < readyRoll)
        {
            AniAngleForce(RollAngleAniData, 0);
            AniForce(RollAniData, 0);
            yield return new WaitForSeconds(duration);
        }

        //힘은 0, Rotation 복구 하기
        foreach (Transform child in _children)
        {
            yield return RestoreRotations(child);
        }
        //yield return StartSlerp(duration);

        _actor.actorState = Actor.ActorState.Stand;
    }

    IEnumerator StartSlerp(float duration)
    {
        float startRollTime = Time.time;

        while (Time.time - startRollTime < 0.07f)
        {
            foreach (Transform child in _children)
            {
                if (_initialRotations.ContainsKey(child))
                {
                    Debug.Log("Slerp아직 잘 안됨 수정 부분임");
                    Vector3 lerpedDirecion = Vector3.Slerp(child.localRotation.eulerAngles, _initialRotations[child].eulerAngles, 0.1f);
                    child.localRotation = Quaternion.LookRotation(lerpedDirecion);
                }
            }
            yield return new WaitForSeconds(duration);
        }
    }

    IEnumerator ForwardRollOld(float duration, float readyRoll)
    {
        _hips.velocity = -_hips.transform.up.normalized * MaxSpeed * 1.5f;
        yield return new WaitForSeconds(0.08f);
        _actor.actorState = ActorState.Roll;

        _actor.StatusHandler.StartCoroutine("ResetBodySpring");
        _hipRB.constraints &= ~RigidbodyConstraints.FreezeRotationX;

        float rollTime = Time.time;
        float startRollTime = Time.time;

        while (Time.time - rollTime < readyRoll)
        {
            AniAngleForce(RollAngleAniData, 0);
            AniForce(RollAniData, 0);
            yield return new WaitForSeconds(duration);
        }

        //힘은 0, Rotation, 스프링 복구 하기
        //RestoreRotations();
        _actor.actorState = Actor.ActorState.Stand;
    }

    IEnumerator RestoreRotations(Transform child)
    {
        _childRigidbody = child.GetComponent<Rigidbody>();
        if (_childRigidbody != null)
        {
            //Debug.Log(_initialRotations[child]);
            // 초기 회전값 복원 Dictionary에서 특정 키의 존재 여부를 확인
            if (_initialRotations.ContainsKey(child))
            {
                //회전 힘과 AddForce 힘을 벡터 0으로 해서 값 빼기
                _childRigidbody.velocity = Vector3.zero;
                _childRigidbody.angularVelocity = Vector3.zero;
                child.localRotation = _initialRotations[child];
                //int count= 0;
                //while (Quaternion.Angle(child.localRotation, _initialRotations[child]) > 1f)
                /*while(count <10)
                {
                    //_initialRotations[child] 목표값
                    //child.localRotation 시작 값
                    child.localRotation = Quaternion.Slerp(child.localRotation, _initialRotations[child], 0.4f);
                    Debug.Log(string.Format("{0}     :  {1:N2}", child.name, Quaternion.Angle(child.localRotation, _initialRotations[child])));
                    count++;
                    yield return new WaitForSeconds(0.07f);
                }*/
            }
            //다시 잠금
            if (_childRigidbody.name == "GreenHip")
                _hipRB.constraints |= RigidbodyConstraints.FreezeRotationX;
        }
        yield return _actor.StatusHandler.RestoreBodySpring(0.07f);
    }

    public void RestoreRotationsOld()
    {
        _actor.StatusHandler.StartCoroutine("RestoreBodySpring");

        foreach (Transform child in _children)
        {
            _childRigidbody = child.GetComponent<Rigidbody>();
            if (_childRigidbody != null)
            {
                //회전 힘과 AddForce 힘을 벡터 0으로 해서 값 빼기
                _childRigidbody.velocity = Vector3.zero;
                _childRigidbody.angularVelocity = Vector3.zero;
                // 초기 회전값 복원 Dictionary에서 특정 키의 존재 여부를 확인
                if (_initialRotations.ContainsKey(child))
                {
                    child.localRotation = _initialRotations[child];
                }

                //다시 잠금
                if (_childRigidbody.name == "GreenHip")
                    _hipRB.constraints |= RigidbodyConstraints.FreezeRotationX;
            }
        }
    }

    #endregion

    #region Animation Direction Force Angle

    Vector3 GetForceDirection(AniFrameData data, int index)
    {
        RollForce _rollState = data.ForceDirections[index];
        Vector3 _direction;

        switch (_rollState)
        {
            case RollForce.Zero:
                _direction = new Vector3(0, 0, 0);
                break;
            case RollForce.ZeroReverse:
                _direction = new Vector3(-1, -1, -1);
                break;
            case RollForce.Forward:
                _direction = -data.StandardRigidbodies[index].transform.up;
                break;
            case RollForce.Backward:
                _direction = data.StandardRigidbodies[index].transform.up;
                break;
            case RollForce.Up:
                _direction = data.StandardRigidbodies[index].transform.forward;
                break;
            case RollForce.Down:
                _direction = -data.StandardRigidbodies[index].transform.forward;
                break;
            case RollForce.Left:
                _direction = -data.StandardRigidbodies[index].transform.right;
                break;
            case RollForce.Right:
                _direction = data.StandardRigidbodies[index].transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }
        return _direction;
    }

    void AniForceVelocityChange(AniFrameData[] _forceSpeed, int _elementCount, Vector3 _dir)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == RollForce.Zero || _forceSpeed[_elementCount].ForceDirections[i] == RollForce.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            }
        }
    }

    void AniForce(AniFrameData[] _forceSpeed, int _elementCount)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
            _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
        }
    }

    public void AniForce(AniFrameData[] _forceSpeed, int _elementCount, Vector3 _dir)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == RollForce.Zero || _forceSpeed[_elementCount].ForceDirections[i] == RollForce.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            }
        }
    }

    void AniForce(AniFrameData[] _forceSpeed, int _elementCount, Vector3 _dir, float _punchpower)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == RollForce.Zero || _forceSpeed[_elementCount].ForceDirections[i] == RollForce.ZeroReverse)
            {
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
        }
    }

    Vector3 GetAngleDirection(AniAngle _angleState, Transform _Transformdirection)
    {
        Vector3 _direction;

        switch (_angleState)
        {
            case AniAngle.Zero:
                _direction = Vector3.zero;
                break;
            case AniAngle.Forward:
                _direction = -_Transformdirection.transform.up;
                break;
            case AniAngle.Backward:
                _direction = _Transformdirection.transform.up;
                break;
            case AniAngle.Up:
                _direction = _Transformdirection.transform.forward;
                break;
            case AniAngle.Down:
                _direction = -_Transformdirection.transform.forward;
                break;
            case AniAngle.Left:
                _direction = -_Transformdirection.transform.right;
                break;
            case AniAngle.Right:
                _direction = _Transformdirection.transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }

        return _direction;
    }

    void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount)
    {
        for (int i = 0; i < _aniAngleData[_elementCount].StandardRigidbodies.Length; i++)
        {
            Vector3 _angleDirection = GetAngleDirection(_aniAngleData[_elementCount].ActionAngleDirections[i],
                _aniAngleData[_elementCount].ActionDirection[i]);
            Vector3 _targetDirection = GetAngleDirection(_aniAngleData[_elementCount].TargetAngleDirections[i],
                _aniAngleData[_elementCount].TargetDirection[i]);

            AlignToVector(_aniAngleData[_elementCount].StandardRigidbodies[i], _angleDirection, _targetDirection,
                _aniAngleData[_elementCount].AngleStability[i], _aniAngleData[_elementCount].AnglePowerValues[i]);
        }
    }

    public void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount, Vector3 _vector)
    {
        for (int i = 0; i < _aniAngleData[_elementCount].StandardRigidbodies.Length; i++)
        {
            Vector3 _angleDirection = GetAngleDirection(_aniAngleData[_elementCount].ActionAngleDirections[i],
                _aniAngleData[_elementCount].ActionDirection[i]);
            Vector3 _targetDirection = GetAngleDirection(_aniAngleData[_elementCount].TargetAngleDirections[i],
                _aniAngleData[_elementCount].TargetDirection[i]);

            AlignToVector(_aniAngleData[_elementCount].StandardRigidbodies[i], _angleDirection, _vector + _targetDirection,
                _aniAngleData[_elementCount].AngleStability[i], _aniAngleData[_elementCount].AnglePowerValues[i]);
        }
    }

    #endregion

    #region DropKick

    private void DropKickTrigger()
    {
        if (!_isCoroutineDrop)
            StartCoroutine(DropKickDelay(2f));
    }

    IEnumerator DropKickDelay(float delay)
    {
        _isCoroutineDrop = true;
        if (!isGrounded)
        {
            StartCoroutine(DropKick());
        }
        yield return new WaitForSeconds(delay);
        _isCoroutineDrop = false;
    }

    IEnumerator DropKick()
    {
        Transform partTransform = _bodyHandler.Hip.transform;
        if (!isGrounded)
        {
            for (int i = 0; i < DropAniData.Length; i++)
            {
                _actor.StatusHandler.StartCoroutine("ResetBodySpring");

                if (i == 0)
                {
                    Transform transform2 = _bodyHandler.RightFoot.transform;
                    _bodyHandler.RightFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.RightThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //데미지
                    Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(DropAniData, i, dir);
                    photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightLeg, true);
                }
                else if (i == 1)
                {
                    Transform transform2 = _bodyHandler.LeftFoot.transform;
                    _bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //데미지
                    Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(DropAniData, i, dir);
                    photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftLeg, true);
                }
                else
                {
                    AniForce(DropAniData, i);
                }
            }

            yield return new WaitForSeconds(2);
            _actor.StatusHandler.StartCoroutine("RestoreBodySpring", 1f);
            _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftLeg, false);
            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightLeg, false);
        }
        yield return null;
    }

    #endregion

    #region Punch
    public void PunchAndGrab()
    {
        if (!_isCoroutineRunning)
        {
            if (_readySide == Side.Left)
            {
                StartCoroutine(PunchWithDelay(Side.Left));
                _readySide = Side.Right;
            }
            else
            {
                StartCoroutine(PunchWithDelay(Side.Right));
                _readySide = Side.Left;
            }
        }
    }

    IEnumerator PunchWithDelay(Side side)
    {
        _isCoroutineRunning = true;
        yield return Punch(side, 0.07f, ReadyPunch, Punching, ResetPunch);
        _isCoroutineRunning = false;
    }

    //값이 들어 오는게 0.01 0.1 0.1 0.3
    public IEnumerator Punch(Side side, float duration, float readyTime, float punchTime, float resetTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ArmActionReadying(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ArmActionPunching(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ArmActionPunchResetting(side);
            yield return new WaitForSeconds(duration);
        }
    }

    //아이템 때문에 추가
    public IEnumerator Punch(Side side, float duration, float readyTime, float punchTime, float resetTime, float itemPower)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ArmActionReadying(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ArmActionPunching(side, itemPower);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ArmActionPunchResetting(side);
            yield return new WaitForSeconds(duration);
        }
    }
    #endregion

    #region PunchAnimation

    public void ArmActionReadying(Side side)
    {
        AniAngleData[] aniAngleDatas = (side == Side.Right) ? RightPunchAniData : LeftPunchAniData;
        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            AniAngleForce(aniAngleDatas, i);
        }
    }

    public void ArmActionPunching(Side side)
    {

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] aniFrameDatas;
        Transform transform2;

        if (side == Side.Left)
        {
            aniFrameDatas = LeftPunchingAniData;
            transform2 = _bodyHandler.LeftHand.transform;
            _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _bodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, true);
        }
        else
        {
            aniFrameDatas = RightPunchingAniData;
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _bodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, true);
        }

        for (int i = 0; i < aniFrameDatas.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);

            if (_isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    AniForce(aniFrameDatas, i, dir, MeowPunchPower);
                else
                    AniForce(aniFrameDatas, i, dir, NuclearPunchPower);
            }
            else
                AniForce(aniFrameDatas, i, dir);
        }
    }

    // 아이템 때문에 추가
    public void ArmActionPunching(Side side, float itemPower)
    {
        if (target)
            return;

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] aniFrameDatas = LeftPunchingAniData;
        Transform transform2 = _bodyHandler.LeftHand.transform;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _bodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (side == Side.Right)
        {
            aniFrameDatas = RightPunchingAniData;
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _bodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        for (int i = 0; i < aniFrameDatas.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
            AniForce(aniFrameDatas, i, dir, itemPower);
        }
    }

    public void ArmActionPunchResetting(Side side)
    {
        Transform partTransform = _bodyHandler.Chest.transform;

        AniAngleData[] aniAngleDatas = LeftPunchResettingAniData;
        
        if (side == Side.Left)
        {
            _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _bodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, false);
        }
        else
        {
            aniAngleDatas = RightPunchResettingAniData;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _bodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, false);
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            AniAngleForce(LeftPunchResettingAniData, i, dir);
        }
    }

    #endregion

    #region Stand
    public void Stand()
    {
        if (isStateChange)
        {
            _idleTimer = 0f;
        }
        if (_idleTimer < 30f)
        {
            _idleTimer = Mathf.Clamp(_idleTimer + Time.deltaTime, -60f, 30f);
        }
        if (_actor.actorState == Actor.ActorState.Run && !leftGrab && !rightGrab)
        {
        }
        else
        {
            AlignToVector(_bodyHandler.Head.PartRigidbody, -_bodyHandler.Head.transform.up, _moveDir + new Vector3(0f, 0.2f, 0f), 0.1f, 2.5f * 1);
            AlignToVector(_bodyHandler.Head.PartRigidbody, _bodyHandler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * 1);
            AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir, 0.1f, 4f * 1);
            AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 4f * 1);
            AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir, 0.1f, 4f * 1);
            AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Waist.transform.forward, Vector3.up, 0.1f, 4f * 1);
            AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 3f * 1);
        }
        
        //빙판이 아닐때 조건추가해야함
        if (_hips.velocity.magnitude > 1f)
            _hips.velocity = _hips.velocity.normalized * _hips.velocity.magnitude* 0.6f;
    }
    #endregion

    #region Jump
    public void Jump()
    {
        if(_actor.debuffState == Actor.DebuffState.Balloon)
        {
            if(_actor.actorState == Actor.ActorState.Jump)
            {
                for (int i = 0; i < MoveForceJumpAniData.Length; i++)
                {
                    AniForce(MoveForceJumpAniData, i, Vector3.up);
                    
                    if (i == 2)
                        AniForce(MoveForceJumpAniData, i, Vector3.down);
                    else
                        MoveForceJumpAniData[i].ForcePowerValues[0] = 500f;

                }
                for (int i = 0; i < MoveAngleJumpAniData.Length; i++)
                {
                    AniAngleForce(MoveAngleJumpAniData, i, _moveDir + new Vector3(0, 0.2f, 0f));
                }
            }
        }
        else
        {
            if (isStateChange)
            {
                isGrounded = false;
                for (int i = 0; i < MoveForceJumpAniData.Length; i++)
                {
                    AniForceVelocityChange(MoveForceJumpAniData, i, Vector3.up);
                    if (i == 2)
                        AniForce(MoveForceJumpAniData, i, Vector3.down);
                }
                for (int i = 0; i < MoveAngleJumpAniData.Length; i++)
                {
                    AniAngleForce(MoveAngleJumpAniData, i, _moveDir + new Vector3(0, 0.2f, 0f));
                }

               
            }
        }
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        _moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

        _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);


        _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 0.5f);
        if (_hips.velocity.magnitude > MaxSpeed)
            _hips.velocity = _hips.velocity.normalized * MaxSpeed;
        if (isGrounded)
        {
            _actor.actorState = Actor.ActorState.Stand;
        }
    }
    #endregion

    #region Heading
    IEnumerator Heading()
    {
        this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
        photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, true);

        for (int i = 0; i < HeadingAniData.Length; i++)
        {
            AniForce(HeadingAniData, i);
        }
        for (int i = 0; i < HeadingAngleAniData.Length; i++)
        {
            if (i == 0)
                AniAngleForce(HeadingAngleAniData, i, _moveDir + new Vector3(0f, 0.2f, 0f));
            if (i == 1)
                AniAngleForce(HeadingAngleAniData, i, _moveDir + new Vector3(0f, 0.2f, 0f));
        }

        yield return new WaitForSeconds(1);
        this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, false);
        isHeading = false;
    }
    #endregion

    #region MoveAnimation
    [PunRPC]
    public void Move()
    {
        if (_actor.actorState == ActorState.Run)
        {
            _cycleSpeed = 0.1f;
        }
        else
        {
            _cycleSpeed = 0.15f;
        }
        if (isStateChange)
        {
            if (Random.Range(0, 2) == 1)
            {
                leftLegPose = Pose.Bent;
                rightLegPose = Pose.Straight;
                leftArmPose = Pose.Straight;
                rightArmPose = Pose.Bent;
            }
            else
            {
                leftLegPose = Pose.Straight;
                rightLegPose = Pose.Bent;
                leftArmPose = Pose.Bent;
                rightArmPose = Pose.Straight;
            }
        }
        //Stand();
        RunCycleUpdate();
        RunCyclePoseBody();
        RunCyclePoseArm(Side.Left, leftArmPose);
        RunCyclePoseArm(Side.Right, rightArmPose);
        RunCyclePoseLeg(Side.Left, leftLegPose);
        RunCyclePoseLeg(Side.Right, rightLegPose);
    }

    private void RunCycleUpdate()
    {
        if (_cycleTimer < _cycleSpeed)
        {
            _cycleTimer += Time.deltaTime;
            return;
        }
        _cycleTimer = 0f;
        int num = (int)leftArmPose;
        num++;
        leftArmPose = ((num <= 3) ? ((Pose)num) : Pose.Bent);
        int num2 = (int)rightArmPose;
        num2++;
        rightArmPose = ((num2 <= 3) ? ((Pose)num2) : Pose.Bent);
        int num3 = (int)leftLegPose;
        num3++;
        leftLegPose = ((num3 <= 3) ? ((Pose)num3) : Pose.Bent);
        int num4 = (int)rightLegPose;
        num4++;
        rightLegPose = ((num4 <= 3) ? ((Pose)num4) : Pose.Bent);
    }

    private void RunCyclePoseLeg(Side side, Pose pose)
    {
        Transform hip = _bodyHandler.Hip.transform;
        Transform thighTrans = null;
        Transform legTrans = null;

        Rigidbody thighRigid = null;
        Rigidbody legRigid = null;
        Rigidbody footRigid = null;

        switch (side)
        {
            case Side.Left:
                thighTrans = _bodyHandler.LeftThigh.transform;
                legTrans = _bodyHandler.LeftLeg.transform;

                thighRigid = _bodyHandler.LeftThigh.GetComponent<Rigidbody>();
                legRigid = _bodyHandler.LeftLeg.PartRigidbody;
                footRigid = _bodyHandler.LeftFoot.PartRigidbody;
                break;
            case Side.Right:
                thighTrans = _bodyHandler.RightThigh.transform;
                legTrans = _bodyHandler.RightLeg.transform;
                thighRigid = _bodyHandler.RightThigh.PartRigidbody;
                legRigid = _bodyHandler.RightLeg.PartRigidbody;
                footRigid = _bodyHandler.RightFoot.PartRigidbody;
                break;
        }

        switch (pose)
        {
            case Pose.Bent:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
                break;
            case Pose.Forward:
                AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
                if (!isDuck)
                {
                    thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
                    legRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);
                }
                break;
            case Pose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                if (!isDuck)
                {
                    thighRigid.AddForce(hip.up * 2f * _applyedForce);
                    legRigid.AddForce(-hip.up * 2f * _applyedForce);
                    legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                }
                break;
            case Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                if (isDuck)
                {
                    _bodyHandler.Hip.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
                    _bodyHandler.Ball.PartRigidbody.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                    legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                }
                break;
        }
    }

    private void RunCyclePoseArm(Side side, Pose pose)
    {
        Vector3 vector = Vector3.zero;
        Transform partTransform = _bodyHandler.Chest.transform;
        Transform transform = null;
        Transform transform2 = null;
        Rigidbody rigidbody = null;
        Rigidbody rigidbody2 = null;
        Rigidbody rigidbody3 = null;

        float armForceCoef = 0.3f;
        float armForceRunCoef = 0.6f;
        switch (side)
        {
            case Side.Left:
                transform = _bodyHandler.LeftArm.transform;
                transform2 = _bodyHandler.LeftForearm.transform;
                rigidbody = _bodyHandler.LeftArm.PartRigidbody;
                rigidbody2 = _bodyHandler.LeftForearm.PartRigidbody;
                rigidbody3 = _bodyHandler.LeftHand.PartRigidbody;
                vector = _bodyHandler.Chest.transform.right;
                break;
            case Side.Right:
                transform = _bodyHandler.RightArm.transform;
                transform2 = _bodyHandler.RightForearm.transform;
                rigidbody = _bodyHandler.RightArm.PartRigidbody;
                rigidbody2 = _bodyHandler.RightForearm.PartRigidbody;
                rigidbody3 = _bodyHandler.RightHand.PartRigidbody;
                vector = -_bodyHandler.Chest.transform.right;
                break;
        }
        if (!isDuck && !isKickDuck && !isRun)
        {
            switch (pose)
            {
                case Pose.Bent:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Forward:
                    AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Straight:
                    AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
                case Pose.Behind:
                    AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                    AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                    break;
            }
            return;
        }
        switch (pose)
        {
            case Pose.Bent:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Forward:
                AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce);
                AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Straight:
                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
            case Pose.Behind:
                AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
                rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
                break;
        }
    }


    private void RunCyclePoseBody()
    {
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        _moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

        _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        if (isRun)
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 1.35f);
            if (_hips.velocity.magnitude > MaxSpeed)
                _hips.velocity = _hips.velocity.normalized * MaxSpeed * 1.15f;
        }
        else
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime);
            if (_hips.velocity.magnitude > MaxSpeed)
                _hips.velocity = _hips.velocity.normalized * MaxSpeed;
        }

    }
    #endregion

    #region AlingToVector
    public void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }
    #endregion

    #region Photon

    [PunRPC]
    private void UpdateDamageModifier(int bodyPart, bool isAttack)
    {
        //Debug.Log("[UpdateDamageModifier] isAttack: " + isAttack + ", bodyPart: " + bodyPart);

        switch((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.LeftFoot:
                if (isAttack)
                    this._bodyHandler.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.RightFoot:
                if (isAttack)
                    this._bodyHandler.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LeftLeg:
                if (isAttack)
                    this._bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.RightLeg: 
                if (isAttack)
                    this._bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LeftThigh: 
                break;
            case Define.BodyPart.RightThigh: 
                break;
            case Define.BodyPart.Hip: 
                break;
            case Define.BodyPart.Waist: 
                break;
            case Define.BodyPart.Chest: 
                break;
            case Define.BodyPart.Head:
                if (isAttack)
                    this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
                else
                    this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LeftArm: 
                break;
            case Define.BodyPart.RightArm: 
                break;
            case Define.BodyPart.LeftForeArm:
                break;
            case Define.BodyPart.RightForeArm:
                break;
            case Define.BodyPart.LeftHand:
                if (isAttack)
                    this._bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                else
                    this._bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.RightHand:
                if (isAttack)
                    this._bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                else
                    this._bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
        }
    }

    #endregion

    #region ItemTwoHand

    public void ItemTwoHandTrigger()
    {
        StartCoroutine(ItemTwoHandDelay(Side.Right));
    }

    IEnumerator ItemTwoHandDelay(Side side)
    {
        yield return ItemTwoHand(side, 0.07f, 0.1f, 0.3f, 0.1f, _itemSwingPower);
    }

    public IEnumerator ItemTwoHand(Side side, float duration, float readyTime, float punchTime, float resetTime, float itemPower)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ItemTwoHandReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ItemTwoHandSwing(side, itemPower);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ItemTwoHandReSet(side);
            yield return new WaitForSeconds(duration);
        }
    }
    #endregion

    #region ItemTwoHandAnimation

    public void ItemTwoHandReady(Side side)
    {
        //upperArm 2 chest1 up right 0.01 20 foreArm chest up back 
        //TestRready 오른쪽 왼쪽 구별해서 좌우로 휘두룰수 있음
        AniAngleData[] itemTwoHands = (side == Side.Right) ? ItemTwoHandAngleData : ItemTwoHandLeftAngleData;
        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            AniAngleForce(itemTwoHands, i);
        }
    }

    public void ItemTwoHandSwing(Side side, float itemSwingPower)
    {
        if (target)
            return;

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] itemTwoHands = ItemTwoHandLeftAniData;
        Transform transform2 = _bodyHandler.LeftHand.transform;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _bodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (side == Side.Right)
        {
            itemTwoHands = ItemTwoHandAniData;
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _bodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
            AniForce(itemTwoHands, i, dir , itemSwingPower);
        }
    }

    public void ItemTwoHandReSet(Side side)
    {
        Transform partTransform = _bodyHandler.Chest.transform;

        AniAngleData[] itemTwoHands = ItemTwoHandLeftAngleData;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _bodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

        if (side == Side.Right)
        {
            itemTwoHands = ItemTwoHandAngleData;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _bodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            AniAngleForce(itemTwoHands, i, dir);
        }
    }

    #endregion

    #region ItemOneHand
    public IEnumerator ItemOwnHand(Side side, float duration, float readyTime, float punchTime, float retime, float resetTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ItemOneHandReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ItemOneHandSwingReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < retime)
        {
            ItemOneHandSwing(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ItemOneHandReSet(side);
            yield return new WaitForSeconds(duration);
        }
    }
    #endregion

    #region ItemOneHandAni
    public void ItemOneHandReady(Side side)
    {
        AniAngleData[] itemTwoHands = (side == Side.Right) ? ItemOneHandAngleAniData : ItemOneHandAngleAniData;
        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            AniAngleForce(itemTwoHands, i);
        }
    }
    public void ItemOneHandSwingReady(Side side)
    {
        AniFrameData[] itemOneHands = ItemOneHandReadyAniData;
        for (int i = 0; i < itemOneHands.Length; i++)
        {
            AniForce(itemOneHands, i);
        }
    }
    public void ItemOneHandSwing(Side side)
    {
        AniFrameData[] itemOneHands = ItemOneHandSwingAniData;

        for (int i = 0; i < itemOneHands.Length; i++)
        {
            AniForce(itemOneHands, i);
        }
    }

    public void ItemOneHandReSet(Side side)
    {
        AniAngleData[] itemOneHands = ItemOneHandAngleAniData;

        for (int i = 0; i < itemOneHands.Length; i++)
        {

            AniAngleForce(itemOneHands, i);
        }
    }
    #endregion

    #region Potion
    public IEnumerator Potion(Side side, float duration, float ready, float start, float drinking, float end)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < ready)
        {
            PotionReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < start)
        {
            PotionStart(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < drinking)
        {
            PotionDrinking(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < end)
        {
            PotionEnd(side);
            yield return new WaitForSeconds(duration);
        }
    }

    #endregion

    #region PotionAni
    void PotionReady(Side side)
    {
        AniAngleData[] potionReadys = (side == Side.Right) ? PotionAngleAniData : PotionAngleAniData;
        for (int i = 0; i < potionReadys.Length; i++)
        {
            AniAngleForce(potionReadys, i);
        }
    }

    void PotionStart(Side side)
    {
        AniFrameData[] potionStarts = PotionReadyAniData;
        for (int i = 0; i < potionStarts.Length; i++)
        {
            AniForce(potionStarts, i);
        }
    }

    void PotionDrinking(Side side)
    {
        AniFrameData[] PotionDrinkings = PotionDrinkingAniData;
        for (int i = 0; i < PotionDrinkings.Length; i++)
        {
            AniForce(PotionDrinkings, i);
        }
    }

    void PotionEnd(Side side)
    {
        AniAngleData[] potionReadys = PotionAngleAniData;
        for (int i = 0; i < potionReadys.Length; i++)
        {
            AniAngleForce(potionReadys, i);
        }
    }
    #endregion

    #region PotionThrow

    public IEnumerator PotionThrow(float duration, float ready, float start,  float end)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < ready)
        {
            PotionThrowReady();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < start)
        {
            PotionThrowing();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < end)
        {
            PotionThrowEnd();
            yield return new WaitForSeconds(duration);
        }
    }

    #endregion

    #region PotionThrowAni

    void PotionThrowReady()
    {
        AniAngleData[] potionReadys = PotionThrowAngleData;
        for (int i = 0; i < potionReadys.Length; i++)
        {
            AniAngleForce(potionReadys, i);
        }
    }

    void PotionThrowing()
    {
        AniFrameData[] potionStarts = PotionThrowAniData;
        for (int i = 0; i < potionStarts.Length; i++)
        {
            AniForce(potionStarts, i);
        }
    }

    void PotionThrowEnd()
    {
        AniAngleData[] potionThrowEnds = PotionThrowAngleData;
        for (int i = 0; i < potionThrowEnds.Length; i++)
        {
            AniAngleForce(potionThrowEnds, i);
        }
    }
    #endregion

    #region RIP

    public IEnumerator DropRip(Side side, float duration, float readyTime, float punchTime, float retime, float resetTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            DropRipReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            DropRipUp(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < retime)
        {
            DropRipDown(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            DropRipReSet(side);
            yield return new WaitForSeconds(duration);
        }

    }

    #endregion

    #region RipAni

    void DropRipReady(Side side)
    {
        AniAngleData[] DropRips = (side == Side.Right) ? RipAngleAniData : RipAngleAniData;
        for (int i = 0; i < DropRips.Length; i++)
        {
            AniAngleForce(DropRips, i);
        }
    }

    void DropRipUp(Side side)
    {
        AniFrameData[] DropRips = PickUpRipAniData;
        for (int i = 0; i < DropRips.Length; i++)
        {
            AniForce(DropRips, i);
        }
    }

    void DropRipDown(Side side)
    {
        AniFrameData[] DropRips = ShootDownRipAniData;
        for (int i = 0; i < DropRips.Length; i++)
        {
            AniForce(DropRips, i);
        }
    }

    void DropRipReSet(Side side)
    {
        AniAngleData[] itemOneHands = RipAngleAniData;

        for (int i = 0; i < itemOneHands.Length; i++)
        {
            AniAngleForce(itemOneHands, i);
        }
    }
    #endregion
}