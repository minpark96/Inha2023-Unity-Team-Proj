using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
//using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using static InteractableObject;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StatusHandler : MonoBehaviour
{
    private float _damageModifer = 1f;

    public Actor actor;

    public bool invulnerable = false;

    [SerializeField]
    private float _health;
    public float Health { get { return _health; } set { _health = value; } }

    private float _maxHealth = 200f;
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


    private float _beforeSpeed;
    private bool hasBuff;

    [Header("빙결 시간")]
    [SerializeField]
    private float _freezeTime = 3f;
    [Header("감전 시간")]
    [SerializeField]
    private float _shockTime = 3f;
    [Header("기절 시간")]
    [SerializeField]
    private float _stunTime = 3f;
    [Header("불끈 시간")]
    [SerializeField]
    private float _powerUpTime = 3f;
    [Header("둔화 시간")]
    [SerializeField]
    private float _slowTime = 3f;


    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _health = _maxHealth;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
            {
                continue;
            }
            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    float startTime = 0;
    float endTime = 0;
    void Update()
    {
        if (_healthDamage != 0f)
            UpdateHealth();

        //CheckConscious();
    }

    private void OnGUI()
    {
        if(this.name == "Ragdoll2")
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "버프상태:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 30, 200, 200), "액션상태:" + actor.actorState.ToString());
            GUI.Label(new Rect(0, 60, 200, 200), "디버프 걸린 시간:" + (endTime - startTime));
        }
    }

    // 충격이 가해지면(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer)
    {
        Debug.Log("AddDamage");

        // 데미지 체크
        damage *= _damageModifer;
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

        // 빙결
        if (type == Damage.Freeze)
        {
            actor.debuffState |= Actor.DebuffState.Freeze;

            // 다른 디버프 체크
            foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            {
                // 빙결 이외의 상태가 켜지면 끄기
                if(state != Actor.DebuffState.Freeze && (actor.debuffState & state) != 0)
                {
                    actor.debuffState &= ~state;
                }
            }
        }
        
        if (actor.debuffState == Actor.DebuffState.Freeze) return;

        // 불끈
        if(type == Damage.PowerUp)
        {
            actor.debuffState |= Actor.DebuffState.PowerUp;
        }

        // 투명
        // 화상

        // 지침

        // 둔화
        if (type == Damage.Slow)
        {
            actor.debuffState |= Actor.DebuffState.Slow;
        }

        // 감전
        if (type == Damage.Shock)
        {
            actor.debuffState |= Actor.DebuffState.Shock; 
        }

        // 기절
        if(actor.actorState == Actor.ActorState.Unconscious || type == Damage.Knockout) // 실험용 Damage.Knockout 씀
        {
            actor.debuffState |= Actor.DebuffState.Stun;
        }
    }


    public void DebuffAction()
    {
        // 2진법 싹돌고 켜져잇느지 확인
        // 켜져있는것만 행동하게끔 swtich문은 1개씩만 행동 X
        // for 2~3~4개 행동 다 돌려지게 

        // 이미 돌고있는 행동이라면? flag 만들어서 이미 돌고있다고 확인시켜주기

        switch (actor.debuffState)
        {
            case Actor.DebuffState.Default:
                break;
            case Actor.DebuffState.PowerUp:
                if (!hasBuff)
                {
                    _beforeSpeed = actor.PlayerControll.RunSpeed;
                    StartCoroutine("PowerUp", _powerUpTime);
                }
                break;
            case Actor.DebuffState.Invisible:
                break;
            case Actor.DebuffState.Burn:
                break;
            case Actor.DebuffState.Exhausted:
                break;
            case Actor.DebuffState.Slow:
                if (!hasBuff)
                {
                    _beforeSpeed = actor.PlayerControll.RunSpeed;
                    StartCoroutine("Slow", _slowTime);
                }
                break;
            case Actor.DebuffState.Freeze:
                StartCoroutine("Freeze", _freezeTime);
                break;
            case Actor.DebuffState.Shock:
                StartCoroutine("Shock");
                break;
            case Actor.DebuffState.Stun:
                StartCoroutine("ResetBodySpring");
                StartCoroutine("Stun");
                break;
            case Actor.DebuffState.Drunk:
                break;
            case Actor.DebuffState.Balloon:
                break;
            case Actor.DebuffState.Ghost:
                break;
        }
    }

    IEnumerator PowerUp(float delay)
    {
        Debug.Log("불끈!");

        // 불끈
        hasBuff = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerControll.RunSpeed = _beforeSpeed * 1.1f;

        yield return new WaitForSeconds(delay);

        // 불끈 해제
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.PowerUp;
        actor.PlayerControll.RunSpeed = _beforeSpeed;
        hasBuff = false;

        endTime = Time.time; // 디버그용
    }

    IEnumerator Slow(float delay)
    {
        Debug.Log("둔화!");

        // 둔화
        hasBuff = true;
        actor.actorState = Actor.ActorState.Debuff;
        actor.PlayerControll.RunSpeed = _beforeSpeed * 0.9f;

        yield return new WaitForSeconds(delay);

        // 둔화 해제
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Slow;
        actor.PlayerControll.RunSpeed = _beforeSpeed;
        hasBuff = false;

        endTime = Time.time; // 디버그용
    }

    IEnumerator Freeze(float delay)
    {
        Debug.Log("빙결!");
        yield return new WaitForSeconds(0.2f);

        // 빙결
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

    IEnumerator Shock()
    {
        Debug.Log("감전!");
        yield return new WaitForSeconds(0.2f);

        // 감전
        actor.actorState = Actor.ActorState.Debuff;
        StartCoroutine("ResetBodySpring");

        float startTime = Time.time;
      
        while (Time.time - startTime < _shockTime)
        {
            if (actor.debuffState == Actor.DebuffState.Freeze)
            {
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine("Stun");
                StopCoroutine("Shock");
            }

            int number = Random.Range(0, 10);
            
            if (number > 7)
            {
                for (int i = 1; i < 15; i++)
                {
                    if (i == 3) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
                }
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {
                    if (i == 3) continue;
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
                }
            }
            yield return null;
        }

        endTime = Time.time; // 디버그용

        // 감전 해제
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Shock;
        StartCoroutine("Stun");
    }

    IEnumerator Stun()
    {
        Debug.Log("기절!");

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        float startTime = Time.time;
       
        while (Time.time < startTime + _stunTime)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / _stunTime;
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

        actor.debuffState &= ~Actor.DebuffState.Stun;
        endTime = Time.time;
    }

    




    public void UpdateHealth()
    {
        if (_isDead)
            return;

        //현재 체력 받아오기
        float tempHealth = _health;


        //무적상태가 아닐때만 데미지 적용
        if (tempHealth > 0f && !invulnerable)
            tempHealth -= _healthDamage;


        float realDamage = _health - tempHealth;


        //기절상태가 아닐때 일정 이상의 데미지를 받으면 기절
        if (actor.actorState != Actor.ActorState.Unconscious)
        {
            if (realDamage >= _knockoutThreshold)
            {
                if (actor.debuffState == Actor.DebuffState.Freeze)
                    return;

                Debug.Log("데미지가 많아서 기절");
                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Stun;
                EnterUnconsciousState();
            }
        }

        //계산한 체력이 0보다 작으면 Death로
        if (tempHealth <= 0f)
        {
            KillPlayer();
            EnterUnconsciousState();
        }

        _health = Mathf.Clamp(tempHealth, 0f, _maxHealth);
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

// 안쓰는 함수 빼놓음

//public void CheckConscious()
//{
//    if (actor.actorState != Actor.ActorState.Unconscious && _unconsciousTime > 0f)
//    {
//        _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
//    }

//    // 기절일때
//    if (actor.actorState == Actor.ActorState.Unconscious)
//    {
//        _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);

//        StartCoroutine("ResetBodySpring");

//        // 기절 해제
//        if (_unconsciousTime <= 0f)
//        {
//            actor.actorState = Actor.ActorState.Stand;
//            StartCoroutine("RestoreBodySpring");
//            _unconsciousTime = 0f;
//        }
//    }
//}
