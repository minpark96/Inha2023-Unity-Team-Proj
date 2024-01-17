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
        //MasterŬ���̾�Ʈ�� PlayerController���� ���� Ŀ�ǵ�� �ش��ϴ� actor���� ��Ʈ���ϴ� ��������
        if(_activeCommand != null)
        {
            _activeCommand.Execute();
            _activeCommand = null;
            Debug.Log("commnad");
        }
    }

    //Ű ����
    private void InitCommnad(AnimationData data)
    {
        commands.Add(COMMAND_KEY.Jump, new CmdJump(data));
    }
    public void InputGetDownKey(KeyCode keyCode, GetKeyType keyType)
    {
        // � Ű�� ȣ�� �б�
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

        // Ű Ȱ��ȭ Ÿ�� �б�
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
            // Ŀ�ǵ� execute ȣ��
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

    #region �ӽ� �ڻ� �׽�Ʈ��
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
