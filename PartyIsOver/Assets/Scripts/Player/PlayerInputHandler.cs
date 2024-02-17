using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;
using Photon.Pun;
using static Define;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System;

public class PlayerInputHandler : MonoBehaviourPun
{
    private Dictionary<COMMAND_KEY, ICommand> commands = new Dictionary<COMMAND_KEY, ICommand>();

    private Queue<ICommand> _activeCommands = new Queue<ICommand>();
    private Define.COMMAND_KEY _activeCommandFlag;


    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;



    private Dictionary<KeyCode, COMMAND_KEY> keyMap = new Dictionary<KeyCode, COMMAND_KEY>
    {
        { KeyCode.W, COMMAND_KEY.Move },
        { KeyCode.A, COMMAND_KEY.Move },
        { KeyCode.S, COMMAND_KEY.Move },
        { KeyCode.D, COMMAND_KEY.Move },
        { KeyCode.Space, COMMAND_KEY.Jump },
        { KeyCode.Mouse0, COMMAND_KEY.LeftBtn },
        { KeyCode.R, COMMAND_KEY.Skill },
        { KeyCode.Mouse2, COMMAND_KEY.HeadButt },
        { KeyCode.Mouse1, COMMAND_KEY.DropKick },
    };

    //키 매핑
    public void InitCommnad(Actor actor)
    {
        commands.Add(COMMAND_KEY.Jump, new CmdJump(actor));
        commands.Add(COMMAND_KEY.Move, new CmdMove(actor));
        commands.Add(COMMAND_KEY.LeftBtn, new CmdLeftBtn(actor));
        commands.Add(COMMAND_KEY.Skill, new CmdSkill(actor));
        commands.Add(COMMAND_KEY.Charge, new CmdCharge(actor));
        commands.Add(COMMAND_KEY.ResetCharge, new CmdResetCharge(actor));
        commands.Add(COMMAND_KEY.HeadButt, new CmdHeadButt(actor));
        commands.Add(COMMAND_KEY.DropKick, new CmdDropKick(actor));
        commands.Add(COMMAND_KEY.Grabbing, new CmdGrabbing(actor));
        commands.Add(COMMAND_KEY.TargetSearch, new CmdSearchTarget(actor));
        commands.Add(COMMAND_KEY.FixJoint, new CmdFixJoint(actor));
        commands.Add(COMMAND_KEY.DestroyJoint, new CmdDestroyJoint(actor));
    }


    private void Awake()
    {
    }
    private void Update()
    {
    }
    private void FixedUpdate()
    {
    }



    public Vector3 GetMoveInput(Transform cameraArm)
    {
        _moveInput.x = Input.GetAxis("Horizontal");
        _moveInput.y = 0;
        _moveInput.z = Input.GetAxis("Vertical");

        _lookForward.x = cameraArm.forward.x;
        _lookForward.y = 0f;
        _lookForward.z = cameraArm.forward.z;
        _lookForward.Normalize();

        _lookRight.x = cameraArm.right.x;
        _lookRight.y = 0f;
        _lookRight.z = cameraArm.right.z;
        _lookRight.Normalize();

        _moveDir = _lookForward * _moveInput.z + _lookRight * _moveInput.x;
        return _moveDir;
    }



    public bool IsMoveInput()
    {
        if (_moveInput.magnitude == 0f)
            return false;
        else
            return true;
    }

    public bool InputCommnadKey(KeyCode keyCode, GetKeyType keyType)
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
            if(_activeCommands.Count == 0 || _activeCommands.Peek() != commands[commandKey])
                EnqueueCommand(commandKey);
            return true;
        }
        return false;
    }

    public void EnqueueCommand(COMMAND_KEY commandKey)
    {
        Debug.Log(commandKey.ToString());
        _activeCommandFlag |= commandKey;

        //if (_activeCommands.Count == 0 || _activeCommands.Peek() != commands[commandKey])
        //    _activeCommands.Enqueue(commands[commandKey]);
    }

    public ICommand GetActiveCommand(COMMAND_KEY key)
    {
        ICommand command = commands[key];
        return command;
    }

    public COMMAND_KEY GetActiveCmdFlag()
    {
        return _activeCommandFlag;
    }

    public void ClearCommand()
    {
        //_activeCommands = null;
        _activeCommandFlag = 0f;
    }
    //void Start()
    //{
    //    if (_actor.PlayerController.isAI)
    //        return;

    //    return;
    //    Managers.Input.MouseAction -= OnMouseEvent;
    //    Managers.Input.MouseAction += OnMouseEvent;
    //    Managers.Input.KeyboardAction -= OnKeyboardEvent;
    //    Managers.Input.KeyboardAction += OnKeyboardEvent;
    //}

    //void OnDestroy()
    //{
    //    commands.Clear();
    //    Managers.Input.MouseAction -= OnMouseEvent;
    //    Managers.Input.KeyboardAction -= OnKeyboardEvent;
    //}

   
    //void OnKeyboardEvent(Define.KeyboardEvent evt)
    //{
    //    if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
    //        return;

    //    if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
    //        (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
    //        (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
    //        return;

    //    _actor.PlayerController.OnKeyboardEvent_Move(evt);

    //    if (_actor.GrabState != Define.GrabState.EquipItem)
    //    {
    //        if(!((_actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
    //            _actor.PlayerController.OnKeyboardEvent_Skill(evt);
    //    }

    //    if (Input.GetKeyUp(KeyCode.F4))
    //    {
    //        KillOneself();
    //    }
    //}

    //#region 임시 자살 테스트용
    //void KillOneself()
    //{
    //    photonView.RPC("TestKill", RpcTarget.MasterClient, photonView.ViewID);
    //}

    //[PunRPC]
    //void TestKill(int ID)
    //{
    //    PhotonView pv = PhotonView.Find(ID);
    //    Actor ac = pv.transform.GetComponent<Actor>();
    //    StartCoroutine(ac.StatusHandler.ResetBodySpring());
    //    ac.actorState = Actor.ActorState.Dead;
    //    ac.StatusHandler._isDead = true;
    //    _actor.Health = 0;
    //    ac.InvokeDeathEvent();
    //    ac.InvokeStatusChangeEvent();
    //}

    //#endregion

    //void OnMouseEvent(Define.MouseEvent evt)
    //{
    //    if (!photonView.IsMine || _actor.actorState == ActorState.Dead)
    //        return;

    //    if ((_actor.debuffState & DebuffState.Ice) == DebuffState.Ice ||
    //        (_actor.debuffState & DebuffState.Shock) == DebuffState.Shock ||
    //        (_actor.debuffState & DebuffState.Stun) == DebuffState.Stun)
    //        return;

    //    if (_actor.GrabState != Define.GrabState.EquipItem)
    //    {
    //        _actor.PlayerController.OnMouseEvent_Skill(evt);

    //        if (!((_actor.debuffState & DebuffState.Burn) == DebuffState.Burn))
    //            if (_actor.GrabState == Define.GrabState.PlayerLift)
    //            {
    //                _actor.Grab.OnMouseEvent_LiftPlayer(evt);
    //                return;
    //            }
    //            else
    //                _actor.PlayerController.OnMouseEvent_Grab(evt);

    //    }
    //    else
    //    {
    //        _actor.Grab.OnMouseEvent_EquipItem(evt);
    //    }
    //}
}
