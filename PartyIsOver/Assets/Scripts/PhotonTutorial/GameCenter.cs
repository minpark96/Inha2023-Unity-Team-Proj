using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;

    #endregion

    #region Private Fields

    string _arenaName = "MJTest";
    
    string _playerPath = "Ragdoll2";

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    public float SpawnPointX = 517.5f;
    public float SpawnPointY = 17f;
    public float SpawnPointZ = 420f;

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
        }

        if (scene.name == _arenaName)
        {
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

            if ("Room" == currentSceneName)
            {
                _audioSources.Stop();
                AudioClip audioClip = Managers.Resource.Load<AudioClip>($"Sounds/Bgm/{path}");
                _audioSources.clip = audioClip;
                _audioSources.volume = 0.1f;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);
            }

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
                Actor actor = go.GetComponent<Actor>();
                ActorViewIDs.Add(viewID);
                Actors.Add(actor);
                SubscribeActorEvent(actor);
            }
            else
            {
                photonView.RPC("RegisterActorInfo", RpcTarget.MasterClient, viewID);
            }
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
                break;
            }
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
            if (PhotonNetwork.IsMasterClient)
            {
                SubscribeActorEvent(actor);
            }
        }
    }

    void DecreaseStamina(int amount)
    {

    }

    void LoadArena()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(_arenaName);
        }
        else
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
    }

    void Start()
    {
        InitRoomUI();
    }

    void InitRoomUI()
    {
        Debug.Log("IsRoom: " + PhotonNetwork.InRoom + ", Scene: " + SceneManager.GetActiveScene().name);
        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();

        if (PhotonNetwork.IsMasterClient)
        {
            _roomUI.IsReady = true;
            _roomUI.SetButtonActive("ready", false);
            _roomUI.AddButtonEvent("play", LoadArena);
            _roomUI.UpdateReadyCountText(_roomUI.IsReady);
            _roomUI.SetButtonInteractable("play", true);
            _roomUI.SetPlayerStatus("Wait for Other Players...");
        }
        else
        {
            _roomUI.SetButtonActive("play", false);
            _roomUI.AddButtonEvent("ready", Ready);
            _roomUI.SetPlayerStatus("Unready");
            photonView.RPC("EnteredRoom", RpcTarget.MasterClient);
        }
    }

    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            _roomUI.SetPlayerStatus("All Players Ready!");
            _roomUI.SetButtonInteractable("play", true);
        }
        else
        {
            _roomUI.SetPlayerStatus("Wait for Other Players...");
            _roomUI.SetButtonInteractable("play", false);
        }
    }

    void Ready()
    {
        _roomUI.IsReady = !_roomUI.IsReady;
        _roomUI.SetPlayerStatus();
        photonView.RPC("PlayerReady", RpcTarget.MasterClient, _roomUI.IsReady);
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

    #region PunRPC Methods

    [PunRPC]
    void EnteredRoom()
    {
        _roomUI.UpdateReadyCountText(_roomUI.PlayerReadyCount);
        UpdateMasterStatus();

        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    [PunRPC]
    void UpdateCount(int count)
    {
        _roomUI.UpdateReadyCountText(count);
    }

    [PunRPC]
    void PlayerReady(bool isReady)
    {
        _roomUI.UpdateReadyCountText(isReady);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    #endregion
}
