using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;
using System.Runtime.CompilerServices;
using static PlayerController;
using static Actor;
using static AniFrameData;
using static AniAngleData;

[System.Serializable]
public class AniFrameData
{
    public enum RollForce
    {
        Zero,
        Forward,
        Backward,
        Up,
        Down,
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
    public float[] AnglePowerValues;
    public float[] AngleStability;

}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public AniFrameData[] RollAniData;

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

    [Header("앞뒤 속도")]
    public float RunSpeed;

    [SerializeField]
    private Rigidbody _hips;
    [SerializeField]
    private Transform _cameraArm;

    [SerializeField]
    private BodyHandler bodyHandler;

    [SerializeField]
    private TargetingHandler targetingHandler;

    private Actor _actor;

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

    private Vector3 _moveInput;
    private Vector3 _moveDir;
    private bool _isCoroutineRunning = false;
    private bool _isCoroutineDrop = false;

    [SerializeField]
    private float _idleTimer = 0;
    private float _punchTimer = 0;
    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 800f;
    private Vector3 _runVectorForce2 = new Vector3(0f,0f,0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    public bool isAI = false;

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;

    Side _readySide = Side.Left;

    InteractableObject target;
    Vector3 _direction;
    Vector3 _angleDirection;
    Vector3 _targetDirection;

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
        if (isAI)
            return;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void Init()
    {
        bodyHandler = GetComponent<BodyHandler>();
        targetingHandler = GetComponent<TargetingHandler>();
        _actor = GetComponent<Actor>();
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        OnKeyboardEvent_Idle(evt);
    }

    void OnKeyboardEvent_Idle(Define.KeyboardEvent evt)
    {
       switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {

                }
                break;
            case Define.KeyboardEvent.PointerDown:
                {
                    
                }
                break;
            case Define.KeyboardEvent.PointerUp:
                {

                }
                break;
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.H)) 
                        Heading();
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        _actor.actorState = Actor.ActorState.Jump;
                    }
                }
                break;
            case Define.KeyboardEvent.Charge:
                {
                    MeowNyangPunch();
                }
                break;
            case Define.KeyboardEvent.Hold:
                {
                    //중일때 확인
                }
                break;
        }
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        OnMouseEvent_Idle(evt);
    }

    void OnMouseEvent_Idle(Define.MouseEvent evt)
    {
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {

                }
                break;
            case Define.MouseEvent.Press:
                {

                }
                break;
            case Define.MouseEvent.PointerUp:
                {

                }
                break;
            case Define.MouseEvent.Click:
                {
                    if(Input.GetMouseButtonUp(0))
                        PunchAndGrab();
                    if(!isGrounded && Input.GetMouseButtonUp(1))
                    {
                        DropKickTrigger();
                    }
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isAI)
            return;

        _moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (_actor.actorState != Actor.ActorState.Jump)
        {
            if (_moveInput.magnitude == 0f)
            {
                _actor.actorState = Actor.ActorState.Stand;
            }
            else
            {
                Stand();
                _actor.actorState = Actor.ActorState.Run;
            }
        }
        if (Input.GetMouseButton(2))
            ForwardRollTrigger();
    }

    private void Update()
    {
        CursorControll();
        LookAround();

        if (Input.GetKey(KeyCode.LeftShift))
            isRun = true;
        else
            isRun = false;
    }

    #region 기즈모
    private Rigidbody ray;

    //Vector3 direction = Vector3.forward;

    private void OnDrawGizmos()
    {
        //Debug.DrawRay(bodyHandler.Head.PartRigidbody.position, -bodyHandler.Head.transform.up * 10f, Color.red);
        //Debug.DrawRay(bodyHandler.Waist.PartRigidbody.position, bodyHandler.Waist.transform.up * 10f, Color.red);
        //Debug.DrawRay(bodyHandler.Head.transform.forward, Vector3.up * 10f, Color.green);
    }
    /*void Ray_1()
    {
        
        ray.origin = _hips.transform.position;
        ray.direction = -_hips.transform.right;
    }*/
    #endregion

    private void ForwardRollTrigger()
    {
        StartCoroutine(ForwardRollDelay(2f));
    }
    
    IEnumerator ForwardRollDelay(float delay)
    {
        StartCoroutine(ForwardRoll());

        yield return new WaitForSeconds(delay);

        //다시 회복
        //RestoreSpringTrigger();
        _actor.StatusHandler.StartCoroutine("RestoreFromFaint");
    }

    public float ForceStrength = 10.0f;

    IEnumerator ForwardRoll()
    {
        //스프링 풀기
        //ResetBodySpring();
        _actor.StatusHandler.StartCoroutine("Faint");


        int _frameCount = 0;
        for(int i =0; i< RollAniData.Length; i++)
        {
            _frameCount = i;
            AniForce(RollAniData, _frameCount);

            if(i == 3)
            {
                //스프링 복구
                //RestoreSpringTrigger();
                _actor.StatusHandler.StartCoroutine("RestoreFromFaint");
            }

        }

        yield return null;
    }

    void AniForce(AniFrameData[] _forceSpeed,int _elementCount)
    {
        if (_forceSpeed[_elementCount].StandardRigidbodies[0] == null)
        {
            for (int i = 0; i < _forceSpeed[_elementCount].ActionRigidbodies.Length; i++)
            {
                RollForce _rollState = _forceSpeed[_elementCount].ForceDirections[i];

                switch (_rollState)
                {
                    case RollForce.Zero:
                        _direction = new Vector3(0, 0, 0);
                        break;
                    case RollForce.Forward:
                        _direction = -_forceSpeed[_elementCount].ActionRigidbodies[i].transform.up;
                        break;
                    case RollForce.Backward:
                        _direction = _forceSpeed[_elementCount].ActionRigidbodies[i].transform.up;
                        break;
                    case RollForce.Up:
                        _direction = _forceSpeed[_elementCount].ActionRigidbodies[i].transform.forward;
                        break;
                    case RollForce.Down:
                        _direction = -_forceSpeed[_elementCount].ActionRigidbodies[i].transform.forward;
                        break;
                }
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
            }
        }
        else
            Debug.Log("StandardRigidbodies가 None이어야 합니다.");
    }

    void AniForce(AniFrameData[] _forceSpeed, int _elementCount,Vector3 _dir)
    {
        if (_forceSpeed[_elementCount].StandardRigidbodies[0] == null)
        {
            if(_forceSpeed[_elementCount].ForceDirections[0] == 0)
            {
                for (int i = 0; i < _forceSpeed[_elementCount].ActionRigidbodies.Length; i++)
                {
                    Debug.Log(_dir);
                    Debug.Log(_forceSpeed[_elementCount].ForcePowerValues[i]);
                    Debug.Log(ForceMode.Impulse);

                    _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ForcePowerValues[i], ForceMode.Impulse);
                }
            }
            
        }
        else
            Debug.Log("StandardRigidbodies가 None이어야 합니다.");
    }

    void AniAngleForce(AniAngleData[] _aniAngleData,int _elementCount)
    {
        for (int i = 0; i < _aniAngleData[_elementCount].ActionDirection.Length; i++)
        {
            AniAngle _angleState = _aniAngleData[_elementCount].ActionAngleDirections[i];
            AniAngle _targetangleState = _aniAngleData[_elementCount].TargetAngleDirections[i];

            switch (_angleState)
            {
                case AniAngle.Zero:
                    _angleDirection = new Vector3(0, 0, 0);
                    break;
                case AniAngle.Forward:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.up;
                    break;
                case AniAngle.Backward:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.up;
                    break;
                case AniAngle.Up:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.forward;
                    break;
                case AniAngle.Down:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.forward;
                    break;
                case AniAngle.Left:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.right;
                    break;
                case AniAngle.Right:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.right;
                    break;
            }
            switch (_targetangleState)
            {
                case AniAngle.Zero:
                    _targetDirection = new Vector3(0, 0, 0);
                    break;
                case AniAngle.Forward:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.up;
                    break;
                case AniAngle.Backward:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.up;
                    break;
                case AniAngle.Up:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.forward;
                    break;
                case AniAngle.Down:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.forward;
                    break;
                case AniAngle.Left:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.right;
                    break;
                case AniAngle.Right:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.right;
                    break;
            }
            AlignToVector(_aniAngleData[_elementCount].StandardRigidbodies[i], _angleDirection, _targetDirection,
                _aniAngleData[_elementCount].AngleStability[i], _aniAngleData[_elementCount].AnglePowerValues[i]);
        }
    }

    void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount, Vector3 _vector)
    {
        for (int i = 0; i < _aniAngleData[_elementCount].ActionDirection.Length; i++)
        {
            AniAngle _angleState = _aniAngleData[_elementCount].ActionAngleDirections[i];
            AniAngle _targetangleState = _aniAngleData[_elementCount].TargetAngleDirections[i];

            switch (_angleState)
            {
                case AniAngle.Zero:
                    _angleDirection = new Vector3(0, 0, 0);
                    break;
                case AniAngle.Forward:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.up;
                    break;
                case AniAngle.Backward:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.up;
                    break;
                case AniAngle.Up:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.forward;
                    break;
                case AniAngle.Down:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.forward;
                    break;
                case AniAngle.Left:
                    _angleDirection = -_aniAngleData[_elementCount].ActionDirection[i].transform.right;
                    break;
                case AniAngle.Right:
                    _angleDirection = _aniAngleData[_elementCount].ActionDirection[i].transform.right;
                    break;
            }
            switch (_targetangleState)
            {
                case AniAngle.Zero:
                    _targetDirection = new Vector3(0, 0, 0);
                    break;
                case AniAngle.Forward:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.up;
                    break;
                case AniAngle.Backward:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.up;
                    break;
                case AniAngle.Up:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.forward;
                    break;
                case AniAngle.Down:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.forward;
                    break;
                case AniAngle.Left:
                    _targetDirection = -_aniAngleData[_elementCount].TargetDirection[i].transform.right;
                    break;
                case AniAngle.Right:
                    _targetDirection = _aniAngleData[_elementCount].TargetDirection[i].transform.right;
                    break;
            }
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
        if(!_isCoroutineDrop)
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
        Vector3 zero = Vector3.zero;
        Transform partTransform = bodyHandler.Hip.transform;
        Transform transform2 = null;
        int _frameCount = 0;

        if (!isGrounded)
        {
            for(int i =0; i < DropAniData.Length; i++)
            {
                //스프링 풀기
                //ResetBodySpring();
                _actor.StatusHandler.StartCoroutine("Faint");


                _frameCount = i;
                if( i== 0 )
                {
                    transform2 = bodyHandler.RightFoot.transform;
                    bodyHandler.RightFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    bodyHandler.RightThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    zero = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(DropAniData, _frameCount, zero);
                }
                else if( i== 1 )
                {
                    transform2 = bodyHandler.LeftFoot.transform;
                    bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    //bodyHandler.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.Punch; 데미지
                    zero = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(DropAniData, _frameCount, zero);
                }
                else
                {
                    AniForce(DropAniData, _frameCount);
                }
            }
            yield return new WaitForSeconds(2);
            //스프링 복구
            //RestoreSpringTrigger();
            _actor.StatusHandler.StartCoroutine("RestoreFromFaint");

        }
        yield return null;
    }

    private void MeowNyangPunch()
    {
        StartCoroutine(MeowNyangPunchDelay());
    } 
    IEnumerator MeowNyangPunchDelay()
    {
        int _punchcount = 0;
        while (_punchcount < 5)
        {
            if (_readySide == Side.Left)
            {
                StartCoroutine(Punch(Side.Left));
                _readySide = Side.Right;
            }
            else
            {
                 StartCoroutine(Punch(Side.Right));
                _readySide = Side.Left;
            }
            yield return new WaitForSeconds(1f);
            _punchcount++;
        }
    }

    IEnumerator PunchWithDelay(Side side, float delay)
    {
        _isCoroutineRunning = true;
        StartCoroutine(Punch(side));
        yield return new WaitForSeconds(delay);
        _isCoroutineRunning = false;
    }

    private void PunchAndGrab()
    {
        targetingHandler.SearchTarget();
        if (!_isCoroutineRunning)
        {
            if (_readySide == Side.Left)
            {
                StartCoroutine(PunchWithDelay(Side.Left, 0.5f));
                _readySide = Side.Right;
            }
            else
            {
                StartCoroutine(PunchWithDelay(Side.Right, 0.5f));
                _readySide = Side.Left;
            }
        }
    }

    IEnumerator Punch(Side side)
    {
        while(_punchTimer <= 0.5f)
        {
            _punchTimer += Time.deltaTime;
            if (_punchTimer < 0.1f)
            {
                ArmActionReadying(side);
            }
            if (_punchTimer >= 0.2f && _punchTimer < 0.3f)
            {
                ArmActionPunching(side);
                yield return null;
            }
            if (_punchTimer >= 0.3f && _punchTimer < 0.5f)
            {
                ArmActionPunchResetting(side);
            }
        }
        _punchTimer = 0f;
        yield return null;
    }

    public void Stand()
    {
        if(isStateChange)
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
            AlignToVector(bodyHandler.Head.PartRigidbody, -bodyHandler.Head.transform.up, _moveDir + new Vector3(0f, 0.2f, 0f), 0.1f, 2.5f * 1);
            AlignToVector(bodyHandler.Head.PartRigidbody, bodyHandler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * 1);
            AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, _moveDir, 0.1f, 4f * 1);
            AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 4f * 1);
            AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, _moveDir, 0.1f, 4f * 1);
            AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Waist.transform.forward, Vector3.up, 0.1f, 4f * 1);
            AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 3f * 1);
        }
    }

    public void ArmActionReadying(Side side)
    {
        int _frameCount = 0;
        switch (side)
        {
            case Side.Left:
                for (int i = 0; i < RightPunchAniData.Length; i++)
                {
                    _frameCount = i;
                    AniAngleForce(RightPunchAniData, _frameCount);
                }
                break;
            case Side.Right:
                for (int i = 0; i < LeftPunchAniData.Length; i++)
                {
                    _frameCount = i;
                    AniAngleForce(LeftPunchAniData, _frameCount);
                }
                break;
        }
    }
    
    public void ArmActionPunching(Side side)
    {
        Transform partTransform = bodyHandler.Chest.transform;
        Transform transform2 = null;
        Vector3 zero = Vector3.zero;
        int _frameCount = 0;

        if (target == null)
        {
            switch (side)
            {
                case Side.Left:
                    for(int i = 0;i< LeftPunchingAniData.Length; i++)
                    {
                        _frameCount = i;
                        transform2 = bodyHandler.LeftHand.transform;
                        bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                        bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        bodyHandler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        zero = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                        AniForce(LeftPunchingAniData, _frameCount, zero);
                    }
                    break;
                case Side.Right:
                    for (int i = 0; i < RightPunchingAniData.Length; i++)
                    {
                        _frameCount = i;
                        transform2 = bodyHandler.RightHand.transform;
                        bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
                        bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        bodyHandler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                        zero = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                        AniForce(RightPunchingAniData, _frameCount, zero);
                    }
                    break;
            }
            return;
        }
    }

    public void ArmActionPunchResetting(Side side)
    {
        Transform partTransform = bodyHandler.Chest.transform;
        Vector3 vector = Vector3.zero;
        int _frameCount = 0;
        switch (side)
        {
            case Side.Left:
                for(int i = 0; i< LeftPunchResettingAniData.Length;i++)
                {
                    _frameCount = i;
                    vector = bodyHandler.Chest.transform.right / 2f;
                    bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                    bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    bodyHandler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    AniAngleForce(LeftPunchResettingAniData, _frameCount, vector);
                }
                break;
            case Side.Right:
                for (int i = 0; i < RightPunchResettingAniData.Length; i++)
                {
                    _frameCount = i;
                    vector = -bodyHandler.Chest.transform.right / 2f;
                    bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                    bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    bodyHandler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    AniAngleForce(RightPunchResettingAniData, _frameCount, vector);
                }
                break;
        }
    }
    
    public void Jump()
    {
        if (isStateChange)
        {
            isGrounded = false;
            int _frameCount;
            if (_moveDir != Vector3.zero)
            {
                for (int i = 0;i< MoveForceJumpAniData.Length;i++)
                {
                    _frameCount = i;
                    //AniForce(MoveForceJumpAniData, _frameCount, _moveDir); 헤드 추가 해주면 됨 values = 6
                    AniForce(MoveForceJumpAniData, _frameCount, Vector3.up);
                    if(i == 2)
                        AniForce(MoveForceJumpAniData, _frameCount, Vector3.down);
                }
            }
            for(int i = 0; i < MoveAngleJumpAniData.Length;i++)
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
        for (int i =0; i< HeadingAniData.Length ;i++) 
        {
            _frameCount = i;
            AniForce(HeadingAniData, _frameCount);
        }
        for(int i = 0; i< HeadingAngleAniData.Length ;i++)
        {
            _frameCount = i;
            if(i == 0) 
                AniAngleForce(HeadingAngleAniData, _frameCount, _moveDir + new Vector3(0f, 0.2f, 0f));
            if(i == 1)
                AniAngleForce(HeadingAngleAniData, _frameCount, _moveDir + new Vector3(0f, 0.2f, 0f));

        }
    }


    public void Move()
    {
        if (isRun)
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
        Transform hip =bodyHandler.Hip.transform;
        Transform thighTrans = null;
        Transform legTrans = null;

        Rigidbody thighRigid = null;
        Rigidbody legRigid = null;
        Rigidbody footRigid = null;

        switch (side)
        {
            case Side.Left:
                thighTrans = bodyHandler.LeftThigh.transform;
                legTrans = bodyHandler.LeftLeg.transform;
                
                thighRigid = bodyHandler.LeftThigh.GetComponent<Rigidbody>();
                legRigid = bodyHandler.LeftLeg.PartRigidbody;
                footRigid = bodyHandler.LeftFoot.PartRigidbody;
                break;
            case Side.Right:
                thighTrans = bodyHandler.RightThigh.transform;
                legTrans = bodyHandler.RightLeg.transform;
                thighRigid = bodyHandler.RightThigh.PartRigidbody;
                legRigid = bodyHandler.RightLeg.PartRigidbody;
                footRigid = bodyHandler.RightFoot.PartRigidbody;
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
                    footRigid.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                }
                break;
            case Pose.Behind:
                AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
                AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
                if (isDuck)
                {
                    bodyHandler.Hip.PartRigidbody.AddForce(_runVectorForce2, ForceMode.VelocityChange);
                    bodyHandler.Ball.PartRigidbody.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                    footRigid.AddForce(-_runVectorForce2 , ForceMode.VelocityChange);
                }
                break;
        }
    }

    private void RunCyclePoseArm(Side side, Pose pose)
    {
        Vector3 vector = Vector3.zero;
        Transform partTransform = bodyHandler.Chest.transform;
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
                transform = bodyHandler.LeftArm.transform;
                transform2 = bodyHandler.LeftForarm.transform;
                rigidbody = bodyHandler.LeftArm.PartRigidbody;
                rigidbody2 = bodyHandler.LeftForarm.PartRigidbody;
                rigidbody3 = bodyHandler.LeftHand.PartRigidbody;
                vector = bodyHandler.Chest.transform.right;
                break;
            case Side.Right:
                transform = bodyHandler.RightArm.transform;
                transform2 = bodyHandler.RightForarm.transform;
                rigidbody = bodyHandler.RightArm.PartRigidbody;
                rigidbody2 = bodyHandler.RightForarm.PartRigidbody;
                rigidbody3 = bodyHandler.RightHand.PartRigidbody;
                vector = -bodyHandler.Chest.transform.right;
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

        bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir) , ForceMode.VelocityChange);
        bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, -bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        if (isRun)
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * Time.deltaTime * 1.5f);
        }
        else
        {
            _hips.AddForce(_moveDir.normalized * RunSpeed * Time.deltaTime);
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

    //카메라 컨트롤
    private void LookAround()
    {
        _cameraArm.parent.transform.position = _hips.transform.position;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }

    private void CursorControll()
    {
        if (Input.anyKeyDown)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
