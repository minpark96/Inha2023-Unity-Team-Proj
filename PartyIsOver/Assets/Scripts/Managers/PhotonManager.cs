using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class PhotonManager : BaseScene 
{
    #region Private Serializable Fields

    #endregion

    #region Private Fields

    static PhotonManager p_instance;

    const byte MAX_PLAYERS_PER_ROOM = 6;

    protected bool _isConnecting;
    string _gameVersion = "1";


    // 프리팹 경로
    string _gameCenterPath = "GameCenter";

    protected string _roomSceneName = "Room"; // "Main";

    #endregion

    #region Public Fields

    public static PhotonManager Instance { get { return p_instance; } }

    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        Init();
    }

    #endregion

    #region Public Methods



    public void LeaveRoom()
    {
        Debug.Log("[LeaveRoom()] Call PhotonNetwork.LeaveRoom()");
        PhotonNetwork.LeaveRoom();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameCenter gameCenter = new GameCenter();

        if (scene.name == _roomSceneName)
        {
            gameCenter.SceneBgmSound("LaxLayoverLOOPING");
        }
        if (scene.name == "Room")
        {
            SceneType = Define.Scene.Lobby;

            for (int i = 0; i < 1; i++)
                Managers.Resource.Instantiate("Effects/Stun_loop");
        }
    }

    #endregion

    #region Private Methods

    void Init()
    {
        if (p_instance == null)
        {
            GameObject _go = GameObject.Find("@Photon Manager");
            if (_go == null)
            {
                _go = new GameObject { name = "@Photon Manager" };
                _go.AddComponent<PhotonManager>();
            }
            DontDestroyOnLoad(_go);
            p_instance = _go.GetComponent<PhotonManager>();

            Screen.SetResolution(800, 480, false);
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.SerializationRate = 20;
            PhotonNetwork.SendRate = 20;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    public void Connect()
    {
        //_loadingPanel.SetActive(true);
        //flag = true;

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");

            // 일단 Room, Join Lobby가 맞는듯
            PhotonNetwork.JoinRandomRoom();

            //if(!flag)
            //    PhotonNetwork.JoinLobby();
        }
        else
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        if (currentSceneName == _roomSceneName)
        {
            AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
            AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
            Managers.Sound.Play(audioClip, Define.Sound.Bgm);
        }
    }

    protected IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Debug.LogFormat("[LoadAsyncScene()] Scene {0} Loaded", SceneManagerHelper.ActiveSceneName);
        InstantiateGameCenter();
    }


    void InstantiateGameCenter()
    {
        if (GameCenter.LocalGameCenterInstance == null)
        {
            //Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalGameCenter from {0}", SceneManagerHelper.ActiveSceneName);
            Managers.Resource.PhotonNetworkInstantiate(_gameCenterPath);
        }
    }


    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        if (_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinLobby();
            _isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        _isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYERS_PER_ROOM });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadAsyncScene(_roomSceneName));
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[OnLeftRoom()] LoadScene(0)");
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        //Debug.LogFormat("[OnPlayerEnteredRoom()] {0}", other.NickName);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        //Debug.LogFormat("[OnPlayerLeftRoom()] {0}", other.NickName);

    }

    #endregion

    public override void Clear()
    {

    }

}
