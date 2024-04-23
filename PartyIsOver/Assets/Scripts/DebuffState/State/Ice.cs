using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }
    public GameObject FogfrostffectObject;

    AudioClip _audioClip = null;
    AudioSource _audioSource;

    public void EnterState()
    {
        effectObject = null;
        FogfrostffectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        PlayerDebuffSound("PlayerEffect/Cartoon-UI-047");
        InstantiateForstEffect("Effects/Fog_frost");
        InstantiateEffect("Effects/IceCube");

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            MyActor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }
    }

    public void UpdateState()
    {
        if(effectObject != null && FogfrostffectObject != null)
        {
            FogfrostffectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y - 2, playerTransform.position.z);
            effectObject.transform.position = playerTransform.position;
        }
    }

    public void ExitState()
    {
        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            MyActor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }

        MyActor.debuffState &= ~Actor.DebuffState.Ice;

        MyActor.InvokeStatusChangeEvent();
        PlayerDebuffSound("PlayerEffect/Item_UI_033");
        RemoveObject("Fog_frost");
        RemoveObject("IceCube");

        _audioClip = null;
    }
    public void InstantiateForstEffect(string path)
    {
        FogfrostffectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
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
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }

}
