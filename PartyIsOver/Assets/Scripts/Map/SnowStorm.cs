using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : MonoBehaviour
{
    public float FirstPhaseDuration = 20f;
    public float SecondPhaseDuration = 20f;
    public float ThirdPhaseDuration = 20f;

    public float FirstPhaseStartTime = 20f;
    public float SecondPhaseStartTime = 40f;
    public float ThirdPhaseStartTime = 40f;

    public Actor Actor;

    private float _stormForce = 7f;

    private void Start()
    {
        StartCoroutine(FirstPhase());
    }

    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(FirstPhaseStartTime);
        
        //Actor.PlayerController.RunSpeed = 200;

        float startTime = Time.time;
        while(Time.time - startTime < FirstPhaseDuration)
        {
            for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
            {
                Actor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(SecondPhase());
    }

    IEnumerator SecondPhase()
    {
        //Actor.PlayerController.RunSpeed = 100;

        yield return new WaitForSeconds(SecondPhaseStartTime);

        //Actor.PlayerController.RunSpeed = 200;

        float startTime = Time.time;
        while (Time.time - startTime < SecondPhaseDuration)
        {
            for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
            {
                Actor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(ThirdPhase());
    }

    IEnumerator ThirdPhase()
    {
        //Actor.PlayerController.RunSpeed = 100;

        yield return new WaitForSeconds(ThirdPhaseStartTime);

        //Actor.PlayerController.RunSpeed = 200;

        float startTime = Time.time;
        while (Time.time - startTime < ThirdPhaseDuration)
        {
            for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
            {
                Actor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }
    }
}