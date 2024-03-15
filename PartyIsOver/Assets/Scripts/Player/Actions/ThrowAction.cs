using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
public class ThrowAction : PlayerAction
{
    public ThrowAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    AnimationData _animData;
    AnimationPlayer _animPlayer;
    BodyHandler _bodyHandler;
    PlayerActionContext _context;

    Define.ObjectType _type;
    InteractableObject _object;
    float _throwingForce = 40f;

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _context = data;
        _bodyHandler = bodyHandler;

        if (_context.RightGrabObject != null)
        {
            _type = _context.RightGrabObject.Type;
            _object = _context.RightGrabObject;
            ThrowObject();
        }

        return true;
    }

    void ThrowObject()
    {
        if (_type == ObjectType.Player)
        {
            _object.PullingForceTrigger(-_bodyHandler.Chest.PartTransform.up, _throwingForce);
            _object.PullingForceTrigger(-_bodyHandler.Chest.PartTransform.up, _throwingForce);
            _object.PullingForceTrigger(-_bodyHandler.Chest.PartTransform.up, _throwingForce);

            _object.PullingForceTrigger(Vector3.up, _throwingForce * 2f);
        }
        else if (_type == ObjectType.Object)
        {
            _object.PullingForceTrigger(-_bodyHandler.Chest.PartTransform.up, 7f);
            _object.PullingForceTrigger(Vector3.up, 2f);
            //_object.PhotonView.RPC("ChangeUseTypeTrigger", RpcTarget.MasterClient, 0.2f, 1f);
        }
        else if (_type == ObjectType.Item)
        {
            if(_object.ItemObject.ItemData.ItemType==Define.ItemType.Consumable)
                CoroutineHelper.StartCoroutine(PotionThrowAnim());
        }
        else
            Debug.LogError("잘못된 던지기 타입");
    }

    IEnumerator PotionThrowAnim()
    {
        CoroutineHelper.StartCoroutine(_object.ItemObject.ThrowItem());
        //CoroutineHelper.StartCoroutine(_object.ItemObject.ThrowItem());
        yield return PotionThrow(0.07f, 0.1f, 0.3f, 0.1f);
    }


    public IEnumerator PotionThrow(float duration, float ready, float start, float end)
    {
        float checkTime = Time.time;

        while (Time.time - checkTime < ready)
        {
            PotionThrowReady();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < start)
        {
            PotionThrowing();
            yield return new WaitForSeconds(duration);
        }
        checkTime = Time.time;

        while (Time.time - checkTime < end)
        {
            PotionThrowEnd();
            yield return new WaitForSeconds(duration);
        }
        _context.IsUpperActionProgress = false;
    }

    void PotionThrowReady()
    {
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.PotionAngleAniData].Length; i++)
        {
            _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.PotionAngleAniData], i);
        }
    }

    void PotionThrowing()
    {
        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.PotionThrowAniData].Length; i++)
        {
            _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.PotionThrowAniData], i);
        }
    }

    void PotionThrowEnd()
    {
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.PotionThrowAngleData].Length; i++)
        {
            _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.PotionThrowAngleData], i);
        }
    }
}
