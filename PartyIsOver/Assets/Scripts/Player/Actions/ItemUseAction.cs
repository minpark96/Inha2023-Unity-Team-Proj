using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUseAction
{
    public ItemUseAction(ActionController actions)
    {
        actions.OnUseItem -= HandleUseItemEvent;
        actions.OnUseItem += HandleUseItemEvent;
    }
    Define.ItemType _type;
    PlayerContext _context;
    BodyHandler _bodyHandler;
    AnimationData _animData;
    AnimationPlayer _animPlayer;
    //public float _turnForce;

    bool HandleUseItemEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _type = data.EquipItem.ItemObject.ItemData.ItemType;
        _context = data;
        _bodyHandler = bodyHandler;
        _animData = animData;
        _animPlayer = animPlayer;


        return true;
    }



    IEnumerator HorizontalAttack()
    {
        if (PhotonNetwork.IsMasterClient)
            _context.EquipItem.ChangeUseTypeTrigger(0f, 1.1f);
        //if (!photonView.IsMine)
        //    yield break;

        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.ItemTwoHand(_side, 0.07f, 0.1f, 0.5f, 0.1f, 3f);
    }

    IEnumerator UsePotionAnim()
    {
        _jointLeft.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        _jointRight.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return _actor.PlayerController.Potion(0.07f, 0.1f, 0.5f, 0.5f, 0.1f);

        photonView.RPC("UseItem", RpcTarget.All);
        GrabResetTrigger();
    }

    private void UseItem()
    {
        EquipItem.GetComponent<Item>().Use();
        _isAttackReady = false;

    }

    public IEnumerator ItemTwoHand(Define.Side side, float duration, float readyTime, float punchTime, float resetTime, float itemPower)
    {
        //photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/WEAPON_Axe");
        float checkTime = Time.time;

        while (Time.time - checkTime < readyTime)
        {
            ItemTwoHandReady(side);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < punchTime)
        {
            ItemTwoHandSwing(side, itemPower);
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < resetTime)
        {
            ItemTwoHandReset(side);
            yield return new WaitForSeconds(duration);
        }
    }

    public void ItemTwoHandReady(Define.Side side)
    {
        //upperArm 2 chest1 up right 0.01 20 foreArm chest up back 
        //TestRready ¿À¸¥ÂÊ ¿ÞÂÊ ±¸º°ÇØ¼­ ÁÂ¿ì·Î ÈÖµÎ·ê¼ö ÀÖÀ½
        AniAngleData[] itemTwoHands = (side == Define.Side.Right) ? _animData.AngleDataLists[Define.AniAngleData.ItemTwoHandAngleData] : _animData.AngleDataLists[Define.AniAngleData.ItemTwoHandLeftAngleData];
        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            _animPlayer.AniAngleForce(itemTwoHands, i);
        }
    }

    public void ItemTwoHandSwing(Define.Side side, float itemSwingPower)
    {

        Transform partTransform = _bodyHandler.Chest.transform;
        AniFrameData[] itemTwoHands = _animData.FrameDataLists[Define.AniFrameData.ItemTwoHandLeftAniData];
        Transform transform2 = _bodyHandler.LeftHand.transform;
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (side == Define.Side.Right)
        {
            itemTwoHands = _animData.FrameDataLists[Define.AniFrameData.ItemTwoHandAniData];
            transform2 = _bodyHandler.RightHand.transform;
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Punch;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            Vector3 dir = Vector3.Normalize(partTransform.position + -partTransform.up + partTransform.forward / 2f - transform2.position);
            _animPlayer.AniForce(itemTwoHands, i, dir, itemSwingPower);
        }
    }

    public void ItemTwoHandReset(Define.Side side)
    {
        Transform partTransform = _bodyHandler.Chest.transform;

        AniAngleData[] itemTwoHands = _animData.AngleDataLists[Define.AniAngleData.ItemTwoHandLeftAngleData];
        _bodyHandler.LeftHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        _bodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _bodyHandler.LeftForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (side == Define.Side.Right)
        {
            itemTwoHands = _animData.AngleDataLists[Define.AniAngleData.ItemTwoHandAngleData];
            _bodyHandler.RightHand.PartInteractable.damageModifier = InteractableObject.Damage.Default;
            _bodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _bodyHandler.RightForeArm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        for (int i = 0; i < itemTwoHands.Length; i++)
        {
            Vector3 dir = partTransform.transform.right / 2f;
            _animPlayer.AniAngleForce(itemTwoHands, i, dir);
        }
    }
    public IEnumerator Potion(float duration, float ready, float start, float drinking, float end)
    {
        //photonView.RPC("PlayerEffectSound", RpcTarget.All, "Sounds/PlayerEffect/Item_UI_042");

        float checkTime = Time.time;

        while (Time.time - checkTime < ready)
        {
            PotionReadyAndEnd();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < start)
        {
            PotionStart();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < drinking)
        {
            PotionDrinking();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < end)
        {
            PotionReadyAndEnd();
            yield return new WaitForSeconds(duration);
        }
    }

    void PotionReadyAndEnd()
    {
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.PotionAngleAniData].Length; i++)
        {
            _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.PotionAngleAniData], i);
        }
    }

    void PotionStart()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.PotionReadyAniData].Length; i++)
        {
            _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.PotionReadyAniData], i);
        }
    }

    void PotionDrinking()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.PotionDrinkingAniData].Length; i++)
        {
            _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.PotionDrinkingAniData], i);
        }
    }
}
