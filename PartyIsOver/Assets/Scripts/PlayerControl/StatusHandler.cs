using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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

    int Creatcount = 0;


    private void Awake()
    {
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
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


    void Update()
    {
        // 지침 디버프 활성화/비활성화
        if (actor.Stamina <= 0)
        {
            if (!_hasExhausted)
            {
                actor.debuffState |= Actor.DebuffState.Exhausted;
                photonView.RPC("Exhausted", RpcTarget.All, _exhaustedTime);
            }
        }
    }


    private void LateUpdate()
    {
        if (effectObject != null && effectObject.name == "Stun_loop")
            photonView.RPC("MoveEffect", RpcTarget.All);
        else if (effectObject != null && effectObject.name == "Fog_frost")
            photonView.RPC("MoveEffect", RpcTarget.All);
        else if (effectObject != null)
            photonView.RPC("PlayerEffect", RpcTarget.All);

    }

    // 충격이 가해지면(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer=null)
    {
        // 데미지 체크
        damage *= _damageModifer;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
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
        _audioSource.volume = 0.2f;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        if (actor.debuffState != Actor.DebuffState.Default)
            return;

        if (actor.debuffState == Actor.DebuffState.Ice) return;
        //if (actor.debuffState == Actor.DebuffState.Balloon) return;

        switch (type)
        {
            case Damage.Ice: // 빙결
                actor.debuffState |= Actor.DebuffState.Ice;
                //foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                //{
                //    if (state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                //    {
                //        actor.debuffState &= ~state;
                //    }
                //}
                break;
            //case Damage.Balloon: // 풍선
            //    {
            //        actor.debuffState |= Actor.DebuffState.Balloon;
            //        photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-049");
            //        foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            //        {
            //            if (state != Actor.DebuffState.Balloon && (actor.debuffState & state) != 0)
            //            {
            //                actor.debuffState &= ~state;
            //            }
            //        }
            //    }
            //    break;
            case Damage.PowerUp: // 불끈
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Burn: // 화상
                actor.debuffState |= Actor.DebuffState.Burn;
                break;
            case Damage.Shock: // 감전
                if (actor.debuffState == Actor.DebuffState.Stun || actor.debuffState == Actor.DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Stun: // 기절
                if (actor.debuffState == Actor.DebuffState.Shock || actor.debuffState == Actor.DebuffState.Drunk )
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Stun;
                break;
            case Damage.Drunk: // 취함
                if (actor.debuffState == Actor.DebuffState.Stun || actor.debuffState == Actor.DebuffState.Shock)
                    break;
                else
                {
                    actor.debuffState |= Actor.DebuffState.Drunk;
                    photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-049");
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
                        photonView.RPC("PowerUp", RpcTarget.All, _powerUpTime);
                    break;
                case Actor.DebuffState.Burn:
                    if (!_hasBurn)
                        photonView.RPC("Burn", RpcTarget.All, _burnTime);
                    break;
                case Actor.DebuffState.Slow:
                    if (!_hasSlow)
                        photonView.RPC("Slow", RpcTarget.All, _slowTime);
                    break;
                case Actor.DebuffState.Shock:
                    if (!_hasShock)
                        photonView.RPC("Shock", RpcTarget.All, _shockTime);
                    break;
                case Actor.DebuffState.Stun:
                    if (!_hasStun)
                    {
                        StartCoroutine(ResetBodySpring());
                        photonView.RPC("Stun", RpcTarget.All, _stunTime);
                    }
                    break;
                case Actor.DebuffState.Ghost:
                    break;
                case Actor.DebuffState.Drunk:
                    if(!HasDrunk)
                    {
                        photonView.RPC("PoisonCreate", RpcTarget.All);
                    }
                    break;
                case Actor.DebuffState.Ice:
                    if (!_hasFreeze)
                        photonView.RPC("Freeze", RpcTarget.All, _freezeTime);
                    break;
            }
        }
    }

    [PunRPC]
    IEnumerator PowerUp(float delay)
    {

        // 불끈
        _hasPowerUp = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;
        PlayerDebuffSound("PlayerEffect/Cartoon-UI-037");
        PowerUpCreate();

        yield return new WaitForSeconds(delay);

        // 불끈 해제
        _hasPowerUp = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;
        DestroyEffect("Aura_acceleration");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    [PunRPC]
    IEnumerator Burn(float delay)
    {
        // 화상
        _hasBurn = true;
        actor.actorState = Actor.ActorState.Debuff;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Burn);


        PlayerDebuffSound("PlayerEffect/SFX_FireBall_Projectile");
        BurnCreate();

        float elapsedTime = 0f;
        float lastBurnTime = Time.time;
        float startTime = Time.time;

        while (elapsedTime < delay)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                _hasBurn = false;
                actor.actorState = Actor.ActorState.Stand;
                StopCoroutine(Burn(delay));
            }

            if (Time.time - lastBurnTime >= 1.0f) // 1초간 데미지+액션
            {
                actor.Health -= _burnDamage;
                actor.BodyHandler.Waist.PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                actor.BodyHandler.Hip.PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                lastBurnTime = Time.time;
            }

            elapsedTime = Time.time - startTime;
            yield return null;
        }
        _burnCount = 0;
        Creatcount = 0;
        // 화상 해제
        _hasBurn = false;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Default);
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Burn;

        DestroyEffect("Fire_large");
        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

   
    [PunRPC]
    IEnumerator Exhausted(float delay)
    {
        // 지침
        _hasExhausted = true;
        WetCreate();
        actor.actorState = Actor.ActorState.Debuff;
        JointDrive angularXDrive;

        angularXDrive = actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive;
        angularXDrive.positionSpring = 0f;
        actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;

        while(actor.Stamina != 100)
        {
            yield return null;
        }

        // 지침 해제
        _hasExhausted = false;
        DestroyEffect("Wet");

        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Exhausted;
        angularXDrive.positionSpring = _xPosSpringAry[0];

        actor.BodyHandler.BodyParts[(int)Define.BodyPart.Head].PartJoint.angularXDrive = angularXDrive;

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
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
    IEnumerator Freeze(float delay)
    {
        _hasFreeze = true;

        yield return new WaitForSeconds(0.2f);

        PlayerDebuffSound("PlayerEffect/Cartoon-UI-047");
        IceCubeCreate();
        IceSmokeCreate();

        // 빙결
        actor.actorState = Actor.ActorState.Debuff;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }

        yield return new WaitForSeconds(delay);

        // 빙결 해제
        _hasFreeze = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Ice;

        actor.InvokeStatusChangeEvent();
        DestroyEffect("Fog_frost");
        DestroyEffect("IceCube");
       
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
        _audioClip = null;
    }

    [PunRPC]
    IEnumerator Shock(float delay)
    {
        _hasShock = true;
        

        if (actor.debuffState == Actor.DebuffState.Ice)
            photonView.RPC("StopShock", RpcTarget.All, delay);

        
        // 감전
        actor.actorState = Actor.ActorState.Debuff;
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Shock);
        PlayerDebuffSound("PlayerEffect/electronic_02");
        ShockCreate();

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
            if (i == (int)Define.BodyPart.Ball) continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }

        float startTime = Time.time;

        while (Time.time - startTime < delay)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                _hasShock = false;
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine(Stun(0.5f));
                //photonView.RPC("Stun", RpcTarget.All, _stunTime);
                photonView.RPC("StopShock", RpcTarget.All, delay);
            }

            yield return new WaitForSeconds(0.2f);

            if (UnityEngine.Random.Range(0, 20) > 10)
            {
                for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
                {
                    if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                    if (i == (int)Define.BodyPart.LeftFoot || 
                        i == (int)Define.BodyPart.RightFoot ||
                        i == (int)Define.BodyPart.Ball) continue;

                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
                {
                    if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                    if (i == (int)Define.BodyPart.LeftFoot ||
                        i == (int)Define.BodyPart.RightFoot ||
                        i == (int)Define.BodyPart.Ball) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        // 감전 해제
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        photonView.RPC("TransferDebuffToPlayer", RpcTarget.MasterClient, (int)InteractableObject.Damage.Default);

        //photonView.RPC("Stun", RpcTarget.All, 0.5f);
        StartCoroutine(Stun(0.5f));
        
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        DestroyEffect("Lightning_aura");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    [PunRPC]
    void StopShock()
    {
        StopCoroutine("Shock");
        // 감전 해제
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        photonView.RPC("Stun", RpcTarget.All, 0.5f);
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        photonView.RPC("DestroyEffect", RpcTarget.All, "Lightning_aura");

        actor.InvokeStatusChangeEvent();
        _audioClip = null;
    }


    [PunRPC]
    IEnumerator Stun(float delay)
    {
        _hasStun = true;
        yield return new WaitForSeconds(delay);
        yield return RestoreBodySpring();
        _hasStun = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Stun;

        actor.InvokeStatusChangeEvent();
        DestroyEffect("Stun_loop");
    }

    [PunRPC]
    public void DestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }

    [PunRPC]
    public void StunCreate()
    {
        //Effects/Stun_loop 생성 
        EffectObjectCreate("Effects/Stun_loop");
    }

    public void BurnCreate()
    {
        EffectObjectCreate("Effects/Fire_large");
    }

    public void ShockCreate()
    {
        EffectObjectCreate("Effects/Lightning_aura");
    }

    public void PowerUpCreate()
    {
        EffectObjectCreate("Effects/Aura_acceleration");
    }

    public void IceCubeCreate()
    {
        EffectObjectCreate("Effects/IceCube");
    }

    public void IceSmokeCreate()
    {
        EffectObjectCreate("Effects/Fog_frost");
    }

    [PunRPC]
    public void PoisonCreate()
    {
        HasDrunk = true;
        EffectObjectCreate("Effects/Fog_poison");
    }

    public void WetCreate()
    {
        EffectObjectCreate("Effects/Wet");
    }

    void EffectObjectCreate(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        effectObject.transform.position = playerTransform.position;
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

    }
    [PunRPC]
    public void PlayerEffect()
    {
        if (effectObject != null)
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
            EnterUnconsciousState();
            KillPlayer();
        }
        else
        {
            //기절상태가 아닐때 일정 이상의 데미지를 받으면 기절
            if (actor.actorState != Actor.ActorState.Unconscious)
            {
                if (realDamage >= _knockoutThreshold)
                {
                    if (actor.debuffState == Actor.DebuffState.Ice) //상태이상 후에 추가
                        return;
                    actor.actorState = Actor.ActorState.Unconscious;
                    photonView.RPC("StunCreate", RpcTarget.All);
                    EnterUnconsciousState();
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
        StartCoroutine(ResetBodySpring());
        actor.actorState = Actor.ActorState.Dead;
        _isDead = true;
        actor.InvokeDeathEvent();
    }

    void EnterUnconsciousState()
    {
        //데미지 이펙트나 사운드 추후 추가

        actor.debuffState = Actor.DebuffState.Stun;
        StartCoroutine(ResetBodySpring());
        actor.Grab.GrabResetTrigger();
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    [PunRPC]
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
        photonView.RPC("SetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }

    public IEnumerator RestoreBodySpring()
    {
        float startTime = Time.time;
        float springLerpDuration = 0.07f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("SetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    [PunRPC]
    public void TransferDebuffToPlayer(int DamageType)
    {
        ChangeDamageModifier((int)Define.BodyPart.LeftFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.Head, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftHand, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightHand, DamageType);
    }



    private void ChangeDamageModifier(int bodyPart, int DamageType)
    {
        switch ((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.LeftFoot:
                actor.BodyHandler.LeftFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightFoot:
                actor.BodyHandler.RightFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftLeg:
                actor.BodyHandler.LeftLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightLeg:
                actor.BodyHandler.RightLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftThigh:
                break;
            case Define.BodyPart.RightThigh:
                break;
            case Define.BodyPart.Hip:
                break;
            case Define.BodyPart.Waist:
                break;
            case Define.BodyPart.Chest:
                break;
            case Define.BodyPart.Head:
                actor.BodyHandler.Head.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
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
                actor.BodyHandler.LeftHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightHand:
                actor.BodyHandler.RightHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
        }
    }


}