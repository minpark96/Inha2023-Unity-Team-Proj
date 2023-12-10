using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrunkState : MonoBehaviourPun
{
    private PlayerController _playerController;
    private Actor _actor;
    private Transform _playerTransform;
    private GameObject effectObject = null;
    private float _drunkActionDuration = 3f;

    public float DrunkDuration = 10f;

    void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
        _playerTransform = transform.Find("GreenHead").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if(effectObject != null)
            photonView.RPC("StatusMoveEffect", RpcTarget.All);
    }

    public IEnumerator DrunkActionReady()
    {
        _actor.BodyHandler.Head.PartRigidbody.AddForce(_actor.BodyHandler.Hip.PartTransform.up * 100f);
        yield return null;
    }

    public IEnumerator DrunkAction()
    {
        StatusCreateEffect("Effects/Flamethrower");
        float startTime = Time.time;

        while (Time.time - startTime < _drunkActionDuration)
        {
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.up * 100f);
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.forward * 30f);
            yield return null;
        }

        _playerController.IsFlambe = false;

        photonView.RPC("StatusDestroyEffect", RpcTarget.All, "Flamethrower");
    }

    public IEnumerator DrunkOff()
    {
        yield return new WaitForSeconds(DrunkDuration);
        _actor.debuffState = Actor.DebuffState.Default;

        _playerController.isDrunk = false;
        _actor.StatusHandler.HasDrunk = false;
        photonView.RPC("StatusDestroyEffect", RpcTarget.All, "Fog_poison");
        
    }

    [PunRPC]
    void StatusCreateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        effectObject.transform.position = _playerTransform.position;
    }

    [PunRPC]
    void StatusDestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    [PunRPC]
    void StatusMoveEffect()
    {
        if (effectObject != null && effectObject.name == "Flamethrower")
        {
            effectObject.transform.position = _playerTransform.position + _playerTransform.forward;
            effectObject.transform.rotation = Quaternion.LookRotation(-_playerTransform.right);
        }
        //else if(effectObject != null)
        //    effectObject.transform.position = _playerTransform.position;

    }
}
