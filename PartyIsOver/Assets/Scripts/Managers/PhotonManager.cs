using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks 
{
    #region Private Serializable Fields

    #endregion

    #region Private Fields

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string _gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool _isConnecting;
    
    GameObject _controlPanel;
    GameObject _progressLabel;
    const byte MAX_PLAYERS_PER_ROOM = 4;

    static PhotonManager p_instance;

    #endregion

    #region Public Fields

    public static PhotonManager Instance { get { return p_instance; } }
    //public string _playerPrefab = "My Robot Kyle";
    public string _playerPrefab = "Ragdoll2";
    //public string _playerPrefab = "Cube";

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        Init();
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

            //_instance._data.Init();
            //_instance._photon.Init();

            Screen.SetResolution(800, 480, false);

            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;

            //Debug.LogFormat("We are Start Scene {0}", SceneManagerHelper.ActiveSceneName);

            _controlPanel = GameObject.Find("Control Panel");
            _progressLabel = GameObject.Find("Progress Label");
            Debug.Log("Control Panel " + _controlPanel);
            Debug.Log("Progress Label " + _progressLabel);
            _progressLabel.SetActive(false);
            _controlPanel.SetActive(true);

            PhotonNetwork.SerializationRate = 20;
            PhotonNetwork.SendRate = 20;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        _progressLabel.SetActive(true);
        _controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");

            // 일단 Room, Join Lobby가 맞는듯

            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            Debug.Log("PUN Basics Tutorial/Launcher: ConnectUsingSettings() was called by PUN");
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }

    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom(): Call PhotonNetwork.LeaveRoom()");
        bool log = PhotonNetwork.LeaveRoom();
    }

    public void LoadArena()
    {
        Debug.Log("외않되;");
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }

        PhotonNetwork.LoadLevel("Arena");
    }

    #endregion

    #region Private Methods

    void JoinLobby()
    {
        Debug.Log("JoinLobby(): Load Lobby Scene");
        SceneManager.LoadScene(1);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (_isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            Debug.Log("PUN Basics Tutorial/Launcher: JoinRandomRoom() was called by PUN");
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
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYERS_PER_ROOM });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("OnJoinedRoom(): Load Room Scene");
            //SceneManager.LoadScene(1);
            StartCoroutine(LoadAsyncScene("Room"));
        }
        else
        {
            InstantiateGameCenter();
            //InstantiatePlayer();
        }
    }

    void InstantiateGameCenter()
    {
        if (GameCenter.LocalGameCenterInstance == null)
        {
            Debug.Log("in room?: " + PhotonNetwork.InRoom);
            Debug.LogFormat("PhotonManager=>We are Instantiating LocalGameCenter from {0}", SceneManagerHelper.ActiveSceneName);
            PhotonNetwork.Instantiate("GameCenter", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        }
    }

    void InstantiatePlayer()
    {
        if (Actor.LocalPlayerInstance == null)
        {
            Debug.Log("in room?: " + PhotonNetwork.InRoom);
            Debug.LogFormat("PhotonManager=>We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(_playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.LogFormat("Scene {0} Loaded", SceneManagerHelper.ActiveSceneName);

        InstantiateGameCenter();
        //InstantiatePlayer();
    }

    #endregion

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom(): LoadScene(1)");
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

    }

    #endregion
}
