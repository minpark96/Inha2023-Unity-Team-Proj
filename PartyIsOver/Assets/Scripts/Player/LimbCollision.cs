using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviourPun
{
    private LowerBodySM bodySM;
    PlayerController playerController;

    private void Start()
    {
        bodySM = GetComponentInParent<LowerBodySM>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (!bodySM.isGrounded)
        {
            //bodySM.isGrounded = true;
            playerController.isGrounded = true;
        }
    }
}
