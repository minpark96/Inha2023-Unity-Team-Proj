using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    public float[] PhaseStartTime = { 0f, 45f, 45f, 45f };
    public float[] PhaseDuration = { 0f, 15f, 15f, 10f };

    public GameObject FreezeImage;


    private float[] _radius = { 0f, 52f, 22f, 11f };
    private float[] _scale = { 0f, 103.2f, 43.1f, 20.0f };
    private Vector3[] _point = { Vector3.zero, new Vector3(465.9f, 6.8f, 414.6f), new Vector3(444.3f, 6.8f, 422.1f), new Vector3(453.9f, 6.8f, 410.5f) };

    private float[] _magneticFieldStack = { 0f, 10f, 14f, 20f, 33f };
    private float _stackRestore = 10f;
    private float _floorStack = 20f;
    private float _delay = 1f;

    private float _stack;
    private float _magneticRadius;
    private double _distance;

    private GameObject MagneticFieldEffect;
    private Vector3[] _effect1Position = { new Vector3(-103.46f, -14.85f, -9.04f), new Vector3(-68f, -9.26f, -2.31f), new Vector3(-52.08f, -4.03f, 3.97f)};
    private Vector3[] _effect3Position = { new Vector3(31.1f, -56.3f, -45), new Vector3(34.41f, -34.21f, -18.44f) };
    private Vector3[] _effect4Position = { new Vector3(80.9f, 27.6f, 42.4f), new Vector3(54.95f, 19.03f, 32.17f), new Vector3(34.39f, 12.27f, 24.06f) };
    private Transform _effect1;
    private Transform _effect3;
    private Transform _effect4;


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
        transform.position = _point[1];
        transform.localScale = new Vector3(_scale[1], _scale[1], _scale[1]);

        MagneticFieldEffect = GameObject.Find("Magnetic Field Effect");
        _effect1 = MagneticFieldEffect.transform.GetChild(0);
        _effect3 = MagneticFieldEffect.transform.GetChild(2);
        _effect4 = MagneticFieldEffect.transform.GetChild(3);

        GameObject mainPanel = GameObject.Find("Main Panel");
        FreezeImage = mainPanel.transform.GetChild(0).gameObject;

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
        ChangeMainPanel();

        if (Actor.MagneticStack >= 100f)
        {
            Actor.MagneticStack = 100f;

            // player 죽음 처리
            //Actor.Health = 0;
            //Actor.actorState = Actor.ActorState.Dead;

            Debug.Log("스택 쌓여서 사망");
        }
    }

    void ChangeMainPanel()
    {
        for (int i = 0; i < 5; i++)
            FreezeImage.transform.GetChild(i).gameObject.SetActive(false);

        if (Actor.MagneticStack >= 20 && Actor.MagneticStack < 40)
            FreezeImage.transform.GetChild(0).gameObject.SetActive(true);
        else if (Actor.MagneticStack >= 40 && Actor.MagneticStack < 60)
            FreezeImage.transform.GetChild(1).gameObject.SetActive(true);
        else if (Actor.MagneticStack >= 60 && Actor.MagneticStack < 80)
            FreezeImage.transform.GetChild(2).gameObject.SetActive(true);
        else if (Actor.MagneticStack >= 80 && Actor.MagneticStack < 100)
            FreezeImage.transform.GetChild(3).gameObject.SetActive(true);
        else if (Actor.MagneticStack >= 100)
            FreezeImage.transform.GetChild(4).gameObject.SetActive(true);
    }

    #region 영역 검사

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

    #endregion

    IEnumerator WaitForSync()
    {
        yield return new WaitForSeconds(7.0f); // actor정보 받기 위한 시간
        _isSynced = true;
    }

    #region 자기장 Phase
    IEnumerator FirstPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        _stack = _magneticFieldStack[1];

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], _radius[index + 1], t) ;
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t));
            transform.position = new Vector3(Mathf.Lerp(_point[index].x, _point[index + 1].x, t), 6.8f, Mathf.Lerp(_point[index].z, _point[index + 1].z, t));

            _effect1.localPosition = new Vector3(Mathf.Lerp(_effect1Position[0].x, _effect1Position[1].x, t), Mathf.Lerp(_effect1Position[0].y, _effect1Position[1].y, t), Mathf.Lerp(_effect1Position[0].z, _effect1Position[1].z, t));

            yield return null;
        }

        yield return StartCoroutine(SecondPhase(2));
    }

    IEnumerator SecondPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        _stack = _magneticFieldStack[2];

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], _radius[index + 1], t);
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t));
            transform.position = new Vector3(Mathf.Lerp(_point[index].x, _point[index + 1].x, t), 6.8f, Mathf.Lerp(_point[index].z, _point[index + 1].z, t));

            _effect3.localPosition = new Vector3(Mathf.Lerp(_effect3Position[0].x, _effect3Position[1].x, t), Mathf.Lerp(_effect3Position[0].y, _effect3Position[1].y, t), Mathf.Lerp(_effect3Position[0].z, _effect3Position[1].z, t));
            _effect4.localPosition = new Vector3(Mathf.Lerp(_effect4Position[0].x, _effect4Position[1].x, t), Mathf.Lerp(_effect4Position[0].y, _effect4Position[1].y, t), Mathf.Lerp(_effect4Position[0].z, _effect4Position[1].z, t));

            yield return null;
        }

        yield return StartCoroutine(ThirdPhase(3));
    }

    IEnumerator ThirdPhase(int index)
    {
        yield return new WaitForSeconds(PhaseStartTime[index]);

        _stack = _magneticFieldStack[3];

        float startTime = Time.time;
        while (Time.time - startTime < PhaseDuration[index])
        {
            float t = (Time.time - startTime) / PhaseDuration[index];
            _magneticRadius = Mathf.Lerp(_radius[index], 0, t);
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t));

            _effect1.localPosition = new Vector3(Mathf.Lerp(_effect1Position[1].x, _effect1Position[2].x, t), Mathf.Lerp(_effect1Position[1].y, _effect1Position[2].y, t), Mathf.Lerp(_effect1Position[1].z, _effect1Position[2].z, t));
            _effect4.localPosition = new Vector3(Mathf.Lerp(_effect4Position[1].x, _effect4Position[2].x, t), Mathf.Lerp(_effect4Position[1].y, _effect4Position[2].y, t), Mathf.Lerp(_effect4Position[1].z, _effect4Position[2].z, t));

            yield return null;
        }


        _stack = _magneticFieldStack[4];
    }
    #endregion


    #region 바닥, 자기장 내/외부에 따른 데미지

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
            Actor.MagneticStack += _stack;
            
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
            Actor.MagneticStack -= _stackRestore;

            if (Actor.MagneticStack < 0)
                Actor.MagneticStack = 0;

            yield return new WaitForSeconds(_delay);
        }
    }

    #endregion

}
