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

    private Dictionary<PlayerState, IBaseState> stateMap = new Dictionary<PlayerState, IBaseState>();
    public Dictionary<PlayerState, IBaseState> StateMap { get { return stateMap; } private set { stateMap = value; } }


    public UpperBodySM(PlayerInputHandler inputHandler, PlayerActionContext playerContext, HandChecker left, HandChecker right,Transform rangeSKin)
    {
        Context = playerContext;
        InputHandler = inputHandler;
        RangeWeaponSkin = rangeSKin;
        LeftHandCheckter = left;
        RightHandCheckter = right;

        for (PlayerState i = PlayerState.IndexUpperStart+1; i < PlayerState.IndexUpperEnd; i++)
            stateMap[i] = CreateState(i);
        Init();
    }

    protected override IBaseState GetInitialState()
    {
        return stateMap[PlayerState.UpperIdle];
    }
    private IBaseState CreateState(Define.PlayerState state)
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

}
