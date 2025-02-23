using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 발이 바닥에 붙어 있는지 체크하는 클래스
public class GroundChecker : MonoBehaviourPun
{
    private LowerBodySM bodySM;

    private void Start()
    {
        bodySM = GetComponentInParent<Actor>().LowerSM;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (!bodySM.IsGrounded)
        {
            bodySM.IsGrounded = true;
        }
    }
}
