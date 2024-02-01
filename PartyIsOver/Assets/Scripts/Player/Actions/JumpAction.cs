using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction
{
    public JumpAction(ActionController actions)
    {
        actions.OnJump -= InvokeJumpEvent;
        actions.OnJump += InvokeJumpEvent;
    }

    void InvokeJumpEvent(AnimationData animData, AnimationPlayer animPlayer, in Define.PlayerDynamicData data)
    {
        for (int i = 0; i < animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()].Length; i++)
        {
            animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.up);
            if (i == 2)
                animPlayer.AniForce(animData.FrameDataLists[Define.AniFrameData.JumpAniForceData.ToString()], i, Vector3.down);
        }
        for (int i = 0; i < animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()].Length; i++)
        {
            animPlayer.AniAngleForce(animData.AngleDataLists[Define.AniAngleData.MoveAngleJumpAniData.ToString()], i,
                new Vector3(data.dirX,data.dirY + 0.2f,data.dirZ));
        }
    }
}
