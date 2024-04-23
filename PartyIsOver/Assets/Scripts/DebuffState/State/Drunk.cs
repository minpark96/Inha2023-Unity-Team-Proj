using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drunk : MonoBehaviourPun, IDebuffState
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

        PlayerDebuffSound("PlayerEffect/Cartoon-UI-049");
        InstantiateEffect("Effects/Fog_poison");
        MyActor.SetFlambe(true);
    }

    public void UpdateState()
    {
        if (effectObject != null)
        {
            effectObject.transform.position = playerTransform.position;
        }
    }

    public void ExitState()
    {
        RemoveObject("Fog_poison");

        MyActor.debuffState &= ~Actor.DebuffState.Drunk;
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
        //사운드 문제 있음
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
}
