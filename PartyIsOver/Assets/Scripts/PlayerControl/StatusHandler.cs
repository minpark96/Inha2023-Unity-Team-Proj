using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Numerics;
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


    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _health = _maxHealth;
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

        damage *= _damageModifer;
        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && actor.actorState != Actor.ActorState.Unconscious)
        {
            //if (numOfDamageTypes < damageTypes.Length)
            //{
            //    damageTypes[numOfDamageTypes] = new Damage(type, damage, causer);
            //    numOfDamageTypes++;
            //    hasDamageTypes = true;
            //}
            _healthDamage += damage;
        }

        DebuffCheck(type);
    }

    // 상태이상 체크
    public void DebuffCheck(InteractableObject.Damage type)
    {
        switch (type)
        {
            case Damage.Ice:
                StartCoroutine("Ice"); // 빙결
                break;
            case Damage.ElectricShock:
                StartCoroutine("ElectricShock"); // 감전
                break;
            default:
                break;
        }
    }

    IEnumerator Ice()
    {
        actor.BodyHandler.LeftHand.PartRigidbody.isKinematic = true;
        actor.BodyHandler.LeftForarm.PartRigidbody.isKinematic = true;
        actor.BodyHandler.LeftArm.PartRigidbody.isKinematic = true;
        actor.BodyHandler.LeftFoot.PartRigidbody.isKinematic = true;
        actor.BodyHandler.LeftLeg.PartRigidbody.isKinematic = true;
        actor.BodyHandler.LeftThigh.PartRigidbody.isKinematic = true;

        actor.BodyHandler.RightHand.PartRigidbody.isKinematic = true;
        actor.BodyHandler.RightForarm.PartRigidbody.isKinematic = true;
        actor.BodyHandler.RightArm.PartRigidbody.isKinematic = true;
        actor.BodyHandler.RightFoot.PartRigidbody.isKinematic = true;
        actor.BodyHandler.RightLeg.PartRigidbody.isKinematic = true;
        actor.BodyHandler.RightThigh.PartRigidbody.isKinematic = true;

        actor.BodyHandler.Head.PartRigidbody.isKinematic = true;
        actor.BodyHandler.Chest.PartRigidbody.isKinematic = true;
        actor.BodyHandler.Waist.PartRigidbody.isKinematic = true;
        actor.BodyHandler.Hip.PartRigidbody.isKinematic = true;
        
        Debug.Log("빙결!");
        yield return new WaitForSeconds(1.5f);

        actor.BodyHandler.LeftHand.PartRigidbody.isKinematic = false;
        actor.BodyHandler.LeftForarm.PartRigidbody.isKinematic = false;
        actor.BodyHandler.LeftArm.PartRigidbody.isKinematic = false;
        actor.BodyHandler.LeftFoot.PartRigidbody.isKinematic = false;
        actor.BodyHandler.LeftLeg.PartRigidbody.isKinematic = false;
        actor.BodyHandler.LeftThigh.PartRigidbody.isKinematic = false;

        actor.BodyHandler.RightHand.PartRigidbody.isKinematic = false;
        actor.BodyHandler.RightForarm.PartRigidbody.isKinematic = false;
        actor.BodyHandler.RightArm.PartRigidbody.isKinematic = false;
        actor.BodyHandler.RightFoot.PartRigidbody.isKinematic = false;
        actor.BodyHandler.RightLeg.PartRigidbody.isKinematic = false;
        actor.BodyHandler.RightThigh.PartRigidbody.isKinematic = false;

        actor.BodyHandler.Head.PartRigidbody.isKinematic = false;
        actor.BodyHandler.Chest.PartRigidbody.isKinematic = false;
        actor.BodyHandler.Waist.PartRigidbody.isKinematic = false;
        actor.BodyHandler.Hip.PartRigidbody.isKinematic = false;
    }


    IEnumerator ElectricShock()
    {
        Debug.Log("감전!");

        float startTime = Time.realtimeSinceStartup;
        float shockDuration = 3f;

        while (Time.realtimeSinceStartup - startTime < shockDuration)
        {
            int number = Random.Range(0, 10);

            if (number > 7)
            {
                actor.BodyHandler.Hip.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.Chest.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.Waist.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.LeftArm.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.RightArm.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.LeftForarm.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
                actor.BodyHandler.RightForarm.transform.rotation = UnityEngine.Quaternion.Euler(30, 0, 0);
            }
            else
            {
                actor.BodyHandler.Hip.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.Chest.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.Waist.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.LeftArm.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.RightArm.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.LeftForarm.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
                actor.BodyHandler.RightForarm.transform.rotation = UnityEngine.Quaternion.Euler(-30, 0, 0);
            }

            yield return null;
        }

        // 감전 해제
        yield return new WaitForSeconds(1.0f);
        UnityEngine.Quaternion originalHip = UnityEngine.Quaternion.Euler(-90,0,0);
        actor.BodyHandler.Hip.transform.rotation = originalHip;
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
                //CheckForDamageAchievements();
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
        //damageCausers.Clear();
        //ClearDamageTypes();
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
        if (actor.actorState == Actor.ActorState.Unconscious)
        {
            _unconsciousTime = Mathf.Clamp(_unconsciousTime - Time.deltaTime, 0f, _maxUnconsciousTime);
            if (_unconsciousTime <= 0f)
            {
                actor.actorState = Actor.ActorState.Stand;
                actor.PlayerControll.RestoreSpringTrigger();
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
}
