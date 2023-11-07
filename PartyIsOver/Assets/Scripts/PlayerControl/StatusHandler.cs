using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StatusHandler : MonoBehaviour
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;
    
    private float _healthDamage;
    private bool _isDead;

    private float _maxUnconsciousTime=5f;
    private float _minUnconsciousTime=3f;
    private float _unconsciousTime = 0f;

    private float _knockoutThreshold=20f;

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
    private bool _hasFreeze;
    private bool _hasShock;
    private bool _hasStun;
    private bool _hasDrunk;
    private bool _hasBalloon;
    private bool _hasGhost;


    [Header("불끈 시간")]
    [SerializeField]
    private float _powerUpTime = 3f;
    [Header("화상 시간")]
    [SerializeField]
    private float _burnTime = 3f;
    [Header("화상 데미지")]
    [SerializeField]
    private float _burnDamage = 1f;
    [Header("지침 시간")]
    [SerializeField]
    private float _exhaustedTime = 5f;
    [Header("둔화 시간")]
    [SerializeField]
    private float _slowTime = 3f;
    [Header("빙결 시간")]
    [SerializeField]
    private float _freezeTime = 3f;
    [Header("감전 시간")]
    [SerializeField]
    private float _shockTime = 3f;
    [Header("기절 시간")]
    [SerializeField]
    private float _stunTime = 3f;



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
        if (_healthDamage != 0f)
            UpdateHealth();

        // 지침 디버프 활성화/비활성화
        if(actor.Stamina == 0)
        {
            if(!_hasExhausted)
            {
                startTime = Time.time; // 디버그용
                actor.debuffState |= Actor.DebuffState.Exhausted;
                StartCoroutine(Exhausted(_exhaustedTime));
            }
        }
    }

    // 디버프 시간 관리 (디버그용)
    float startTime = 0;
    float endTime = 0;
    private void OnGUI()
    {
        if(this.name == "Ragdoll2")
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "버프상태:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 20, 200, 200), "액션상태:" + actor.actorState.ToString());
            GUI.Label(new Rect(0, 40, 200, 200), "디버프 걸린 시간:" + (endTime - startTime));

            GUI.contentColor = Color.blue;
            GUI.Label(new Rect(0, 60, 200, 200), "체력: " + actor.Health);
            GUI.Label(new Rect(0, 80, 200, 200), "스테미나: " + actor.Stamina);
        }
    }

    // 충격이 가해지면(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        // 데미지 체크
        damage *= _damageModifer;

        if (type == Damage.PowerUp) // 공격력 10% 증가
            damage *= 1.1f;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            _healthDamage += damage;
        }

        // 상태이상 체크
        DebuffCheck(type);
        DebuffAction();
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        startTime = Time.time; // 디버그용

        if (actor.debuffState == Actor.DebuffState.Freeze) return;

        switch (type)
        {
            case Damage.Freeze: // 빙결
                actor.debuffState |= Actor.DebuffState.Freeze;
                // 다른 디버프 체크
                foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
                {
                    // 빙결 이외의 상태가 켜지면 끄기
                    if (state != Actor.DebuffState.Freeze && (actor.debuffState & state) != 0)
                    {
                        actor.debuffState &= ~state;
                    }
                }
                break;
            case Damage.PowerUp: // 불끈
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Slow: // 둔화
                actor.debuffState |= Actor.DebuffState.Slow;
                break;
            case Damage.Shock: // 감전
                actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Knockout: // 기절 (실험용 Damage.Knockout 씀)
                actor.debuffState |= Actor.DebuffState.Stun;
                break;
            case Damage.Burn: // 화상
                actor.debuffState |= Actor.DebuffState.Burn;
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
                case Actor.DebuffState.Freeze:
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
                        StartCoroutine(Stun(_stunTime));
                    }
                    break;
                case Actor.DebuffState.Drunk:
                    break;
                case Actor.DebuffState.Balloon:
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

        yield return new WaitForSeconds(delay);

        // 불끈 해제
        _hasPowerUp = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerController.RunSpeed -= _maxSpeed * 0.1f;

        endTime = Time.time; // 디버그용
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
            if (actor.debuffState == Actor.DebuffState.Freeze)
            {
                _hasBurn = false;
                actor.actorState = Actor.ActorState.Stand;
                StopCoroutine(Burn(delay));
            }

            if (Time.time - lastBurnTime >= 1.0f) // 1초간 데미지+액션
            {
                actor.Health -= _burnDamage;
                actor.BodyHandler.BodyParts[2].PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                actor.BodyHandler.BodyParts[3].PartRigidbody.AddForce((actor.BodyHandler.Hip.transform.right) * 25, ForceMode.VelocityChange);
                lastBurnTime = Time.time;
            }

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        // 화상 해제
        _hasBurn = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Burn;

        endTime = Time.time; // 디버그용
    }
    IEnumerator Exhausted(float delay)
    {
        // 지침
        _hasExhausted = true;
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
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Exhausted;
        angularXDrive.positionSpring = _xPosSpringAry[0];

        actor.BodyHandler.BodyParts[0].PartJoint.angularXDrive = angularXDrive;
        actor.Stamina = 100;

        endTime = Time.time; // 디버그용
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

        endTime = Time.time; // 디버그용
    }
    IEnumerator Freeze(float delay)
    {
        yield return new WaitForSeconds(0.2f);

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
        actor.debuffState &= ~Actor.DebuffState.Freeze;

        // 이펙트 삭제
        if (hasObject)
        {
            hasObject = false;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }

        endTime = Time.time; // 디버그용
    }
    IEnumerator Shock(float delay)
    {
        yield return new WaitForSeconds(0.2f);

        // 감전
        _hasShock = true;
        actor.actorState = Actor.ActorState.Debuff;

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
            if (actor.debuffState == Actor.DebuffState.Freeze)
            {
                _hasShock = false;
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine(Stun(_stunTime));
                StopCoroutine(Shock(delay));
            }

            if (Random.Range(0, 20) > 17)
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

        endTime = Time.time; // 디버그용

        // 감전 해제
        _hasShock = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        StartCoroutine(ResetBodySpring());
        StartCoroutine(Stun(3));
    }
    IEnumerator Stun(float delay)
    {
        _hasStun = true;
        yield return new WaitForSeconds(delay);

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        float startTime = Time.time;
        float springLerpDuration = 2f;
        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            int j = 0;

            for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
                angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;

                actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

                angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
                angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
                actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
                j++;

                yield return null;
            }
        }

        _hasStun = false;
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Stun;
        endTime = Time.time;
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

        //기절상태가 아닐때 일정 이상의 데미지를 받으면 기절
        if (actor.actorState != Actor.ActorState.Unconscious)
        {
            if (_unconsciousTime >= 0f)
                _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            
            if (realDamage >= _knockoutThreshold)
            {
                if (actor.debuffState == Actor.DebuffState.Freeze)
                    return;

                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Stun;
                EnterUnconsciousState();
            }
        }

        // 기절일때
        if (actor.actorState == Actor.ActorState.Unconscious)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            StartCoroutine(ResetBodySpring());
            StartCoroutine(Stun(_unconsciousTime));
            _unconsciousTime = 0f;
        }

        //계산한 체력이 0보다 작으면 Death로
        if (tempHealth <= 0f)
        {
            KillPlayer();
            EnterUnconsciousState();
        }

        actor.Health = Mathf.Clamp(tempHealth, 0f, actor.MaxHealth);
        _healthDamage = 0f;
    }

    void KillPlayer()
    {

    }

    void EnterUnconsciousState()
    {
        //데미지 이펙트나 사운드 추후 추가

        //actor.BodyHandler.ResetLeftGrab();
        //actor.BodyHandler.ResetRightGrab();
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.LeftForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        actor.BodyHandler.RightForarm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    IEnumerator ResetBodySpring()
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
                continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }

        yield return null;
    }

    // player controller로 옮기기
    IEnumerator RestoreBodySpring()
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        float startTime = Time.time;
        float springLerpDuration = 2f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            int j = 0;

            for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
            {
                if (i == 3)
                {
                    continue;
                }
                angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
                angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;

                actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

                angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
                angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
                actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
                j++;

                yield return null;
            }
        }
    }
}

