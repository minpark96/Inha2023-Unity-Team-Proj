using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    public Actor Actor;

    public float FirstPhaseDuration = 15f;
    public float SecondPhaseDuration = 15f;
    public float FirstPhaseStartTime = 3f;
    public float SecondPhaseStartTime = 3f;

    private float _firstRadius = 35f;
    private float _secondRadius = 22f;
    private float _thirdRadius = 11f;
    private float _radius;

    private void Start()
    {
        _radius = _firstRadius;
        StartCoroutine(FirstPhase());
    }
    private void Update()
    {
        InsideArea();
    }

    void InsideArea()
    {
        Vector2 player = new Vector2(Actor.BodyHandler.Hip.transform.position.x, Actor.BodyHandler.Hip.transform.position.z);
        Vector2 map = new Vector2(transform.position.x, transform.position.z);

        double distance = Math.Sqrt(Math.Pow((player.x - map.x), 2) + Math.Pow((player.y - map.y), 2));

        if (distance < _radius)
            Debug.Log("in");
    }

    [PunRPC]
    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(FirstPhaseStartTime);

        float startTime = Time.time;
        while (Time.time - startTime < FirstPhaseDuration)
        {
            float t = (Time.time - startTime) / FirstPhaseDuration;
            _radius = Mathf.Lerp(_firstRadius, _secondRadius, t);
            transform.localScale = new Vector3(Mathf.Lerp(80, 50, t), Mathf.Lerp(80, 50, t), Mathf.Lerp(80, 50, t));
            transform.position = new Vector3(Mathf.Lerp(515, 508, t), 14, Mathf.Lerp(440, 450, t));

            yield return null;
        }

        yield return new WaitForSeconds(SecondPhaseStartTime);
        StartCoroutine(SecondPhase());
    }

    [PunRPC]
    IEnumerator SecondPhase()
    {
        float startTime = Time.time;
        while (Time.time - startTime < SecondPhaseDuration)
        {
            float t = (Time.time - startTime) / SecondPhaseDuration;
            _radius = Mathf.Lerp(_secondRadius, _thirdRadius, t);
            transform.localScale = new Vector3(Mathf.Lerp(50, 25, t), Mathf.Lerp(50, 25, t), Mathf.Lerp(50, 25, t));
            transform.position = new Vector3(Mathf.Lerp(508, 503, t), 14, Mathf.Lerp(450, 440, t));

            yield return null;
        }
    }



}
