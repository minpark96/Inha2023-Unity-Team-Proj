using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HandChecker : MonoBehaviourPun
{
    public bool isCheck = false;
    private Actor _actor; 

    // Start is called before the first frame update
    void Start()
    {
        _actor = transform.root.GetComponent<Actor>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;
        if(collision.collider == null) return;

        if (collision.collider.tag == "ItemHandle" && _actor.Grab._isGrabbing)
        {
            _actor.Grab.GrabObjectType = Define.GrabObjectType.Item;
            isCheck = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!photonView.IsMine) return;
        if (collision.collider == null) return;

        if (collision.collider.tag == "ItemHandle")
        {
            _actor.Grab.GrabObjectType = Define.GrabObjectType.None;
            isCheck = false;
        }
    }
}
