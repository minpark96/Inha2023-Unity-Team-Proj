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
    ScoreBoardUI _scoreBoardUI;

    MagneticField _magneticField;
    SnowStorm _snowStorm;
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
    bool _isDelayed;

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
    public Image ImageStaminusBar;
    public Image Portrait;

    public float GhostSpawnDelay = 4f;
    public float RoundEndDelay = 7f;
    public float UpdateScoreBoardDelay = 2f;
    public float DeleteDelay = 2f;

    public Actor MyActor;
    public int MyActorViewID;

    //public Vector3[] DefaultPos = new Vector3[17];
    //public Quaternion[] DefaultRot = new Quaternion[17];

    public int AlivePlayerCounts = 1;
    public int RoundCounts = 1;
    public const int MAX_ROUND = 3;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _arenaName)
        {
            Debug.Log("아레나 로딩완료!!!");
            AlivePlayerCounts = PhotonNetwork.CurrentRoom.PlayerCount;

            StartCoroutine(InstantiatePlayer());

            //if (RoundCounts == 1)
            //{
            //    InstantiatePlayer();
            //}
            //else if (RoundCounts > 1 && PhotonNetwork.IsMasterClient)
            //{
            //    photonView.RPC("SendDefaultInfo", RpcTarget.All);
            //}

            
        }
    }

    IEnumerator InitArenaScene()
    {
        yield return new WaitForSeconds(2f);

        SceneType = Define.Scene.Game;
        SetSceneBgmSound("BigBangBattleLOOPING");

        GameObject mainPanel = GameObject.Find("Main Panel");
        ImageHPBar = mainPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        ImageStaminusBar = mainPanel.transform.GetChild(0).GetChild(2).GetComponent<Image>();

        GameObject portrait = mainPanel.transform.GetChild(0).GetChild(0).gameObject;

        for (int i = 0; i < Define.MAX_PLAYERS_PER_ROOM; i++)
        {
            if (i == PhotonNetwork.LocalPlayer.ActorNumber - 1)
                portrait.transform.GetChild(i).gameObject.SetActive(true);
            else
                portrait.transform.GetChild(i).gameObject.SetActive(false);
        }

        _scoreBoardUI = GameObject.Find("ScoreBoard Panel").GetComponent<ScoreBoardUI>();
        _scoreBoardUI.SetScoreBoard();
        _scoreBoardUI.isSetup = true;

        _magneticField = GameObject.Find("Magnetic Field").GetComponent<MagneticField>();
        _snowStorm = GameObject.Find("Snow Storm").GetComponent<SnowStorm>();

        if (RoundCounts == 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _scores[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 0;
                _nicknames[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.NickName;
                _actorNumbers[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.LocalPlayer.ActorNumber;

                _magneticField.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
                _snowStorm.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
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

        //if (!_isDelayed)
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        _isDelayed = true;
        //        StartCoroutine(UpdateScoreBoardWithDelay());
        //    }
        //}
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

    IEnumerator InstantiatePlayer()
    {
        yield return new WaitForSeconds(3f);

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
            MyActor = go.GetComponent<Actor>();
            //SaveDefaultInfo(go);

            PhotonView pv = go.GetComponent<PhotonView>();
            MyActorViewID = pv.ViewID;

            if (PhotonNetwork.IsMasterClient)
            {
                ActorViewIDs.Add(MyActorViewID);
                AddActor(MyActorViewID);
            }
            else
            {
                Debug.Log("RegisterActorInfo 보냄");
                photonView.RPC("RegisterActorInfo", RpcTarget.MasterClient, MyActorViewID);
            }
        }

        StartCoroutine(InitArenaScene());
    }

    void SaveDefaultInfo(GameObject go)
    {
        Actor actor = go.GetComponent<Actor>();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            //DefaultPos[i] = actor.BodyHandler.BodyParts[i].transform.localPosition;

            //DefaultRot[i] = actor.BodyHandler.BodyParts[i].transform.localRotation;

        }
    }

    IEnumerator InstantiateGhost(Vector3 spawnPos)
    {
        if (Ghost.LocalGhostInstance == null)
        {
            Vector3 spawnAirPos = spawnPos + new Vector3(0f, 10f, 0f);
            MyGraveStone = Managers.Resource.PhotonNetworkInstantiate(_graveStonePath, pos: spawnAirPos);
            yield return new WaitForSeconds(GhostSpawnDelay);
            MyGhost = Managers.Resource.PhotonNetworkInstantiate(_ghostPath, pos: spawnPos);
            MyActor.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        style.fontSize = 30;

        GUI.backgroundColor = Color.white;
        for (int i = 0; i < ActorViewIDs.Count; i++)
        {
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(0, 340 + i * 60, 200, 200), "Actor View ID: " + ActorViewIDs[i] + " / HP: " + Actors[i].Health, style);
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 360 + i * 60, 200, 200), "Status: " + Actors[i].actorState + " / Debuff: " + Actors[i].debuffState, style);
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

    void AnnounceDeath(int viewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

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
                photonView.RPC("ReduceAlivePlayerCounts", RpcTarget.MasterClient, viewID);
                Vector3 deadPos = Actors[i].BodyHandler.Hip.transform.position;
                Debug.Log("HandleDeath: " + Actors[i].actorState);
                StartCoroutine(InstantiateGhost(deadPos));
            }
        }
    }

    [PunRPC]
    void ReduceAlivePlayerCounts(int viewID)
    {
        Debug.Log("[Only Master] " + viewID + " Player is Dead!");
        AlivePlayerCounts--;

        if (AlivePlayerCounts == 1)
            StartCoroutine(EndRound());
    }

    IEnumerator EndRound()
    {
        Debug.Log(RoundEndDelay + "초 뒤 라운드 종료 예정");
        photonView.RPC("FindWinner", RpcTarget.All);
        yield return new WaitForSeconds(RoundEndDelay);
        
        Debug.Log("라운드 종료 오브젝트 삭제");
        photonView.RPC("DestroyObjects", RpcTarget.All);
        yield return new WaitForSeconds(DeleteDelay);

        RoundCounts++;

        if (RoundCounts == MAX_ROUND)
        {
            photonView.RPC("QuitRoom", RpcTarget.All);
        }
        else
        {
            photonView.RPC("ReloadSameScene", RpcTarget.All);
        }
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

    void SetScoreBoard()
    {
        _scoreBoardUI.ChangeScoreBoard(_scores, _nicknames, _actorNumbers);

        photonView.RPC("SyncScoreBoard", RpcTarget.Others, _scores, _nicknames, _actorNumbers);
    }

    [PunRPC]
    void QuitRoom()
    {
        // 여기 수정하세요
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("[2]Main");
        PhotonManager.Instance.Connect();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    [PunRPC]
    void ReloadSameScene()
    {
        SceneManager.LoadScene(_arenaName);
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
        Managers.Resource.Destroy(MyActor.gameObject);
        Debug.Log("리스트 초기화");
        ActorViewIDs.Clear();
        Actors.Clear();
    }

    [PunRPC]
    void DestroyGhost()
    {
        if (MyGhost != null)
        {
            if (MyActor != null)
            {
                Debug.Log("플레이어 카메라 복원");
                MyActor.transform.GetChild(0).gameObject.SetActive(true);
                MyGhost.transform.GetChild(0).gameObject.SetActive(false);
            }

            Debug.Log("고스트 삭제");
            Managers.Resource.Destroy(MyGhost);
            MyGhost = null;
            Debug.Log("비석 삭제");
            Managers.Resource.Destroy(MyGraveStone);
            MyGraveStone = null;
        }
    }

    [PunRPC]
    void SendDefaultInfo()
    {
        Debug.Log("디폴트 값 전송!!");
        MyActor.transform.GetChild(0).localPosition = new Vector3(0f, -0.67f, 0f);

        float[] w = new float[17];
        float[] x = new float[17];
        float[] y = new float[17];
        float[] z = new float[17];

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            //w[i] = DefaultRot[i].w;
            //x[i] = DefaultRot[i].x;
            //y[i] = DefaultRot[i].y;
            //z[i] = DefaultRot[i].z;
        }

        //photonView.RPC("ResetPlayer", RpcTarget.All, MyActorViewID, DefaultPos, w, x, y, z);
    }

    [PunRPC]
    void ResetPlayer(int viewID, Vector3[] defaultPos, float[] w, float[] x, float[] y, float[] z)
    {
        Debug.Log(viewID + " 플레이어 리셋!!");

        Quaternion[] defaultRot = new Quaternion[w.Length];
        for (int i = 0; i < w.Length; i++)
        {
            defaultRot[i] = new Quaternion(w[i], x[i], y[i], z[i]);
        }

        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].StatusHandler.invulnerable = true;
                Actors[i].debuffState = DebuffState.Default;
                Actors[i].actorState = ActorState.Stand;
                Actors[i].Health = 200f;

                for (int j = 0; j < MyActor.BodyHandler.BodyParts.Count; j++)
                {
                    Actors[i].BodyHandler.BodyParts[j].transform.localPosition = defaultPos[j];
                    Actors[i].BodyHandler.BodyParts[j].transform.localRotation = defaultRot[j];
                }
                Actors[i].StatusHandler.invulnerable = false;
            }
        }
    }

    void SendInfo(float hp, float stamina, Actor.ActorState actorState, Actor.DebuffState debuffstate, int viewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("SyncInfo", RpcTarget.All, hp, actorState, debuffstate, viewID);
    }

    [PunRPC]
    void SyncInfo(float hp, Actor.ActorState actorState, Actor.DebuffState debuffstate, int viewID)
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
        ActorViewIDs.Add(viewID);
        AddActor(viewID);

        Debug.Log("SyncActorsList 보냄");
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

        if (_magneticField.Actor == null)
            _magneticField.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        if(_snowStorm.Actor == null)
            _snowStorm.Actor = Actors[PhotonNetwork.LocalPlayer.ActorNumber - 1];

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

        if (PhotonNetwork.IsMasterClient)
            _roomUI.ReadyButton.SetActive(false);
        else
            _roomUI.PlayButton.SetActive(false);



    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == _roomName)
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

        if (SceneManager.GetActiveScene().name == _arenaName)
        {
            for (int i = 0; i < Actors.Count; i++)
            {
                if (Actors[i].photonView.IsMine && ImageHPBar != null)
                {
                    ImageHPBar.fillAmount = Actors[i].Health / Actors[i].MaxHealth;
                    ImageStaminusBar.fillAmount = Actors[i].Stamina / Actors[i].MaxStamina;
                }
            }
        }
    }

    IEnumerator UpdateScoreBoardWithDelay()
    {
        yield return new WaitForSeconds(UpdateScoreBoardDelay);
        SetScoreBoard();
        _isDelayed = false;
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
