using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdJump : PlayerCommand
{
    public CmdJump(Actor actor)
    {
        animPlayer = actor.animPlayer;
        animData = actor.animData;
    }

    public override void Execute(Vector3 moveDir = default)
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
        Debug.Log("jump");


    }
}
