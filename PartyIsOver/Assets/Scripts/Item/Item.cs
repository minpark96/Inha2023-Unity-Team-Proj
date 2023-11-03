using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviour
{
    public ItemType ItemType;

    private Actor _owner;
    
    public InteractableObject InteractableObject;
    public InteractableObject.Damage damageType;
    public float mass;

    public Transform OneHandedPos;
    public Transform TwoHandedPos;
    public Transform Head;

    // Start is called before the first frame update
    void Start()
    {
        InteractableObject = GetComponent<InteractableObject>();
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



    public virtual void Equip(ItemType ItemType)
    {
        
    }

    

}
