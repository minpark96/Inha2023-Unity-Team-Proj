using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static Actor;
using static AniFrameData;
using static AniAngleData;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using System.IO;
using System;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class AniFrameData
{
    public enum ForceDirection
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
    public Rigidbody[] ActionRigidbodies;
    public ForceDirection[] ForceDirections;
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
    public Rigidbody[] ActionRigidbodies;
    public Transform[] StandardPart;
    public Transform[] TargetPart;
    public AniAngle[] StandardDirections;
    public AniAngle[] TargetDirections;
    public float[] AngleStability;
    public float[] AnglePowerValues;

}

public class PlayerController : MonoBehaviourPun
{


    Dictionary<string, AniFrameData[]> frameDataLists = new Dictionary<string, AniFrameData[]>();
    Dictionary<string, AniAngleData[]> angleDataLists = new Dictionary<string, AniAngleData[]>();

    Rigidbody StringToRigid(string text)
    {
        Rigidbody rb = new Rigidbody();
        Define.BodyPart part;
        if(Enum.TryParse(text, out part))
        {
            switch (part)
            {
                case Define.BodyPart.FootL:
                    rb = _bodyHandler.LeftFoot.PartRigidbody; break;
                case Define.BodyPart.FootR:
                    rb = _bodyHandler.RightFoot.PartRigidbody; break;
                case Define.BodyPart.LegLowerL:
                    rb = _bodyHandler.LeftLeg.PartRigidbody; break;
                case Define.BodyPart.LegLowerR:
                    rb = _bodyHandler.RightLeg.PartRigidbody; break;
                case Define.BodyPart.LegUpperL:
                    rb = _bodyHandler.LeftThigh.PartRigidbody; break;
                case Define.BodyPart.LegUpperR:
                    rb = _bodyHandler.RightThigh.PartRigidbody; break;
                case Define.BodyPart.Hip:
                    rb = _bodyHandler.Hip.PartRigidbody; break;
                case Define.BodyPart.Waist:
                    rb = _bodyHandler.Waist.PartRigidbody; break;
                case Define.BodyPart.Chest:
                    rb = _bodyHandler.Chest.PartRigidbody; break;
                case Define.BodyPart.Head:
                    rb = _bodyHandler.Head.PartRigidbody; break;
                case Define.BodyPart.LeftArm:
                    rb = _bodyHandler.LeftArm.PartRigidbody; break;
                case Define.BodyPart.RightArm:
                    rb = _bodyHandler.RightArm.PartRigidbody; break;
                case Define.BodyPart.LeftForeArm:
                    rb = _bodyHandler.LeftForeArm.PartRigidbody; break;
                case Define.BodyPart.RightForeArm:
                    rb = _bodyHandler.RightForeArm.PartRigidbody; break;
                case Define.BodyPart.LeftHand:
                    rb = _bodyHandler.LeftHand.PartRigidbody; break;
                case Define.BodyPart.RightHand:
                    rb = _bodyHandler.RightHand.PartRigidbody; break;
                default:
                    Debug.Log("애니메이션 파츠 불러오기 에러1" + part.ToString());
                    break;
            }
        }
        else
            Debug.Log("애니메이션 파츠 불러오기 에러2" + text);
        return rb;
    }
    ForceDirection StringToForceDir(string text)
    {
        ForceDirection dir = new ForceDirection();
        Define.AnimDirection eDirection;
        if (Enum.TryParse(text, out eDirection))
        {
            switch (eDirection)
            {
                case Define.AnimDirection.Zero:
                    dir = ForceDirection.Zero; break;
                case Define.AnimDirection.Forward:
                    dir = ForceDirection.Forward; break;
                case Define.AnimDirection.Backward:
                    dir = ForceDirection.Backward; break;
                case Define.AnimDirection.Up:
                    dir = ForceDirection.Up; break;
                case Define.AnimDirection.Down:
                    dir = ForceDirection.Down; break;
                case Define.AnimDirection.Right:
                    dir = ForceDirection.Right; break;
                case Define.AnimDirection.Left:
                    dir = ForceDirection.Left; break;
                default:
                    Debug.Log("포스방향 불러오기 에러1"); break;
            }
        }
        else
            Debug.Log("포스방향 불러오기 에러2"+text);

        return dir;
    }

    AniAngle StringToRotateDir(string text)
    {
        AniAngle dir = new AniAngle();
        Define.AnimDirection eDirection;
        if (Enum.TryParse(text, out eDirection))
        {
            switch (eDirection)
            {
                case Define.AnimDirection.Zero:
                    dir = AniAngle.Zero; break;
                case Define.AnimDirection.Forward:
                    dir = AniAngle.Forward; break;
                case Define.AnimDirection.Backward:
                    dir = AniAngle.Backward; break;
                case Define.AnimDirection.Up:
                    dir = AniAngle.Up; break;
                case Define.AnimDirection.Down:
                    dir = AniAngle.Down; break;
                case Define.AnimDirection.Right:
                    dir = AniAngle.Right; break;
                case Define.AnimDirection.Left:
                    dir = AniAngle.Left; break;
                default:
                    Debug.Log("포스방향 불러오기 에러1"); break;
            }
        }
        else
            Debug.Log("포스방향 불러오기 에러2" + text);

        return dir;
    }

    void LoadAnimForceData()
    {
        Define.AniFrameData[] frameDataNames = (Define.AniFrameData[])Enum.GetValues(typeof(Define.AniFrameData));
        int actionCount = 0;
        List<int> partCount = new List<int>();
        List<Rigidbody> standardRb = new List<Rigidbody>();
        List<Rigidbody> actionRb = new List<Rigidbody>();
        List<ForceDirection> forceDir = new List<ForceDirection>();
        List<int> forceVal = new List<int>();



        for (int i = 0; i < (int)Define.AniFrameData.End; i++)
        {
            string filePath = $"Animations/ForceData/{frameDataNames[i]}";
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            //리스트들 클리어해야함
            partCount.Clear();
            standardRb.Clear();
            actionRb.Clear();
            forceDir.Clear();
            forceVal.Clear();
            int index = 0;

            if (textAsset != null)
            {
                string[] lines = textAsset.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        Define.AnimForceValue frameValue;
                        Enum.TryParse(key, out frameValue);

                        if (frameValue == Define.AnimForceValue.ActionCount)
                            actionCount = int.Parse(value);
                        else if (frameValue == Define.AnimForceValue.PartCount)
                            partCount.Add(int.Parse(value));
                        else if (frameValue == Define.AnimForceValue.StandardPart)
                            standardRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimForceValue.ActionPart)
                            actionRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimForceValue.ForceDirection)
                            forceDir.Add(StringToForceDir(value));
                        else if (frameValue == Define.AnimForceValue.ForcePowerValues)
                            forceVal.Add(int.Parse(value));
                    }
                }

                AniFrameData[] datas = new AniFrameData[actionCount];

                for (int j = 0; j < actionCount; j++)
                {
                    AniFrameData data = new AniFrameData();
                    data.StandardRigidbodies = new Rigidbody[partCount[j]];
                    data.ActionRigidbodies = new Rigidbody[partCount[j]];
                    data.ForceDirections = new ForceDirection[partCount[j]];
                    data.ForcePowerValues = new float[partCount[j]];

                    for (int k = 0; k < partCount[j]; k++)
                    {
                        data.StandardRigidbodies[k] = standardRb[index];
                        data.ActionRigidbodies[k] = actionRb[index];
                        data.ForceDirections[k] = forceDir[index];
                        data.ForcePowerValues[k] = forceVal[index];
                        index++;
                    }
                    datas[j] = data;
                }
                frameDataLists[frameDataNames[i].ToString()] = datas;
            }
            else
                Debug.LogError("File not found: " + filePath);
        }
    }
    void LoadAnimRotateData()
    {
        Define.AniAngleData[] rotateDataNames = (Define.AniAngleData[])Enum.GetValues(typeof(Define.AniAngleData));
        int actionCount = 0;
        List<int> partCount = new List<int>();
        List<Rigidbody> actionRb = new List<Rigidbody>();
        List<Transform> standardPart = new List<Transform>();
        List<Transform> targetPart = new List<Transform>();
        List<AniAngle> standardDir = new List<AniAngle>();
        List<AniAngle> targetDir = new List<AniAngle>();
        List<float> stability = new List<float>();
        List<float> forceVal = new List<float>();



        for (int i = 0; i < (int)Define.AniFrameData.End; i++)
        {
            string filePath = $"Animations/RotateData/{rotateDataNames[i]}";
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            //리스트들 클리어해야함
            partCount.Clear();

            actionRb.Clear();
            standardPart.Clear();
            targetPart.Clear();
            standardDir.Clear();
            targetDir.Clear();

            stability.Clear();
            forceVal.Clear();

            int index = 0;

            if (textAsset != null)
            {
                string[] lines = textAsset.text.Split('\n');
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        Define.AnimRotateValue frameValue;
                        Enum.TryParse(key, out frameValue);

                        if (frameValue == Define.AnimRotateValue.ActionCount)
                            actionCount = int.Parse(value);
                        else if (frameValue == Define.AnimRotateValue.PartCount)
                            partCount.Add(int.Parse(value));
                        else if (frameValue == Define.AnimRotateValue.ActionRigidbodies)
                            actionRb.Add(StringToRigid(value));
                        else if (frameValue == Define.AnimRotateValue.StandardPart)
                            standardPart.Add(StringToRigid(value).transform);
                        else if (frameValue == Define.AnimRotateValue.TartgetPart)
                            targetPart.Add(StringToRigid(value).transform);
                        else if (frameValue == Define.AnimRotateValue.StandardDirections)
                            standardDir.Add(StringToRotateDir(value));
                        else if (frameValue == Define.AnimRotateValue.TargetDirections)
                            targetDir.Add(StringToRotateDir(value));
                        else if (frameValue == Define.AnimRotateValue.AngleStability)
                            stability.Add(float.Parse(value));
                        else if (frameValue == Define.AnimRotateValue.AnglePowerValues)
                            forceVal.Add(float.Parse(value));


                    }
                }

                AniAngleData[] datas = new AniAngleData[actionCount];

                for (int j = 0; j < actionCount; j++)
                {
                    AniAngleData data = new AniAngleData();
                    data.ActionRigidbodies = new Rigidbody[partCount[j]];
                    data.StandardPart = new Transform[partCount[j]];
                    data.TargetPart = new Transform[partCount[j]];
                    data.StandardDirections = new AniAngle[partCount[j]];
                    data.TargetDirections = new AniAngle[partCount[j]];
                    data.AngleStability = new float[partCount[j]];
                    data.AnglePowerValues = new float[partCount[j]];


                    for (int k = 0; k < partCount[j]; k++)
                    {
                        data.ActionRigidbodies[k] = actionRb[index];
                        data.StandardPart[k] = standardPart[index];
                        data.TargetPart[k] = targetPart[index];
                        data.StandardDirections[k] = standardDir[index];

                        data.TargetDirections[k] = targetDir[index];
                        data.AngleStability[k] = stability[index];
                        data.AnglePowerValues[k] = forceVal[index];
                        index++;
                    }
                    datas[j] = data;
                }
                angleDataLists[rotateDataNames[i].ToString()] = datas;
            }
            else
                Debug.LogError("File not found: " + filePath);
        }
    }
   



    [Header("Speed")]
    public float RunSpeed;
    private float MaxSpeed;

    [SerializeField]
    private Rigidbody _hips;

    public Transform _cameraArm;

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
    public bool isStateChange;
    public bool isMeowNyangPunch = false;
    public bool _isRSkillCheck;
    public bool isHeading;

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

    public float HeadingCoolTime = 1f;
    public float DropkickCoolTime = 2f;

    //차지 시간
    public float ChargeTime = 1.3f;

    public bool IsFlambe;

    private float _itemSwingPower;

    [Header("MoveControll")]
    private float _runSpeedOffset = 350f;
    public Vector3 MoveInput;
    private Vector3 _moveDir;
    private bool _isCoroutineRunning;
    public bool _isCoroutineDrop;
    private float _cycleTimer = 0;
    private float _cycleSpeed;
    private float _applyedForce = 800f;

    private Vector3 _runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    private Vector3 _runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    private Vector3 _runVectorForce10 = new Vector3(0f, 0f, 0.8f);

    public AudioSource _audioSource;
    AudioClip _audioClip;

    [Header("Dummy")]
    public bool isAI = false;

    Pose leftArmPose;
    Pose rightArmPose;
    Pose leftLegPose;
    Pose rightLegPose;

    Side _readySide = Side.Left;

    float startChargeTime;
    float endChargeTime = 0f;
    int _checkHoldTimeCount = 0;

    GameObject effectObject = null;
    Transform _playerTransform = null;
    float _drunkActionDuration = 3f;
    bool isTestCheck;

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
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
         
        childJoints = GetComponentsInChildren<ConfigurableJoint>();
        originalYMotions = new ConfigurableJointMotion[childJoints.Length];
        originalZMotions = new ConfigurableJointMotion[childJoints.Length];


        // 원래의 angularXMotion 값을 저장
        for (int i = 0; i < childJoints.Length; i++)
        {
            originalYMotions[i] = childJoints[i].angularYMotion;
            originalZMotions[i] = childJoints[i].angularZMotion;
        }
        _grab = GetComponent<Grab>();


        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");
        MaxSpeed = statData.MaxSpeed;
        RunSpeed = statData.RunSpeed;
        _itemSwingPower = statData.ItemSwingPower;


        LoadAnimForceData();
        LoadAnimRotateData();
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
                        if ((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted)
                            return;
                        //_actor.Stamina -= 5;


                        if (_actor.Stamina <= 0)
                            photonView.RPC("SetStemina", RpcTarget.MasterClient, 0f);

                        //_actor.Stamina = 0;

                        if ( _actor.actorState == Actor.ActorState.Jump && !_isCoroutineDrop)
                        {
                            photonView.RPC("DecreaseStamina", RpcTarget.MasterClient, 5f);
                            DropKickTrigger();
                        }
                    }
                }
                break;
            case Define.MouseEvent.Click:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        PunchAndGrab();
                    }

                    if (Input.GetMouseButtonUp(2) && _actor.Stamina >= 0)
                    {
                        if ((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted)
                            return;

                        if (_actor.Stamina <= 0)
                            photonView.RPC("SetStemina", RpcTarget.MasterClient, 0f);

                        //_actor.Stamina = 0;
                        if(!isHeading)
                        {
                            // _actor.Stamina -= 5;
                            photonView.RPC("DecreaseStamina", RpcTarget.MasterClient, 5f);
                            StartCoroutine(Heading());
                        }
                    }
                }
                break;
        }
    }
    #endregion

    #region OnKeyboardEvent_Move

    public void OnKeyboardEvent_Move(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (_actor.GrabState == Define.GrabState.Climb)
                            _actor.Grab.ClimbJump();
                        _actor.actorState = Actor.ActorState.Jump;
                    }

                    if ((_actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
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
                        photonView.RPC("DecreaseStamina", RpcTarget.MasterClient, 30f);

                        if (_actor.Stamina <= 0)
                        {
                            photonView.RPC("SetStemina", RpcTarget.MasterClient, 0f);
                            _actor.debuffState |= DebuffState.Exhausted;
                        }

                        if ((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted)
                            return;
                        else
                        {
                            if (!((_actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk))
                            //if (_actor.debuffState != DebuffState.Drunk)
                            {
                                if (!_isRSkillCheck)
                                {
                                    //EffectCreate("Effects/Love_aura");
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
                    if (Input.GetKey(KeyCode.LeftShift) && _actor.actorState!=ActorState.Jump && MoveInput.magnitude != 0)
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
                        //RSkillDestroyEffect("Love_aura");
                        photonView.RPC("RSkillDestroyEffect", RpcTarget.All, "Love_aura");
                    }
                }
                break;
            case Define.KeyboardEvent.Charge:
                {
                    if (Input.GetKeyUp(KeyCode.R) && (_actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk && IsFlambe)
                    {
                        photonView.RPC("DrunkAction", RpcTarget.All);
                        //StartCoroutine("DrunkAction");
                    }
                    else
                    {
                        photonView.RPC("RestoreOriginalMotions", RpcTarget.All);
                        if (Input.GetKeyUp(KeyCode.R) && isMeowNyangPunch)
                            MeowNyangPunch();
                        else
                            NuclearPunch();        
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
                        if ((_actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
                        {
                            //취함 애니메이션
                            StartCoroutine(DrunkActionReady());
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
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        _playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
    }

    [PunRPC]
    void RSkillDestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    [PunRPC]
    void RSkillMoveEffect()
    {
        effectObject.transform.position = _playerTransform.position;
    }

    #endregion

    #region Drunk

    IEnumerator DrunkActionReady()
    {
        _actor.BodyHandler.Head.PartRigidbody.AddForce(_actor.BodyHandler.Hip.PartTransform.up * 100f);

        yield return null;
    }

    [PunRPC]
    IEnumerator DrunkAction()
    {
        _playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        yield return StatusCreateEffect("Effects/Flamethrower");

        float startTime = Time.time;

        yield return Flamethrower(startTime, _drunkActionDuration);

        IsFlambe = false;

        yield return StatusDestroyEffect("Flamethrower");
        isTestCheck = false;
    }

    IEnumerator Flamethrower(float startTime, float drunkDuration)
    {
        isTestCheck = true;
        while (Time.time - startTime < drunkDuration)
        {
            yield return null;
        }
    }

    IEnumerator StatusCreateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        effectObject.transform.position = _playerTransform.position + _playerTransform.forward;
        effectObject.transform.rotation = Quaternion.LookRotation(-_playerTransform.right);

        yield return null;
    }

    IEnumerator StatusDestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
        yield return null;
    }

    [PunRPC]
    public void ASDStatusMoveEffect()
    {
        if (effectObject != null && _playerTransform.position != null)
        {
            effectObject.transform.position = _playerTransform.position + _playerTransform.forward;
            effectObject.transform.rotation = Quaternion.LookRotation(-_playerTransform.right);
        }
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

        for (int i = 0; i < angleDataLists[Define.AniAngleData.RSkillAngleAniData.ToString()].Length; i++)
        {
            AniAngleForce(angleDataLists[Define.AniAngleData.RSkillAngleAniData.ToString()], i);
        }
        StartCoroutine(ForceRready(ChargeAniHoldTime));
        yield return null;
    }

    IEnumerator ForceRready(float _delay)
    {
        startChargeTime = Time.time;
        for (int i = 0; i < frameDataLists[Define.AniFrameData.RSkillAniData.ToString()].Length; i++)
        {
            AniForce(frameDataLists[Define.AniFrameData.RSkillAniData.ToString()], i);
        }
        yield return new WaitForSeconds(_delay);
        //물체의 모션을 고정
        Rigidbody _RPartRigidbody;
        for (int i = 0; i < frameDataLists[Define.AniFrameData.RSkillAniData.ToString()].Length; i++)
        {
            for (int j = 0; j < frameDataLists[Define.AniFrameData.RSkillAniData.ToString()][i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = frameDataLists[Define.AniFrameData.RSkillAniData.ToString()][i].ActionRigidbodies[j];
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

        for (int i = 0; i < frameDataLists[Define.AniFrameData.RSkillAniData.ToString()].Length; i++)
        {
            for (int j = 0; j < frameDataLists[Define.AniFrameData.RSkillAniData.ToString()][i].StandardRigidbodies.Length; j++)
            {
                _RPartRigidbody = frameDataLists[Define.AniFrameData.RSkillAniData.ToString()][i].ActionRigidbodies[j];
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
        photonView.RPC("DestroyEffect", RpcTarget.All, "Love_aura");
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
        photonView.RPC("DestroyEffect", RpcTarget.All, "Love_aura");
        StartCoroutine(MeowNyangPunchDelay());
        photonView.RPC("ResetCharge", RpcTarget.All);
    }

    IEnumerator MeowNyangPunchDelay()
    {
        int _punchcount = 0;
        _readySide = Side.Right;
        while (_punchcount < 5)
        {
            photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/WEAPON_Spear");
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
    [PunRPC]
    void CheckSever()
    {
        if (PhotonNetwork.IsConnected)
            Debug.Log("Photon is Connected!");
        else
            Debug.Log("Photon is not Connected!");
    }

    private void FixedUpdate()
    {
        photonView.RPC("CheckSever", RpcTarget.All);

        if (effectObject != null && IsFlambe && isTestCheck)
        {
            photonView.RPC("ASDStatusMoveEffect", RpcTarget.All);
        }
        else if(effectObject != null && _isRSkillCheck)
        {
            //RSkillMoveEffect();
            photonView.RPC("RSkillMoveEffect", RpcTarget.All);
        }

        if (_isRSkillCheck == true)
        {
            if ((_actor.debuffState & DebuffState.Stun) == DebuffState.Stun ||
                (_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
                (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
                (_actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk
                )
            {
                photonView.RPC("ResetCharge", RpcTarget.All);
                _isRSkillCheck = false;
            }
        }
        
        if (!photonView.IsMine || _actor.actorState == ActorState.Dead) return;

        if (isAI)
            return;

        if (_actor.actorState != Actor.ActorState.Jump && _actor.actorState != Actor.ActorState.Roll 
            && _actor.actorState != Actor.ActorState.Run )//&& _actor.actorState != ActorState.Unconscious)
        {
            if(!((_actor.debuffState & DebuffState.Stun) == DebuffState.Stun))
            {
                if (MoveInput.magnitude == 0f)
                {
                    _actor.actorState = Actor.ActorState.Stand;
                }
                else
                {
                    _actor.actorState = Actor.ActorState.Walk;

                    //Stand();
                }
            }
        }
    }

    #endregion

    #region Animation Direction Force Angle

    Vector3 GetForceDirection(AniFrameData data, int index)
    {
        ForceDirection _rollState = data.ForceDirections[index];
        Vector3 _direction;

        switch (_rollState)
        {
            case ForceDirection.Zero:
                _direction = new Vector3(0, 0, 0);
                break;
            case ForceDirection.Forward:
                _direction = -data.StandardRigidbodies[index].transform.up;
                break;
            case ForceDirection.Backward:
                _direction = data.StandardRigidbodies[index].transform.up;
                break;
            case ForceDirection.Up:
                _direction = data.StandardRigidbodies[index].transform.forward;
                break;
            case ForceDirection.Down:
                _direction = -data.StandardRigidbodies[index].transform.forward;
                break;
            case ForceDirection.Left:
                _direction = -data.StandardRigidbodies[index].transform.right;
                break;
            case ForceDirection.Right:
                _direction = data.StandardRigidbodies[index].transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }
        return _direction;
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

    void AniForce(AniFrameData[] aniForceData, int _elementCount, Vector3 _dir = default, float _punchpower = 1f)
    {
        for (int i = 0; i < aniForceData[_elementCount].StandardRigidbodies.Length; i++)
        {
            if (aniForceData[_elementCount].ForceDirections[i] == ForceDirection.Zero)
            {
                aniForceData[_elementCount].ActionRigidbodies[i].AddForce(_dir * aniForceData[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
            else
            {
                Vector3 _direction = GetForceDirection(aniForceData[_elementCount], i);
                aniForceData[_elementCount].ActionRigidbodies[i].AddForce(_direction * aniForceData[_elementCount].ForcePowerValues[i] * _punchpower, ForceMode.Impulse);
            }
        }
    }

    public void AniAngleForce(AniAngleData[] _aniAngleData, int _elementCount, Vector3 _vector = default)//default는 vector3.zero
    {
        for (int i = 0; i < _aniAngleData[_elementCount].ActionRigidbodies.Length; i++)
        {
            Vector3 _angleDirection = GetAngleDirection(_aniAngleData[_elementCount].StandardDirections[i],
                _aniAngleData[_elementCount].StandardPart[i]);
            Vector3 _targetDirection = GetAngleDirection(_aniAngleData[_elementCount].TargetDirections[i],
                _aniAngleData[_elementCount].TargetPart[i]);

            AlignToVector(_aniAngleData[_elementCount].ActionRigidbodies[i], _angleDirection, _vector + _targetDirection,
                _aniAngleData[_elementCount].AngleStability[i], _aniAngleData[_elementCount].AnglePowerValues[i]);
        }
    }

    #endregion

    #region DropKick

    private void DropKickTrigger()
    {
        if (!_isCoroutineDrop)
            StartCoroutine(DropKickDelay(HeadingCoolTime));
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
            for (int i = 0; i < frameDataLists[Define.AniFrameData.DropAniData.ToString()].Length; i++)
            {
                _actor.StatusHandler.StartCoroutine("ResetBodySpring");

                if (i == 0)
                {
                    Transform transform2 = _bodyHandler.RightFoot.transform;
                    _bodyHandler.RightFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    _bodyHandler.RightThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //데미지
                    Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(frameDataLists[Define.AniFrameData.DropAniData.ToString()], i, dir);
                    photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerR, true);
                }
                else if (i == 1)
                {
                    Transform transform2 = _bodyHandler.LeftFoot.transform;
                    _bodyHandler.LeftFoot.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    _bodyHandler.LeftThigh.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick; //데미지
                    Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
                    AniForce(frameDataLists[Define.AniFrameData.DropAniData.ToString()], i, dir);
                    photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerL, true);
                }
                else
                {
                    AniForce(frameDataLists[Define.AniFrameData.DropAniData.ToString()], i);
                }
            }

            yield return new WaitForSeconds(2);
            _actor.StatusHandler.StartCoroutine("RestoreBodySpring", 1f);
            _bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerL, false);
            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LegLowerR, false);
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
        AniAngleData[] aniAngleDatas = (side == Side.Right) ? angleDataLists[Define.AniAngleData.RightPunchAniData.ToString()] : angleDataLists[Define.AniAngleData.LeftPunchAniData.ToString()];
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
            aniFrameDatas = frameDataLists[Define.AniFrameData.LeftPunchingAniData.ToString()];
            transform2 = _bodyHandler.LeftHand.transform;
            if (_isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, true);
        }
        else
        {
            aniFrameDatas = frameDataLists[Define.AniFrameData.RightPunchingAniData.ToString()];
            transform2 = _bodyHandler.RightHand.transform;
            if (_isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

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
        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] aniFrameDatas = frameDataLists[Define.AniFrameData.LeftPunchingAniData.ToString()];
        Transform transform2 = _bodyHandler.LeftHand.transform;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (side == Side.Right)
        {
            aniFrameDatas = frameDataLists[Define.AniFrameData.RightPunchingAniData.ToString()];
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
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

        AniAngleData[] aniAngleDatas = angleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()];
        
        if (side == Side.Left)
        {
            _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, false);
        }
        else
        {
            aniAngleDatas = angleDataLists[Define.AniAngleData.RightPunchResettingAniData.ToString()];
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, false);
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            AniAngleForce(angleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()], i, dir);
        }
    }

    #endregion

    #region Stand
    public void Stand()
    {
        AlignToVector(_bodyHandler.Head.PartRigidbody, -_bodyHandler.Head.transform.up, _moveDir + new Vector3(0f, 0.2f, 0f), 0.1f, 2.5f * 1);
        AlignToVector(_bodyHandler.Head.PartRigidbody, _bodyHandler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * 1);
        AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir, 0.1f, 4f * 1);
        AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 4f * 1);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir, 0.1f, 4f * 1);
        AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Waist.transform.forward, Vector3.up, 0.1f, 4f * 1);
        AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 3f * 1);

        //빙판이 아닐때 조건추가해야함
        if (_hips.velocity.magnitude > 1f)
            _hips.velocity = _hips.velocity.normalized * _hips.velocity.magnitude* 0.6f;
    }
    #endregion

    #region Jump
    public void Jump()
    {
        //if (isStateChange)
        //{
            
        //    isGrounded = false;
        //    for (int i = 0; i < frameDataLists[Define.AniFrameData.JumpAniForceData.ToString()].Length; i++)
        //    {
        //        AniForce(frameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.up);
        //        if (i == 2)
        //            AniForce(frameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.down);
        //    }
        //    for (int i = 0; i < angleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()].Length; i++)
        //    {
        //        AniAngleForce(angleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()], i, _moveDir + new Vector3(0, 0.2f, 0f));
        //    }
        //}
        

        ////방향서치,상태에서 하는게 아니라 _moveDir만 actor등에서 알아서 업데이트 하면서 가지고 있어야함
        //Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        //Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        //_moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

        //_bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
        //_bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

        //AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        //AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        //AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
        //AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
        //AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
        //AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

        ////Fall상태로 빼야 할수도
        //_hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 0.5f);

        //if (_hips.velocity.magnitude > MaxSpeed)
        //    _hips.velocity = _hips.velocity.normalized * MaxSpeed;

        ////상태나가기
        //if (isGrounded)
        //{
        //    _actor.actorState = Actor.ActorState.Stand;
        //}
    }
    #endregion

    #region Heading
    IEnumerator Heading()
    {
        isHeading = true;

        this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
        photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, true);

        for (int i = 0; i < frameDataLists[Define.AniFrameData.HeadingAniData.ToString()].Length; i++)
        {
            AniForce(frameDataLists[Define.AniFrameData.HeadingAniData.ToString()], i);
        }
        for (int i = 0; i < angleDataLists[Define.AniAngleData.HeadingAngleAniData.ToString()].Length; i++)
        {
            if (i == 0)
                AniAngleForce(angleDataLists[Define.AniAngleData.HeadingAngleAniData.ToString()], i, _moveDir + new Vector3(0f, 0.2f, 0f));
            if (i == 1)
                AniAngleForce(angleDataLists[Define.AniAngleData.HeadingAngleAniData.ToString()], i, _moveDir + new Vector3(0f, 0.2f, 0f));
        }

        yield return new WaitForSeconds(HeadingCoolTime);
        this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, false);

        isHeading = false;
    }
    #endregion

    #region MoveAnimation

    //public void Move()
    //{
    //    if(MoveInput.magnitude == 0f)
    //        _actor.actorState = Actor.ActorState.Stand;
        
    //    if (_actor.actorState == ActorState.Run)
    //    {
    //        _cycleSpeed = 0.1f;
    //    }
    //    else
    //    {
    //        _cycleSpeed = 0.15f;
    //    }
    //    if (isStateChange)
    //    {
    //        if (UnityEngine.Random.Range(0, 2) == 1)
    //        {
    //            leftLegPose = Pose.Bent;
    //            rightLegPose = Pose.Straight;
    //            leftArmPose = Pose.Straight;
    //            rightArmPose = Pose.Bent;
    //        }
    //        else
    //        {
    //            leftLegPose = Pose.Straight;
    //            rightLegPose = Pose.Bent;
    //            leftArmPose = Pose.Bent;
    //            rightArmPose = Pose.Straight;
    //        }
    //    }
    //    //Stand();
    //    RunCycleUpdate();
    //    RunCyclePoseBody();
    //    RunCyclePoseArm(Side.Left, leftArmPose);
    //    RunCyclePoseArm(Side.Right, rightArmPose);
    //    RunCyclePoseLeg(Side.Left, leftLegPose);
    //    RunCyclePoseLeg(Side.Right, rightLegPose);
    //}

    //private void RunCycleUpdate()
    //{
    //    if (_cycleTimer < _cycleSpeed)
    //    {
    //        _cycleTimer += Time.deltaTime;
    //        return;
    //    }
    //    _cycleTimer = 0f;
    //    int num = (int)leftArmPose;
    //    num++;
    //    leftArmPose = ((num <= 3) ? ((Pose)num) : Pose.Bent);
    //    int num2 = (int)rightArmPose;
    //    num2++;
    //    rightArmPose = ((num2 <= 3) ? ((Pose)num2) : Pose.Bent);
    //    int num3 = (int)leftLegPose;
    //    num3++;
    //    leftLegPose = ((num3 <= 3) ? ((Pose)num3) : Pose.Bent);
    //    int num4 = (int)rightLegPose;
    //    num4++;
    //    rightLegPose = ((num4 <= 3) ? ((Pose)num4) : Pose.Bent);
    //}

    //private void RunCyclePoseLeg(Side side, Pose pose)
    //{
    //    Transform hip = _bodyHandler.Hip.transform;
    //    Transform thighTrans = null;
    //    Transform legTrans = null;

    //    Rigidbody thighRigid = null;
    //    Rigidbody legRigid = null;

    //    switch (side)
    //    {
    //        case Side.Left:
    //            thighTrans = _bodyHandler.LeftThigh.transform;
    //            legTrans = _bodyHandler.LeftLeg.transform;

    //            thighRigid = _bodyHandler.LeftThigh.GetComponent<Rigidbody>();
    //            legRigid = _bodyHandler.LeftLeg.PartRigidbody;
    //            break;
    //        case Side.Right:
    //            thighTrans = _bodyHandler.RightThigh.transform;
    //            legTrans = _bodyHandler.RightLeg.transform;
    //            thighRigid = _bodyHandler.RightThigh.PartRigidbody;
    //            legRigid = _bodyHandler.RightLeg.PartRigidbody;
    //            break;
    //    }

    //    switch (pose)
    //    {
    //        case Pose.Bent:
    //            AlignToVector(thighRigid, -thighTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, legTrans.forward, _moveDir, 0.1f, 2f * _applyedForce);
    //            break;
    //        case Pose.Forward:
    //            AlignToVector(thighRigid, -thighTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
    //            AlignToVector(legRigid, -legTrans.forward, _moveDir + -hip.up / 2f, 0.1f, 4f * _applyedForce);
    //            thighRigid.AddForce(-_moveDir / 2f, ForceMode.VelocityChange);
    //            legRigid.AddForce(_moveDir / 2f, ForceMode.VelocityChange);

    //            break;
    //        case Pose.Straight:
    //            AlignToVector(thighRigid, thighTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, legTrans.forward, Vector3.up, 0.1f, 2f * _applyedForce);
    //            thighRigid.AddForce(hip.up * 2f * _applyedForce);
    //            legRigid.AddForce(-hip.up * 2f * _applyedForce);
    //            legRigid.AddForce(-_runVectorForce2, ForceMode.VelocityChange);

    //            break;
    //        case Pose.Behind:
    //            AlignToVector(thighRigid, thighTrans.forward, _moveDir * 2f, 0.1f, 2f * _applyedForce);
    //            AlignToVector(legRigid, -legTrans.forward, -_moveDir * 2f, 0.1f, 2f * _applyedForce);
    //            break;
    //    }
    //}

    //private void RunCyclePoseArm(Side side, Pose pose)
    //{
    //    Vector3 vector = Vector3.zero;
    //    Transform partTransform = _bodyHandler.Chest.transform;
    //    Transform transform = null;
    //    Transform transform2 = null;
    //    Rigidbody rigidbody = null;
    //    Rigidbody rigidbody2 = null;
    //    Rigidbody rigidbody3 = null;

    //    float armForceCoef = 0.3f;
    //    float armForceRunCoef = 0.6f;
    //    switch (side)
    //    {
    //        case Side.Left:
    //            transform = _bodyHandler.LeftArm.transform;
    //            transform2 = _bodyHandler.LeftForeArm.transform;
    //            rigidbody = _bodyHandler.LeftArm.PartRigidbody;
    //            rigidbody2 = _bodyHandler.LeftForeArm.PartRigidbody;
    //            rigidbody3 = _bodyHandler.LeftHand.PartRigidbody;
    //            vector = _bodyHandler.Chest.transform.right;
    //            break;
    //        case Side.Right:
    //            transform = _bodyHandler.RightArm.transform;
    //            transform2 = _bodyHandler.RightForeArm.transform;
    //            rigidbody = _bodyHandler.RightArm.PartRigidbody;
    //            rigidbody2 = _bodyHandler.RightForeArm.PartRigidbody;
    //            rigidbody3 = _bodyHandler.RightHand.PartRigidbody;
    //            vector = -_bodyHandler.Chest.transform.right;
    //            break;
    //    }
    //    if (!isRun)
    //    {
    //        switch (pose)
    //        {
    //            case Pose.Bent:
    //                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, -_moveDir / 4f, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Forward:
    //                AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, _moveDir / 4f + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Straight:
    //                AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //            case Pose.Behind:
    //                AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //                AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //                break;
    //        }
    //        return;
    //    }
    //    switch (pose)
    //    {
    //        case Pose.Bent:
    //            AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, -_moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-_moveDir * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(_moveDir * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Forward:
    //            AlignToVector(rigidbody, transform.forward, _moveDir + -vector, 0.1f, 4f * _applyedForce);
    //            AlignToVector(rigidbody2, transform2.forward, _moveDir + -partTransform.forward + -vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Straight:
    //            AlignToVector(rigidbody, transform.forward, partTransform.forward + vector, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody2.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //        case Pose.Behind:
    //            AlignToVector(rigidbody, transform.forward, _moveDir, 0.1f, 4f * _applyedForce * armForceCoef);
    //            AlignToVector(rigidbody2, transform2.forward, partTransform.forward, 0.1f, 4f * _applyedForce * armForceCoef);
    //            rigidbody.AddForce(-Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            rigidbody3.AddForce(Vector3.up * armForceRunCoef, ForceMode.VelocityChange);
    //            break;
    //    }
    //}


    //private void RunCyclePoseBody()
    //{
    //    Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
    //    Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
    //    _moveDir = lookForward * MoveInput.z + lookRight * MoveInput.x;

    //    _bodyHandler.Chest.PartRigidbody.AddForce((_runVectorForce10 + _moveDir), ForceMode.VelocityChange);
    //    _bodyHandler.Hip.PartRigidbody.AddForce((-_runVectorForce5 + -_moveDir), ForceMode.VelocityChange);

    //    AlignToVector(_bodyHandler.Chest.PartRigidbody, -_bodyHandler.Chest.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
    //    AlignToVector(_bodyHandler.Chest.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Waist.PartRigidbody, -_bodyHandler.Waist.transform.up, _moveDir / 4f + -Vector3.up, 0.1f, 4f * _applyedForce);
    //    AlignToVector(_bodyHandler.Waist.PartRigidbody, _bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Hip.PartRigidbody, -_bodyHandler.Hip.transform.up, _moveDir, 0.1f, 8f * _applyedForce);
    //    AlignToVector(_bodyHandler.Hip.PartRigidbody, _bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * _applyedForce);

    //    if (isRun)
    //    {
    //        _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime * 1.35f);
    //        if (_hips.velocity.magnitude > MaxSpeed)
    //            _hips.velocity = _hips.velocity.normalized * MaxSpeed * 1.15f;
    //    }
    //    else
    //    {
    //        _hips.AddForce(_moveDir.normalized * RunSpeed * _runSpeedOffset * Time.deltaTime);
    //        if (_hips.velocity.magnitude > MaxSpeed)
    //            _hips.velocity = _hips.velocity.normalized * MaxSpeed;
    //    }

    //}
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
            case Define.BodyPart.FootL:
                if (isAttack)
                    this._bodyHandler.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.LeftFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.FootR:
                if (isAttack)
                    this._bodyHandler.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.RightFoot.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegLowerL:
                if (isAttack)
                    this._bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.LeftLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegLowerR: 
                if (isAttack)
                    this._bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.DropKick;
                else
                    this._bodyHandler.RightLeg.PartInteractable.damageModifier = InteractableObject.Damage.Default;
                break;
            case Define.BodyPart.LegUpperL: 
                break;
            case Define.BodyPart.LegUpperR: 
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


    public IEnumerator ItemTwoHand(Side side, float duration, float readyTime, float punchTime, float resetTime, float itemPower)
    {
        photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/WEAPON_Axe");
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
        AniAngleData[] itemTwoHands = (side == Side.Right) ? angleDataLists[Define.AniAngleData.ItemTwoHandAngleData.ToString()] : angleDataLists[Define.AniAngleData.ItemTwoHandLeftAngleData.ToString()];
        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            AniAngleForce(itemTwoHands, i);
        }
    }

    public void ItemTwoHandSwing(Side side, float itemSwingPower)
    {

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] itemTwoHands = frameDataLists[Define.AniFrameData.ItemTwoHandLeftAniData.ToString()];
        Transform transform2 = _bodyHandler.LeftHand.transform;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (side == Side.Right)
        {
            itemTwoHands = frameDataLists[Define.AniFrameData.ItemTwoHandAniData.ToString()];
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
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

        AniAngleData[] itemTwoHands = angleDataLists[Define.AniAngleData.ItemTwoHandLeftAngleData.ToString()];
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (side == Side.Right)
        {
            itemTwoHands = angleDataLists[Define.AniAngleData.ItemTwoHandAngleData.ToString()];
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            AniAngleForce(itemTwoHands, i, dir);
        }
    }

    #endregion

    #region Potion
    public IEnumerator Potion(float duration, float ready, float start, float drinking, float end)
    {
        photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/Item_UI_042");

        float checkTime = Time.time;

        while (Time.time - checkTime < ready)
        {
            PotionReady();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < start)
        {
            PotionStart();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < drinking)
        {
            PotionDrinking();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < end)
        {
            PotionEnd();
            yield return new WaitForSeconds(duration);
        }
    }

    #endregion

    #region PotionAni
    void PotionReady()
    {
        for (int i = 0; i < angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()].Length; i++)
        {
            AniAngleForce(angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()], i);
        }
    }

    void PotionStart()
    {
        for (int i = 0; i < frameDataLists[Define.AniFrameData.PotionReadyAniData.ToString()].Length; i++)
        {
            AniForce(frameDataLists[Define.AniFrameData.PotionReadyAniData.ToString()], i);
        }
    }

    void PotionDrinking()
    {
        for (int i = 0; i < frameDataLists[Define.AniFrameData.PotionDrinkingAniData.ToString()].Length; i++)
        {
            AniForce(frameDataLists[Define.AniFrameData.PotionDrinkingAniData.ToString()], i);
        }
    }

    void PotionEnd()
    {
        for (int i = 0; i < angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()].Length; i++)
        {
            AniAngleForce(angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()], i);
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
        for (int i = 0; i < angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()].Length; i++)
        {
            AniAngleForce(angleDataLists[Define.AniAngleData.PotionAngleAniData.ToString()], i);
        }
    }

    void PotionThrowing()
    {
        for (int i = 0; i < frameDataLists[Define.AniFrameData.PotionThrowAniData.ToString()].Length; i++)
        {
            AniForce(frameDataLists[Define.AniFrameData.PotionThrowAniData.ToString()], i);
        }
    }

    void PotionThrowEnd()
    {
        for (int i = 0; i < angleDataLists[Define.AniAngleData.PotionThrowAngleData.ToString()].Length; i++)
        {
            AniAngleForce(angleDataLists[Define.AniAngleData.PotionThrowAngleData.ToString()], i);
        }
    }
    #endregion
}