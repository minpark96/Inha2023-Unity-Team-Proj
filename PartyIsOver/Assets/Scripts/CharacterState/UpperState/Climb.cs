using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 벽타기 상태
public class Climb : BaseState
{
    private UpperBodySM _sm;
    private bool _isClimb = false;
    private float _releaseTime = 0.3f;
    private float _timer;
    public Climb(StateMachine stateMachine) : base(PlayerState.Climb, stateMachine)
    {
        _sm = (UpperBodySM)stateMachine;
    }

    public override void Enter()
    {
        // 벽타기 상태 돌입과 동시에 FixJoint 커맨드를 예약
        InvokeReserveCommand(COMMAND_KEY.FixJoint);
        _isClimb = true;
        _timer = 0f;
    }

    public override void UpdateLogic()
    {
        if(!_isClimb)
        {
            _timer += Time.deltaTime;
            if(_timer > _releaseTime)
                _sm.ChangeState(_sm.StateMap[PlayerState.Grabbing]);
        }
    }

    // 마우스 좌클릭을 유지하면서 점프를 누르면 벽을 타고, 마우스 좌클릭을 떼면 벽을 떠난다.
    public override void GetInput()
    {
        // 벽타기 도중에 점프 키를 누르면
        if(InputCommand(COMMAND_KEY.Jump, KeyType.Down) && _isClimb)
        {
            // 관절을 해제하고, 점프 커맨드를 예약
            InvokeReserveCommand(COMMAND_KEY.Jump);
            InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
            _isClimb = false;
        }
        
        // 벽타기 도중에 마우스 좌클릭을 떼면 벽타기를 종료하고 Idle 상태로
        if(!InputCommand(COMMAND_KEY.LeftBtn, KeyType.Press))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        
    }

    public override void Exit()
    {
        // 벽타기 상태 해제시 손의 관절을 해제하도록 커맨드 예약
        InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
        _sm.PlayerContext.RightGrabObject = null;
        _sm.PlayerContext.LeftGrabObject = null;
    }
}
