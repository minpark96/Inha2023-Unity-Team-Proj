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

    bool HandleJumpEvent(AnimationData animData, AnimationPlayer animPlayer,BodyHandler bodyHandler, in Define.PlayerDynamicData data)
    {
        for (int i = 0; i < animData.FrameDataLists[Define.AniFrameData.JumpAniForceData].Length; i++)
        {
            animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData], i, Vector3.up);
            if (i == 2)
                animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData], i, Vector3.down);
        }
        for (int i = 0; i < animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData].Length; i++)
        {
            animPlayer.AniAngleForce(animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData], i,
                new Vector3(data.dirX,data.dirY + 0.2f,data.dirZ));
        }

        return true;
    }
}
