using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks 
{
    #region Private Serializable Fields

    #endregion

    #region Private Fields

    static PhotonManager p_instance;

    string _gameVersion = "1";
    bool _isConnecting;
    const byte MAX_PLAYERS_PER_ROOM = 6;

    // 일단 넣어 놓은 것, LaucherUI 로 분리해야 함
    Button _buttonStart;
    GameObject _controlPanel;
    GameObject _progressLabel;

    // 프리팹 경로
    string _gameCenterPath = "GameCenter";

    string _roomSceneName = "room";

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

    public void Connect()
    {
        _progressLabel.SetActive(true);
        _controlPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");

            // 일단 Room, Join Lobby가 맞는듯
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }

    public void LeaveRoom()
    {
        Debug.Log("[LeaveRoom()] Call PhotonNetwork.LeaveRoom()");
        PhotonNetwork.LeaveRoom();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _roomSceneName)
        {
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

            _controlPanel = GameObject.Find("Control Panel");
            _progressLabel = GameObject.Find("Progress Label");
            _buttonStart = GameObject.Find("Start Button").transform.GetComponent<Button>();

            _progressLabel.SetActive(false);
            _controlPanel.SetActive(true);
            _buttonStart.onClick.AddListener(Connect);

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

    void JoinLobby()
    {
        Debug.Log("[JoinLobby()] Load Lobby Scene");
        SceneManager.LoadScene(1);
    }

    void InstantiateGameCenter()
    {
        if (GameCenter.LocalGameCenterInstance == null)
        {
            //Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalGameCenter from {0}", SceneManagerHelper.ActiveSceneName);
            Managers.Resource.PhotonNetworkInstantiate(_gameCenterPath);
        }
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Debug.LogFormat("[LoadAsyncScene()] Scene {0} Loaded", SceneManagerHelper.ActiveSceneName);

        InstantiateGameCenter();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        if (_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _progressLabel.SetActive(false);
        _controlPanel.SetActive(true);

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
}
