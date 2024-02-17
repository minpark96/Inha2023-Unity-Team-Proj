using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HandChecker : MonoBehaviourPun
{
    private UpperBodySM bodySM;
    public GrabObjectType CollisionObjectType = GrabObjectType.None;
    public InteractableObject CollisionObject = null;

    // Start is called before the first frame update
    void Start()
    {
        bodySM = GetComponentInParent<Actor>().UpperSM;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //태그로 처리하는걸로 수정해야함
    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;
        if(collision.collider == null) return;

        if(bodySM.IsGrabbingInProgress && collision.gameObject.GetComponent<InteractableObject>() != null)
        {
            CollisionObject = collision.gameObject.GetComponent<InteractableObject>();

            if (collision.collider.tag == "ItemHandle")
            {
                CollisionObjectType = Define.GrabObjectType.Item;
                return;
            }
            if (collision.gameObject.GetComponent<BodyPart>())
            {
                CollisionObjectType = Define.GrabObjectType.Player;
                return;
            }
            CollisionObjectType = Define.GrabObjectType.Object;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!photonView.IsMine) return;
        if (collision.collider == null) return;

        if (collision.gameObject.GetComponent<InteractableObject>() != null)
        {
            CollisionObjectType = Define.GrabObjectType.None;
            CollisionObject = null;
        }
    }
}
