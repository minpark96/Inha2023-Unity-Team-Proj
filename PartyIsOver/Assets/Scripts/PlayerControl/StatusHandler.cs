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

    void Update()
    {
        if (_healthDamage != 0f)
            UpdateHealth();

        //CheckConscious();
        DebuffAction();
    }

    private void OnGUI()
    {
        if(this.name == "Ragdoll2")
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 0, 200, 200), "버프상태:" + actor.debuffState.ToString());
            GUI.Label(new Rect(0, 30, 200, 200), "액션상태:" + actor.actorState.ToString());
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
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        // 빙결
        if (type == Damage.Ice)
        {
            actor.debuffState |= Actor.DebuffState.Ice; // 빙결 디버프 활성화

            // 다른 디버프 체크
            foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
            {
                // 빙결 이외의 상태가 켜지면 끄기
                if(state != Actor.DebuffState.Ice && (actor.debuffState & state) != 0)
                {
                    actor.debuffState &= ~state;
                }
            }
        }
        
        if (actor.debuffState == Actor.DebuffState.Ice) return;

        // 감전
        if(type == Damage.ElectricShock)
        {
            actor.debuffState |= Actor.DebuffState.ElectricShock; // 감전 디버프 활성화
        }

        // 기절
        if(actor.actorState == Actor.ActorState.Unconscious || type == Damage.Knockout) // 실험용 데미지 녹아웃 씀
        {
            actor.debuffState |= Actor.DebuffState.Unconscious; // 기절 디버프 활성화
        }

    }


    public void DebuffAction()
    {
        switch (actor.debuffState)
        {
            case Actor.DebuffState.Default:
                break;
            case Actor.DebuffState.Balloon:
                break;
            case Actor.DebuffState.Unconscious:
                StartCoroutine("ResetBodySpring");
                StartCoroutine("RestoreBodySpring");
                break;
            case Actor.DebuffState.Drunk:
                break;
            case Actor.DebuffState.ElectricShock:
                StartCoroutine("ElectricShock");
                break;
            case Actor.DebuffState.Ice:
                StartCoroutine("Ice");
                break;
            case Actor.DebuffState.Fire:
                break;
            case Actor.DebuffState.Invisible:
                break;
            case Actor.DebuffState.Strong:
                break;
        }
    }

    IEnumerator Ice()
    {
        Debug.Log("빙결!");
        yield return new WaitForSeconds(0.2f);

        // [빙결]
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

        yield return new WaitForSeconds(3f);

        // [빙결 해제]
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.Ice;

        // 이펙트 삭제
        if (hasObject)
        {
            hasObject = false;
        }

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
    }


    IEnumerator ElectricShock()
    {
        Debug.Log("감전!");
        yield return new WaitForSeconds(0.2f);

        // 감전
        StartCoroutine("ResetBodySpring");


        float startTime = Time.time;
        float shockDuration = 3f; // 감전시간
        while (Time.time - startTime < shockDuration)
        {
            if (actor.debuffState == Actor.DebuffState.Ice)
            {
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine("RestoreBodySpring");
                StopCoroutine("ElectricShock");
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

        yield return new WaitForSeconds(2.0f);

        // 감전 해제
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState &= ~Actor.DebuffState.ElectricShock;
        StartCoroutine("RestoreBodySpring");
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
                if (actor.debuffState == Actor.DebuffState.Ice)
                    return;

                Debug.Log("데미지가 많아서 기절");
                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
                actor.debuffState = Actor.DebuffState.Unconscious;
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
        yield return new WaitForSeconds(2.0f);

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

        actor.debuffState &= ~Actor.DebuffState.Unconscious;
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
