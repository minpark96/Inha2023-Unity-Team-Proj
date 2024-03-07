using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
