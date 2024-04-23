using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Actor;
using System.Reflection;
using System.Linq;
using Unity.VisualScripting;
using static Define;

public class GameCenter : BaseScene
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;
    
    [SerializeField]
    EndingUI _endingUI;

    ScoreBoardUI _scoreBoardUI;
    ArenaSettingsUI _arenaSettingsUI;

    MagneticField _magneticField;
    Floor _floor;
    SnowStorm _snowStorm;
    #endregion

    #region Private Fields

    string _roomName = "[4]Room";
    string _arenaName = "[5]Arena";

    string _playerPath1 = "Players/Player1";
    string _playerPath2 = "Players/Player2";
    string _playerPath3 = "Players/Player3";
    string _playerPath4 = "Players/Player4";
    string _playerPath5 = "Players/Player5";
    string _playerPath6 = "Players/Player6";

    string _ghostPath = "Players/Ghost";
    string _graveStonePath = "Item/GraveStone";


    public int[] _scores = new int[Define.MAX_PLAYERS_PER_ROOM] { 0, 0, 0, 0, 0, 0 };
    public string[] _nicknames = new string[Define.MAX_PLAYERS_PER_ROOM] { "", "", "", "", "", "" };
    public int[] _actorNumbers = new int[Define.MAX_PLAYERS_PER_ROOM] { 0, 0, 0, 0, 0, 0 };


    int[] _checkAreaName = new int[Define.MAX_PLAYERS_PER_ROOM];
    bool[] _checkInside = new bool[Define.MAX_PLAYERS_PER_ROOM];

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    // 스폰 포인트 6인 기준
    public List<Vector3> PlayerSpawnPoints = new List<Vector3>() 
    {
        new Vector3(480.5f, 15f, 401.23f),
        new Vector3(473.24f, 15f, 402.1f),
        new Vector3(482.55f, 15f, 405.68f),
        new Vector3(472.08f, 15f, 407.17f),
        new Vector3(481.82f, 15f, 411.03f),
        new Vector3(473.66f, 15f, 412.15f)
    };

    public List<Vector3> TwoHandedItemSpawnPoints = new List<Vector3>()
    {
        new Vector3(476.441f, 20f, 406.541f),
        new Vector3(460.6f, 20f, 407.8f)
    };

    public List<Vector3> RangedItemSpawnPoints = new List<Vector3>()
    {
        new Vector3(478.16f, 20f, 393.72f),
        new Vector3(471.17f, 20f, 395.25f),
        new Vector3(443.3f, 20f, 420.7f),
        new Vector3(450.7f, 20f, 412.2f)
    };

    public List<Vector3> ConsumableItemSpawnPoints = new List<Vector3>()
    {
        new Vector3(486.98f, 20f, 408.49f),
        new Vector3(480.49f, 20f, 422.68f),
        new Vector3(469.49f, 20f, 414.05f),
        new Vector3(452.4f, 20f, 426.6f),
        new Vector3(456.5f, 20f, 409.7f),
        new Vector3(443.6f, 20f, 412.2f)
    };

    public List<int> ActorViewIDs = new List<int>();
    public List<Actor> Actors = new List<Actor>();

    public GameObject MyGraveStone = null;
    public GameObject MyGhost = null;

    // Arena UI
    public Image ImageHPBar;
    public Image ImageStaminaBar;
    public Image Portrait;

    public float GhostSpawnDelay = 4f;
    public float RoundEndDelay = 7f;
    public float ItemSpawnInterval = 30f;

    public Actor MyActor;
    public int MyActorViewID;

    public int AlivePlayerCount = 1;
    public int RoundCount = 1;
    public const int MAX_ROUND = 3;
    public const int MAX_POINTS = 3;
    public bool IsFinished = false;

    public int LoadingCompleteCount = 0;
    public int DestroyingCompleteCount = 0;
    public int EndingCount = 0;

    public bool IsMeowNyangPunch = false;
    public int winner;

    public List<Transform> Items = new List<Transform>();
    public List<int> ItemSpawnCount = new List<int>();
    public int ItemSpawnPhaseCount = 5;

    private Coroutine StartItemSpawnTimerCoroutine;

    #endregion

    

    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalGameCenterInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == _roomName)
        {
            _roomUI.UnsubscribeKeyboardEvent();
            _roomUI.OnChangeSkiilEvent -= ToggleSkillSelection;
            _roomUI.OnLeaveRoom -= LeaveRoom;
            _roomUI.OnReadyEvent -= AnnouncePlayerReady;
        }
        else if (SceneManager.GetActiveScene().name == _arenaName)
        {
            MyActor.OnChangeStaminaBar -= UpdateStaminaBar;
            _arenaSettingsUI.ReloadArenaSettingsUI();

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (Actor actor in Actors)
                {
                    actor.OnChangePlayerStatus -= SendInfo;
                    actor.OnKillPlayer -= AnnounceDeath;
                }

                _magneticField.CheckMagneticFieldArea -= CheckPlayerArea;
                _magneticField.UpdateMagneticStack -= SendPlayerMagneticStack;
                _floor.CheckFloorStack -= CheckPlayerFloor;
            }
        }
    }

    public void SetSceneBgmSound(string path)
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

                Managers.Sound.ChangeVolume(Define.Sound.Bgm, Define.Sound.UISound);
            }

            if (_roomName == currentSceneName)
            {
                _audioSources.Stop();
                AudioClip audioClip = Managers.Resource.Load<AudioClip>($"Sounds/Bgm/{path}");
                _audioSources.clip = audioClip;
                _audioSources.volume = 0.1f;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);

                Managers.Sound.ChangeVolume(Define.Sound.Bgm, Define.Sound.UISound);
            }
        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 30;
    //    GUI.backgroundColor = Color.white;
    //    for (int i = 0; i < ActorViewIDs.Count; i++)
    //    {
    //        //GUI.contentColor = Color.black;
    //        //GUI.Label(new Rect(0, 340 + i * 60, 200, 200), "Actor View ID: " + ActorViewIDs[i] + " / HP: " + Actors[i].Health, style);
    //        //GUI.contentColor = Color.red;
    //        //GUI.Label(new Rect(0, 360 + i * 60, 200, 200), "Status: " + Actors[i].actorState + " / Debuff: " + Actors[i].debuffState, style);

    //        GUI.contentColor = Color.red;
    //        GUI.Label(new Rect(0, 360 + i * 60, 200, 200), "Stack: " + Actors[i].MagneticStack, style);
    //    }
    //}

    void UpdateStaminaBar()
    {
        if (ImageStaminaBar != null)
            ImageStaminaBar.fillAmount = MyActor.StatContext.Stamina / MyActor.StatContext.MaxStamina;
    }

    #region ScoreBoard
    void SetScoreBoard()
    {
        _scoreBoardUI.ChangeScoreBoard(_scores, _nicknames, _actorNumbers);

        photonView.RPC("SyncScoreBoard", RpcTarget.Others, _scores, _nicknames, _actorNumbers);
    }

    [PunRPC]
    void SyncScoreBoard(int[] scores, string[] nicknames, int[] actorNumbers)
    {
        for (int i = 0; i < scores.Length; i++)
        {
            _scores[i] = scores[i];
            _nicknames[i] = nicknames[i];
            _actorNumbers[i] = actorNumbers[i];
        }
        
        _scoreBoardUI.ChangeScoreBoard(_scores, _nicknames, _actorNumbers);
    }

    [PunRPC]
    void InitUIInfo(string nickname, int actorNumber)
    {
        for (int i = 0; i < _scores.Length; i++)
        {
            if (i == (actorNumber - 1))
            {
                _scores[i] = 0;
                _nicknames[i] = nickname;
                _actorNumbers[i] = actorNumber;

                SetScoreBoard();
            }
        }

    }
    #endregion

    void Start()
    {
        InitRoomUI();
    }

    #region RoomUI
    void InitRoomUI()
    {
        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();
        Debug.Log(_roomUI);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            _roomUI.ReadyButton.SetActive(false);
        else
            _roomUI.PlayButton.SetActive(false);

        _roomUI.PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        _roomUI.OnChangeSkiilEvent -= ToggleSkillSelection;
        _roomUI.OnChangeSkiilEvent += ToggleSkillSelection;
        _roomUI.OnLeaveRoom -= LeaveRoom;
        _roomUI.OnLeaveRoom += LeaveRoom;
        _roomUI.OnReadyEvent -= AnnouncePlayerReady;
        _roomUI.OnReadyEvent += AnnouncePlayerReady;


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdateMasterStatus();
        }

        photonView.RPC("UpdateMasterStatus", RpcTarget.MasterClient);
        Debug.Log("roomUI PlayerCount " + _roomUI.PlayerCount);
        photonView.RPC("UpdatePlayerNumber", RpcTarget.All, _roomUI.PlayerCount);
    }

    void ToggleSkillSelection(bool isChange)
    {
        IsMeowNyangPunch = isChange;
    }

    void LeaveRoom()
    {
        photonView.RPC("UnsubscribeRoomEvent", RpcTarget.All);
    }

    [PunRPC]
    void UnsubscribeRoomEvent()
    {
        _roomUI.UnsubscribeKeyboardEvent();
        _roomUI.OnChangeSkiilEvent -= ToggleSkillSelection;
        _roomUI.OnLeaveRoom -= LeaveRoom;
        _roomUI.OnReadyEvent -= AnnouncePlayerReady;
    }

    [PunRPC]
    void UpdatePlayerNumber(int totalPlayerNumber)
    {
        Debug.Log("TotalPlayerNumber " + totalPlayerNumber);
        _roomUI.UpdatePlayerNumber(totalPlayerNumber);
    }

    [PunRPC]
    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            _roomUI.ChangeMasterButton(true);
            _roomUI.CanPlay = true;
        }
        else
        {
            _roomUI.ChangeMasterButton(false);
            _roomUI.CanPlay = false;
        }
    }

    void AnnouncePlayerReady(bool isReady)
    {
        photonView.RPC("UpdateCount", RpcTarget.MasterClient, isReady);
        photonView.RPC("UpdatePlayerReady", RpcTarget.All, _roomUI.ActorNumber, isReady);
    }

    [PunRPC]
    void UpdateCount(bool isReady)
    {
        if (isReady)
            _roomUI.PlayerReadyCount++;
        else
            _roomUI.PlayerReadyCount--;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdateMasterStatus();
        }

    }

    [PunRPC]
    void UpdatePlayerReady(int actorNumber, bool isReady)
    {
        if (isReady)
            _roomUI.SpawnPoint.transform.GetChild(actorNumber - 1).GetChild(0).gameObject.SetActive(true);
        else
            _roomUI.SpawnPoint.transform.GetChild(actorNumber - 1).GetChild(0).gameObject.SetActive(false);
    }

    #endregion

    #region 아레나 초기화

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            Debug.Log("아레나 로딩완료!!!");

            GameObject mainPanel = GameObject.Find("Main Panel");
            ImageHPBar = mainPanel.transform.GetChild(1).GetChild(1).GetComponent<Image>();
            ImageStaminaBar = mainPanel.transform.GetChild(1).GetChild(2).GetComponent<Image>();

            GameObject portrait = mainPanel.transform.GetChild(1).GetChild(0).gameObject;

            for (int i = 0; i < Define.MAX_PLAYERS_PER_ROOM; i++)
            {
                if (i == PhotonNetwork.LocalPlayer.ActorNumber - 1)
                    portrait.transform.GetChild(i).gameObject.SetActive(true);
                else
                    portrait.transform.GetChild(i).gameObject.SetActive(false);
            }

            _scoreBoardUI = GameObject.Find("ScoreBoard Panel").GetComponent<ScoreBoardUI>();
            _scoreBoardUI.InitScoreBoard();

            _arenaSettingsUI = GameObject.Find("Settings Panel").GetComponent<ArenaSettingsUI>();

            SceneType = Define.SceneType.Game;
            SetSceneBgmSound("BigBangBattleLOOPING");

            InitItems();
            Actor.LayerCnt = (int)Define.Layer.Player1;

            photonView.RPC("SendLoadingComplete", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

            //if (RoundCount == 1)
            //{
            //    InstantiatePlayer();
            //}
            //else if (RoundCount > 1 && PhotonNetwork.IsMasterClient)
            //{
            //    photonView.RPC("SendDefaultInfo", RpcTarget.All);
            //}
        }

        if(scene.name == "[6]Ending")
        {
            photonView.RPC("EndingComplete", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    void InitItems()
    {
        ItemSpawnCount = Enumerable.Repeat(0, (int)Define.SpawnableItemType.End).ToList();
        Transform items = GameObject.Find("Items").transform;
        for (int i = 0; i < items.childCount; i++)
        {
            Items.Add(items.GetChild(i));
        }

        for (int j = 0; j < items.childCount; j++)
        {
            SetItemsAllInactive(j);
        }
    }

    void SetItemsAllInactive(int itemType)
    {
        int rootItemsCount = GetRootItemsCount(itemType);
        for (int i = 0; i < rootItemsCount; i++)
        {
            Transform rootItem = Items[itemType].GetChild(i).transform;
            for (int j = 0; j < rootItem.childCount; j++)
            {
                rootItem.GetChild(j).gameObject.SetActive(false);
            }
        }
    }

    int GetRootItemsCount(int itemType)
    {
        int rootItemsCount = 0;
        switch (itemType)
        {
            case (int)Define.SpawnableItemType.TwoHanded:
                rootItemsCount = (int)Define.TwoHandedItemType.End;
                break;
            case (int)Define.SpawnableItemType.Ranged:
                rootItemsCount = (int)Define.RangedItemType.End;
                break;
            case (int)Define.SpawnableItemType.Consumable:
                rootItemsCount = (int)Define.ConsumableItemType.End;
                break;
        }

        return rootItemsCount;
    }

    [PunRPC]
    void EndingComplete(int actorNumber)
    {
        EndingCount++;
        Debug.Log("Num: " + actorNumber + " Loading Complete!!");
        Debug.Log("Current LoadingCompleteCount: " + EndingCount);

        if (EndingCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("End Game");
            photonView.RPC("InitEndingScene", RpcTarget.All);
            EndingCount = 0;
        }
    }

    [PunRPC]
    void InitEndingScene()
    {
        int max = _scores[0];

        for (int i = 0; i < _scores.Length; i++)
        {
            if (max < _scores[i]) 
                winner = i;
        }

        _endingUI = GameObject.Find("Canvas").GetComponent<EndingUI>();
        _endingUI.SetWinner(winner);
        _endingUI.WinnerName.text = _nicknames[winner];
    }

    [PunRPC]
    void SendLoadingComplete(int actorNumber)
    {
        LoadingCompleteCount++;
        Debug.Log("Num: " + actorNumber + " Loading Complete!!");
        Debug.Log("Current LoadingCompleteCount: " + LoadingCompleteCount);

        if (LoadingCompleteCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            AlivePlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            photonView.RPC("CreatePlayer", RpcTarget.All);
            StartItemSpawnTimerCoroutine = StartCoroutine(StartItemSpawnTimer());
            LoadingCompleteCount = 0;
        }
    }

    IEnumerator StartItemSpawnTimer()
    {
        for (int i = 0; i < ItemSpawnPhaseCount; i++)
        {
            SpawnItems(i);
            yield return new WaitForSeconds(ItemSpawnInterval);
        }
    }

    void SpawnItems(int phase)
    {
        switch(phase)
        {
            case 0:
                {
                    SetItemsActive((int)Define.SpawnableItemType.TwoHanded, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.TwoHanded, 1));
                    SetItemsActiveForTest(); // 정식판에선 주석
                }
                break;
            case 1:
                {
                    SetItemsActive((int)Define.SpawnableItemType.Consumable, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Consumable, 3));
                    SetItemsActive((int)Define.SpawnableItemType.Ranged, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Ranged, 2));
                }
                break;
            case 2:
                {
                    SetItemsActive((int)Define.SpawnableItemType.Consumable, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Consumable, 1));
                    SetItemsActive((int)Define.SpawnableItemType.Ranged, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Ranged, 1));
                }
                break;
            case 3:
                {
                    SetItemsActive((int)Define.SpawnableItemType.Consumable, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Consumable, 2));
                }
                break;
            case 4:
                {
                    SetItemsActive((int)Define.SpawnableItemType.TwoHanded, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.TwoHanded, 1));
                    SetItemsActive((int)Define.SpawnableItemType.Ranged, ChooseRandomItemRootTypes((int)Define.SpawnableItemType.Ranged, 1));
                }
                break;
        }
    }

    void SetItemsActiveForTest()
    {
        Transform targetItem1 = Items[1].GetChild(0).GetChild(0); // ICE
        int viewID1 = targetItem1.GetComponent<PhotonView>().ViewID;
        Vector3 spawnPoint1 = new Vector3(478.16f, 20f, 393.72f);
        photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID1, spawnPoint1);

        Transform targetItem2 = Items[1].GetChild(1).GetChild(0); // TASER
        int viewID2 = targetItem2.GetComponent<PhotonView>().ViewID;
        Vector3 spawnPoint2 = new Vector3(486.98f, 20f, 408.49f);
        photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID2, spawnPoint2);

        Transform targetItem3 = Items[2].GetChild(3).GetChild(0); // DONUTS
        int viewID3 = targetItem3.GetComponent<PhotonView>().ViewID;
        Vector3 spawnPoint3 = new Vector3(476.98f, 20f, 408.49f);
        photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID3, spawnPoint3);

        Transform targetItem4 = Items[2].GetChild(3).GetChild(1); // DONUTS
        int viewID4 = targetItem4.GetComponent<PhotonView>().ViewID;
        Vector3 spawnPoint4 = new Vector3(471.98f, 20f, 408.49f);
        photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID4, spawnPoint4);

        Transform targetItem5 = Items[2].GetChild(3).GetChild(2); // DONUTS
        int viewID5 = targetItem5.GetComponent<PhotonView>().ViewID;
        Vector3 spawnPoint5 = new Vector3(468.98f, 20f, 408.49f);
        photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID5, spawnPoint5);
    }

    void SetItemsActive(int itemType, List<int> rootTypes)
    {
        for (int i = 0; i < rootTypes.Count; i++)
        {
            Transform targetItem = null;
            Transform targetItemRoot = Items[itemType].GetChild(rootTypes[i]);
            for (int j = 0; j < targetItemRoot.childCount; j++)
            {
                targetItem = targetItemRoot.GetChild(j);
                if (!targetItem.GetComponent<Item>().IsSpawned) break;
            }

            if (targetItem != null)
            {
                int viewID = targetItem.GetComponent<PhotonView>().ViewID;
                Vector3 spawnPoint = GetItemSpawnPoints(itemType, ItemSpawnCount[itemType]);
                ItemSpawnCount[itemType]++;
                photonView.RPC("SetItemsActiveInLocal", RpcTarget.All, viewID, spawnPoint);
            }
        }
    }

    Vector3 GetItemSpawnPoints(int itemType, int spawnCount)
    {
        Vector3 spawnPoint = Vector3.zero;

        switch (itemType)
        {
            case 0:
                {
                    spawnPoint = TwoHandedItemSpawnPoints[spawnCount];
                }
                break;
            case 1:
                {
                    spawnPoint = RangedItemSpawnPoints[spawnCount];
                }
                break;
            case 2:
                {
                    spawnPoint = ConsumableItemSpawnPoints[spawnCount];
                }
                break;
        }

        return spawnPoint;
    }

    [PunRPC]
    void SetItemsActiveInLocal(int viewID, Vector3 spawnPos)
    {
        Transform item = PhotonNetwork.GetPhotonView(viewID).transform;
        item.gameObject.SetActive(true);
        Debug.Log("[NAME] " + item.name + ", [ID] " + viewID + " Spawn!!");
        item.position = spawnPos;
        item.GetComponent<Item>().IsSpawned = true;
    }

    List<int> ChooseRandomItemRootTypes(int itemType, int amount)
    {
        int rootItemsCount = GetRootItemsCount(itemType);

        List<int> rootTypes = new List<int>(amount);
        for (int i = 0; i < rootTypes.Count; i++)
        {
            rootTypes[i] = -1;
        }

        for (int j = 0; j < amount; j++)
        {
            int numPicked = UnityEngine.Random.Range(0, rootItemsCount);
            if (rootTypes.Contains(numPicked))
            {
                j--;
                continue;
            }
            else
            {
                rootTypes.Add(numPicked);
            }
        }

        return rootTypes;
    }

    [PunRPC]
    void CreatePlayer()
    {
        Debug.Log("CreatePlayer -> " + Actor.LocalPlayerInstance);
        if (Actor.LocalPlayerInstance == null)
        {
            //Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            GameObject go = null;

            string key = string.Format("_playerPath{0}", PhotonNetwork.LocalPlayer.ActorNumber);
            FieldInfo field = typeof(GameCenter).GetField(key, BindingFlags.Instance |
                                                 BindingFlags.Static |
                                                 BindingFlags.Public |
                                                 BindingFlags.NonPublic);
            string playerPath = (string)field.GetValue(this);
            go = Managers.Resource.PhotonNetworkInstantiate(playerPath, pos: PlayerSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1]);

            MyActor = go.GetComponent<Actor>();
            MyActor.OnChangeStaminaBar -= UpdateStaminaBar;
            MyActor.OnChangeStaminaBar += UpdateStaminaBar;

            PhotonView pv = go.GetComponent<PhotonView>();
            MyActorViewID = pv.ViewID;

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                ActorViewIDs.Add(MyActorViewID);
                AddActor(MyActorViewID);
            }
            else
            {
                photonView.RPC("RegisterActorInfo", RpcTarget.MasterClient, MyActorViewID);
            }
        }

        StartCoroutine(InitArenaScene());
    }

    void AddActor(int id)
    {
        PhotonView targetPV = PhotonView.Find(id);

        if (targetPV != null)
        {
            Actor actor = targetPV.transform.GetComponent<Actor>();
            Actors.Add(actor);

            actor.SetSkill(IsMeowNyangPunch);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                SubscribeActorEvent(actor);
            }
        }
    }

    void SubscribeActorEvent(Actor actor)
    {
        if (actor != null)
        {
            Debug.Log("구독 부분 " + actor.photonView.ViewID);
            actor.OnChangePlayerStatus -= SendInfo;
            actor.OnChangePlayerStatus += SendInfo;
            actor.OnKillPlayer -= AnnounceDeath;
            actor.OnKillPlayer += AnnounceDeath;
        }
    }

    [PunRPC]
    void RegisterActorInfo(int viewID)
    {
        Debug.Log("RegisterActorInfo: " + viewID);

        ActorViewIDs.Add(viewID);
        AddActor(viewID);

        for (int i = 0; i < ActorViewIDs.Count; i++)
        {
            Debug.Log(ActorViewIDs[i]);
            Debug.Log(Actors[i]);
        }

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

    IEnumerator InitArenaScene()
    {
        yield return new WaitForSeconds(5f);

        _scoreBoardUI.SetScoreBoard();

        _magneticField = GameObject.Find("Magnetic Field").GetComponent<MagneticField>();
        _floor = GameObject.Find("Trigger Floor").GetComponent<Floor>();
        _snowStorm = GameObject.Find("Snow Storm").GetComponent<SnowStorm>();


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _magneticField.CheckMagneticFieldArea -= CheckPlayerArea;
            _magneticField.CheckMagneticFieldArea += CheckPlayerArea;

            _magneticField.UpdateMagneticStack -= SendPlayerMagneticStack;
            _magneticField.UpdateMagneticStack += SendPlayerMagneticStack;

            _floor.CheckFloorStack -= CheckPlayerFloor;
            _floor.CheckFloorStack += CheckPlayerFloor;
        }



        if (RoundCount == 1)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _scores[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 0;
                _nicknames[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.NickName;
                _actorNumbers[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.LocalPlayer.ActorNumber;
            }
            else
            {
                photonView.RPC("InitUIInfo", RpcTarget.MasterClient, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
        else
        {
            _scoreBoardUI.ChangeScoreBoard(_scores, _nicknames, _actorNumbers);
        }

        _magneticField.ActorList = Actors;
        _snowStorm.ActorList = Actors;
        _arenaSettingsUI.ActorList = Actors;

        _arenaSettingsUI.SetInitSettings();
    }

    #endregion

    #region 플레이어 동기화

    void SendInfo(float hp, float stamina,  Actor.DebuffState debuffstate, int viewID)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        photonView.RPC("SyncInfo", RpcTarget.All, hp, stamina, debuffstate, viewID);
    }

    [PunRPC]
    void SyncInfo(float hp, float stamina,  Actor.DebuffState debuffstate, int viewID)
    {
        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i] == null || Actors[i].photonView == null)
                continue;

            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].StatContext.Health = hp;
                //Actors[i].actorState = actorState;
                Actors[i].debuffState = debuffstate;
                Actors[i].StatContext.Stamina = stamina;

                if (Actors[i].photonView.IsMine && ImageHPBar != null)
                {
                    ImageHPBar.fillAmount = Actors[i].StatContext.Health / Actors[i].StatContext.MaxHealth;
                }
                break;
            }
        }
    }

    void AnnounceDeath(int viewID)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        photonView.RPC("HandleDeath", RpcTarget.All, viewID);
    }

    [PunRPC]
    void HandleDeath(int viewID)
    {
        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].StatContext.IsAlive = true;

                if (Actors[i].photonView.IsMine == true)
                //if (Actors[i].photonView.ViewID == viewID && Actors[i].photonView.IsMine == true)
                {
                    Actors[i].CameraControl.Camera.GetComponent<GrayscaleEffect>().StartGrayscalseEffect();
                    photonView.RPC("ReduceAlivePlayerCount", RpcTarget.MasterClient, viewID);
                    Vector3 deadPos = Actors[i].BodyHandler.Hip.transform.position;
                    //Debug.Log("Ha: " + Actors[i].StatContext.IsAlive);
                    StartCoroutine(InstantiateGhost(deadPos));
                }
            }
        }
    }

    [PunRPC]
    void ReduceAlivePlayerCount(int viewID)
    {
        Debug.Log("[Only Master] " + viewID + " Player is Dead!");
        AlivePlayerCount--;

        if (AlivePlayerCount == 1)
            StartCoroutine(BookRoundEnd());
    }

    IEnumerator InstantiateGhost(Vector3 spawnPos)
    {
        Vector3 spawnAirPos = spawnPos + new Vector3(0f, 10f, 0f);
        MyGraveStone = Managers.Resource.PhotonNetworkInstantiate(_graveStonePath, pos: spawnAirPos);
        yield return new WaitForSeconds(GhostSpawnDelay);
        MyGhost = Managers.Resource.Instantiate(_ghostPath, pos: spawnPos);
        Destroy(MyActor.AudioListener);
        MyActor.CameraControl = null;
    }

    #endregion


    #region 자기장 스택 체크
    void CheckPlayerArea(int[] areaName, int actorNum, bool[] isInside)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _checkAreaName[actorNum] = areaName[actorNum];
            _checkInside[actorNum] = isInside[actorNum];

            DamageByMagneticField(actorNum);

            photonView.RPC("SendInsideCheck", RpcTarget.All, _checkInside);
        }
    }
    
    void CheckPlayerFloor(int[] areaName, int actorNum)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _checkAreaName[actorNum] = areaName[actorNum];
            DamageByMagneticField(actorNum);
        }
    }

    void DamageByMagneticField(int index)
    {
        if (_checkAreaName[index] == (int)Define.Area.Floor)
        {
            StartCoroutine(_magneticField.DamagedByFloor(index));
        }
        else if (_checkAreaName[index] == (int)Define.Area.Inside)
        {
            StartCoroutine(_magneticField.RestoreMagneticDamage(index));
        }
        else if (_checkAreaName[index] == (int)Define.Area.Outside)
        {
            StartCoroutine(_magneticField.DamagedByMagnetic(index));
        }
    }

    
    void SendPlayerMagneticStack(int[] magneticStack)
    {
        _magneticField.ChangeMainPanel();

        photonView.RPC("SyncPlayerMagneticStack", RpcTarget.Others, magneticStack);
    }

    [PunRPC]
    void SyncPlayerMagneticStack(int[] magneticStack)
    {
        for (int i = 0; i < Actors.Count; i++)
        {
            Actors[i].StatContext.MagneticStack = magneticStack[i];
        }

        _magneticField.ChangeMainPanel();
    }

    [PunRPC]
    void SendInsideCheck(bool[] inside)
    {
        _magneticField.IsInside = inside;
    }

    #endregion

    #region 라운드 종료

    IEnumerator BookRoundEnd()
    {
        Debug.Log(RoundEndDelay + "초 뒤 라운드 종료 예정");
        photonView.RPC("FindWinner", RpcTarget.All);
        StopCoroutine(StartItemSpawnTimerCoroutine);
        yield return new WaitForSeconds(RoundEndDelay);

        Debug.Log("라운드 종료 오브젝트 삭제");
        photonView.RPC("DestroyObjects", RpcTarget.All);

    }

    [PunRPC]
    void FindWinner()
    {
        if (MyActor.StatContext.IsAlive)
        {
            photonView.RPC("GiveScoreToWinner", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    void GiveScoreToWinner(int ActorNum)
    {
        for (int i = 0; i < _actorNumbers.Length; i++)
        {
            Debug.Log(_actorNumbers[i]);
            if (_actorNumbers[i] == ActorNum)
            {
                Debug.Log("승자: " + _actorNumbers[i]);
                _scores[i]++;
                if (_scores[i] == MAX_POINTS)
                {
                    IsFinished = true;
                }
            }
        }

        SetScoreBoard();
        photonView.RPC("FixScoreBoard", RpcTarget.All);
        photonView.RPC("UnsubscribeSettings", RpcTarget.All);
    }

    [PunRPC]
    void UnsubscribeSettings()
    {
        _arenaSettingsUI.ReloadArenaSettingsUI();
    }

    [PunRPC]
    void FixScoreBoard()
    {
        Debug.Log("스코어보드 상시 출력 변경");
        _scoreBoardUI.DisplayFixedScoreBoard();
    }

    [PunRPC]
    void DestroyObjects()
    {
        if (MyGhost != null)
        {
            MyGhost = null;
            Debug.Log("비석 삭제");
            Managers.Resource.Destroy(MyGraveStone);
            MyGraveStone = null;
        }

        Debug.Log("플레이어 삭제");
        MyActor.OnChangeStaminaBar -= UpdateStaminaBar;
        Managers.Resource.Destroy(MyActor.gameObject);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("마스터 구독 취소");
            foreach (Actor actor in Actors)
            {
                actor.OnChangePlayerStatus -= SendInfo;
                actor.OnKillPlayer -= AnnounceDeath;
            }

            _magneticField.CheckMagneticFieldArea -= CheckPlayerArea;
            _magneticField.UpdateMagneticStack -= SendPlayerMagneticStack;
            _floor.CheckFloorStack -= CheckPlayerFloor;
        }
        ClearList();
        RoundCount++;

        photonView.RPC("SendDestroyingComplete", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    void ClearList()
    {
        Debug.Log("리스트 초기화");
        ActorViewIDs.Clear();
        Actors.Clear();
        Items.Clear();
        ItemSpawnCount.Clear();
    }

    [PunRPC]
    void SendDestroyingComplete(int actorNumber)
    {
        DestroyingCompleteCount++;
        Debug.Log("Num: " + actorNumber + " Destroying Complete!!");
        Debug.Log("Current DestroyingCompleteCount: " + DestroyingCompleteCount);

        if (DestroyingCompleteCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Round 찐 종료");
            DestroyingCompleteCount = 0;
            //if (RoundCount == MAX_ROUND)
            if (IsFinished)
            {
                photonView.RPC("QuitRoom", RpcTarget.All);
            }
            else
            {
                photonView.RPC("ReloadSameScene", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void QuitRoom()
    {
        SceneManager.LoadScene("[6]Ending");
    }

    [PunRPC]
    void ReloadSameScene()
    {
        SceneManager.LoadScene(_arenaName);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Methods

    public override void Clear()
    {
    }

    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion
}
