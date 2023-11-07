using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;
using System.Runtime.CompilerServices;
using static PlayerController;
using static Actor;
using static AniFrameData;
using static AniAngleData;
using Unity.VisualScripting;
using Photon.Pun;

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
    public AniFrameData[] TestRready1;

    [SerializeField]
    public AniAngleData[] TestRready2;

    [Header("앞뒤 속도")]
    public float RunSpeed;

    [SerializeField]
    private Rigidbody _hips;
    [SerializeField]
    private Transform _cameraArm;

    [SerializeField]
    private BodyHandler _bodyHandler;

    [SerializeField]
    private TargetingHandler targetingHandler;

    private Grab _grab;
    private Actor _actor;

    public float ReadyPunch = 0.1f;
    public float Punching = 0.1f;
    public float ResetPunch = 0.3f;

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
    public float RSkillDelayTime = 5;
    public float MaxSpeed = 10f;
    private float _runSpeedOffset = 350f;
    public float PunchDelay = 0.5f;

    private Vector3 _moveInput;
    private Vector3 _moveDir;
    private bool _isCoroutineRunning = false;
    private bool _isCoroutineDrop = false;
    private bool _isCoroutineRoll = false;

    [SerializeField]
    private float _idleTimer = 0;
    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 800f;
    private float _rollTime = 0;
    private Vector3 _runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    public bool isAI = false;

    Rigidbody _hipRB;

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;

    Side _readySide = Side.Left;

    InteractableObject target;
    Vector3 _direction;
    Vector3 _angleDirection;
    Vector3 _targetDirection;

    Rigidbody _childRigidbody;
    Transform[] _children;
    private Dictionary<Transform, Quaternion> _initialRotations = new Dictionary<Transform, Quaternion>();
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
    }

    private ConfigurableJoint[] childJoints;
    private ConfigurableJointMotion[] originalYMotions;
    private ConfigurableJointMotion[] originalZMotions;

    void Init()
    {
        _bodyHandler = GetComponent<BodyHandler>();
        targetingHandler = GetComponent<TargetingHandler>();
        _actor = GetComponent<Actor>();
        _hipRB = transform.Find("GreenHip").GetComponent<Rigidbody>();

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
    }

    void RestoreOriginalMotions()
    {
        for (int i = 0; i < childJoints.Length; i++)
        {
            childJoints[i].angularYMotion = originalYMotions[i];
            childJoints[i].angularZMotion = originalZMotions[i];
        }
    }

    void ForceRready()
    {
        for (int i = 0; i < TestRready1.Length; i++)
        {
            AniForce(TestRready1, i);
        }
    }

    IEnumerator Rready()
    {
        for (int i = 0; i < childJoints.Length; i++)
        {
            childJoints[i].angularYMotion = ConfigurableJointMotion.Locked;
            childJoints[i].angularZMotion = ConfigurableJointMotion.Locked;
        }

        for (int i = 0; i < TestRready2.Length; i++)
        {
            AniAngleForce(TestRready2, i);
        }
        yield return new WaitForSeconds(1f);
    }

    public void OnKeyboardEvent_Move(Define.KeyboardEvent evt)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    {
                        _moveInput.z = Input.GetAxis("Vertical");
                    }
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        _moveInput.x = Input.GetAxis("Horizontal");
                    }
                }
                break;
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                    {
                        _moveInput.z = 0;
                    }
                    if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    {
                        _moveInput.x = 0;
                    }
                }
                break;
        }

    }
    public void OnKeyboardEvent_Skill(Define.KeyboardEvent evt)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _actor.actorState = Actor.ActorState.Run;
                        isRun = true;
                    }
                }
                break;
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.LeftShift))
                    {
                        _actor.actorState = Actor.ActorState.Stand;
                        isRun = false;
                    }

                    if (Input.GetKeyUp(KeyCode.H))
                        Heading();
                    if (Input.GetKeyUp(KeyCode.Space))
                        _actor.actorState = Actor.ActorState.Jump;
                }
                break;
            case Define.KeyboardEvent.Charge:
                {
                    RestoreOriginalMotions();
                    if (Input.GetKeyUp(KeyCode.R))
                        MeowNyangPunch();
                }
                break;
            case Define.KeyboardEvent.Hold:
                {
                    //중일때 확인 ex 이펙트 출현하는 코드를 넣어주면 기모아지는 것 첨 될듯
                    StartCoroutine(Rready());
                    ForceRready();
                }
                break;
        }
    }

    public void OnMouseEvent_Grab(Define.MouseEvent evt)
    {
        if (!photonView.IsMine)
        {
            return;
        }

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
                        _grab.GrabReset();
                    }
                }
                break;
        }
    }
    public void OnMouseEvent_Skill(Define.MouseEvent evt)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {

                }
                break;
            case Define.MouseEvent.Click:
                {
                    if (Input.GetMouseButtonUp(0))
                        PunchAndGrab();
                    if (!isGrounded && Input.GetMouseButtonUp(1))
                        DropKickTrigger();
                    if (!_isCoroutineRoll && Input.GetMouseButtonUp(2))
                        ForwardRollTrigger();
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (isAI)
            return;

        if (_actor.actorState != Actor.ActorState.Jump && _actor.actorState != Actor.ActorState.Roll && _actor.actorState != Actor.ActorState.Run)
        {
            if (_moveInput.magnitude == 0f)
            {
                _actor.actorState = Actor.ActorState.Stand;
            }
            else
            {
                _actor.actorState = Actor.ActorState.Walk;
                Stand();
            }
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }
    
    private void TestCase()
    {
        StartCoroutine(testcase());
    }

    IEnumerator testcase()
    {
        for (int i = 0; i < TestRready2.Length; i++)
        {
            AniAngleForce(TestRready2, i);
        }

        yield return null;
    }

    private void ForwardRollTrigger()
    {
        if (!_isCoroutineRoll)
        {
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms)
            {
                _initialRotations[childTransform] = childTransform.localRotation;
            }
            _actor.actorState = Actor.ActorState.Jump;
            StartCoroutine(ForwardRollDelay(3f));
        }
    }

    IEnumerator ForwardRollDelay(float delay)
    {
        _isCoroutineRoll = true;
        yield return ForwardRoll(0.07f,1.5f);
        yield return new WaitForSeconds(delay);
        _isCoroutineRoll = false;
    }

    IEnumerator ForwardRoll(float duration, float readyRoll)
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
        //힘은 0, Rotation 복구 하기
        RestoreRotations();
        _actor.actorState = Actor.ActorState.Stand;
    }
    // 진행 순서 구르기 -> 회전하고 남는 힘 0 대입과 구르기 전 Rotation 값 넣기 -> Freeze Rotationx 축 잠금
    // -> 스프링값 올리기 (스프링 값을 천천히 올려야 누워 있다가 일어서는 것 처럼 보임)
    public void RestoreRotations()
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

                if (_childRigidbody.name == "GreenHip")
                    _hipRB.constraints |= RigidbodyConstraints.FreezeRotationX;
            }
        }
    }

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

    void AniForce(AniFrameData[] _forceSpeed, int _elementCount)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
            _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
        }
    }

    void AniForce(AniFrameData[] _forceSpeed, int _elementCount, Vector3 _dir)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ForceDirections[i] == RollForce.Zero || _forceSpeed[_elementCount].ForceDirections[i] == RollForce.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i] , ForceMode.Impulse);
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

    void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount, Vector3 _vector)
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

    IEnumerator TurnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

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
                }
                else if (i == 1)
                {
                    Transform transform2 = _bodyHandler.LeftFoot.transform;
                    _bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //데미지
                    Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(DropAniData, i, dir);
                }
                else
                {
                    AniForce(DropAniData, i);
                }
            }
            yield return new WaitForSeconds(2);
            _actor.StatusHandler.StartCoroutine("RestoreBodySpring");
            _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        }
        yield return null;
    }

    private void MeowNyangPunch()
    {
        StartCoroutine(MeowNyangPunchDelay());
        StartCoroutine(RDelay());
    }
    IEnumerator MeowNyangPunchDelay()
    {
        int _punchcount = 0;
        while (_punchcount < 5)
        {
            if (_readySide == Side.Left)
            {
                yield return MeowPunch(Side.Left, 0.07f, 0.1f, 0.1f, 0.3f);
                _readySide = Side.Right;
            }
            else
            {
                yield return MeowPunch(Side.Right, 0.07f, 0.1f, 0.1f, 0.3f);
                _readySide = Side.Left;
            }
            yield return new WaitForSeconds(0.2f);
            _punchcount++;
        }
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


    IEnumerator RDelay()
    {
        yield return new WaitForSeconds(RSkillDelayTime);
    }

    IEnumerator PunchWithDelay(Side side, float delay)
    {

        _isCoroutineRunning = true;
        yield return Punch(side, 0.07f, ReadyPunch, Punching, ResetPunch);
        yield return new WaitForSeconds(delay);
        _isCoroutineRunning = false;
    }

    public void PunchAndGrab()
    {
        targetingHandler.SearchTarget();
        if (!_isCoroutineRunning)
        {
            if (_readySide == Side.Left)
            {
                StartCoroutine(PunchWithDelay(Side.Left, PunchDelay));
                _readySide = Side.Right;
            }
            else
            {
                StartCoroutine(PunchWithDelay(Side.Right, PunchDelay));
                _readySide = Side.Left;
            }
        }
    }

    /*
    
    요구 사항 
    HP 잔량 체크 스테미너 - O
    키 입력시 스킬 동작 시간 빼기

    주먹 펀치 차징 멈추기
     */

    //값이 들어 오는게 0.01 0.1 0.1 0.3
    IEnumerator Punch(Side side, float duration, float readyTime, float punchTime,float resetTime)
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
    }

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
        if (target)
            return;

        Transform partTransform = _bodyHandler.Chest.transform;

        BodyPart bodyPartHand = _bodyHandler.LeftHand;
        BodyPart bodyPartForarm = _bodyHandler.LeftForarm;
        AniFrameData[] aniFrameDatas = LeftPunchingAniData;

        if(side == Side.Right)
        {
            bodyPartHand = _bodyHandler.RightHand;
            bodyPartForarm = _bodyHandler.RightForarm;
            aniFrameDatas = RightPunchingAniData;
        }

        for (int i = 0; i < aniFrameDatas.Length; i++)
        {
            Transform transform2 = bodyPartHand.transform;
            bodyPartHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            bodyPartHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            bodyPartForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
            AniForce(aniFrameDatas, i, dir);
        }
    }

    public void ArmActionPunchResetting(Side side)
    {
        Transform partTransform = _bodyHandler.Chest.transform;

        BodyPart bodyPartHand = _bodyHandler.LeftHand;
        BodyPart bodyPartForarm = _bodyHandler.LeftForarm;
        AniAngleData[] aniAngleDatas = LeftPunchResettingAniData;

        if (side == Side.Right)
        {
            bodyPartHand = _bodyHandler.RightHand;
            bodyPartForarm = _bodyHandler.RightForarm;
            aniAngleDatas = RightPunchResettingAniData;
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            bodyPartHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            bodyPartHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            bodyPartForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Vector3 dir = partTransform.transform.right / 2f;
            AniAngleForce(LeftPunchResettingAniData, i, dir);
        }
    }

    public void Jump()
    {
        if (isStateChange)
        {
            isGrounded = false;
            int _frameCount;
            for (int i = 0; i < MoveForceJumpAniData.Length; i++)
            {
                _frameCount = i;
                //AniForce(MoveForceJumpAniData, _frameCount, _moveDir); 헤드 추가 해주면 됨 values = 6
                AniForce(MoveForceJumpAniData, _frameCount, Vector3.up);
                if (i == 2)
                    AniForce(MoveForceJumpAniData, _frameCount, Vector3.down);
            }
            for (int i = 0; i < MoveAngleJumpAniData.Length; i++)
            {
                _frameCount = i;
                AniAngleForce(MoveAngleJumpAniData, _frameCount, _moveDir + new Vector3(0, 0.2f, 0f));
            }
        }
        if (isGrounded)
        {
            _actor.actorState = Actor.ActorState.Stand;
        }
    }

    private void Heading()
    {
        int _frameCount;
        for (int i = 0; i < HeadingAniData.Length; i++)
        {
            _frameCount = i;
            AniForce(HeadingAniData, _frameCount);
        }
        for (int i = 0; i < HeadingAngleAniData.Length; i++)
        {
            _frameCount = i;
            if (i == 0)
                AniAngleForce(HeadingAngleAniData, _frameCount, _moveDir + new Vector3(0f, 0.2f, 0f));
            if (i == 1)
                AniAngleForce(HeadingAngleAniData, _frameCount, _moveDir + new Vector3(0f, 0.2f, 0f));

        }
    }

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
        Stand();
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
                    footRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);
                }
                break;
            case Pose.Straight:
                AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
                if (!isDuck)
                {
                    thighRigid.AddForce(hip.up * 2f * _applyedForce);
                    footRigid.AddForce(-hip.up * 2f * _applyedForce);
                    footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                }
                break;
            case Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                if (isDuck)
                {
                    _bodyHandler.Hip.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
                    _bodyHandler.Ball.PartRigidbody.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
                    footRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);
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
                transform2 = _bodyHandler.LeftForarm.transform;
                rigidbody = _bodyHandler.LeftArm.PartRigidbody;
                rigidbody2 = _bodyHandler.LeftForarm.PartRigidbody;
                rigidbody3 = _bodyHandler.LeftHand.PartRigidbody;
                vector = _bodyHandler.Chest.transform.right;
                break;
            case Side.Right:
                transform = _bodyHandler.RightArm.transform;
                transform2 = _bodyHandler.RightForarm.transform;
                rigidbody = _bodyHandler.RightArm.PartRigidbody;
                rigidbody2 = _bodyHandler.RightForarm.PartRigidbody;
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
        _moveDir = lookForward * _moveInput.z + lookRight * _moveInput.x;

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
            _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 1.5f);
            if (_hips.velocity.magnitude > MaxSpeed)
                _hips.velocity = _hips.velocity.normalized * MaxSpeed * 1.5f;
        }
        else
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime);
            if (_hips.velocity.magnitude > MaxSpeed)
                _hips.velocity = _hips.velocity.normalized * MaxSpeed;
        }
    }

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
}
