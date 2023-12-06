using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkState : MonoBehaviourPun
{
    private PlayerController _playerController;
    private Actor _actor;
    public Transform _playerTransform;
    public GameObject effectObject = null;
    private float _drunkActionDuration = 3f;

    public float DrunkDuration = 10f;

    void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
        _playerTransform = _actor.StatusHandler.playerTransform;
        effectObject = _actor.StatusHandler.effectObject;
    }

    public IEnumerator DrunkActionReady()
    {
        _actor.BodyHandler.Head.PartRigidbody.AddForce(_actor.BodyHandler.Hip.PartTransform.up * 100f);
        yield return null;
    }

    public IEnumerator DrunkAction()
    {
        photonView.RPC("CreateEffect", RpcTarget.All, "Effects/Flamethrower");
        float startTime = Time.time;

        while (Time.time - startTime < _drunkActionDuration)
        {
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.up * 100f);
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.forward * 30f);
            yield return null;
        }

        _playerController.IsFlambe = false;

        photonView.RPC("TestDestroyEffect", RpcTarget.All, "Flamethrower");
    }

    public IEnumerator DrunkOff()
    {
        yield return new WaitForSeconds(DrunkDuration);
        _actor.debuffState = Actor.DebuffState.Default;

        _playerController.isDrunk = false;
        _actor.StatusHandler.HasDrunk = false;
        photonView.RPC("TestDestroyEffect", RpcTarget.All, "Fog_poison");
    }

    [PunRPC]
    void CreateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }

    [PunRPC]
    void TestDestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }
    
    public void TestPlayerEffect()
    {
        effectObject.transform.position = _actor.StatusHandler.playerTransform.position;
    }

}
