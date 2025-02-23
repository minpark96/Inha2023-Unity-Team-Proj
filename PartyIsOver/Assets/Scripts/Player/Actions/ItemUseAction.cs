using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

// 아이템 사용 액션
public class ItemUseAction : BaseAction
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

    // 아이템 종류에 따라 사용 함수를 다르게 실행
    // 근접 무기는 휘두르는 애니메이션
    // 원거리 무기는 바로 UseItem 함수로 투사체 생성
    // 포션형 아이템은 먹는 동작 후 UseItem함수를 실행해 자신한테 버프 디버프 적용
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

    // 양손 무기 공격
    IEnumerator HorizontalAttack()
    {
        // 1.1초 동안 마스터 클라이언트에서만 데미지 타입을 변경해 충돌을 처리하도록 한다.
        if (PhotonNetwork.IsMasterClient)
            _item.ChangeUseTypeTrigger(0f, 1.1f);

        //_bodyHandler.LeftHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        //_bodyHandler.RightHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        
        // 무기를 횡으로 휘두르는 애니메이션 코루틴을 실행
        yield return ItemTwoHand(_context.ItemHandleSide, 0.07f, 0.1f, 0.5f, 0.1f, 3f);
    }
    
    // 포션을 먹는 애니메이션 동작
    IEnumerator UsePotionAnim()
    {
        //_bodyHandler.LeftHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));
        //_bodyHandler.RightHand.GetComponent<Rigidbody>().AddForce(new Vector3(_turnForce * 3, 0, 0));

        yield return Potion(0.07f, 0.1f, 0.5f, 0.5f, 0.1f);
        
        // 먹는 동작이 끝나면 UseItem 함수로 자신에게 버프, 디버프를 적용
        UseItem();
        _context.IsUpperActionProgress = false;
    }

    // 아이템의 Use 함수를 실행
    private void UseItem()
    {
        _item.ItemObject.Use();
    }

    // 양손 무기를 횡으로 휘두르는 애니메이션
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

    // 양손무기 준비동작 애니메이션
    public void ItemTwoHandReady(Define.Side side)
    {
        //upperArm 2 chest1 up right 0.01 20 foreArm chest up back 
        //TestRready 오른쪽 왼쪽 구별해서 좌우로 휘두룰수 있음
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
