using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollision : MonoBehaviour
{
    public PlayerControll playerControll;

    private void Start()
    {
        playerControll = GameObject.FindObjectOfType<PlayerControll>().GetComponent<PlayerControll>();
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("a");
        if(!playerControll.isGrounded)
            playerControll.isGrounded = true;
    }

}
