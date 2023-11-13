using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonCollision : MonoBehaviourPun
{
    public BalloonState Balloon;

    private void Start()
    {
        Balloon = GetComponentInParent<BalloonState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (!Balloon.IsGrounded)
            Balloon.IsGrounded = true;
    }

}
