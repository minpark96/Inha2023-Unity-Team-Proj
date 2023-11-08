using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static Actor;

public class PlayerInputHandler : MonoBehaviour
{
    private Actor _actor;


    private void Awake()
    {
        _actor = GetComponent<Actor>();
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }



    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        if (_actor.actorState == ActorState.Debuff)
            return;

        if (_actor.Grab.GrabItem == null)
            _actor.PlayerController.OnKeyboardEvent_Idle(evt);

    }



    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (_actor.actorState == ActorState.Debuff)
            return;


        if (_actor.Grab.GrabItem == null)
        {
            _actor.PlayerController.OnMouseEvent_Idle(evt);
        }
        else
        {
            _actor.Grab.OnMouseEvent_EquipItem(evt);
        }
    }



}
