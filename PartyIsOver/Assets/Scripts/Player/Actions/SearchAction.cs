using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 주변의 상호작용 가능한 오브젝트가 있는지 탐색하는 액션
// 초당 60번 실행되며 탐색한 오브젝트를 context에 저장한다.
public class SearchAction : BaseAction
{
    public SearchAction(ActionController actions, Define.ActionEventName name) : base(actions, name)
    {
    }

    PlayerActionContext _context;
    BodyHandler _bodyHandler;
    protected override bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _bodyHandler = bodyHandler;

        SearchTarget();
        return true;
    }


    private void SearchTarget()
    {
        // 타겟서치 태그설정 주의할것
        // TargetingHandler의 SearchTarget를 사용해 서치된 결과를 Context에 저장한다.
        _context.LeftSearchTarget = TargetingHandler.SearchTarget(Define.Side.Left,_bodyHandler.Chest.PartTransform,_context.Layer,_context.IsGrounded);
        _context.RightSearchTarget = TargetingHandler.SearchTarget(Define.Side.Right,_bodyHandler.Chest.PartTransform,_context.Layer, _context.IsGrounded);

        Debug.Log(_context.RightSearchTarget);

        // 서치한 오브젝트가 null이 아니고, PhotonView를 보유하고 있을 경우
        // 타겟의 PhotonView ID를 받아와서 BroadcastFoundTarget로 모든 클라이언트의 이 캐릭터가
        // _context.LeftSearchTarget에 좀 전에 받아온 PhotonView id를 저장하게 한다.
        if (_context.LeftSearchTarget != null && _context.LeftSearchTarget.PhotonView != null)
        {
            int id = _context.LeftSearchTarget.PhotonView.ViewID;
            //PhotonNetwork.GetPhotonView(_context.Id).RPC("BroadcastFoundTarget", RpcTarget.All, 0, id);
        }
        if (_context.RightSearchTarget != null && _context.RightSearchTarget.PhotonView != null)
        {
            int id = _context.RightSearchTarget.PhotonView.ViewID;
            //PhotonNetwork.GetPhotonView(_context.Id).RPC("BroadcastFoundTarget", RpcTarget.All, 1, id);
        }

        // 양 손이 물체에 제일 가까운 방향을 찾기 위해 FindClosestCollisionPoint함수를 사용하여 나온 방향을 저장
        if (_context.LeftSearchTarget != null)
            _context.LeftTargetDir = TargetingHandler.FindClosestCollisionPoint(_context.Position,_context.LeftSearchTarget.ColliderObject,_context.Layer);
        else
            _context.LeftTargetDir = Vector3.zero;

        if (_context.RightSearchTarget != null)
            _context.RightTargetDir = TargetingHandler.FindClosestCollisionPoint(_context.Position,_context.RightSearchTarget.ColliderObject,_context.Layer);
        else
            _context.RightTargetDir = Vector3.zero;
    }


    [PunRPC]
    private void BroadcastFoundTarget(int side, int id)
    {
        Debug.Log("BroadcastTarget");

        if (side == 0)
        {
            _context.LeftSearchTarget = PhotonNetwork.GetPhotonView(id).transform.GetComponent<InteractableObject>();
        }
        else if (side == 1)
        {
            _context.RightSearchTarget = PhotonNetwork.GetPhotonView(id).transform.GetComponent<InteractableObject>();
        }
    }
}
