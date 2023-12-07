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
    
    private float _magneticFieldStack = 10f;
    private float _magneticFieldStackRestore = 10f;
    private float _floorStack = 2f;
    private float _delay = 1f;

    private float _magneticRadius;
    private double _distance;

    bool _isSynced;

    // 1초간 스택 쌓는 조건
    bool _isInside;
    bool _isFloor;

    // 코루틴 1번 실행하게하는 flag
    bool _isDamaging;
    bool _isRestoring;
    bool _isFloorDamaging;

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


        TouchedFloor();


        if (Actor.MagneticStack >= 100f)
        {
            Actor.MagneticStack = 100f;

            // player 죽음 처리
            //Actor.Health = 0;
            //Actor.actorState = Actor.ActorState.Dead;

            Debug.Log("스택 쌓여서 사망");
        }
    }

    void TouchedFloor()
    {
        float player = Actor.BodyHandler.Hip.transform.position.y;
        float floor = 4.8f;

        if (player <= floor)
        {
            _isFloor = true;

            if(!_isFloorDamaging)
                StartCoroutine(DamagedByFloor());
        }
        else
        {
            _isFloor = false;

            if (!InsideArea() && !_isDamaging)
                StartCoroutine(DamagedByMagnetic());
            else if (InsideArea() && !_isRestoring)
                StartCoroutine(RestoreMagneticDamage());
        }
    }


    bool InsideArea()
    {
        Vector2 player = new Vector2(Actor.BodyHandler.Hip.transform.position.x, Actor.BodyHandler.Hip.transform.position.z);
        Vector2 map = new Vector2(transform.position.x, transform.position.z);

        _distance = Math.Sqrt(Math.Pow((player.x - map.x), 2) + Math.Pow((player.y - map.y), 2));

        if (_distance > _magneticRadius)
        {
            _isInside = false;
            return false;
        }
        else
        {
            _isInside = true;
            return true;
        }
    }


    IEnumerator WaitForSync()
    {
        yield return new WaitForSeconds(7.0f); // actor정보 받기 위한 시간
        _isSynced = true;
    }

    #region 자기장 Phase
    IEnumerator FirstPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        _magneticFieldStack = 10f;

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

        _magneticFieldStack = 14f;

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

        _magneticFieldStack = 20f;

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], 0, t);
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t));

            yield return null;
        }

        _magneticFieldStack = 33f;

    }
    #endregion


    IEnumerator DamagedByFloor()
    {
        _isFloorDamaging = true;
        _isDamaging = false;
        _isRestoring = false;


        while (Actor.MagneticStack <= 100 && _isFloor)
        {
            Actor.MagneticStack += _floorStack;

            yield return new WaitForSeconds(_delay);
        }
    }


    IEnumerator DamagedByMagnetic()
    {
        _isDamaging = true;
        _isRestoring = false;
        _isFloorDamaging = false;


        while (Actor.MagneticStack <= 100 && !_isInside && !_isFloor)
        {
            Actor.MagneticStack += _magneticFieldStack;
            
            yield return new WaitForSeconds(_delay);
        }
    }

    IEnumerator RestoreMagneticDamage()
    {
        _isRestoring = true;
        _isDamaging = false;
        _isFloorDamaging = false;


        while (Actor.MagneticStack > 0 && _isInside && !_isFloor)
        {
            Actor.MagneticStack -= _magneticFieldStackRestore;

            if (Actor.MagneticStack < 0)
                Actor.MagneticStack = 0;

            yield return new WaitForSeconds(_delay);
        }
    }

}
