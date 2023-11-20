using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : MonoBehaviour
{
    public Actor Actor;

    public float FirstPhaseDuration = 20f;
    public float SecondPhaseDuration = 20f;
    public float FirstPhaseStartTime = 20f;
    public float SecondPhaseStartTime = 80f;

    private void Start()
    {
        StartCoroutine(FirstPhase());
    }
    IEnumerator FirstPhase()
    {
        //yield return new WaitForSeconds(FirstPhaseStartTime);

        float startTime = Time.time;
        while(Time.time - startTime < FirstPhaseDuration)
        {
            Debug.Log("¹ÐÄ§");

            for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
            {
                Actor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * 1500f * Time.deltaTime, ForceMode.Force);
            }

            yield return null;
        }

        StartCoroutine(SecondPhase());
    }

    IEnumerator SecondPhase()
    {
        yield return new WaitForSeconds(SecondPhaseStartTime);

        float startTime = Time.time;
        while (Time.time - startTime < SecondPhaseDuration)
        {
            for (int i = 0; i < Actor.BodyHandler.BodyParts.Count; i++)
            {
                Actor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(Vector3.forward * 350f * Time.deltaTime);
            }

            yield return null;
        }
    }

}