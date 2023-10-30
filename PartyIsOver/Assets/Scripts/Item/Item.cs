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

    // Start is called before the first frame update
    void Start()
    {
        InteractableObject = GetComponent<InteractableObject>();
        OneHandedPos = transform.GetChild(0).transform;
        if(transform.GetChild(1) != null )
        {
            TwoHandedPos = transform.GetChild(1).transform;
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
