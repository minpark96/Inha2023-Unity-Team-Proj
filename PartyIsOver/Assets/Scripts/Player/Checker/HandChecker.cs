using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HandChecker : MonoBehaviourPun
{
    private UpperBodySM bodySM;
    public ObjectType CollisionObjectType = ObjectType.None;
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

    //�±׷� ó���ϴ°ɷ� �����ؾ���
    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;
        if(collision.collider == null) return;

        if(bodySM.IsGrabbingInProgress && collision.gameObject.GetComponent<InteractableObject>() != null)
        {
            CollisionObject = collision.gameObject.GetComponent<InteractableObject>();

            if (collision.collider.tag == "ItemHandle")
            {
                CollisionObjectType = Define.ObjectType.Item;
                return;
            }
            if (collision.gameObject.GetComponent<BodyPart>())
            {
                CollisionObjectType = Define.ObjectType.Player;
                return;
            }
            CollisionObjectType = Define.ObjectType.Object;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!photonView.IsMine) return;
        if (collision.collider == null) return;

        if (collision.gameObject.GetComponent<InteractableObject>() != null)
        {
            CollisionObjectType = Define.ObjectType.None;
            CollisionObject = null;
        }
    }
}