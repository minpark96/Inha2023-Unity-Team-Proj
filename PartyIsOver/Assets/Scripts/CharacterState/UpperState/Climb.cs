using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

    public override void GetInput()
    {
        if(Input.GetButtonDown(COMMAND_KEY.Jump.ToString()) && _isClimb)
        {
            InvokeReserveCommand(COMMAND_KEY.Jump);
            InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
            _isClimb = false;
        }

        if(!Input.GetButton(COMMAND_KEY.LeftBtn.ToString()))
            _sm.ChangeState(_sm.StateMap[PlayerState.UpperIdle]);
        
    }

    public override void Exit()
    {
        InvokeReserveCommand(COMMAND_KEY.DestroyJoint);
        _sm.Context.RightGrabObject = null;
        _sm.Context.LeftGrabObject = null;
    }
}
