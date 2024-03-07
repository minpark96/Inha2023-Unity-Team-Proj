using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangeWeapon : Item
{
    private int _bulletCount = 10;

    public override void Use()
    {
        if(_bulletCount-- > 0)
        {
            ProjectileBase projectile = Managers.Pool.Pop(ItemData.Projectile.gameObject).GetComponent<ProjectileBase>();
            projectile.gameObject.layer = Owner.gameObject.layer;

            Vector3 forward = -Owner.BodyHandler.Chest.PartTransform.up;
            projectile.transform.position = Owner.UpperSM.FirePoint.position + (forward * 0.2f) + (Vector3.up * 0.1f);
            projectile.transform.rotation = Quaternion.LookRotation(forward + new Vector3(0f, 0.37f, 0f));
            projectile.Shoot(this);
            Owner.PlayerEffectSound("PlayerEffect/Cartoon-UI-040");
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                projectile.GetComponent<InteractableObject>().ChangeUseTypeTrigger(0f, 5f);
            }
        }
    }

}
