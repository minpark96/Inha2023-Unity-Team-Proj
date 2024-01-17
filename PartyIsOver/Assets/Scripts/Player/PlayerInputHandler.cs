using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;
using Photon.Pun;
using static Define;

public class PlayerInputHandler : MonoBehaviourPun
{
    private Actor _actor;
    private Dictionary<COMMAND_KEY, ICommand> commands = new Dictionary<COMMAND_KEY, ICommand>();
    private ICommand _activeCommand;
    private void Awake()
    {
        _actor = GetComponent<Actor>();
        
    }
    private void Update()
    {

      
    }
    private void FixedUpdate()
    {
        //Master클라이어트의 PlayerController에서 받은 커맨드로 해당하는 actor들을 컨트롤하는 방향으로
        if(_activeCommand != null)
        {
            _activeCommand.Execute();
            _activeCommand = null;
            Debug.Log("commnad");
        }
    }

    //키 매핑
    private void InitCommnad(AnimationData data)
    {
        commands.Add(COMMAND_KEY.Jump, new CmdJump(data));
    }
    public void InputGetDownKey(KeyCode keyCode, GetKeyType keyType)
    {
        // 어떤 키값 호출 분기
        COMMAND_KEY commandKey = COMMAND_KEY.None;
        switch (keyCode)
        {
            case KeyCode.Space:
                commandKey = COMMAND_KEY.Jump;
                break;
            default:
                commandKey = COMMAND_KEY.None;
                break;
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
            // 커맨드 execute 호출
            //this.commands[commandKey].Execute();
            _activeCommand = commands[commandKey];
        }
    }

    void Start()
    {
        if (_actor.PlayerController.isAI)
            return;

        //_actor.BodyHandler.BodySetup();
                                                  
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
