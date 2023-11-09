using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyBullet : ProjectileStandard
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {


        base.OnHit(point, normal, collider);
    }
}
