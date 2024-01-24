using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction
{
    PlayerActions actions;
    JumpAction(PlayerActions actions)
    {
        this.actions = actions;
        this.actions.OnJump -= Jump;
        this.actions.OnJump += Jump;
    }

    void Jump(AnimationData animData, AnimationPlayer animPlayer, Vector3 moveDir)
    {
        for (int i = 0; i < animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()].Length; i++)
        {
            animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.up);
            if (i == 2)
                animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.down);
        }
        for (int i = 0; i < animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()].Length; i++)
        {
            animPlayer.AniAngleForce(animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()], i, moveDir + new Vector3(0, 0.2f, 0f));
        }
    }
}
