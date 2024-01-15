using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Burn : MonoBehaviourPun , IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }

    AudioClip _audioClip = null;
    AudioSource _audioSource;

    float _burnDamage = 5f;
    float lastBurnTime;

    public void EnterState()
    {
        effectObject = null;
        lastBurnTime = Time.time;

        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();

        InstantiateEffect("Effects/Fire_large");
        PlayerDebuffSound("PlayerEffect/SFX_FireBall_Projectile");
        TransferDebuffToPlayer((int)InteractableObject.Damage.Burn);
        //StartCoroutine("BurnDmanage");

    }

    public void UpdateState()
    {
        if (effectObject != null)
            effectObject.transform.position = playerTransform.position;

        if (Time.time - lastBurnTime >= 1.0f) // 1초간 데미지+액션
        {
            MyActor.Health -= _burnDamage;
            MyActor.BodyHandler.Waist.PartRigidbody.AddForce((MyActor.BodyHandler.Hip.transform.right) * 40f, ForceMode.VelocityChange);
            MyActor.BodyHandler.Hip.PartRigidbody.AddForce((MyActor.BodyHandler.Hip.transform.right) * 40f, ForceMode.VelocityChange);
            lastBurnTime = Time.time;
        }
    }

    public void ExitState()
    {
        TransferDebuffToPlayer((int)InteractableObject.Damage.Default);
        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Burn;

        RemoveObject("Fire_large");
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
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
    public void TransferDebuffToPlayer(int DamageType)
    {
        ChangeDamageModifier((int)Define.BodyPart.FootL, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.FootR, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LegLowerL, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LegLowerR, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.Head, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftHand, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightHand, DamageType);
    }
    private void ChangeDamageModifier(int bodyPart, int DamageType)
    {
        switch ((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.FootL:
                MyActor.BodyHandler.LeftFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.FootR:
                MyActor.BodyHandler.RightFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LegLowerL:
                MyActor.BodyHandler.LeftLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LegLowerR:
                MyActor.BodyHandler.RightLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LegUpperL:
                break;
            case Define.BodyPart.LegUpperR:
                break;
            case Define.BodyPart.Hip:
                break;
            case Define.BodyPart.Waist:
                break;
            case Define.BodyPart.Chest:
                break;
            case Define.BodyPart.Head:
                MyActor.BodyHandler.Head.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftArm:
                break;
            case Define.BodyPart.RightArm:
                break;
            case Define.BodyPart.LeftForeArm:
                break;
            case Define.BodyPart.RightForeArm:
                break;
            case Define.BodyPart.LeftHand:
                MyActor.BodyHandler.LeftHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightHand:
                MyActor.BodyHandler.RightHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
        }
    }
}
