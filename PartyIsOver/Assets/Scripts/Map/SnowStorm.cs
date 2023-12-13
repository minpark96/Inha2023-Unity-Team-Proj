using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : MonoBehaviour
{
    public float[] PhaseDuration = { 0f, 3f, 3f, 3f };
    public float[] PhaseStartTime = { 0f, 30f, 30f, 30f };
    public float StormForce = 10f;


    public List<Actor> ActorList;
    public Actor MyActor;

    private int _childCount = 3;
    private GameObject[] _snowStormChild = new GameObject[3];

    private void Start()
    {
        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i] = this.transform.GetChild(i).gameObject;
            _snowStormChild[i].SetActive(false);
        }

        StartCoroutine(FirstPhase());
    }

    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(PhaseStartTime[1]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }

        for (int i = 0; i  < ActorList.Count; i++)
        {
            if (ActorList[i].photonView.IsMine)
            {
                MyActor = ActorList[i];
            }
        }
       
        float startTime = Time.time;
        while(Time.time - startTime < PhaseDuration[1])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * StormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(SecondPhase());
    }

    IEnumerator SecondPhase()
    {
        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }

        yield return new WaitForSeconds(PhaseStartTime[2]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }


        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[2])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * StormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(ThirdPhase());
    }

    IEnumerator ThirdPhase()
    {
        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }

        yield return new WaitForSeconds(PhaseStartTime[3]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }


        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[3])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * StormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }
    }
}