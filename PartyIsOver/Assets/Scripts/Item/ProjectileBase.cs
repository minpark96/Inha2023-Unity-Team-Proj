using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ProjectileBase : MonoBehaviour
{
    public Actor Owner;
    public Vector3 InitialPosition;
    public Vector3 InitialDirection;

    public InteractableObject InteractableObject;

    public UnityAction OnShoot;


    public void Shoot(Item item)
    {
        Owner = item.Owner;
        InitialPosition = transform.position;
        InitialDirection = transform.forward;

        if (OnShoot != null)
            OnShoot.Invoke();
    }

 }
