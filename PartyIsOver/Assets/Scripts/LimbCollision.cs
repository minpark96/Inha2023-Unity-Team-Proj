using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public Playercontroller playerController;

    private void Start()
    {
        playerController = GameObject.FindObjectOfType<Playercontroller>().GetComponent<Playercontroller>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerController.isGrounded = true;
    }

}
