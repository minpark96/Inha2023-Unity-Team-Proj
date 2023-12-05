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
using System.Runtime.CompilerServices;

public class GameCenter : BaseScene
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;
    ScoreBoardUI _scoreBoardUI;
    [SerializeField]
    EndingUI _endingUI;

    //MagneticField _magneticField;
    //SnowStorm _snowStorm;
    #endregion

    #region Private Fields

    string _roomName = "[4]Room";
    string _arenaName = "PO_Map_KYH";

    string _playerPath1 = "Players/Player1";
    string _playerPath2 = "Players/Player2";
    string _playerPath3 = "Players/Player3";
    string _playerPath4 = "Players/Player4";
    string _playerPath5 = "Players/Player5";
    string _playerPath6 = "Players/Player6";

    string _ghostPath = "Players/Ghost";
    string _graveStonePath = "Item/GraveStone";

    bool _isChecked;

    public int[] _scores = new int[Define.MAX_PLAYERS_PER_ROOM] { 0, 0, 0, 0, 0, 0 };
    public string[] _nicknames = new string[Define.MAX_PLAYERS_PER_ROOM] { "", "", "", "", "", "" };
    public int[] _actorNumbers = new int[Define.MAX_PLAYERS_PER_ROOM] { 0, 0, 0, 0, 0, 0 };

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    public float SpawnPointX = 485f;
    public float SpawnPointY = 12f;
    public float SpawnPointZ = 411f;

    // 스폰 포인트 6인 기준
    public List<Vector3> SpawnPoints = new List<Vector3>();

    public List<int> ActorViewIDs = new List<int>();
    public List<Actor> Actors = new List<Actor>();

    public GameObject MyGraveStone = null;
    public GameObject MyGhost = null;

    // Arena UI
    public Image ImageHPBar;
    public Image ImageStaminaBar;
    public Image Portrait;

    public float DelayInGhostSpawn = 4f;
    public float DelayInRoundEnd = 7f;
    public float DelayInUpdateScoreBoard = 2f;
    public float DelayInDestroyObjects = 2f;

    public Actor MyActor;
    public int MyActorViewID;

    //public Vector3[] DefaultPos = new Vector3[17];
    //public Quaternion[] DefaultRot = new Quaternion[17];

    public int AlivePlayerCount = 1;
    public int RoundCount = 1;
    public const int MAX_ROUND = 2;

    public int LoadingCompleteCount = 0;
    public int DestroyingCompleteCount = 0;
    public int EndingCount = 0;

    public bool IsMeowNyangPunch = false;
    public int winner;

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

        SpawnPoints.Add(new Vector3(0f, 0f, 0f));


        if (photonView.IsMine)
        {
            LocalGameCenterInstance = this.gameObject;
        }

        DontDestroyOnLoad(this.gameObject);
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

                Managers.Sound.ChangeVolume();
            }

            if (_roomName == currentSceneName)
            {
                _audioSources.Stop();
                AudioClip audioClip = Managers.Resource.Load<AudioClip>($"Sounds/Bgm/{path}");
                _audioSources.clip = audioClip;
                _audioSources.volume = 0.1f;
                Managers.Sound.Play(audioClip, Define.Sound.Bgm);

                Managers.Sound.ChangeVolume();
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
    //        GUI.contentColor = Color.black;
    //        GUI.Label(new Rect(0, 340 + i * 60, 200, 200), "Actor View ID: " + ActorViewIDs[i] + " / HP: " + Actors[i].Health, style);
    //        GUI.contentColor = Color.red;
    //        GUI.Label(new Rect(0, 360 + i * 60, 200, 200), "Status: " + Actors[i].actorState + " / Debuff: " + Actors[i].debuffState, style);
    //    }
    //}

    void SetScoreBoard()
    {
        _scoreBoardUI.ChangeScoreBoard(_scores, _nicknames, _actorNumbers);

        photonView.RPC("SyncScoreBoard", RpcTarget.Others, _scores, _nicknames, _actorNumbers);
    }

    void UpdateStaminaBar()
    {
        if (ImageStaminaBar != null)
            ImageStaminaBar.fillAmount = MyActor.Stamina / MyActor.MaxStamina;
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
            if (i == actorNumber - 1)
            {
                _scores[i] = 0;
                _nicknames[i] = nickname;
                _actorNumbers[i] = actorNumber;
            }
        }

        SetScoreBoard();
    }

    void Start()
    {
        InitRoomUI();
    }

    void InitRoomUI()
    {
        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();

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
        _roomUI.OnChangeSkiilEvent -= ToggleSkillSelection;
        _roomUI.OnLeaveRoom -= LeaveRoom;
        _roomUI.OnReadyEvent -= AnnouncePlayerReady;
    }

    //void Update()
    //{
    //    if (SceneManager.GetActiveScene().name == _roomName)
    //    {
    //        if (PhotonNetwork.LocalPlayer.IsMasterClient)
    //        {
    //            UpdateMasterStatus();
    //        }
    //        else
    //        {
    //            if (_roomUI.Ready == true)
    //            {
    //                if (_isChecked == false)
    //                {
    //                    AnnouncePlayerReady();
    //                    _isChecked = true;
    //                }

    //                photonView.RPC("UpdatePlayerReady", RpcTarget.All, _roomUI.ActorNumber, true);
    //            }
    //            else
    //            {
    //                if (_isChecked == true)
    //                {
    //                    AnnouncePlayerReady();
    //                    _isChecked = false;
    //                }

    //                photonView.RPC("UpdatePlayerReady", RpcTarget.All, _roomUI.ActorNumber, false);
    //            }
    //        }
    //    }
    //}

    [PunRPC]
    void UpdatePlayerReady(int actorNumber, bool isReady)
    {
        if (isReady)
            _roomUI.SpawnPoint.transform.GetChild(actorNumber - 1).GetChild(0).gameObject.SetActive(true);
        else
            _roomUI.SpawnPoint.transform.GetChild(actorNumber - 1).GetChild(0).gameObject.SetActive(false);
    }

    [PunRPC]
    void UpdatePlayerNumber(int totalPlayerNumber)
    {
        _roomUI.UpdatePlayerNumber(totalPlayerNumber);
    }

    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            _roomUI.ChangeMasterButton(true);
        else
            _roomUI.ChangeMasterButton(false);
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

    #endregion

    #region 아레나 초기화

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            Debug.Log("아레나 로딩완료!!!");

            GameObject mainPanel = GameObject.Find("Main Panel");
            ImageHPBar = mainPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            ImageStaminaBar = mainPanel.transform.GetChild(0).GetChild(2).GetComponent<Image>();

            GameObject portrait = mainPanel.transform.GetChild(0).GetChild(0).gameObject;

            for (int i = 0; i < Define.MAX_PLAYERS_PER_ROOM; i++)
            {
                if (i == PhotonNetwork.LocalPlayer.ActorNumber - 1)
                    portrait.transform.GetChild(i).gameObject.SetActive(true);
                else
                    portrait.transform.GetChild(i).gameObject.SetActive(false);
            }

            _scoreBoardUI = GameObject.Find("ScoreBoard Panel").GetComponent<ScoreBoardUI>();
            if (_scoreBoardUI == null)
                Debug.Log("스코어보드 null");
            _scoreBoardUI.InitScoreBoard();

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
        int max1 = _scores[0];

        for (int i = 0; i < _scores.Length; i++)
        {
            int max = _scores[i];

            if (max1 < max) winner = i;
        }

        _endingUI = GameObject.Find("Canvas").GetComponent<EndingUI>();
        _endingUI.SetWinner(winner);
    }

    [PunRPC]
    void SendLoadingComplete(int actorNumber)
    {
        LoadingCompleteCount++;
        Debug.Log("Num: " + actorNumber + " Loading Complete!!");
        Debug.Log("Current LoadingCompleteCount: " + LoadingCompleteCount);

        if (LoadingCompleteCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Start Game");
            AlivePlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            photonView.RPC("CreatePlayer", RpcTarget.All);
            LoadingCompleteCount = 0;
        }
    }

    [PunRPC]
    void CreatePlayer()
    {
        Debug.Log("CreatePlayer -> " + Actor.LocalPlayerInstance);
        if (Actor.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            GameObject go = null;

            string key = string.Format("_playerPath{0}", PhotonNetwork.LocalPlayer.ActorNumber);
            FieldInfo field = typeof(GameCenter).GetField(key, BindingFlags.Instance |
                                                 BindingFlags.Static |
                                                 BindingFlags.Public |
                                                 BindingFlags.NonPublic);
            string playerPath = (string)field.GetValue(this);
            go = Managers.Resource.PhotonNetworkInstantiate(playerPath, pos: SpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1]);

            Debug.Log(go);
            MyActor = go.GetComponent<Actor>();
            Debug.Log(MyActor);
            MyActor.OnChangeStaminaBar -= UpdateStaminaBar;
            MyActor.OnChangeStaminaBar += UpdateStaminaBar;

            PhotonView pv = go.GetComponent<PhotonView>();
            Debug.Log(pv);
            MyActorViewID = pv.ViewID;
            Debug.Log(MyActorViewID);

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

            if (IsMeowNyangPunch)
                actor.PlayerController.isMeowNyangPunch = true;
            else
                actor.PlayerController.isMeowNyangPunch = false;

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
        Debug.Log("여기까지 오셨을까요?");

        for (int i = ActorViewIDs.Count; i < ids.Length; i++)
        {
            ActorViewIDs.Add(ids[i]);
            AddActor(ids[i]);
        }

        //if (_magneticField.Actor == null)
        //    _magneticField.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        //if (_snowStorm.Actor == null)
            //_snowStorm.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
    }

    IEnumerator InitArenaScene()
    {
        yield return new WaitForSeconds(5f);

        SceneType = Define.Scene.Game;
        SetSceneBgmSound("BigBangBattleLOOPING");

        Debug.Log("InitArenaScene");
        _scoreBoardUI.SetScoreBoard();

        //_magneticField = GameObject.Find("Magnetic Field").GetComponent<MagneticField>();
        //_snowStorm = GameObject.Find("Snow Storm").GetComponent<SnowStorm>();

        if (RoundCount == 1)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                _scores[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 0;
                _nicknames[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.NickName;
                _actorNumbers[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.LocalPlayer.ActorNumber;

                //_magneticField.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
                //_snowStorm.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
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
    }

    #endregion

    #region 플레이어 동기화

    void SendInfo(float hp, Actor.ActorState actorState, Actor.DebuffState debuffstate, int viewID)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        photonView.RPC("SyncInfo", RpcTarget.All, hp, actorState, debuffstate, viewID);
    }

    [PunRPC]
    void SyncInfo(float hp, Actor.ActorState actorState, Actor.DebuffState debuffstate, int viewID)
    {
        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].Health = hp;
                Actors[i].actorState = actorState;
                Actors[i].debuffState = debuffstate;

                if (Actors[i].photonView.IsMine && ImageHPBar != null)
                {
                    ImageHPBar.fillAmount = Actors[i].Health / Actors[i].MaxHealth;
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
            if (Actors[i].photonView.ViewID == viewID && Actors[i].photonView.IsMine == true)
            {
                Actors[i].CameraControl.Camera.GetComponent<GrayscaleEffect>().StartGrayscalseEffect();
                photonView.RPC("ReduceAlivePlayerCount", RpcTarget.MasterClient, viewID);
                Vector3 deadPos = Actors[i].BodyHandler.Hip.transform.position;
                Debug.Log("HandleDeath: " + Actors[i].actorState);
                StartCoroutine(InstantiateGhost(deadPos));
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
        if (Ghost.LocalGhostInstance == null)
        {
            Vector3 spawnAirPos = spawnPos + new Vector3(0f, 10f, 0f);
            MyGraveStone = Managers.Resource.PhotonNetworkInstantiate(_graveStonePath, pos: spawnAirPos);
            yield return new WaitForSeconds(DelayInGhostSpawn);
            MyGhost = Managers.Resource.PhotonNetworkInstantiate(_ghostPath, pos: spawnPos);
            MyActor.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    #endregion

    #region 라운드 종료

    IEnumerator BookRoundEnd()
    {
        Debug.Log(DelayInRoundEnd + "초 뒤 라운드 종료 예정");
        photonView.RPC("FindWinner", RpcTarget.All);
        yield return new WaitForSeconds(DelayInRoundEnd);

        Debug.Log("라운드 종료 오브젝트 삭제");
        photonView.RPC("DestroyObjects", RpcTarget.All);

    }

    [PunRPC]
    void FindWinner()
    {
        if (MyActor.actorState != ActorState.Dead)
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
            }
        }

        SetScoreBoard();
        photonView.RPC("FixScoreBoard", RpcTarget.All);
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
            Debug.Log("고스트 삭제");
            Managers.Resource.Destroy(MyGhost);
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
        }

        Debug.Log("리스트 초기화");
        ActorViewIDs.Clear();
        Actors.Clear();

        RoundCount++;

        photonView.RPC("SendDestroyingComplete", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
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
            if (RoundCount == MAX_ROUND)
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
