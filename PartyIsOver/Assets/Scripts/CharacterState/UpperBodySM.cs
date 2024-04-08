using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class UpperBodySM : StateMachine
{
    public PlayerActionContext Context;

    public Define.Side ReadySide = Define.Side.Left;
    public bool IsGrabbingInProgress=false;

    public Transform RangeWeaponSkin;
    public Transform FirePoint;
    public HandChecker LeftHandCheckter;
    public HandChecker RightHandCheckter;

    private Dictionary<PlayerState, BodyState> stateMap = new Dictionary<PlayerState, BodyState>();
    public Dictionary<PlayerState, BodyState> StateMap { get { return stateMap; } private set { stateMap = value; } }


    public UpperBodySM(PlayerInputHandler inputHandler, PlayerActionContext playerContext, HandChecker left, HandChecker right,Transform rangeSkin)
    {
        Context = playerContext;
        InputHandler = inputHandler;
        RangeWeaponSkin = rangeSkin;
        LeftHandCheckter = left;
        RightHandCheckter = right;

        for (PlayerState i = PlayerState.IndexUpperStart+1; i < PlayerState.IndexUpperEnd; i++)
            stateMap[i] = CreateState(i);
        Init();
    }

    private BodyState CreateState(Define.PlayerState state)
    {
        switch (state)
        {
            case PlayerState.UpperIdle:         return new UpperIdle(this);
            case PlayerState.PunchAndGrabReady: return new ArmReady(this);
            case PlayerState.Punch:             return new Punch(this);
            case PlayerState.Grabbing:          return new Grabbing(this);
            case PlayerState.SkillReady:        return new SkillReady(this);
            case PlayerState.Skill:             return new Skill(this);
            case PlayerState.HeadButt:          return new HeadButt(this);
            case PlayerState.EquipItem:         return new EquipItem(this);
            case PlayerState.LiftObject:        return new LiftObject(this);
            case PlayerState.Climb:             return new Climb(this);

            default: throw new ArgumentException($"Unknown state: {state}");
        }
    }

    protected override BodyState GetInitialState()
    {
        return stateMap[PlayerState.UpperIdle];
    }
}
