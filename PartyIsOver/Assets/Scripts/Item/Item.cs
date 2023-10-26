using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Item : MonoBehaviour
{
    public ItemType ItemType;
    public ItemNumber ItemNumber;


    public virtual void Use()
    {
        Debug.Log(name + "아이템을 사용하였습니다");

        switch (ItemNumber)
        {
            case ItemNumber.Hammer:

                break;

            case ItemNumber.Portion:
                Debug.Log("포션사용");
                //PlayerStats.instance.HealHealth(PlayerStats.instance.maxHealth);
                break;
        }
    }



    public virtual void Equip(ItemType ItemType)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
