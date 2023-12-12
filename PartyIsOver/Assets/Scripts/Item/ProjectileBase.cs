using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class ProjectileBase : MonoBehaviourPun
{
    public Actor Owner;
    public Item Gun;
    public Vector3 InitialPosition;
    public Vector3 InitialDirection;

    public InteractableObject InteractableObject;

    public UnityAction OnShoot;


    public void Shoot(Item item)
    {
        Owner = item.Owner;
        Gun = item;
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        gameObject.layer = Owner.gameObject.layer;

        if (OnShoot != null)
            OnShoot.Invoke();
    }

 }
