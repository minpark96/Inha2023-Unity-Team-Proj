using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 캐릭터의 상체를 담당하는 스테이트머신
public class UpperBodySM : StateMachine
{
    public PlayerActionContext PlayerContext;

    // 공격할 준비가 된 주먹의 좌우 위치
    public Define.Side ReadySide = Define.Side.Left;
    // 현재 오브젝트 잡기 동작이 진행중인지 여부
    public bool IsGrabbingInProgress=false;

    // 무기의 스킨, 무기 투사체 발사 지점 
    public Transform RangeWeaponSkin;
    public Transform FirePoint;
    
    // 손과 충돌한 오브젝트를 체크하는 클래스
    public HandChecker LeftHandCheckter;
    public HandChecker RightHandCheckter;

    // 상체 상태머신에서 보관할 스테이트들
    private Dictionary<PlayerState, BaseState> stateMap = new Dictionary<PlayerState, BaseState>();
    public Dictionary<PlayerState, BaseState> StateMap { get { return stateMap; } private set { stateMap = value; } }


    public UpperBodySM(PlayerInputHandler inputHandler, PlayerActionContext playerContext, 
        CommandDelegate cmdReserveHandler, HandChecker left, HandChecker right,Transform rangeSkin )
    {
        PlayerContext = playerContext;
        RangeWeaponSkin = rangeSkin;
        LeftHandCheckter = left;
        RightHandCheckter = right;
        // InputHandler의 커맨드 예약 함수를 CommandReserveHandler에 바인딩
        CommandReserveHandler -= cmdReserveHandler;
        CommandReserveHandler += cmdReserveHandler;
        
        // 상체 상태머신에서 담당할 스테이트들 생성
        for (PlayerState i = PlayerState.IndexUpperStart+1; i < PlayerState.IndexUpperEnd; i++)
            stateMap[i] = CreateState(i);
        Init();
    }

    private BaseState CreateState(Define.PlayerState state)
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

    protected override BaseState GetInitialState()
    {
        return stateMap[PlayerState.UpperIdle];
    }
}
