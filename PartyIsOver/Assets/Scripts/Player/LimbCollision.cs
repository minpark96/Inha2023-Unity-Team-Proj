using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviourPun
{
    public PlayerController PlayerController;

    private void Start()
    {
         PlayerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine || PlayerController == null) return;
        if (!PlayerController.isGrounded)
            PlayerController.isGrounded = true;
    }
}
