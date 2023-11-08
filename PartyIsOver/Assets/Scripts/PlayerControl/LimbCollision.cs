using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviourPun
{
    public PlayerController playerController;

    private void Start()
    {
         playerController = GetComponentInParent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (!playerController.isGrounded)
            playerController.isGrounded = true;
    }

}
