using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviour
{
    public Actor Owner;  
    public InteractableObject InteractableObject;
    public ItemData ItemData;


    public Transform OneHandedPos;
    public Transform TwoHandedPos;
    public Transform Head;
    public Transform Body;

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

    // Update is called once per frame
    void Update()
    {
    
    }

    public virtual void Use()
    {
        //포션사용
        InteractableObject.damageModifier = ItemData.UseDamageType;
        Destroy(gameObject,1f);
        //뚫어뻥 만들땐 스크립트 하나 더 파고 Item을 상속받아서 Use를 관절연결하는 함수로 오버라이드
        //방사형 만들때 ItemData 스크립트에서 Projectile을 일반 원거리무기의 투사체랑 같이 쓸 수 있게 하거나
        //ItemData 스크립트에서 Projectile을 빼고 원거리랑 방사형은 따로 새롭게 작업하는 식으로 진행
        
    }
}
