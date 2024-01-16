using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;
using Photon.Pun;
public class PlayerInputHandler : MonoBehaviourPun
{
    private Actor _actor;
    //private Moving _moving;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
        //_moving = GetComponent<Moving>();
    }

    void Start()
    {
        if (_actor.PlayerController.isAI)
            return;

        _actor.BodyHandler.BodySetup();
                                                  
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnDestroy()
    {
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
        // TODO : _moving.OnKeyboardEvent_Move(evt); 로 변경 예정
        //_moving.OnKeyboardEvent_Move(evt);
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
