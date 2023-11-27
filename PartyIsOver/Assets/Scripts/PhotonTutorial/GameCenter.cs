using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Actor;

public class GameCenter : BaseScene
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;

    #endregion

    string _roomName = "[4]Room";
   
    #region Private Fields

    string _arenaName = "PO_Map_KYH";
    
    string _playerPath = "Ragdoll2";

    string _roomPlayerPath = "Ragdoll2_Room";

    string _ghostPath = "Spook";


    bool _isChecked;


    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    public float SpawnPointX = 484.604f;
    public float SpawnPointY = 17f;
    public float SpawnPointZ = 402.4796f;

    // 스폰 포인트 6인 기준
    public List<Vector3> SpawnPoints = new List<Vector3>();

    public List<int> ActorViewIDs = new List<int>();
    public List<Actor> Actors = new List<Actor>();

    #endregion

    #region Private Methods

    void Awake()
    {
        SpawnPoints.Add(new Vector3(SpawnPointX + 5f, SpawnPointY, SpawnPointZ + 0f));
        SpawnPoints.Add(new Vector3(SpawnPointX + 2.5f, SpawnPointY, SpawnPointZ + 4.33f));
        SpawnPoints.Add(new Vector3(SpawnPointX + -2.5f, SpawnPointY, SpawnPointZ + 4.33f));
        SpawnPoints.Add(new Vector3(SpawnPointX + -5f, SpawnPointY, SpawnPointZ + 0f));
        SpawnPoints.Add(new Vector3(SpawnPointX + -2.5f, SpawnPointY, SpawnPointZ + -4.33f));
        SpawnPoints.Add(new Vector3(SpawnPointX + 2.5f, SpawnPointY, SpawnPointZ + -4.33f));

        SpawnPoints.Add(new Vector3(0f,0f,0f));


        if (photonView.IsMine)
        {
            LocalGameCenterInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            InstantiatePlayer();
            SceneType = Define.Scene.Game;
            SceneBgmSound("BigBangBattleLOOPING");
        }
    }

    public void SceneBgmSound(string path)
    {
        GameObject root = GameObject.Find("@Sound");
        if (root != null)
        {
            AudioSource _audioSources = Managers.Sound.GetBgmAudioSource();

            SceneManagerEx sceneManagerEx = new SceneManagerEx();
            string currentSceneName = sceneManagerEx.GetCurrentSceneName();

            if (_arenaName == currentSceneName)
            {
                _audioSources.Stop();
                AudioClip audioClip = Managers.Resource.Load<AudioClip>($"Sounds/Bgm/{path}");
                _audioSources.clip = audioClip;
                _audioSources.volume = 0.1f;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);
            }

            if (_roomName == currentSceneName)
            {
                _audioSources.Stop();
                AudioClip audioClip = Managers.Resource.Load<AudioClip>($"Sounds/Bgm/{path}");
                _audioSources.clip = audioClip;
                _audioSources.volume = 0.1f;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);
            }

        }
    }

    void InstantiatePlayerInRoom()
    {
        GameObject go = null;

        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
            case 2:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
            case 3:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
            case 4:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
            case 5:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
            case 6:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                break;
        }
    }

    void InstantiatePlayer()
    {
        if (Actor.LocalPlayerInstance == null)
        {
            //Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            GameObject go = null;

            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[0]);
                    break;
                case 2:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[1]);
                    break;
                case 3:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[2]);
                    break;
                case 4:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[3]);
                    break;
                case 5:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[4]);
                    break;
                case 6:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[5]);
                    break;
            }

            PhotonView pv = go.GetComponent<PhotonView>();
            int viewID = pv.ViewID;

            if (PhotonNetwork.IsMasterClient)
            {
                ActorViewIDs.Add(viewID);
                AddActor(viewID);
            }
            else
            {
                photonView.RPC("RegisterActorInfo", RpcTarget.MasterClient, viewID);
            }

            Debug.Log("ActorViewIDs.Count: " + ActorViewIDs.Count);
        }
    }

    private void OnGUI()
    {
        GUI.backgroundColor = Color.white;
        for (int i = 0; i <ActorViewIDs.Count; i++)
        {
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(0, 140 + i * 40, 200, 200), "Actor View ID: " + ActorViewIDs[i] + " / HP: " + Actors[i].Health);
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 160 + i * 40, 200, 200), "Status: " + Actors[i].actorState + " / Debuff: " + Actors[i].debuffState);
        }
    }

    void SubscribeActorEvent(Actor actor)
    {
        if (actor != null)
        {
            Debug.Log("구독 부분 " + actor.photonView.ViewID);
            actor.OnPlayerStatusChanges -= SendInfo;
            actor.OnPlayerStatusChanges += SendInfo;
            //actor.OnPlayerExhaust -= DecreaseStamina;
            //actor.OnPlayerExhaust += DecreaseStamina;
        }
    }
   
    void SendInfo(float hp, float stamina, Actor.ActorState actorState, Actor.DebuffState debuffstate, int viewID)
    {
        Debug.Log("[master Event] SendInfo()");

        photonView.RPC("SyncInfo", RpcTarget.Others, hp, actorState, debuffstate, viewID);
    }

    [PunRPC]
    void SyncInfo(float hp, Actor.ActorState actorState,Actor.DebuffState debuffstate, int viewID)
    {
        Debug.Log("[except master received] SyncInfo()");

        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].Health = hp;
                Actors[i].actorState = actorState;
                Actors[i].debuffState = debuffstate;

                if (Actors[i].actorState == ActorState.Dead)
                    InitiateGhost();
                break;
            }
        }
    }

    void InitiateGhost()
    {
        if (Ghost.LocalGhostInstance == null)
        {
            Managers.Resource.PhotonNetworkInstantiate(_ghostPath);
        }
    }

    [PunRPC]
    void RegisterActorInfo(int viewID)
    {
        Debug.Log("마스터: RegisterActorInfo");
        Debug.Log(viewID);

        ActorViewIDs.Add(viewID);
        AddActor(viewID);

        photonView.RPC("SyncActorsList", RpcTarget.Others, ActorViewIDs.ToArray());
    }

    [PunRPC]
    void SyncActorsList(int[] ids)
    {
        for (int i = ActorViewIDs.Count; i < ids.Length; i++)
        {
            ActorViewIDs.Add(ids[i]);
            AddActor(ids[i]);
        }
    }

    void AddActor(int id)
    {
        PhotonView targetPV = PhotonView.Find(id);

        if (targetPV != null)
        {
            Actor actor = targetPV.transform.GetComponent<Actor>();
            Actors.Add(actor);

            if (_roomUI.SkillChange)
                actor.PlayerController.isMeowNyangPunch = true;
            else
                actor.PlayerController.isMeowNyangPunch = false;


            if (PhotonNetwork.IsMasterClient)
            {
                SubscribeActorEvent(actor);
            }
        }
    }


    void Start()
    {
        InitRoomUI();
    }

    void InitRoomUI()
    {
        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();

        if (PhotonNetwork.IsMasterClient)
            _roomUI.ReadyButton.SetActive(false);
        else
            _roomUI.PlayButton.SetActive(false);
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == _roomName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UpdateMasterStatus();
            }
            else
            {
                if (_roomUI.Ready == true)
                {
                    if (_isChecked == false)
                    {
                        PlayerReady();
                        _isChecked = true;
                    }
                }
                else
                {
                    if (_isChecked == true)
                    {
                        PlayerReady();
                        _isChecked = false;
                    }
                }
            }
        }
    }

    void PlayerReady()
    {
        photonView.RPC("UpdateCount", RpcTarget.MasterClient, _roomUI.Ready);
    }

    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            _roomUI.CanPlay = true;
        else
            _roomUI.CanPlay = false;
    }

    [PunRPC]
    void UpdateCount(bool isReady)
    {
        if (isReady)
            _roomUI.PlayerReadyCount++;
        else
            _roomUI.PlayerReadyCount--;
    }

    #endregion

    #region MonoBehaviourPunCallbacks Methods

    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    public override void Clear()
    {
    }

    #endregion

}
