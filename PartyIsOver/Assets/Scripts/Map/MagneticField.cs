using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    public float[] PhaseStartTime = { 0f, 45f, 45f, 45f };
    public float[] PhaseDuration = { 0f, 15f, 15f, 10f };
    
    private float[] _radius = { 0f, 52f, 22f, 11f };
    private float[] _scale = { 0f, 103.2f, 43.1f, 20.0f };
    private Vector3[] Point = { Vector3.zero, new Vector3(465.9f, 6.8f, 414.6f), new Vector3(444.3f, 6.8f, 422.1f), new Vector3(453.9f, 6.8f, 410.5f) };
    
    private float _magneticFieldStack = 2f;
    private float _magneticFieldStackRestore = 1f;
    private float _delay;

    private float _magneticRadius;
    private double _distance;

    private bool _isSynced;
    private bool _isInside;
    private bool _repeatFlag = false;


    public Actor Actor;

    private void Start()
    {
        _magneticRadius = _radius[1];
        transform.position = Point[1];
        transform.localScale = new Vector3(_scale[1], _scale[1], _scale[1]);

        StartCoroutine(FirstPhase(1));
    }

    private void Update()
    {
        if(!_isSynced)
        {
            StartCoroutine(WaitForSync());
            return;
        }

        InsideArea();

        if(Actor.MagneticStack >= 100f)
        {
            Actor.MagneticStack = 100f;

            // player 죽음 처리
            //Actor.Health = 0;
            //Actor.actorState = Actor.ActorState.Dead;
        }
    }
  

    void InsideArea()
    {
        Vector2 player = new Vector2(Actor.BodyHandler.Hip.transform.position.x, Actor.BodyHandler.Hip.transform.position.z);
        Vector2 map = new Vector2(transform.position.x, transform.position.z);

        _distance = Math.Sqrt(Math.Pow((player.x - map.x), 2) + Math.Pow((player.y - map.y), 2));

        if (_distance > _magneticRadius && _repeatFlag == false)
        {
            _isInside = false;
            _repeatFlag = true;
            StartCoroutine(MagneticDamage());
        }
        else if(_distance <= _magneticRadius)
        {
            _isInside = true;
            _repeatFlag = false;
            if (Actor.MagneticStack > 0)
                StartCoroutine(MagneticDamageRestore());
            else
                Actor.MagneticStack = 0;
        }
    }

    IEnumerator WaitForSync()
    {
        yield return new WaitForSeconds(7.0f); // actor정보 받기 위한 시간
        _isSynced = true;
    }

    IEnumerator FirstPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], _radius[index + 1], t) ;
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t));
            transform.position = new Vector3(Mathf.Lerp(Point[index].x, Point[index + 1].x, t), 6.8f, Mathf.Lerp(Point[index].z, Point[index + 1].z, t));

            yield return null;
        }

        yield return StartCoroutine(SecondPhase(2));
    }

    IEnumerator SecondPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], _radius[index + 1], t);
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t));
            transform.position = new Vector3(Mathf.Lerp(Point[index].x, Point[index + 1].x, t), 6.8f, Mathf.Lerp(Point[index].z, Point[index + 1].z, t));

            yield return null;
        }

        yield return StartCoroutine(ThirdPhase(3));
    }

    IEnumerator ThirdPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], 0, t);
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t));

            yield return null;
        }
    }

    IEnumerator MagneticDamage()
    {
        while (!_isInside)
        {
            Actor.MagneticStack += _magneticFieldStack;
            
            yield return new WaitForSeconds(_delay);
        }
    }

    IEnumerator MagneticDamageRestore()
    {
        while (_isInside)
        {
            Actor.MagneticStack -= _magneticFieldStackRestore;

            yield return new WaitForSeconds(_delay);
        }
    }

}
