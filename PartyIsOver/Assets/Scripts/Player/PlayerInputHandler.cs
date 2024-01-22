using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;
using Photon.Pun;
using static Define;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class PlayerInputHandler : MonoBehaviourPun
{
    private Actor _actor;
    private Dictionary<COMMAND_KEY, ICommand> commands = new Dictionary<COMMAND_KEY, ICommand>();
    private ICommand _activeCommand;
    
    private Transform _cameraArm;
    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;

    public BodyPose leftArmPose;
    public BodyPose rightArmPose;
    public BodyPose leftLegPose;
    public BodyPose rightLegPose;

    private Dictionary<KeyCode, COMMAND_KEY> keyMap = new Dictionary<KeyCode, COMMAND_KEY>
    {
        { KeyCode.W, COMMAND_KEY.Move },
        { KeyCode.A, COMMAND_KEY.Move },
        { KeyCode.S, COMMAND_KEY.Move },
        { KeyCode.D, COMMAND_KEY.Move },
        { KeyCode.Space, COMMAND_KEY.Jump }
    };


    private void Awake()
    {
        _actor = GetComponent<Actor>();
        InitCommnad(_actor);
        if (photonView.IsMine)
            _cameraArm = _actor.CameraControl.CameraArm;
    }
    private void Update()
    {
        GetMoveInput();
    }
    private void FixedUpdate()
    {
        //Master클라이어트에게 받은 커맨드로 해당하는 actor들을 컨트롤하는 방향으로 혹은 마스터에서 바로 actor.execute
        if(_activeCommand != null)
        {
            _activeCommand.Execute(_moveDir);
            _activeCommand = null;
            Debug.Log("commnad");
        }
    }

    private void GetMoveInput()
    {
        _moveInput.x = Input.GetAxis("Horizontal");
        _moveInput.y = 0;
        _moveInput.z = Input.GetAxis("Vertical");

        _lookForward.x = _cameraArm.forward.x;
        _lookForward.y = 0f;
        _lookForward.z = _cameraArm.forward.z;
        _lookForward.Normalize();

        _lookRight.x = _cameraArm.right.x;
        _lookRight.y = 0f;
        _lookRight.z = _cameraArm.right.z;
        _lookRight.Normalize();

        _moveDir = _lookForward * _moveInput.z + _lookRight * _moveInput.x;
    }

    //키 매핑
    private void InitCommnad(Actor actor)
    {
        commands.Add(COMMAND_KEY.Jump, new CmdJump(actor));
        commands.Add(COMMAND_KEY.InAirMove, new CmdInAirMove(actor));
        commands.Add(COMMAND_KEY.Move, new CmdMove(actor));

    }

    public bool IsMoveInput()
    {
        if (_moveInput.magnitude == 0f)
            return false;
        else
            return true;
    }

    public bool InputGetDownKey(KeyCode keyCode, GetKeyType keyType,bool isGround = true)
    {
        // 어떤 키값 호출 분기
        COMMAND_KEY commandKey = COMMAND_KEY.None;
        if (keyMap.ContainsKey(keyCode))
        {
            commandKey = keyMap[keyCode];
        }

        // 키 활성화 타입 분기
        bool isEnabledKey = false;
        switch (keyType)
        {
            case GetKeyType.Press:
                isEnabledKey = Input.GetKey(keyCode);
                break;
            case GetKeyType.Down:
                isEnabledKey = Input.GetKeyDown(keyCode);
                break;
            case GetKeyType.Up:
                isEnabledKey = Input.GetKeyUp(keyCode);
                break;
            default:
                isEnabledKey = false;
                break;
        }

        if (isEnabledKey && commandKey != COMMAND_KEY.None && commands.ContainsKey(commandKey))
        {
            //이 자리에서 마스터한테 커맨드를 보낼수도
            // 커맨드 execute 호출
            //this.commands[commandKey].Execute();
            if (commandKey == Define.COMMAND_KEY.Move && !isGround)
                commandKey = Define.COMMAND_KEY.InAirMove;
            _activeCommand = commands[commandKey];


            return true;
        }
        return false;
    }

    void Start()
    {
        if (_actor.PlayerController.isAI)
            return;

        return;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnDestroy()
    {
        commands.Clear();
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
    }

   
    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
            return;

        if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
            (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
            (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
            return;

        _actor.PlayerController.OnKeyboardEvent_Move(evt);

        if (_actor.GrabState != Define.GrabState.EquipItem)
        {
            if(!((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
                _actor.PlayerController.OnKeyboardEvent_Skill(evt);
        }

        if (Input.GetKeyUp(KeyCode.F4))
        {
            KillOneself();
        }
    }

    #region 임시 자살 테스트용
    void KillOneself()
    {
        photonView.RPC("TestKill", RpcTarget.MasterClient, photonView.ViewID);
    }

    [PunRPC]
    void TestKill(int ID)
    {
        PhotonView pv = PhotonView.Find(ID);
        Actor ac = pv.transform.GetComponent<Actor>();
        StartCoroutine(ac.StatusHandler.ResetBodySpring());
        ac.actorState = Actor.ActorState.Dead;
        ac.StatusHandler._isDead = true;
        _actor.Health = 0;
        ac.InvokeDeathEvent();
        ac.InvokeStatusChangeEvent();
    }

    #endregion

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
            return;

        if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
            (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
            (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
            return;

        if (_actor.GrabState != Define.GrabState.EquipItem)
        {
            _actor.PlayerController.OnMouseEvent_Skill(evt);

            if (!((_actor.debuffState & DebuffState.Burn) == DebuffState.Burn))
                if (_actor.GrabState == Define.GrabState.PlayerLift)
                {
                    _actor.Grab.OnMouseEvent_LiftPlayer(evt);
                    return;
                }
                else
                    _actor.PlayerController.OnMouseEvent_Grab(evt);

        }
        else
        {
            _actor.Grab.OnMouseEvent_EquipItem(evt);
        }
    }
}
