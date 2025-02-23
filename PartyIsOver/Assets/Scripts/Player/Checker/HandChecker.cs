using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

// 손에 충돌한 오브젝트의 타입을 체크하는 클래스
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

    //태그로 처리하는걸로 수정해야함
    private void OnCollisionStay(Collision collision)
    {
        if (!photonView.IsMine) return;
        if(collision.collider == null) return;
        
        // 현재 잡기 동작이 수행중이고, 충돌한 오브젝트가 상호작용 가능한 오브젝트라면
        if(bodySM.IsGrabbingInProgress && collision.gameObject.GetComponent<InteractableObject>() != null)
        {
            CollisionObject = collision.gameObject.GetComponent<InteractableObject>();
            
            // 충돌한 오브젝트가 아이템인지, 플레이어인지, 일반 오브젝트인지 판별
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
