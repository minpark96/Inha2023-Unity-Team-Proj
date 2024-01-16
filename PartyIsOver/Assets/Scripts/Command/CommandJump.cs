using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandJump : PlayerCommand
{
    AnimationData animData;

    public CommandJump(AnimationData data)
    {
        animData = data;
       
    }

    public override void Execute(Vector3 moveDir = default)
    {
        for (int i = 0; i < animData.ForceAniData.Length; i++)
        {
            animData.AniForce(animData.ForceAniData, i, Vector3.up);
            if (i == 2)
                animData.AniForce(animData.ForceAniData, i, Vector3.down);
        }
        for (int i = 0; i < animData.AngleAniData.Length; i++)
        {
            animData.AniAngleForce(animData.AngleAniData, i, moveDir + new Vector3(0, 0.2f, 0f));
        }
    }
}
