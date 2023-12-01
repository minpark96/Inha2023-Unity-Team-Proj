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
    private bool _isDead;

    
    private float _knockoutThreshold=15f;

    // 초기 관절값
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();


    // 이펙트 생성
    private bool hasObject = false;

    // 초기 속도
    private float _maxSpeed;

    // 버프 확인용 플래그
    private bool _hasPowerUp;
    private bool _hasBurn;
    private bool _hasExhausted;
    private bool _hasSlow;
    public bool _hasFreeze;
    private bool _hasShock;
    private bool _hasStun;

    public Transform playerTransform;
    public GameObject effectObject = null;

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
            if (i == 3)
                continue;

            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    
    void Update()
    {
        // 지침 디버프 활성화/비활성화
        if(actor.Stamina <= 0)
        {
            if(!_hasExhausted)
            {
                actor.debuffState |= Actor.DebuffState.Exhausted;
                StartCoroutine(Exhausted(_exhaustedTime));
            }
        }
    }


    private void LateUpdate()
    {
        if (effectObject != null && effectObject.name == "Stun_loop")
            photonView.RPC("MoveEffect", RpcTarget.All);
        else if(effectObject != null)
            photonView.RPC("PlayerEffect", RpcTarget.All);

    }

    private void OnGUI()
    {
        if (this.name == "Ragdoll2" && photonView.IsMine)
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "버프상태:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 20, 200, 200), "액션상태:" + actor.actorState.ToString());

            GUI.contentColor = Color.blue;
            GUI.Label(new Rect(0, 60, 200, 200), "체력: " + actor.Health);
            GUI.Label(new Rect(0, 80, 200, 200), "스테미나: " + actor.Stamina);
        }

        if (this.name == "Dummy")
        {
            GUI.contentColor = Color.blue;
            GUI.Label(new Rect(0, 120, 200, 200), "더미 체력: " + actor.Health);
        }
    }

    // 충격이 가해지면(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
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
        }

        actor.StatusChangeEventInvoke();
    }



    [PunRPC]
    void PlayerDebuffSound(string path)
    {
        if(_audioClip == null)
        {
            _audioClip = Managers.Sound.GetOrAddAudioClip(path);
            _audioSource.clip = _audioClip;
            _audioSource.volume = 0.2f;
            _audioSource.spatialBlend = 1;
            Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect);
        }
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        if (actor.debuffState == Actor.DebuffState.Ice) return;
        if (actor.debuffState == Actor.DebuffState.Balloon) return;

        switch (type)
        {
            case Damage.Ice: // 빙결
                actor.debuffState |= Actor.DebuffState.Ice;
                foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                {
                    if (state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                    {
                        actor.debuffState &= ~state;
                    }
                }
                break;
            case Damage.Balloon: // 풍선
                {
                    actor.debuffState |= Actor.DebuffState.Balloon;
                    photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-049");
                    foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                    {
                        if (state != Actor.DebuffState.Balloon && (actor.debuffState & state) != 0)
                        {
                            actor.debuffState &= ~state;
                        }
                    }
                }
                break;
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
                if (actor.debuffState == Actor.DebuffState.Shock || actor.debuffState == Actor.DebuffState.Drunk)
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
                    photonView.RPC("PoisonCreate", RpcTarget.All);
                    
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
                    if(!_hasPowerUp)
                        StartCoroutine(PowerUp(_powerUpTime));
                    break;
                case Actor.DebuffState.Burn:
                    if (!_hasBurn)
                        StartCoroutine(Burn(_burnTime));
                    break;
                case Actor.DebuffState.Slow:
                    if(!_hasSlow)
                        StartCoroutine(Slow(_slowTime));
                    break;
                case Actor.DebuffState.Ice:
                    if(!_hasFreeze)
                        StartCoroutine(Freeze(_freezeTime));
                    break;
                case Actor.DebuffState.Shock:
                    if (!_hasShock)
                        StartCoroutine(Shock(_shockTime));
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
            }
        }
    }

    IEnumerator PowerUp(float delay)
    {

        // 불끈
        _hasPowerUp = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerController.RunSpeed += _maxSpeed * 0.1f;
        photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-037");

        yield return new WaitForSeconds(delay);

        // 불끈 해제
        _hasPowerUp = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;

        actor.StatusChangeEventInvoke();
        _audioClip = null;
    }
    IEnumerator Burn(float delay)
    {
        // 화상
        _hasBurn = true;
        actor.actorState = Actor.ActorState.Debuff;

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

        // 화상 해제
        _hasBurn = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Burn;

        actor.StatusChangeEventInvoke();
    }
    IEnumerator Exhausted(float delay)
    {
        // 지침
        _hasExhausted = true;
        photonView.RPC("WetCreate", RpcTarget.All);
        actor.actorState = Actor.ActorState.Debuff;
        JointDrive angularXDrive;

        angularXDrive = actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive;
        angularXDrive.positionSpring = 0f;
        actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive = angularXDrive;

        float startTime = Time.time;
        while (Time.time < startTime + delay)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / delay;

            actor.Stamina = Mathf.Clamp(actor.MaxStamina * percentage, 0, actor.MaxStamina);
            yield return null;
        }

        // 지침 해제
        _hasExhausted = false;
        DestroyEffect("Wet");
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Exhausted;
        angularXDrive.positionSpring = _xPosSpringAry[0];

        actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive = angularXDrive;
        actor.Stamina = 100;


        actor.StatusChangeEventInvoke();
    }
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

        actor.StatusChangeEventInvoke();
    }
    IEnumerator Freeze(float delay)
    {
        yield return new WaitForSeconds(0.2f);
        photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/Cartoon-UI-047");

        // 빙결
        _hasFreeze = true;
        actor.actorState = Actor.ActorState.Debuff;

        // 이펙트 생성
        if (!hasObject)
        {
            hasObject = true;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }

        yield return new WaitForSeconds(delay);

        // 빙결 해제
        _hasFreeze = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Ice;

        actor.StatusChangeEventInvoke();

        // 이펙트 삭제
        if (hasObject)
        {
            hasObject = false;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
        _audioClip = null;
    }
    IEnumerator Shock(float delay)
    {
        yield return new WaitForSeconds(0.2f);

        // 감전
        _hasShock = true;
        actor.actorState = Actor.ActorState.Debuff;
        photonView.RPC("PlayerDebuffSound", RpcTarget.All, "PlayerEffect/electronic_02");

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 4; i < actor.BodyHandler.BodyParts.Count; i++)
        {
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
                photonView.RPC("Stun", RpcTarget.All, _stunTime);
                StopCoroutine(Shock(delay));
            }

            if (UnityEngine.Random.Range(0, 20) > 17)
            {
                for (int i = 4; i < 14; i++)
                {
                    if (i == 9) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 4; i < 14; i++)
                {
                    if (i == 9) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        // 감전 해제
        _hasShock = false;
        StartCoroutine(ResetBodySpring());
        photonView.RPC("Stun", RpcTarget.All, 0.5f);
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;

        actor.StatusChangeEventInvoke();
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

        actor.StatusChangeEventInvoke();

        DestroyEffect("Stun_loop");
    }

    public void DestroyEffect(string name)
    {
        Debug.Log("Start DestroyEffect");
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

    [PunRPC]
    public void PoisonCreate()
    {
        EffectObjectCreate("Effects/Fog_poison");
    }

    [PunRPC]
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
        if (effectObject != null)
            effectObject.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
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
        photonView.RPC("InvulnerableState", RpcTarget.All, 0.5f);
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
            if (i == 3)
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

    public IEnumerator RestoreBodySpring(float _springLerpTime=1f)
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
}