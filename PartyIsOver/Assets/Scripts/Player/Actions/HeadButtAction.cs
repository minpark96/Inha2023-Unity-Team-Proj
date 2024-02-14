using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButtAction
{
    public HeadButtAction(ActionController actions)
    {
        actions.OnHeadButt -= HandleHeadButtEvent;
        actions.OnHeadButt += HandleHeadButtEvent;
        this._actions = actions;
    }

    ActionController _actions;
    AnimationPlayer _animPlayer;
    //BodyHandler _bodyHandler;
    AnimationData _animData;
    float _headButtCoolTime = 1f;
    Vector3 _moveDir = new Vector3();

    public bool HandleHeadButtEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        //_bodyHandler = bodyhandler;

        _moveDir.x = data.DirX;
        _moveDir.y = data.DirY;
        _moveDir.z = data.DirZ;

        CoroutineHelper.StartCoroutine(HeadButt());
        return true;
    }

    IEnumerator HeadButt()
    {
        //this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, true);

        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.HeadingAniData].Length; i++)
        {
            _animPlayer.AniForce(_animData.FrameDataLists[Define.AniFrameData.HeadingAniData], i);
        }
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData].Length; i++)
        {
            if (i == 0)
                _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData], i, _moveDir + new Vector3(0f, 0.2f, 0f));
            if (i == 1)
                _animPlayer.AniAngleForce(_animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData], i, _moveDir + new Vector3(0f, 0.2f, 0f));
        }

        yield return new WaitForSeconds(_headButtCoolTime);
        //this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, false);

        _actions.UpperActionEnd();
    }

}
