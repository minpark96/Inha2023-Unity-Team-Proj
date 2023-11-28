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

    #region Private Fields

    string _roomName = "[4]Room";

    string _arenaName = "PO_Map_KYH";
    

    string _roomPlayerPath = "Ragdoll2_Room";

    string _playerPath1 = "Players/Player1";
    string _playerPath2 = "Players/Player2";
    string _playerPath3 = "Players/Player3";
    string _playerPath4 = "Players/Player4";
    string _playerPath5 = "Players/Player5";
    string _playerPath6 = "Players/Player6";

    string _ghostPath = "Spook";
    string _graveStonePath = "Item/GraveStone";

    bool _isChecked;

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    public int AlivePlayerCounts = 1;
    public float SpawnPointX = 485f;
    public float SpawnPointY = 12f;
    public float SpawnPointZ = 411f;

    // 스폰 포인트 6인 기준
    public List<Vector3> SpawnPoints = new List<Vector3>();

    public List<int> ActorViewIDs = new List<int>();
    public List<Actor> Actors = new List<Actor>();
    public List<int> Scores = new List<int>();

    // Arena UI
    public Image ImageHPBar;
    public Image ImageStaminusBar;
    public Image Portrait;

    public float GhostSpawnDelay = 3f;

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

        //if (SceneManager.GetActiveScene().name == _roomName)
        //    InstantiatePlayerInRoom();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            AlivePlayerCounts = PhotonNetwork.CurrentRoom.PlayerCount;
            InstantiatePlayer();

            SceneType = Define.Scene.Game;
            SceneBgmSound("BigBangBattleLOOPING");

            GameObject mainPanel = GameObject.Find("Main Panel");
            ImageHPBar = mainPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            ImageStaminusBar = mainPanel.transform.GetChild(0).GetChild(2).GetComponent<Image>();

            GameObject portrait = mainPanel.transform.GetChild(0).GetChild(0).gameObject;

            for (int i = 1; i <= 6; i++)
            {
                if (i == PhotonNetwork.LocalPlayer.ActorNumber)
                    portrait.transform.GetChild(i-1).gameObject.SetActive(true);
                else
                    portrait.transform.GetChild(i-1).gameObject.SetActive(false);
            }
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

    List<GameObject> temp = new List<GameObject>();

    void InstantiatePlayerInRoom()
    {
        GameObject go = null;

        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                go = Managers.Resource.PhotonNetworkInstantiate(_roomPlayerPath, pos: SpawnPoints[6]);
                temp.Add(go);
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
            Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            GameObject go = null;

            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath1, pos: SpawnPoints[0]);
                    break;
                case 2:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath2, pos: SpawnPoints[1]);
                    break;
                case 3:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath3, pos: SpawnPoints[2]);
                    break;
                case 4:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath4, pos: SpawnPoints[3]);
                    break;
                case 5:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath5, pos: SpawnPoints[4]);
                    break;
                case 6:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath6, pos: SpawnPoints[5]);
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

    IEnumerator InitiateGhost(Vector3 spawnPos)
    {
        if (Ghost.LocalGhostInstance == null)
        {
            Vector3 gsPos = spawnPos + new Vector3(0f, 10f, 0f);
            Managers.Resource.PhotonNetworkInstantiate(_graveStonePath, pos: gsPos);
            yield return new WaitForSeconds(GhostSpawnDelay);
            Managers.Resource.PhotonNetworkInstantiate(_ghostPath, pos: spawnPos);
        }
    }

    private void OnGUI()
    {
        GUI.backgroundColor = Color.white;
        for (int i = 0; i < ActorViewIDs.Count; i++)
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
            actor.OnChangePlayerStatus -= SendInfo;
            actor.OnChangePlayerStatus += SendInfo;
            actor.OnKillPlayer -= DealPlayerDeath;
            actor.OnKillPlayer += DealPlayerDeath;
        }
    }

    void AnnounceDeath(int viewID)
    {
        photonView.RPC("HandleDeath", RpcTarget.All, viewID);
    }

    void HandleDeath(int viewID)
    {

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
                {
                    photonView.RPC("PlayerDead", RpcTarget.MasterClient, viewID);
                    Vector3 deadPos = Actors[i].BodyHandler.Hip.transform.position;
                    StartCoroutine(InitiateGhost(deadPos));
                }

                break;
            }
        }
    }

    [PunRPC]
    void PlayerDead(int viewID)
    {
        Debug.Log("[Only Master] " + viewID + " Player is Dead!");

        AlivePlayerCounts--;

        if (AlivePlayerCounts == 1)
            EndRound();
    }

    void EndRound()
    {
        // To-Do: 라운드 종료
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
            Debug.Log("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

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

        if(SceneManager.GetActiveScene().name == _arenaName)
        {
            for (int i = 0; i < Actors.Count; i++)
            {
                if(Actors[i].photonView.IsMine)
                {
                    ImageHPBar.fillAmount = Actors[i].Health / Actors[i].MaxHealth;
                    ImageStaminusBar.fillAmount = Actors[i].Stamina / Actors[i].MaxStamina;
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
}
