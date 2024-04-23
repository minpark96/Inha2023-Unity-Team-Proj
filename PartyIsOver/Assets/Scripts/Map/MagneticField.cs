using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;


public class MagneticField : MonoBehaviour
{
    // 자기장 수치
    public float[] _phaseStartTime = { 0f, 120f, 60f, 60f };
    private float[] _phaseDuration = { 0f, 15f, 15f, 10f };
    private float[] _scale = { 0f, 103.2f, 43.1f, 20.0f };
    private Vector3[] _point = { Vector3.zero, new Vector3(465.9f, 5f, 414.6f), new Vector3(444.3f, 5f, 422.1f), new Vector3(453.9f, 5f, 410.5f) };

    private int _stackRestore = 1;
    private int _magneticFieldStack = 1;
    private int _floorStack = 1;
    private float _magneticDelay = 0.1f;
    private float _floorDelay = 0.1f;

    // 사운드
    private bool _isPlayingStackSound;


    // 이펙트
    private GameObject MagneticFieldEffect;
    private Vector3[] _effect1Position = { new Vector3(-103.46f, -14.85f, -9.04f), new Vector3(-68f, -9.26f, -2.31f), new Vector3(-52.08f, -4.03f, 3.97f)};
    private Vector3[] _effect3Position = { new Vector3(31.1f, -56.3f, -45), new Vector3(34.41f, -34.21f, -18.44f) };
    private Vector3[] _effect4Position = { new Vector3(80.9f, 27.6f, 42.4f), new Vector3(54.95f, 19.03f, 32.17f), new Vector3(34.39f, 12.27f, 24.06f) };
    private Transform _effect1;
    private Transform _effect3;
    private Transform _effect4;

    public GameObject FreezeImage;


    // 서버를 통해 받는 정보
    public List<Actor> ActorList;
    public int[] AreaNames = new int[Define.MAX_PLAYERS_PER_ROOM];
    public int[] ActorStack = new int[Define.MAX_PLAYERS_PER_ROOM];
    public bool[] IsInside = new bool[Define.MAX_PLAYERS_PER_ROOM] { true, true, true, true, true, true };


    public delegate void CheckArea(int[] areaName, int actorNum, bool[] isInside);
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

        StartCoroutine(FirstPhase(1));

        // 라운드 재시작시
        GameObject sound = GameObject.Find("@Sound").transform.GetChild((int)Define.Sound.InGameStackSound).gameObject;
        sound.GetComponent<AudioSource>().Stop();
        _isPlayingStackSound = false;
    }

  

    void InvokeDeath(int index)
    {
        StartCoroutine(ActorList[index].BodyHandler.ResetBodySpring());
        ActorList[index].StatContext.IsAlive = false;
        ActorList[index].StatusHandler._isDead = true;
        ActorList[index].StatContext.Health = 0;
        ActorList[index].StatContext.MagneticStack = 0;
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
                if (ActorList[i].StatContext.IsAlive == false)
                {
                    FreezeImage.SetActive(false);

                    if (_isPlayingStackSound == true)
                    {
                        GameObject sound = GameObject.Find("@Sound").transform.GetChild((int)Define.Sound.InGameStackSound).gameObject;
                        sound.GetComponent<AudioSource>().Stop();
                        _isPlayingStackSound = false;
                    }

                    return;
                }

                if (ActorList[i].StatContext.MagneticStack >= 20 && ActorList[i].StatContext.MagneticStack < 40)
                    FreezeImage.transform.GetChild(0).gameObject.SetActive(true);
                else if (ActorList[i].StatContext.MagneticStack >= 40 && ActorList[i].StatContext.MagneticStack < 60)
                    FreezeImage.transform.GetChild(1).gameObject.SetActive(true);
                else if (ActorList[i].StatContext.MagneticStack >= 60 && ActorList[i].StatContext.MagneticStack < 80)
                    FreezeImage.transform.GetChild(2).gameObject.SetActive(true);
                else if (ActorList[i].StatContext.MagneticStack >= 80 && ActorList[i].StatContext.MagneticStack < 100)
                    FreezeImage.transform.GetChild(3).gameObject.SetActive(true);
                else if (ActorList[i].StatContext.MagneticStack >= 100)
                    FreezeImage.transform.GetChild(4).gameObject.SetActive(true);


                if(ActorList[i].StatContext.MagneticStack >= 50)
                {
                    if(_isPlayingStackSound == false)
                    {
                        AudioClip magneticWarning = Managers.Sound.GetOrAddAudioClip("MagneticField/stack beep");
                        Managers.Sound.Play(magneticWarning, Define.Sound.InGameStackSound);
                        _isPlayingStackSound = true;
                    }
                }
                else
                {
                    if(_isPlayingStackSound == true)
                    {
                        GameObject sound = GameObject.Find("@Sound").transform.GetChild((int)Define.Sound.InGameStackSound).gameObject;
                        sound.GetComponent<AudioSource>().Stop();
                        _isPlayingStackSound = false;
                    }
                }
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
                    IsInside[i] = true;
                    CheckMagneticFieldArea(AreaNames, i, IsInside);
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
                    IsInside[i] = false;
                    CheckMagneticFieldArea(AreaNames, i, IsInside);
                    break;
                }
            }
        }
    }


    #endregion


    #region 자기장 Phase
    IEnumerator FirstPhase(int index)
    {
        yield return new WaitForSeconds(_phaseStartTime[index]);

        // Sound
        AudioClip warningSound = Managers.Sound.GetOrAddAudioClip("MagneticField/magnetic field warning sound");
        Managers.Sound.Play(warningSound, Define.Sound.MagneticWarning);

        float startTime = Time.time;
        while (Time.time - startTime < _phaseDuration[index])
        {
            float t = (Time.time - startTime) / _phaseDuration[index];
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t), Mathf.Lerp(_scale[index], _scale[index + 1], t));
            transform.position = new Vector3(Mathf.Lerp(_point[index].x, _point[index + 1].x, t), 6.8f, Mathf.Lerp(_point[index].z, _point[index + 1].z, t));

            _effect1.localPosition = new Vector3(Mathf.Lerp(_effect1Position[0].x, _effect1Position[1].x, t), Mathf.Lerp(_effect1Position[0].y, _effect1Position[1].y, t), Mathf.Lerp(_effect1Position[0].z, _effect1Position[1].z, t));

            yield return null;
        }

        yield return StartCoroutine(SecondPhase(2));
    }

    IEnumerator SecondPhase(int index)
    {
        yield return new WaitForSeconds(_phaseStartTime[index]);

        // Sound
        AudioClip warningSound = Managers.Sound.GetOrAddAudioClip("MagneticField/magnetic field warning sound");
        Managers.Sound.Play(warningSound, Define.Sound.MagneticWarning);


        float startTime = Time.time;
        while (Time.time - startTime < _phaseDuration[index])
        {
            float t = (Time.time - startTime) / _phaseDuration[index];
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
        yield return new WaitForSeconds(_phaseStartTime[index]);

        // Sound
        AudioClip warningSound = Managers.Sound.GetOrAddAudioClip("MagneticField/magnetic field warning sound");
        Managers.Sound.Play(warningSound, Define.Sound.MagneticWarning);


        float startTime = Time.time;
        while (Time.time - startTime < _phaseDuration[index])
        {
            float t = (Time.time - startTime) / _phaseDuration[index];
            transform.localScale = new Vector3(Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t), Mathf.Lerp(_scale[index], 0, t));

            _effect1.localPosition = new Vector3(Mathf.Lerp(_effect1Position[1].x, _effect1Position[2].x, t), Mathf.Lerp(_effect1Position[1].y, _effect1Position[2].y, t), Mathf.Lerp(_effect1Position[1].z, _effect1Position[2].z, t));
            _effect4.localPosition = new Vector3(Mathf.Lerp(_effect4Position[1].x, _effect4Position[2].x, t), Mathf.Lerp(_effect4Position[1].y, _effect4Position[2].y, t), Mathf.Lerp(_effect4Position[1].z, _effect4Position[2].z, t));

            yield return null;
        }
    }
    #endregion


    #region 바닥, 자기장 내/외부에 따른 데미지

    public IEnumerator DamagedByFloor(int index)
    {
        if (ActorList[index].StatContext.IsAlive == false) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            while (ActorList[index].StatContext.MagneticStack <= 100 && AreaNames[index] == (int)Define.Area.Floor)
            {
                ActorList[index].StatContext.MagneticStack += _floorStack;

                if (ActorList[index].StatContext.MagneticStack > 100)
                {
                    ActorList[index].StatContext.MagneticStack = 100;
                    AreaNames[index] = (int)Define.Area.Default;
                    InvokeDeath(index);
                }

                ActorStack[index] = ActorList[index].StatContext.MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_floorDelay);
            }
        }
    }

    public IEnumerator DamagedByMagnetic(int index)
    {
        if (ActorList[index].StatContext.IsAlive == false) yield return null;

        if (PhotonNetwork.IsMasterClient)   
        {
            while (ActorList[index].StatContext.MagneticStack <= 100 && AreaNames[index] == (int)Define.Area.Outside)
            {
                ActorList[index].StatContext.MagneticStack += _magneticFieldStack;

                if (ActorList[index].StatContext.MagneticStack > 100)
                {
                    ActorList[index].StatContext.MagneticStack = 100;
                    AreaNames[index] = (int)Define.Area.Default;
                    InvokeDeath(index);
                }

                ActorStack[index] = ActorList[index].StatContext.MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_magneticDelay);
            }
        }
    }

    public IEnumerator RestoreMagneticDamage(int index)
    {
        if (ActorList[index].StatContext.IsAlive == false) yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            while (ActorList[index].StatContext.MagneticStack > 0 && AreaNames[index] == (int)Define.Area.Inside)
            {
                ActorList[index].StatContext.MagneticStack -= _stackRestore;

                if (ActorList[index].StatContext.MagneticStack < 0)
                    ActorList[index].StatContext.MagneticStack = 0;

                ActorStack[index] = ActorList[index].StatContext.MagneticStack;
                UpdateMagneticStack(ActorStack);

                yield return new WaitForSeconds(_magneticDelay);
            }
        }
    }

    #endregion

}
