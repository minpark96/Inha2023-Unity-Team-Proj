using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class ProjectileBase : MonoBehaviourPun
{
    public Actor Owner;
    public Item Gun;

    public InteractableObject InteractableObject;

    public UnityAction OnShoot;


    public void Shoot(Item item)
    {
        Owner = item.Owner;
        Gun = item;
        gameObject.layer = Owner.gameObject.layer;

        if (OnShoot != null)
            OnShoot.Invoke();
    }

 }
