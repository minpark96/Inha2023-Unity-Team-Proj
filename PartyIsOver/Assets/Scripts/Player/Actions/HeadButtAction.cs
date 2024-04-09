using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButtAction : BaseAction
{
    public HeadButtAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }


    AnimationPlayer _animPlayer;
    AnimationData _animData;
    PlayerActionContext _context;
    float _headButtCoolTime = 1f;
    Vector3 _moveDir = new Vector3();

    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _animData = animData;
        _animPlayer = animPlayer;
        _moveDir.x = data.InputDirX;
        _moveDir.y = data.InputDirY;
        _moveDir.z = data.InputDirZ;
        _context = data;
        CoroutineHelper.StartCoroutine(HeadButt());
        return true;
    }

    IEnumerator HeadButt()
    {
        //this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Headbutt;
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, true);

        for (int i = 0; i < _animData.FrameDataLists[Define.AniFrameData.HeadingAniData].Length; i++)
        {
            _animPlayer.PlayAnimForce(_animData.FrameDataLists[Define.AniFrameData.HeadingAniData], i);
        }
        for (int i = 0; i < _animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData].Length; i++)
        {
            if (i == 0)
                _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData], i, _moveDir + new Vector3(0f, 0.2f, 0f));
            if (i == 1)
                _animPlayer.PlayAnimAngle(_animData.AngleDataLists[Define.AniAngleData.HeadingAngleAniData], i, _moveDir + new Vector3(0f, 0.2f, 0f));
        }

        yield return new WaitForSeconds(_headButtCoolTime);
        //this._bodyHandler.Head.PartInteractable.damageModifier = InteractableObject.Damage.Default;
        //photonView.RPC("UpdateDamageModifier", RpcTarget.MasterClient, (int)Define.BodyPart.Head, false);

        _context.IsUpperActionProgress = false;

    }

}
