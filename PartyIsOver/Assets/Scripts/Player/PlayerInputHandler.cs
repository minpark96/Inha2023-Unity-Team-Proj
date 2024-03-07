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
    private Define.COMMAND_KEY _activeCommandFlag;
    private KeyCode[] _keyCodes;
    private bool _isEnabledKey;

    private Vector3 _moveDir;
    private Vector3 _moveInput;
    private Vector3 _lookForward;
    private Vector3 _lookRight;


    //키 매핑 및 커맨드 등록
    private Dictionary<COMMAND_KEY, KeyCode[]> keyMap = new Dictionary<COMMAND_KEY, KeyCode[]>
    {
    { COMMAND_KEY.Move     , new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D } },
    { COMMAND_KEY.Jump     , new KeyCode[] { KeyCode.Space } },
    { COMMAND_KEY.LeftBtn  , new KeyCode[] { KeyCode.Mouse0 } },
    { COMMAND_KEY.RightBtn , new KeyCode[] { KeyCode.Mouse1 } },
    { COMMAND_KEY.HeadButt , new KeyCode[] { KeyCode.Mouse2 } },
    { COMMAND_KEY.Skill    , new KeyCode[] { KeyCode.R } }
    };

    public void InitCommnad(Actor actor)
    {
        commands.Add(COMMAND_KEY.Jump, new CmdJump(actor));
        commands.Add(COMMAND_KEY.Move, new CmdMove(actor));
        commands.Add(COMMAND_KEY.LeftBtn, new CmdLeftBtn(actor));
        commands.Add(COMMAND_KEY.Skill, new CmdSkill(actor));
        commands.Add(COMMAND_KEY.Charge, new CmdCharge(actor));
        commands.Add(COMMAND_KEY.ResetCharge, new CmdResetCharge(actor));
        commands.Add(COMMAND_KEY.HeadButt, new CmdHeadButt(actor));
        commands.Add(COMMAND_KEY.RightBtn, new CmdRightBtn(actor));
        commands.Add(COMMAND_KEY.Grabbing, new CmdGrabbing(actor));
        commands.Add(COMMAND_KEY.TargetSearch, new CmdSearchTarget(actor));
        commands.Add(COMMAND_KEY.FixJoint, new CmdFixJoint(actor));
        commands.Add(COMMAND_KEY.DestroyJoint, new CmdDestroyJoint(actor));
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

    public bool CheckInputCommand(COMMAND_KEY commandKey, GetKeyType keyType)
    {
        // COMMAND_KEY에 대응하는 KeyCode 배열 가져오기
        if (keyMap.TryGetValue(commandKey, out _keyCodes))
        {
            // 각 KeyCode에 대해 입력 확인
            foreach (KeyCode keyCode in _keyCodes)
            {
                _isEnabledKey = false;
                switch (keyType)
                {
                    case GetKeyType.Press:
                        _isEnabledKey = Input.GetKey(keyCode);
                        break;
                    case GetKeyType.Down:
                        _isEnabledKey = Input.GetKeyDown(keyCode);
                        break;
                    case GetKeyType.Up:
                        _isEnabledKey = Input.GetKeyUp(keyCode);
                        break;
                    default:
                        _isEnabledKey = false;
                        break;
                }

                // 입력이 발생하면 해당 커맨드를 실행하고 true 반환
                if (_isEnabledKey)
                {
                    //ReserveCommand(commandKey);
                    return true;
                }
            }
        }

        // 입력이 발생하지 않았거나 COMMAND_KEY에 대응하는 KeyCode 배열이 없는 경우 false 반환
        return false;
    }

    public void ReserveCommand(COMMAND_KEY commandKey)
    {
        _activeCommandFlag |= commandKey;
    }

    public ICommand GetCommand(COMMAND_KEY key)
    {
        return commands[key];
    }

    public COMMAND_KEY GetActiveCmdFlag()
    {
        return _activeCommandFlag;
    }

    public void ClearCommand()
    {
        _activeCommandFlag = 0f;
    }


   
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
