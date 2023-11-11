using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviourPun, IPunObservable
{
    public delegate void PlayerStatusChanges(float HP, float Stamina, ActorState actorState, DebuffState debuffstate, int viewID);
    public event PlayerStatusChanges OnPlayerStatusChanges;

    public Transform CameraArm;

    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerController;
    public Grab Grab;
    public BalloonState BalloonState;

    public enum ActorState
    {
        Dead = 0x1,
        Unconscious = 0x2,
        Stand = 0x4,
        Walk = 0x8,
        Run = 0x10,
        Roll = 0x20,
        Jump = 0x40,
        Fall = 0x80,
        Climb = 0x100,
        Debuff = 0x200,
        BalloonWalk = 0x400,

    }

    public enum DebuffState
    {
        Default =   0x0,
        PowerUp =   0x1,
        Burn =      0x2,
        Exhausted = 0x4,
        Slow =      0x8,
        Ice =       0x10,
        Shock =     0x20, 
        Stun =      0x40, 
        Drunk =     0x80,  
        Balloon =   0x100, 
        Ghost =     0x200,
    }

    public float HeadMultiple = 1.5f;
    public float ArmMultiple = 0.8f;
    public float HandMultiple = 0.8f;
    public float LegMultiple = 0.8f;
    public float DamageReduction = 0f;
    public float PlayerAttackPoint = 1f;

    // 체력
    [SerializeField]
    private float _health;
    [SerializeField]
    private float _maxHealth = 200f;
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _maxHealth; } }

    // 스테미나
    [SerializeField]
    private float _stamina = 100f;
    [SerializeField]
    private float _maxStamina = 100f;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _maxStamina; } }

    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    public static int LayerCnt = 26;

    public void StatusChangeEventInvoke()
    {
        Debug.Log("StatusChangeEventInvoke()");

        if (OnPlayerStatusChanges == null)
            Debug.Log(photonView.ViewID + " 이벤트 null");

        //Debug.Log("_health: " + _health + " debuffState: " + debuffState + " photonView.ViewID: " + photonView.ViewID);
        //OnPlayerStatusChanges(_health, _stamina, actorState, debuffState, photonView.ViewID);
    }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        else
        {
            // 다른 클라이언트 카메라 끄기
            transform.GetChild(0).gameObject.SetActive(false);
        }
        DontDestroyOnLoad(this.gameObject);

        CameraArm = transform.GetChild(0).GetChild(0);
        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerController = GetComponent<PlayerController>();
        Grab = GetComponent<Grab>();
        BalloonState = GetComponent<BalloonState>();

        ChangeLayerRecursively(gameObject, LayerCnt++);

        _health = _maxHealth;
    }

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine || actorState == ActorState.Dead) return;

        LookAround();
        CursorControl();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || actorState == ActorState.Dead) return;
        
        if (actorState != lastActorState)
        {
            PlayerController.isStateChange = true;
        }
        else
        {
            PlayerController.isStateChange = false;
        }

        switch (actorState)
        {
            case ActorState.Dead:
                break;
            case ActorState.Unconscious:
                break;
            case ActorState.Stand:
                PlayerController.Stand();
                break;
            case ActorState.Walk:
                PlayerController.Move();
                break;
            case ActorState.Run:
                PlayerController.Move();
                break;
            case ActorState.Jump:
                PlayerController.Jump();
                break;
            case ActorState.Fall:
                break;
            case ActorState.Climb:
                break;
            case ActorState.Roll:
                break;
            case ActorState.BalloonWalk:
                BalloonState.BalloonMove();
                break;
        }

        lastActorState = actorState;
    }

    //카메라 컨트롤
    private void LookAround()
    {
        CameraArm.parent.transform.position = BodyHandler.Hip.transform.position;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        CameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void CursorControl()
    {
        if (Input.anyKeyDown)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!Cursor.visible && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonSerializeView");
        if (stream.IsWriting)
        {
            Debug.Log("Writing");
            // We own this player: send the others our data
            Debug.Log("Writing actorState: " + actorState);
            stream.SendNext(actorState);
        }
        else
        {
            Debug.Log("Receiving");
            // Network player, receive data
            Debug.Log("Receiving B actorState: " + actorState);
            this.actorState = (ActorState)stream.ReceiveNext();
            Debug.Log("Receiving A actorState: " + actorState);
        }
    }
}
