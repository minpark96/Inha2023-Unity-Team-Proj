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

    string _roomName = "[4]Room";

    #region Private Fields

    string _arenaName = "PO_Map_KYH";

    string _playerPath1 = "Players/Player1";
    string _playerPath2 = "Players/Player2";
    string _playerPath3 = "Players/Player3";
    string _playerPath4 = "Players/Player4";
    string _playerPath5 = "Players/Player5";
    string _playerPath6 = "Players/Player6";

    string _ghostPath = "Spook";


    bool _isChecked;
    bool _isDelayed;

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

    // Arena UI
    public Image ImageHPBar;
    public Image ImageStaminusBar;
    public Image Portrait;

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
            //Debug.Log("구독 부분 " + actor.photonView.ViewID);
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
        //Debug.Log("마스터: RegisterActorInfo");
        //Debug.Log(viewID);

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

    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void Clear()
    {
    }
    #endregion
}