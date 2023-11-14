using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrunkState : MonoBehaviour
{
    public PlayerController PlayerController;
    private Actor _actor;

    private float _drunkActionDuration = 1f;
    public float DrunkDuration = 10f;


    void Start()
    {
        PlayerController = GetComponentInParent<PlayerController>();
        _actor = GetComponentInParent<Actor>();
    }

   public IEnumerator DrunkActionReady()
    {
        _actor.BodyHandler.Head.PartRigidbody.AddForce(_actor.BodyHandler.Hip.PartTransform.up * 100f);
        yield return null;
    }
    public IEnumerator DrunkAction()
    {
        float startTime = Time.time;
        while (Time.time - startTime < _drunkActionDuration)
        {
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.up * 100f);
            _actor.BodyHandler.Head.PartRigidbody.AddForce(-_actor.BodyHandler.Hip.PartTransform.forward * 30f);
            yield return null;
        }
    }
    public IEnumerator DrunkOff()
    {
        yield return new WaitForSeconds(DrunkDuration);
        _actor.debuffState = Actor.DebuffState.Default;
        PlayerController.isDrunk = false;
    }
}
