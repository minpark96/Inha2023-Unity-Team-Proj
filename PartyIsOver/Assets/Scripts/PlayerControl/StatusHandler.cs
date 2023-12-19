using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Actor;
using static InteractableObject;

public class StatusHandler : MonoBehaviourPun
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;

    private float _healthDamage;
    public bool _isDead;


    private float _knockoutThreshold = 15f;

    // 초기 관절값
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

   
    // 초기 속도
    private float _maxSpeed;

    // 추후 DebuffTime Actor에서만 사용할 예정
    public float StunTime = 2f;
    public float BurnTime = 3f;
    public float IceTime = 3f;
    public float PowerUpTime = 3f;
    public float Drunktime = 5f;
    public float Slowtime = 5f;
    public float ShockTime = 5f;


    // 버프 확인용 플래그
    private bool _hasPowerUp;
    private bool _hasBurn;
    private bool _hasExhausted;
    private bool _hasSlow;
    public bool _hasFreeze;
    public bool _hasShock;
    private bool _hasStun;
    public bool HasDrunk;

    public Transform playerTransform;
    public GameObject effectObject = null;
    int _burnCount = 0;

    AudioClip _audioClip = null;
    AudioSource _audioSource;

    [Header("Debuff Duration")]
    [SerializeField]
    private float _powerUpTime;
    [SerializeField]
    private float _burnTime;
    [SerializeField]
    private float _exhaustedTime;
    [SerializeField]
    private float _slowTime;
    [SerializeField]
    private float _freezeTime;
    [SerializeField]
    private float _shockTime;
    [SerializeField]
    private float _stunTime;

    [Header("Debuff Damage")]
    [SerializeField]
    public float _iceDamage;
    [SerializeField]
    public float _burnDamage;

    public Context _context;
    Stun stunInStance;
    Burn burnInStance;
    Ice IceInStance;
    PowerUp powerUpInStance;
    Drunk drunkInStance;
    Shock shockInStance;
    Exhausted exhaustedInStance;


    private void Init()
    {
        StatusData data = Managers.Resource.Load<StatusData>("ScriptableObject/StatusData");
        _stunTime = data.StunTime;
        _burnTime = data.BurnTime;
        _freezeTime = data.FreezeTime;
        _powerUpTime = data.PowerUpTime;
        //_drunkTime = data.DrunkTime; // _drunkTime 없음
        _shockTime = data.ShockTime;
    }

    private void Awake()
    {
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        _context = GetComponent<Context>();
        stunInStance = gameObject.AddComponent<Stun>();
        burnInStance = gameObject.AddComponent<Burn>();
        IceInStance = gameObject.AddComponent<Ice>();
        powerUpInStance = gameObject.AddComponent<PowerUp>();
        drunkInStance = gameObject.AddComponent<Drunk>();
        shockInStance = gameObject.AddComponent<Shock>();
        exhaustedInStance = gameObject.AddComponent<Exhausted>();
    }

    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _maxSpeed = actor.PlayerController.RunSpeed;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    private void LateUpdate()
    {
        // 지침 디버프 활성화/비활성화
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (actor.Stamina <= 0)
            {
                if ((actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted)
                {
                    if (actor.GrabState != Define.GrabState.PlayerLift)
                    {
                        actor.GrabState = Define.GrabState.None;
                        actor.Grab.GrabResetTrigger();
                    }
                    actor.debuffState |= Actor.DebuffState.Exhausted;
                    photonView.RPC("RPCExhaustedCreate", RpcTarget.All);
                }
            }
        }
    }

    // 충격이 가해지면(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer=null)
    {
        // 데미지 체크
        damage *= _damageModifer;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && !((actor.debuffState & Actor.DebuffState.Stun) == DebuffState.Stun))
        {
            _healthDamage += damage;
        }

        if (_healthDamage != 0f)
            UpdateHealth();

        if (actor.actorState != Actor.ActorState.Dead)
        {
            // 상태이상 체크
            DebuffCheck(type);
            DebuffAction();
            //CheckProjectile(causer);
        }

        photonView.RPC("InvulnerableState", RpcTarget.All, 0.5f);
        actor.InvokeStatusChangeEvent();
    }

    void CheckProjectile(GameObject go)
    {
        if (go.GetComponent<ProjectileStandard>() != null)
        {
            go.GetComponent<ProjectileStandard>().DestoryProjectileTrigger();
        }
    }

    [PunRPC]
    void PlayerDebuffSound(string path)
    {
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        switch (type)
        {
            case Damage.Ice: // 빙결
                actor.debuffState |= Actor.DebuffState.Ice;
                break;
            case Damage.PowerUp: // 불끈
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Burn: // 화상
                actor.debuffState |= Actor.DebuffState.Burn;
                break;
            case Damage.Shock: // 감전
                    if ((actor.debuffState & DebuffState.Stun) == DebuffState.Stun || (actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Stun: // 기절
                if ((actor.debuffState & DebuffState.Shock) == DebuffState.Shock || (actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Stun;
                break;
            case Damage.Drunk: // 취함
                if ((actor.debuffState & DebuffState.Stun) == DebuffState.Stun || (actor.debuffState & DebuffState.Shock) == DebuffState.Shock)
                    break;
                else
                {
                    actor.debuffState |= Actor.DebuffState.Drunk;
                }
                break;
        }
    }

    public void DebuffAction()
    {
        foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
        {
            Actor.DebuffState checking = actor.debuffState & state;
            switch (checking)
            {
                case Actor.DebuffState.Default:
                    break;
                case Actor.DebuffState.PowerUp:
                    if (!_hasPowerUp)
                        photonView.RPC("RPCPowerUpCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Burn:
                    if (!_hasBurn)
                        photonView.RPC("RPCBurnCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Slow:
                    if (!_hasSlow)
                        photonView.RPC("Slow", RpcTarget.All, _slowTime);
                    break;
                case Actor.DebuffState.Shock:
                    if (!_hasShock)
                        photonView.RPC("RPCShockCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Stun:
                    if (!_hasStun)
                        EnterUnconsciousState();
                    break;
                case Actor.DebuffState.Ghost:
                    break;
                case Actor.DebuffState.Drunk:
                    if(!HasDrunk)
                        photonView.RPC("RPCPoisonCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Ice:
                    if (!_hasFreeze)
                        photonView.RPC("RPCIceCreate", RpcTarget.All);
                    break;
            }
        }
    }

    [PunRPC]
    void RPCPoisonCreate()
    {
        _context.ChangeState(drunkInStance, Drunktime);
    }

    [PunRPC]
    void RPCShockCreate()
    {
        actor.Grab.GrabResetTrigger();
        _context.ChangeState(shockInStance, ShockTime);
    }

    [PunRPC]
    void RPCExhaustedCreate()
    {
        _context.ChangeState(exhaustedInStance);
    }
    [PunRPC]
    void RPCPowerUpCreate()
    {
        _context.ChangeState(powerUpInStance, PowerUpTime);
    }

    [PunRPC]
    void RPCBurnCreate()
    {
        actor.Grab.GrabResetTrigger();
        _context.ChangeState(burnInStance, BurnTime);
    }

    [PunRPC]
    void RPCIceCreate()
    {
        _context.ChangeState(IceInStance, IceTime);
    }

    [PunRPC]
    IEnumerator Slow(float delay)
    {
        // 둔화
        _hasSlow = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;

        yield return new WaitForSeconds(delay);

        // 둔화 해제
        _hasSlow = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Slow;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;

        actor.InvokeStatusChangeEvent();
    }

    [PunRPC]
    public void DestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    public void EffectObjectCreate(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        //effectObject.transform.position = playerTransform.position;
    }
    [PunRPC]
    public void MoveEffect()
    {
        //LateUpdate여서 늦게 갱신이 되어서 NullReference가 떠서 같은 if 문을 넣어줌
        if (effectObject != null && effectObject.name == "Stun_loop")
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
        else if (effectObject != null && effectObject.name == "Fog_frost")
        {
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y - 2, playerTransform.position.z);
        }
        else
            effectObject.transform.position = playerTransform.position;
    }


    public void UpdateHealth()
    {
        if (_isDead)
            return;

        //현재 체력 받아오기
        float tempHealth = actor.Health;

        //무적상태가 아닐때만 데미지 적용
        if (tempHealth > 0f && !invulnerable)
            tempHealth -= _healthDamage;

        float realDamage = actor.Health - tempHealth;

        //계산한 체력이 0보다 작으면 Death로
        if (tempHealth <= 0f)
        {
            KillPlayer();
        }
        else
        {
            //기절상태가 아닐때 일정 이상의 데미지를 받으면 기절
            if (!((actor.debuffState & Actor.DebuffState.Stun) == DebuffState.Stun))
            {

                if (realDamage >= _knockoutThreshold)
                {
                    if ((actor.debuffState & DebuffState.Ice) == DebuffState.Ice) //상태이상 후에 추가
                        return;

                    actor.debuffState |= Actor.DebuffState.Stun;
                }
            }
        }
        actor.Health = Mathf.Clamp(tempHealth, 0f, actor.MaxHealth);

        _healthDamage = 0f;
    }
    [PunRPC]
    IEnumerator InvulnerableState(float time)
    {
        invulnerable = true;
        yield return new WaitForSeconds(time);
        invulnerable = false;
    }

    void KillPlayer()
    {
        actor.actorState = Actor.ActorState.Dead;
        _isDead = true;
        actor.Grab.GrabResetTrigger();
        actor.InvokeDeathEvent();
    }

    void EnterUnconsciousState()
    {
        //데미지 이펙트나 사운드 추후 추가

        //actor.debuffState = Actor.DebuffState.Stun;
        actor.Grab.GrabResetTrigger();
        photonView.RPC("ChangeStateMachines", RpcTarget.All, _stunTime);
        //StartCoroutine(ResetBodySpring());
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    [PunRPC]
    void ChangeStateMachines(float durationTime)
    {
        _context.ChangeState(stunInStance, durationTime);
    }

    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //기절과 회복에 모두 관여 기절시엔 퍼센티지를 0으로해서 사용
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }

    public IEnumerator ResetBodySpring()
    {
        SetJointSpring(0f);
        yield return null;
    }

    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            SetJointSpring(percentage);
            yield return null;
        }
    }
}