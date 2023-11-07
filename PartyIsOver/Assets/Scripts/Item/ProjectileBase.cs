using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ProjectileBase : MonoBehaviour
{
    public Actor owner { get; private set; }
    public Vector3 initialPosition { get; private set; }
    public Vector3 initialDirection { get; private set; }

    public UnityAction OnShoot;

    public void Shoot(Item item)
    {
        owner = item.Owner;
        initialPosition = transform.position;
        initialDirection = transform.forward;

        if (OnShoot != null)
            OnShoot.Invoke();
    }

 }
