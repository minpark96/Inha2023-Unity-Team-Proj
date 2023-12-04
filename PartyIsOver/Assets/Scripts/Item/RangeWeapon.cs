using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangeWeapon : Item
{
    public override void Use()
    {
        Fire();
    }

    void Fire()
    {
        ProjectileBase projectile  = Managers.Pool.Pop(ItemData.Projectile.gameObject).GetComponent<ProjectileBase>();

        Vector3 forward = -Owner.BodyHandler.Chest.PartTransform.up;
        projectile.transform.position = Owner.Grab.FirePoint.position + (forward * 0.2f) + (Vector3.up*0.1f);
        projectile.transform.rotation = Quaternion.LookRotation(forward + new Vector3(0f, 0.37f, 0f));
        projectile.Shoot(this);
        Owner.PlayerController.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");
        if (PhotonNetwork.IsMasterClient)
        {
            projectile.GetComponent<InteractableObject>().ChangeUseTypeTrigger(0.08f, 5f);
        }
    }
}
