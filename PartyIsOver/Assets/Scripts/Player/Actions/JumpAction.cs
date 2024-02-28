using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction
{
    public JumpAction(ActionController actions)
    {
        actions.OnJump -= HandleJumpEvent;
        actions.OnJump += HandleJumpEvent;
    }

    AnimationData _animData;
    AnimationPlayer _animPlayer;
    PlayerContext _context;
    BodyHandler _bodyHandler;
    bool HandleJumpEvent(AnimationData animData, AnimationPlayer animPlayer,BodyHandler bodyHandler, in PlayerContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _context = data;
        _bodyHandler = bodyHandler;

        
        if (_context.RightGrabObject != null && _context.RightGrabObject.Type == Define.ObjectType.Wall)
            Climb();
        else
            Jump();

        return true;
    }

    void Jump()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.JumpAniForceData].Length; i++)
        {
            _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.JumpAniForceData], i, Vector3.up);
            if (i == 2)
                _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.JumpAniForceData], i, Vector3.down);
        }
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData].Length; i++)
        {
            _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData], i,
                new Vector3(_context.InputDirX, _context.InputDirY + 0.2f, _context.InputDirZ));
        }
    }

    void Climb()
    {
        _bodyHandler.Hip.PartRigidbody.AddForce(Vector3.up * 150f, ForceMode.VelocityChange);

    }
}
