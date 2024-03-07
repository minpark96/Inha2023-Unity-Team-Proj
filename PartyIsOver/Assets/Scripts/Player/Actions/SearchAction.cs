using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction
{
    public SearchAction(ActionController actions)
    {
        actions.OnTargetSearch -= HandleSearchEvent;
        actions.OnTargetSearch += HandleSearchEvent;
    }
    PlayerActionContext _context;
    BodyHandler _bodyHandler;
    public bool HandleSearchEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext data)
    {
        _context = data;
        _bodyHandler = bodyHandler;

        SearchTarget();
        return true;
    }


    private void SearchTarget()
    {
        //타겟서치 태그설정 주의할것
        _context.LeftSearchTarget = TargetingHandler.SearchTarget(Define.Side.Left,_bodyHandler.Chest.PartTransform,_context.Layer,_context.IsGrounded);
        _context.RightSearchTarget = TargetingHandler.SearchTarget(Define.Side.Right,_bodyHandler.Chest.PartTransform,_context.Layer, _context.IsGrounded);

        Debug.Log(_context.RightSearchTarget);

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
