using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAction
{
    protected ActionController actions;

    public PunchAction(ActionController actions)
    {
        this.actions = actions;
        Init();
    }

    protected AnimationData animData;
    protected AnimationPlayer animPlayer;
    protected BodyHandler bodyHandler;

    protected float duration = 0.07f;
    protected float readyTime = 0.1f;
    protected float punchTime = 0.1f;
    protected float resetTime = 0.3f;

    protected bool isRSkillCheck;
    protected bool isMeowNyangPunch;
    protected float meowPunchPower;
    protected float nuclearPunchPower;

    protected virtual void Init()
    {
        actions.OnPunch -= InvokePunchEvent;
        actions.OnPunch += InvokePunchEvent;
    }

    bool InvokePunchEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler,in Define.PlayerDynamicData data)
    {
        this.animData = animData;
        this.animPlayer = animPlayer;
        this.bodyHandler = bodyHandler;
        CoroutineHelper.StartCoroutine(Punch(data.side,duration,readyTime,punchTime,resetTime));
        return true;
    }

    protected IEnumerator Punch(Define.Side side, float duration, float readyTime, float punchTime, float resetTime)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ArmActionReadying(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ArmActionPunching(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ArmActionPunchResetting(side);
            yield return new WaitForSeconds(duration);
        }

        actions.UpperActionEnd();
    }
    
    void ArmActionReadying(Define.Side side)
    {
        AniAngleData[] aniAngleDatas = (side == Define.Side.Right) ? animData.AngleDataLists[Define.AniAngleData.RightPunchAniData.ToString()] : animData.AngleDataLists[Define.AniAngleData.LeftPunchAniData.ToString()];
        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            animPlayer.AniAngleForce(aniAngleDatas, i);
        }
    }

    void ArmActionPunching(Define.Side side)
    {

        Transform partTransform = bodyHandler.Chest.transform;
        AniFrameData[] aniFrameDatas;
        Transform transform2;

        if (side == Define.Side.Left)
        {
            aniFrameDatas = animData.FrameDataLists[Define.AniFrameData.LeftPunchingAniData.ToString()];
            transform2 = bodyHandler.LeftHand.transform;
            if (isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, true);
        }
        else
        {
            aniFrameDatas = animData.FrameDataLists[Define.AniFrameData.RightPunchingAniData.ToString()];
            transform2 = bodyHandler.RightHand.transform;
            if (isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.MeowNyangPunch;
                else
                    bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.NuclearPunch;
            }
            else
                bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, true);
        }

        for (int i = 0; i < aniFrameDatas.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);

            if (isRSkillCheck)
            {
                if (isMeowNyangPunch)
                    animPlayer.AniForce(aniFrameDatas, i, dir, meowPunchPower);
                else
                    animPlayer.AniForce(aniFrameDatas, i, dir, nuclearPunchPower);
            }
            else
                animPlayer.AniForce(aniFrameDatas, i, dir);
        }
    }

    void ArmActionPunchResetting(Define.Side side)
    {
        Transform partTransform = bodyHandler.Chest.transform;

        AniAngleData[] aniAngleDatas = animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()];

        if (side == Define.Side.Left)
        {
            bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, false);
        }
        else
        {
            aniAngleDatas = animData.AngleDataLists[Define.AniAngleData.RightPunchResettingAniData.ToString()];
            bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, false);
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            animPlayer.AniAngleForce(animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData.ToString()], i, dir);
        }
    }
}
