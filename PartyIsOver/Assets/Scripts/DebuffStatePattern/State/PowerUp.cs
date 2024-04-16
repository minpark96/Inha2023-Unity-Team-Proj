using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviourPun, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }
    AudioClip _audioClip = null;
    AudioSource _audioSource;


    public void EnterState()
    {
        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        MyActor.StatContext.RunSpeed += MyActor.StatContext.MaxSpeed * 0.1f;
        PlayerDebuffSound("PlayerEffect/Cartoon-UI-037");
        InstantiateEffect("Effects/Aura_acceleration");
    }

    public void UpdateState()
    {
        if(effectObject != null)
            effectObject.transform.position = playerTransform.position;
    }

    public void ExitState()
    {
        MyActor.debuffState &= ~Actor.DebuffState.PowerUp;
        MyActor.StatContext.RunSpeed = MyActor.StatContext.MaxSpeed;
        RemoveObject("Aura_acceleration");
        MyActor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    public void InstantiateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }
    public void RemoveObject(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }
    void PlayerDebuffSound(string path)
    {
        //���� ���� ����
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
}
