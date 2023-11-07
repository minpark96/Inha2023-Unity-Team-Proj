using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviour
{
    public Actor Owner;

    
    public ItemType ItemType;
    public InteractableObject InteractableObject;
    InteractableObject.Damage UseDamageType  = InteractableObject.Damage.Default; //나중에 아이템데이터에서 대입
    public float damage;


    public ItemData ItemData;


    public Transform OneHandedPos;
    public Transform TwoHandedPos;
    public Transform Head;
    public Transform Body;

    // Start is called before the first frame update
    void Start()
    {
        InteractableObject = GetComponent<InteractableObject>();
        Body = transform.GetChild(0);
        Head = transform.GetChild(1);
        OneHandedPos = transform.GetChild(2);
        if(transform.GetChild(3) != null && (ItemType == ItemType.TwoHanded || ItemType == ItemType.Ranged))
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

    }



}
