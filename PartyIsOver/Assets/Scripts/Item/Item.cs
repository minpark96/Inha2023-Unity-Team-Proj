using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviourPun
{
    public Actor Owner;  
    public InteractableObject InteractableObject;
    public ItemData ItemData;

    public Transform OneHandedPos;
    public Transform TwoHandedPos;
    public Transform Head;
    public Transform Body;

    public bool IsSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        InteractableObject = GetComponent<InteractableObject>();
        InteractableObject.damageModifier = InteractableObject.Damage.Default;
        GetComponent<Rigidbody>().mass = 10f;


        Body = transform.GetChild(0);
        Head = transform.GetChild(1);
        OneHandedPos = transform.GetChild(2);
        if(transform.GetChild(3) != null && (ItemData.ItemType == ItemType.TwoHanded 
            || ItemData.ItemType == ItemType.Ranged || ItemData.ItemType == ItemType.Gravestone))
        {
            TwoHandedPos = transform.GetChild(3);
        }
    }


    
    public virtual void Use()
    {
        //포션사용
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            Owner.StatusHandler.AddDamage(ItemData.UseDamageType, 0f, null);

        Destroy(gameObject,1f);
        //뚫어뻥 만들땐 스크립트 하나 더 파고 Item을 상속받아서 Use를 관절연결하는 함수로 오버라이드
        //방사형 만들때 ItemData 스크립트에서 Projectile을 일반 원거리무기의 투사체랑 같이 쓸 수 있게 하거나
        //ItemData 스크립트에서 Projectile을 빼고 원거리랑 방사형은 따로 새롭게 작업하는 식으로 진행
        
    }


    public IEnumerator ThrowItem()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);

        if(photonView.IsMine)
        {
            //ProjectileBase projectile = Managers.Pool.Pop(ItemData.Projectile.gameObject).GetComponent<ProjectileBase>();
            object[] instantiationData = new object[] { photonView.ViewID,false };
            string str = $"Prefabs/Projectiles/";
            str += ItemData.Projectile.name;
            ProjectileBase projectile = PhotonNetwork.Instantiate(str, Vector3.zero, Quaternion.identity, 0,instantiationData).GetComponent<ProjectileBase>();

            //projectile.gameObject.layer = Owner.gameObject.layer;

            Vector3 forward = new Vector3(-Owner.BodyHandler.Head.PartTransform.up.x, 0f, -Owner.BodyHandler.Head.PartTransform.up.z).normalized;
            Vector3 right = new Vector3(Owner.BodyHandler.Head.PartTransform.right.z, 0f, Owner.BodyHandler.Head.PartTransform.right.z).normalized;
            projectile.transform.position = Owner.BodyHandler.Chest.transform.position + (forward * 0.2f) + (Vector3.up * 0.1f) + (right * 0.2f);

            //Vector3 pos1 = Owner.BodyHandler.Chest.transform.position + forward * 10f + Vector3.up * 2f + right * 3f;


            Transform camArm = Owner.CameraControl.CameraArm;
            Vector3 lookForward = new Vector3(camArm.forward.x, 0f, camArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(camArm.right.x, 0f, camArm.right.z).normalized;

            Vector3 pos1 = Owner.BodyHandler.Chest.transform.position + lookForward.normalized * 10f + Vector3.up * 3f;

            Vector3 input = Owner.PlayerController.MoveInput;
            Vector3 moveDir = lookForward * input.z + lookRight * input.x;



            Vector3 pos2 = Owner.BodyHandler.Chest.transform.position + moveDir.normalized * 10f + Vector3.up * 3f;

            if (input.magnitude == 0f)
                projectile.transform.LookAt(pos1);
            else
                projectile.transform.LookAt(pos2);
            projectile.Shoot(this);

            projectile.photonView.RPC("ChangeUseTypeTrigger", RpcTarget.MasterClient, 0f, -1f);
            //projectile.GetComponent<InteractableObject>().ChangeUseTypeTrigger(0.08f, -1f);
        }

        Owner.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");
        Destroy(this, 1f);

    }
}

