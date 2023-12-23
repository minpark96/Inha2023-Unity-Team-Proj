using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ProjectileStandard : ProjectileBase
{
    public Transform root;

    [SerializeField]
    private float _speed = 15f;
    public float maxLifeTime = 5f;

    private ProjectileBase projectileBase;


    //이동
    private Vector3 lastRootPosition;
    private Vector3 velocity;

    //중력
    public float gravityDownAcceleration = 0f;

    //Hit
    public List<Collider> _ignoredColliders;
    public float radius = 0.01f;

    public GameObject impactVfxPrefab;
    public float impactVfxOffset = 0.01f;
    public float impactVfxLifeTime = 3f;

    private void OnEnable()
    {
        projectileBase = this.GetComponent<ProjectileBase>();
        projectileBase.InteractableObject = GetComponent<InteractableObject>();
        projectileBase.OnShoot += OnShoot;


        if(GetComponent<PhotonView>()!=null && photonView.ViewID != 0)
        {
            Owner = PhotonNetwork.GetPhotonView((int)photonView.InstantiationData[0]).GetComponent<Item>().Owner;
            gameObject.layer = Owner.gameObject.layer;
            GetComponent<Poolable>().IsUsing = false;
        }
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(maxLifeTime);
        GetComponent<InteractableObject>().ResetType();

        if (GetComponent<Poolable>() != null && !GetComponent<Poolable>().IsUsing)
        {
            if(photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            if (gameObject.activeSelf)
                Managers.Resource.Destroy(gameObject);
        }
    }

    new void OnShoot()
    {
        lastRootPosition = root.position;
        velocity = transform.forward * _speed;

        //무시되는 충돌체 가져오기
        _ignoredColliders = new List<Collider>();
        Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
        _ignoredColliders.AddRange(ownerColliders);
    }

    private void Update()
    {
        //중력
        if (gravityDownAcceleration > 0f)
        {
            velocity += Vector3.down * gravityDownAcceleration * Time.deltaTime;
        }

        transform.position += velocity * Time.deltaTime;

        //Hit
        RaycastHit nearHit = new RaycastHit();
        nearHit.distance = Mathf.Infinity;
        bool isfindHit = false;

        Vector3 displamentSinceLastFrame = root.position - lastRootPosition;
        RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, radius,
            displamentSinceLastFrame.normalized, displamentSinceLastFrame.magnitude);
        foreach (var hit in hits)
        {
            if(IsHitValid(hit))
            {
                if(hit.distance < nearHit.distance)
                {
                    nearHit = hit;
                    isfindHit = true;
                }
            }
        }

        if (isfindHit)
        {
            if(nearHit.collider.GetComponentInParent<CollisionHandler>())
            {
                Debug.Log(nearHit.transform.gameObject);
                OnHit(nearHit.point, nearHit.normal, nearHit.collider);
                //Enemy enemy = nearHit.collider.GetComponentInParent<Enemy>();
                //enemy.SetState(EnemyState.E_OnDamage);
            }
        }


        transform.forward = velocity.normalized;
        lastRootPosition = root.position;
    }

    [PunRPC]
    public virtual void OnHit(Vector3 point, Vector3 normal,Collider collider)
    {
        //이펙트
        if (impactVfxPrefab!=null)
        {
            GameObject impactEff = Instantiate(impactVfxPrefab, point + (normal * impactVfxOffset), Quaternion.LookRotation(normal));
            if (impactVfxLifeTime > 0)
            {
                Destroy(impactEff.gameObject, impactVfxLifeTime);

            }
        }

        DestroyProjectile();
    }
    
    public void DestoryProjectileTrigger()
    {
        //photonView.RPC("DestroyProjectile", RpcTarget.All);
        DestroyProjectile();
    }

    [PunRPC]
    public void DestroyProjectile()
    {
        GetComponent<InteractableObject>().ResetType();

        if (GetComponent<Poolable>() != null && !GetComponent<Poolable>().IsUsing)
        {
            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
        else
        {
           Managers.Resource.Destroy(gameObject);
        }
    }

    private bool IsHitValid(RaycastHit hit)
    {
        if (_ignoredColliders != null && _ignoredColliders.Contains(hit.collider))
        {
            return false;
        }
        return true;
    }
}
