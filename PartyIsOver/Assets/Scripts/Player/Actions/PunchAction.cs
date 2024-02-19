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
    protected float resetTime = 0.1f;

    protected bool isRSkillCheck;
    protected bool isMeowPunch;
    protected float meowPunchPower =3f;
    protected float nuclearPunchPower =5f;

    protected ConfigurableJoint[] _childJoints;
    protected ConfigurableJointMotion[] _originalYMotions;
    protected ConfigurableJointMotion[] _originalZMotions;

    protected virtual void Init()
    {
        actions.OnPunch -= HandlePunchEvent;
        actions.OnPunch += HandlePunchEvent;
    }

    bool HandlePunchEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler,in PlayerContext data)
    {
        this.animData = animData;
        this.animPlayer = animPlayer;
        this.bodyHandler = bodyHandler;
        isRSkillCheck = false;

        CoroutineHelper.StartCoroutine(Punch(data.PunchSide,duration,readyTime,punchTime,resetTime));
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

        if(!isRSkillCheck)
            actions.UpperActionEnd();
    }




    void ArmActionReadying(Define.Side side)
    {
        AniAngleData[] aniAngleDatas = (side == Define.Side.Right) ? animData.AngleDataLists[Define.AniAngleData.RightPunchAniData] : animData.AngleDataLists[Define.AniAngleData.LeftPunchAniData];
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
            aniFrameDatas = animData.FrameDataLists[Define.AniFrameData.LeftPunchingAniData];
            transform2 = bodyHandler.LeftHand.transform;
            if (isRSkillCheck)
            {
                if (isMeowPunch)
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
            aniFrameDatas = animData.FrameDataLists[Define.AniFrameData.RightPunchingAniData];
            transform2 = bodyHandler.RightHand.transform;
            if (isRSkillCheck)
            {
                if (isMeowPunch)
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
                if (isMeowPunch)
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

        AniAngleData[] aniAngleDatas = animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData];

        if (side == Define.Side.Left)
        {
            bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.LeftHand, false);
        }
        else
        {
            aniAngleDatas = animData.AngleDataLists[Define.AniAngleData.RightPunchResettingAniData];
            bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.RightHand, false);
        }

        for (int i = 0; i < aniAngleDatas.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            animPlayer.AniAngleForce(animData.AngleDataLists[Define.AniAngleData.LeftPunchResettingAniData], i, dir);
        }
    }
}
