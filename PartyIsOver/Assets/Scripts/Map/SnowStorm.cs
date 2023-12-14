using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStorm : MonoBehaviour
{
    public float[] PhaseDuration = { 0f, 8f, 8f, 8f };
    public float[] PhaseStartTime = { 0f, 30f, 30f, 30f };
    private float _stormForce = 7.5f;


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

        StartCoroutine(FirstPhase(1));
    }

    IEnumerator FirstPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }

        for (int i = 0; i  < ActorList.Count; i++)
        {
            if (ActorList[i].photonView.IsMine)
            {
                MyActor = ActorList[i];
                break;
            }
        }
       
        float startTime = Time.time;
        while(Time.time - startTime < PhaseDuration[index])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(SecondPhase(2));
    }

    IEnumerator SecondPhase(int index)
    {
        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }

        yield return new WaitForSeconds(PhaseStartTime[index]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }


        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        StartCoroutine(ThirdPhase(3));
    }

    IEnumerator ThirdPhase(int index)
    {
        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }

        yield return new WaitForSeconds(PhaseStartTime[index]);

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(true);
        }


        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                MyActor.BodyHandler.BodyParts[i].PartRigidbody.AddForce(transform.right * _stormForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            yield return null;
        }

        for (int i = 0; i < _childCount; i++)
        {
            _snowStormChild[i].SetActive(false);
        }
    }
}