using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    public float[] PhaseStartTime = { 0f, 45f, 45f, 45f };
    public float[] PhaseDuration = { 0f, 15f, 15f, 10f };
    private float[] _scale = { 0f, 103.2f, 43.1f, 20.0f };
    private Vector3[] _point = { Vector3.zero, new Vector3(465.9f, 6.8f, 414.6f), new Vector3(444.3f, 6.8f, 422.1f), new Vector3(453.9f, 6.8f, 410.5f) };

    private int[] _magneticFieldStack = { 0, 10, 14, 20, 33 };
    private int _stackRestore = 10;
    private int _floorStack = 20;
    private int _delay = 1;
    private int _stack;

    private GameObject MagneticFieldEffect;
    private Vector3[] _effect1Position = { new Vector3(-103.46f, -14.85f, -9.04f), new Vector3(-68f, -9.26f, -2.31f), new Vector3(-52.08f, -4.03f, 3.97f)};
    private Vector3[] _effect3Position = { new Vector3(31.1f, -56.3f, -45), new Vector3(34.41f, -34.21f, -18.44f) };
    private Vector3[] _effect4Position = { new Vector3(80.9f, 27.6f, 42.4f), new Vector3(54.95f, 19.03f, 32.17f), new Vector3(34.39f, 12.27f, 24.06f) };
    private Transform _effect1;
    private Transform _effect3;
    private Transform _effect4;


    public GameObject FreezeImage;

    bool _isSynced;


    public List<Actor> ActorList;
    public int[] AreaNames = new int[Define.MAX_PLAYERS_PER_ROOM];
    public int[] ActorStack = new int[Define.MAX_PLAYERS_PER_ROOM];

    public bool Checking;

    public delegate void CheckArea(int[] areaName, int index);
    public event CheckArea CheckMagneticFieldArea;
    public delegate void UpdateStack(int[] stacks);
    public event UpdateStack UpdateMagneticStack;


    private void Start()
    {
        transform.position = _point[1];
        transform.localScale = new Vector3(_scale[1], _scale[1], _scale[1]);

        MagneticFieldEffect = GameObject.Find("Magnetic Field Effect");
        _effect1 = MagneticFieldEffect.transform.GetChild(0);
        _effect3 = MagneticFieldEffect.transform.GetChild(2);
        _effect4 = MagneticFieldEffect.transform.GetChild(3);

        GameObject mainPanel = GameObject.Find("Main Panel");
        FreezeImage = mainPanel.transform.GetChild(0).gameObject;

        Checking = true;

        StartCoroutine(FirstPhase(1));
    }

    private void Update()
    {
        if(!_isSynced)
        {
            StartCoroutine(WaitForSync());
            return;
        }
    }

    void InvokeDeath(int index)
    {
        StartCoroutine(ActorList[index].StatusHandler.ResetBodySpring());
        ActorList[index].actorState = Actor.ActorState.Dead;
        ActorList[index].StatusHandler._isDead = true;
        ActorList[index].Health = 0;
        ActorList[index].InvokeDeathEvent();
        ActorList[index].InvokeStatusChangeEvent();
    }

    public void ChangeMainPanel()
    {
        for (int i = 0; i < 5; i++)
            FreezeImage.transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < ActorList.Count; i++)
        {
            if(ActorList[i].photonView.IsMine)
            {
                if (ActorList[i].MagneticStack >= 20 && ActorList[i].MagneticStack < 40)
                    FreezeImage.transform.GetChild(0).gameObject.SetActive(true);
                else if (ActorList[i].MagneticStack >= 40 && ActorList[i].MagneticStack < 60)
                    FreezeImage.transform.GetChild(1).gameObject.SetActive(true);
                else if (ActorList[i].MagneticStack >= 60 && ActorList[i].MagneticStack < 80)
                    FreezeImage.transform.GetChild(2).gameObject.SetActive(true);
                else if (ActorList[i].MagneticStack >= 80 && ActorList[i].MagneticStack < 100)
                    FreezeImage.transform.GetChild(3).gameObject.SetActive(true);
                else if (ActorList[i].MagneticStack >= 100)
                    FreezeImage.transform.GetChild(4).gameObject.SetActive(true);
            }
        }
    }

    #region 영역 검사

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.IsConnected == true) return;
        if (ActorList == null) return;


        for (int i = 0; i < ActorList.Count; i++)
        {
            if (other.name == ActorList[i].BodyHandler.Hip.name)
            {
                Transform collided = other.gameObject.transform.parent;

                if (collided.name == ActorList[i].name)
                {
                    AreaNames[i] = (int)Define.Area.Inside;
                    CheckMagneticFieldArea(AreaNames, i);
                    break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.IsConnected == true) return;
        if (ActorList == null) return;


        for (int i = 0; i < ActorList.Count; i++)
        {
            if (other.name == ActorList[i].BodyHandler.Hip.name)
            {
                Transform collided = other.gameObject.transform.parent;

                if (collided.name == ActorList[i].name)
                {
                    AreaNames[i] = (int)Define.Area.Outside;
                    CheckMagneticFieldArea(AreaNames, i);
                    break;
                }
            }
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
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t));

            _effect1.localPosition = new Vector3(Mathf.Lerp(_effect1Position[1].x, _effect1Position[2].x, t), Mathf.Lerp(_effect1Position[1].y, _effect1Position[2].y, t), Mathf.Lerp(_effect1Position[1].z, _effect1Position[2].z, t));
            _effect4.localPosition = new Vector3(Mathf.Lerp(_effect4Position[1].x, _effect4Position[2].x, t), Mathf.Lerp(_effect4Position[1].y, _effect4Position[2].y, t), Mathf.Lerp(_effect4Position[1].z, _effect4Position[2].z, t));

            yield return null;
        }


        _stack = _magneticFieldStack[4];
    }
    #endregion


    #region 바닥, 자기장 내/외부에 따른 데미지

    public IEnumerator DamagedByFloor(int index)
    {
        if (ActorList[index].actorState == Actor.ActorState.Dead) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            while (ActorList[index].MagneticStack <= 100 && AreaNames[index] == (int)Define.Area.Floor)
            {
                ActorList[index].MagneticStack += _floorStack;

                if (ActorList[index].MagneticStack > 100)
                {
                    ActorList[index].MagneticStack = 100;
                    AreaNames[index] = (int)Define.Area.Default;
                    InvokeDeath(index);
                }

                ActorStack[index] = ActorList[index].MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_delay);
            }
        }
    }

    public IEnumerator DamagedByMagnetic(int index)
    {
        if (ActorList[index].actorState == Actor.ActorState.Dead) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            while (ActorList[index].MagneticStack <= 100 && AreaNames[index] == (int)Define.Area.Outside)
            {
                ActorList[index].MagneticStack += _stack;

                if (ActorList[index].MagneticStack > 100)
                {
                    ActorList[index].MagneticStack = 100;
                    AreaNames[index] = (int)Define.Area.Default;
                    InvokeDeath(index);
                }

                ActorStack[index] = ActorList[index].MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_delay);
            }
        }
    }

    public IEnumerator RestoreMagneticDamage(int index)
    {
        if (ActorList[index].actorState == Actor.ActorState.Dead) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            while (ActorList[index].MagneticStack > 0 && AreaNames[index] == (int)Define.Area.Inside)
            {
                ActorList[index].MagneticStack -= _stackRestore;

                if (ActorList[index].MagneticStack < 0)
                    ActorList[index].MagneticStack = 0;

                ActorStack[index] = ActorList[index].MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_delay);
            }
        }
    }

    #endregion

}
