using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
//using System.Numerics;
using UnityEngine;
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
    private float _healthRegeneration;
    private float _healthDamage;
    private bool _isDead;

    private float _maxUnconsciousTime=5f;
    private float _minUnconsciousTime=3f;
    private float _unconsciousTime = 0f;

    private float _knockoutThreshold=20f;


    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();


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

        CheckConscious();
    }

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
        if(type == Damage.Ice)
        {
            StartCoroutine("Ice"); // 빙결 행동
            actor.debuffState |= Actor.DebuffState.Ice; // 빙결 디버프 활성화

            // 다른 디버프 체크
            foreach (Actor.DebuffState state in Actor.DebuffState.GetValues(typeof(Actor.DebuffState)))
            {
                if((actor.debuffState & state) != 0)
                {
                    //state &= ~state;
                }
            }
        }
        else if(type == Damage.ElectricShock)
        {
            StartCoroutine("ElectricShock");
        }


        
    }

    IEnumerator Ice()
    {
        Debug.Log("빙결!");
        yield return new WaitForSeconds(0.2f);

        // 빙결
        actor.actorState = Actor.ActorState.Debuff;
        actor.debuffState = Actor.DebuffState.Ice;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = true;
        }

        yield return new WaitForSeconds(1.5f);

        // 빙결 해제
        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState = Actor.DebuffState.Default;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            actor.BodyHandler.BodyParts[i].PartRigidbody.isKinematic = false;
        }
    }


    IEnumerator ElectricShock()
    {
        Debug.Log("감전!");
        yield return new WaitForSeconds(0.5f);

        // 감전
        actor.actorState = Actor.ActorState.Debuff;
        actor.debuffState = Actor.DebuffState.ElectricShock;


        float startTime = Time.realtimeSinceStartup;
        float shockDuration = 3f; // 감전시간

        while (Time.realtimeSinceStartup - startTime < shockDuration)
        {
            int number = Random.Range(0, 11);

            if (number > 8)
            {
                for (int i = 1; i < 15; i++)
                {
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(30, 0, 0);
                }
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {
                    actor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-30, 0, 0);
                }
            }
            yield return null;
        }

        // 감전 해제
        yield return new WaitForSeconds(1.0f);

        actor.actorState = Actor.ActorState.Stand;
        actor.debuffState = Actor.DebuffState.Default;

        actor.BodyHandler.Hip.transform.rotation = Quaternion.identity;
        actor.BodyHandler.Hip.transform.Rotate(-90, 0, 0, Space.Self);
        StartCoroutine(RestoreFromFaint());

        //actor.BodyHandler.Hip.transform.rotation = Quaternion.Euler(-90, 0, 0);

        Debug.Log(actor.BodyHandler.Hip.transform.rotation.eulerAngles.x);
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
                Debug.Log("기절");

                _maxUnconsciousTime = Mathf.Clamp(_maxUnconsciousTime + 1.5f, _minUnconsciousTime, 20f);
                _unconsciousTime = _maxUnconsciousTime;
                actor.actorState = Actor.ActorState.Unconscious;
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

    public void CheckConscious()
    {
        if (actor.actorState != Actor.ActorState.Unconscious && _unconsciousTime > 0f)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
        }

        // 기절일때
        if (actor.actorState == Actor.ActorState.Unconscious)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);

            StartCoroutine(Faint());

            // 기절 해제
            if (_unconsciousTime <= 0f)
            {
                actor.actorState = Actor.ActorState.Stand;
                StartCoroutine(RestoreFromFaint());
                _unconsciousTime = 0f;
            }
        }
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

    IEnumerator Faint()
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == 3)
            {
                continue;
            }

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }

        yield return null;
    }

    IEnumerator RestoreFromFaint()
    {
        float springLerpDuration = 2f;

        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        float startTime = Time.time;


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
