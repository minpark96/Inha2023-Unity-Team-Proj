using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Actor : MonoBehaviourPun
{
    public Transform CameraArm;

    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerController;
    public Grab Grab;

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
    }

    public enum DebuffState
    {
        Default =   0x0,  // X
        // 버프
        PowerUp =   0x1,  // 불끈
        // 디버프
        Burn =      0x2,  // 화상
        Exhausted = 0x4,  // 지침
        Slow =      0x8, // 둔화
        Ice =    0x10, // 빙결
        Shock =     0x20, // 감전
        Stun =      0x40, // 기절
        // 상태변화
        Drunk =     0x80, // 취함
        Balloon =   0x100, // 풍선
        Ghost =     0x200, // 유령
    }

    public float HeadMultiple = 1.5f;
    public float ArmMultiple = 0.8f;
    public float HandMultiple = 0.8f;
    public float LegMultiple = 0.8f;
    public float DamageReduction = 0f;
    public float PlayerAttackPoint = 1f;


    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    public static int LayerCnt = 26;

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

        ChangeLayerRecursively(gameObject, LayerCnt++);
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
        if (!photonView.IsMine) return;

        LookAround();
        CursorControl();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        
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
}
