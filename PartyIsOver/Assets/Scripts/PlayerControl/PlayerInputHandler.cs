using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actor;

public class PlayerInputHandler : MonoBehaviour
{
    private Actor _actor;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
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


    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        if (_actor.debuffState == DebuffState.Ice || _actor.debuffState == DebuffState.Shock || _actor.debuffState == DebuffState.Stun)
            return;

        _actor.PlayerController.OnKeyboardEvent_Move(evt);

        if (_actor.Grab.GrabItem == null)
        {
            if(_actor.debuffState != DebuffState.Exhausted || _actor.debuffState != DebuffState.Balloon)
                _actor.PlayerController.OnKeyboardEvent_Skill(evt);
        }
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (_actor.debuffState == DebuffState.Ice || _actor.debuffState == DebuffState.Balloon)
            return;
        if (_actor.debuffState == DebuffState.Shock || _actor.debuffState == DebuffState.Stun)
            return;

        if (_actor.Grab.GrabItem == null)
        {
            _actor.PlayerController.OnMouseEvent_Skill(evt);

            if (_actor.debuffState != DebuffState.Burn)
                _actor.PlayerController.OnMouseEvent_Grab(evt);
        }
        else
        {
            _actor.Grab.OnMouseEvent_EquipItem(evt);
        }
    }
}
