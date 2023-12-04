using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

        photonView.RPC("DestroyEffect", RpcTarget.All, "Flamethrower");
    }

    public IEnumerator DrunkOff()
    {
        yield return new WaitForSeconds(DrunkDuration);
        _actor.debuffState = Actor.DebuffState.Default;

        _playerController.isDrunk = false;
        _actor.StatusHandler.HasDrunk = false;
        photonView.RPC("DestroyEffect", RpcTarget.All, "Fog_poison");
    }

    [PunRPC]
    void CreateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        effectObject.transform.position = _playerTransform.position;
    }

    [PunRPC]
    void DestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }
}
