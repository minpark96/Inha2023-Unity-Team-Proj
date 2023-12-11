using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }
    AudioClip _audioClip = null;
    AudioSource _audioSource;

    private float _maxSpeed;

    public void EnterState()
    {
        _maxSpeed = MyActor.PlayerController.RunSpeed;

        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        MyActor.PlayerController.RunSpeed += _maxSpeed * 0.1f;
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
        MyActor.debuffState = Actor.DebuffState.Default;
        MyActor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;
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
        //사운드 문제 있음
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.volume = 0.2f;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
}
