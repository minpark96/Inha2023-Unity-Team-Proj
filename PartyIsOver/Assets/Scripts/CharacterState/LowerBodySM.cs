using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

// 캐릭터의 하체를 담당하는 스테이트머신
public class LowerBodySM : StateMachine
{
    public PlayerActionContext PlayerContext;

    public bool IsGrounded=false;
    public bool IsRun = false;

    // 하체에서 담당할 스테이트들 선언
    public BaseState JumpingState;
    public BaseState IdleState;
    public BaseState MovingState;
    public BaseState DropKickState;

    // 걷기 동작에서 사용될 각 포즈들
    public BodyPose LeftArmPose;
    public BodyPose RightArmPose;
    public BodyPose LeftLegPose;
    public BodyPose RightLegPose;

    int[] _aryBodyPose = new int[4];

    
    public LowerBodySM(PlayerInputHandler inputHandler, PlayerActionContext playerContext, CommandDelegate cmdReserveHandler)
    {
        // 하체의 State들 생성
        IdleState = new LowerIdle(this);
        JumpingState = new Jumping(this);
        MovingState = new Moving(this);
        DropKickState = new DropKick(this);

        // InputHandler의 커맨드 예약 함수를 CommandReserveHandler에 바인딩
        CommandReserveHandler -= cmdReserveHandler;
        CommandReserveHandler += cmdReserveHandler;
        // 플레이어 상태 컨텍스트 받아오기
        PlayerContext = playerContext;
        base.Init();
    }
    
    // 캐릭터 걷기 동작의 현재 상태 반환
    public int[] GetBodyPose()
    {
        _aryBodyPose[0] = (int)LeftArmPose;
        _aryBodyPose[1] = (int)RightArmPose;
        _aryBodyPose[2] = (int)LeftLegPose;
        _aryBodyPose[3] = (int)RightLegPose;
        return _aryBodyPose;
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }
}
