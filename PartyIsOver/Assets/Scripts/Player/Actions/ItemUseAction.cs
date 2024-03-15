using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class ItemUseAction : PlayerAction
{
    public ItemUseAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }
    Define.ItemType _type;
    PlayerActionContext _context;
    BodyHandler _bodyHandler;
    AnimationData _animData;
    AnimationPlayer _animPlayer;
    InteractableObject _item;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _type = data.EquipItem.ItemObject.ItemData.ItemType;
        _item = data.EquipItem;
        _context = data;
        _bodyHandler = bodyHandler;
        _animData = animData;
        _animPlayer = animPlayer;
        ItemUse();

        return true;
    }


    void ItemUse()
    {
        switch (_type)
        {
            case ItemType.TwoHanded:
                CoroutineHelper.StartCoroutine(HorizontalAttack());
                break;
            case ItemType.Ranged:
                UseItem();
                break;
            case ItemType.Consumable:
                CoroutineHelper.StartCoroutine(UsePotionAnim());
                break;
        }
    }


    IEnumerator HorizontalAttack()
    {
        if (PhotonNetwork.IsMasterClient)
            _item.ChangeUseTypeTrigger(0f, 1.1f);

        //_bodyHandler.LeftHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        //_bodyHandler.RightHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return ItemTwoHand(_context.ItemHandleSide, 0.07f, 0.1f, 0.5f, 0.1f, 3f);
    }

    IEnumerator UsePotionAnim()
    {
        //_bodyHandler.LeftHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        //_bodyHandler.RightHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return Potion(0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
        UseItem();
        _context.IsUpperActionProgress = false;
    }

    private void UseItem()
    {
        _item.ItemObject.Use();
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
            _animPlayer.PlayAnimAngle(itemTwoHands, i);
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
            _animPlayer.PlayAnimForce(itemTwoHands, i, dir, itemSwingPower);
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
            _animPlayer.PlayAnimAngle(itemTwoHands, i, dir);
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
            _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.PotionAngleAniData], i);
        }
    }

    void PotionStart()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.PotionReadyAniData].Length; i++)
        {
            _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.PotionReadyAniData], i);
        }
    }

    void PotionDrinking()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.PotionDrinkingAniData].Length; i++)
        {
            _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.PotionDrinkingAniData], i);
        }
    }
}
