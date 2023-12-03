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
    public GameObject MyGraveStone = null;
    public GameObject MyGhost = null;

    // Arena UI
    public Image ImageHPBar;
    public Image ImageStaminusBar;
    public Image Portrait;


    public float GhostSpawnDelay = 4f;
    public float RoundEndDelay = 7f;

    public Actor MyActor;
    public int MyActorViewID;

    public Vector3[] DefaultPos = new Vector3[17];
    public Quaternion[] DefaultRot = new Quaternion[17];

    public int RoundCounts = 1;

    public struct Ranking
    {
        public int score;
        public string nickName;
        public int rank;
    };
    public List<Ranking> Rank = new List<Ranking>();

    private int[] _rankScore = new int[6] { 0,0,0,0,0,0};
    private string[] _rankNickName = new string[6] { "","","","","","" };
    private int[] _rankRank = new int[6] { 0, 0, 0, 0, 0, 0 };

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
            
            InstantiatePlayer();
            
            //if (RoundCounts == 1)
            //{
            //InstantiatePlayer();
            //}
            //else if (RoundCounts > 1 && PhotonNetwork.IsMasterClient)
            //{
            //    photonView.RPC("SendDefaultInfo", RpcTarget.All);
            //}

            SceneType = Define.Scene.Game;
            SceneBgmSound("BigBangBattleLOOPING");


            GameObject mainPanel = GameObject.Find("Main Panel");
            ImageHPBar = mainPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            ImageStaminusBar = mainPanel.transform.GetChild(0).GetChild(2).GetComponent<Image>();

            GameObject portrait = mainPanel.transform.GetChild(0).GetChild(0).gameObject;

            for (int i = 1; i <= 6; i++)
            {
                if (i == PhotonNetwork.LocalPlayer.ActorNumber)
                    portrait.transform.GetChild(i - 1).gameObject.SetActive(true);
                else
                    portrait.transform.GetChild(i - 1).gameObject.SetActive(false);
            }

            _scoreBoardUI = GameObject.Find("ScoreBoard Panel").GetComponent<ScoreBoardUI>();
            _scoreBoardUI.ScoreBoardSetup();
            _scoreBoardUI.isSetup = true;

            
            if (PhotonNetwork.IsMasterClient)
            {
                _rankScore[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 0;
                _rankNickName[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.NickName;
                _rankRank[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.LocalPlayer.ActorNumber;
            }
            else
            {
                Debug.Log("client PhotonNetwork.NickName : " + PhotonNetwork.NickName);
                Debug.Log("client PhotonNetwork.LocalPlayer.ActorNumber : " + PhotonNetwork.LocalPlayer.ActorNumber);

                _rankScore[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 0;
                _rankNickName[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.NickName;
                _rankRank[PhotonNetwork.LocalPlayer.ActorNumber - 1] = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("AddUIInfoToMaster", RpcTarget.MasterClient, _rankScore, _rankNickName, _rankRank);
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
                photonView.RPC("RegisterActorInfo", RpcTarget.MasterClient, MyActorViewID);
            }
        }
    }

    void SaveDefaultInfo(GameObject go)
    {
        Actor actor = go.GetComponent<Actor>();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            DefaultPos[i] = actor.BodyHandler.BodyParts[i].transform.localPosition;
            
            DefaultRot[i] = actor.BodyHandler.BodyParts[i].transform.localRotation;
            
        }
    }

    IEnumerator InitiateGhost(Vector3 spawnPos)
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
            GUI.Label(new Rect(0, 140 + i * 60, 200, 200), "Actor View ID: " + ActorViewIDs[i] + " / HP: " + Actors[i].Health, style);
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, 160 + i * 60, 200, 200), "Status: " + Actors[i].actorState + " / Debuff: " + Actors[i].debuffState, style);
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
                StartCoroutine(InitiateGhost(deadPos));
            }
        }
    }

    [PunRPC]
    void ReduceAlivePlayerCounts(int viewID)
    {
        Debug.Log("[Only Master] " + viewID + " Player is Dead!");

        AlivePlayerCounts--;

        if (AlivePlayerCounts == 1)
            StartCoroutine(EndRound(RoundEndDelay));
    }

    IEnumerator EndRound(float time)
    {
        Debug.Log(time + "초 뒤 라운드 종료 예정");
        yield return new WaitForSeconds(time);
        Debug.Log("라운드 종료");
        //photonView.RPC("DestroyGhost", RpcTarget.All);
        photonView.RPC("DestroyObjects", RpcTarget.All);
        yield return new WaitForSeconds(time);
        RoundCounts++;
        PhotonNetwork.LoadLevel(_arenaName);
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
                MyGhost.transform.GetChild(0).gameObject.SetActive(false);
                MyActor.transform.GetChild(0).gameObject.SetActive(true);
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
            w[i] = DefaultRot[i].w;
            x[i] = DefaultRot[i].x;
            y[i] = DefaultRot[i].y;
            z[i] = DefaultRot[i].z;
        }

        photonView.RPC("ResetPlayer", RpcTarget.All, MyActorViewID, DefaultPos, w, x, y, z);
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

    [PunRPC]
    void UpdateScoreBoard(int[] score, string[] name, int[] rank)
    {

        for (int i = 0; i < score.Length; i++)
        {
            _rankScore[i] = score[i];
            _rankNickName[i] = name[i];
            _rankRank[i] = rank[i];
        }

        if(!PhotonNetwork.IsMasterClient)
            _scoreBoardUI.ChangeScoreBoard(_rankScore, _rankNickName, _rankRank);
    }

    [PunRPC]
    void AddUIInfoToMaster(int[] score, string[] name, int[] rank)
    {
        for (int i = 0; i < score.Length; i++)
        {
            if (i == PhotonNetwork.LocalPlayer.ActorNumber-1)
            {
                score[i] = 0;
                name[i] = PhotonNetwork.NickName;
                rank[i] = PhotonNetwork.LocalPlayer.ActorNumber;
            }
        }

        photonView.RPC("UpdateScoreBoard", RpcTarget.MasterClient, score, name, rank);
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
                if (Actors[i].photonView.IsMine)
                {
                    ImageHPBar.fillAmount = Actors[i].Health / Actors[i].MaxHealth;
                    ImageStaminusBar.fillAmount = Actors[i].Stamina / Actors[i].MaxStamina;
                }
            }

            if (!_isDelayed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    _isDelayed = true;
                    StartCoroutine(GetDelayTime());

                    _scoreBoardUI.ChangeScoreBoard(_rankScore, _rankNickName, _rankRank);

                    photonView.RPC("UpdateScoreBoard", RpcTarget.Others, _rankScore, _rankNickName, _rankRank);
                }
            }


        }
    }

    IEnumerator GetDelayTime()
    {
        yield return new WaitForSeconds(5.0f);
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
