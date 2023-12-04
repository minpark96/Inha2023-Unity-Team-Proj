using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    public float FirstPhaseStartTime = 45f;
    public float SecondPhaseStartTime = 45f;
    public float ThirdPhaseStartTime = 45f;

    public float FirstPhaseDuration = 15f;
    public float SecondPhaseDuration = 15f;
    public float ThirdPhaseDuration = 10f;

    private float _magneticFieldStack = 2f;
    private float _magneticFieldStackRestore = 1f;

    private float _firstRadius = 52f;
    private float _secondRadius = 22f;
    private float _thirdRadius = 11f;
    private float _radius;
    private double _distance;
    private bool _isInside;
    private bool _repeatFlag = false;
    private bool _waitFlag;

    private Vector3 FirstPoint = new Vector3(465.9f, 6.8f, 414.6f);
    private Vector3 SecondPoint = new Vector3(444.3f, 6.8f, 422.1f);
    private Vector3 ThirdPoint = new Vector3(453.9f, 6.8f, 410.5f);

    private float FirstScale = 103.2f;
    private float SecondScale = 43.1f;
    private float ThirdScale = 20.0f;

    public Actor Actor;

    private void Start()
    {
        _radius = _firstRadius;
        transform.position = FirstPoint;
        transform.localScale = new Vector3(FirstScale, FirstScale, FirstScale);

        StartCoroutine(FirstPhase());
    }

    private void Update()
    {
        return;
        if(!_waitFlag)
        {
            StartCoroutine(WaitForSync());
            return;
        }

        InsideArea();

        if(Actor.MagneticStack >= 100f)
        {
            Actor.MagneticStack = 100f;

            // player Á×À½ Ã³¸®
            //Actor.Health = 0;
            //Actor.actorState = Actor.ActorState.Dead;

        }
    }

    IEnumerator WaitForSync()
    {
        yield return new WaitForSeconds(FirstPhaseStartTime);

        _waitFlag = true;
    }

    void InsideArea()
    {
        Vector2 player = new Vector2(Actor.BodyHandler.Hip.transform.position.x, Actor.BodyHandler.Hip.transform.position.z);
        Vector2 map = new Vector2(transform.position.x, transform.position.z);

        _distance = Math.Sqrt(Math.Pow((player.x - map.x), 2) + Math.Pow((player.y - map.y), 2));

        if (_distance >= _radius && _repeatFlag == false)
        {
            _isInside = false;
            _repeatFlag = true;
            StartCoroutine(MagneticDamage());
        }
        else if(_distance < _radius)
        {
            _isInside = true;
            _repeatFlag = false;
            if (Actor.MagneticStack > 0)
                StartCoroutine(MagneticDamageRestore());
            else
                Actor.MagneticStack = 0;
        }
    }

    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(FirstPhaseStartTime);

        float startTime = Time.time;
        while (Time.time - startTime < FirstPhaseDuration)
        {
            float t = (Time.time - startTime) / FirstPhaseDuration;
            _radius = Mathf.Lerp(_firstRadius, _secondRadius, t);
            transform.localScale = new Vector3(Mathf.Lerp(FirstScale, SecondScale, t), Mathf.Lerp(FirstScale, SecondScale, t), Mathf.Lerp(FirstScale, SecondScale, t));
            transform.position = new Vector3(Mathf.Lerp(FirstPoint.x, SecondPoint.x, t), 6.8f, Mathf.Lerp(FirstPoint.z, SecondPoint.z, t));

            yield return null;
        }

        yield return new WaitForSeconds(SecondPhaseStartTime);
        StartCoroutine(SecondPhase());
    }

    IEnumerator SecondPhase()
    {
        float startTime = Time.time;
        while (Time.time - startTime < SecondPhaseDuration)
        {
            float t = (Time.time - startTime) / SecondPhaseDuration;
            _radius = Mathf.Lerp(_secondRadius, _thirdRadius, t);
            transform.localScale = new Vector3(Mathf.Lerp(SecondScale, ThirdScale, t), Mathf.Lerp(SecondScale, ThirdScale, t), Mathf.Lerp(SecondScale, ThirdScale, t));
            transform.position = new Vector3(Mathf.Lerp(SecondPoint.x, ThirdPoint.x, t), 6.8f, Mathf.Lerp(SecondPoint.z, ThirdPoint.z, t));

            yield return null;
        }

        yield return new WaitForSeconds(ThirdPhaseStartTime);
        StartCoroutine(ThirdPhase());
    }

    IEnumerator ThirdPhase()
    {
        float startTime = Time.time;
        while (Time.time - startTime < ThirdPhaseDuration)
        {
            float t = (Time.time - startTime) / ThirdPhaseDuration;
            _radius = Mathf.Lerp(_thirdRadius, 0, t);
            transform.localScale = new Vector3(Mathf.Lerp(ThirdScale, 0, t), Mathf.Lerp(ThirdScale, 0, t), Mathf.Lerp(ThirdScale, 0, t));

            yield return null;
        }
    }

    IEnumerator MagneticDamage()
    {
        while (!_isInside)
        {
            Actor.MagneticStack += _magneticFieldStack;
            
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator MagneticDamageRestore()
    {
        while (_isInside)
        {
            Actor.MagneticStack -= _magneticFieldStackRestore;

            yield return new WaitForSeconds(1.0f);
        }
    }

}
