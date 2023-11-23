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
    static PhotonManager p_instance;

    const byte MAX_PLAYERS_PER_ROOM = 6;

    bool IsConnecting;
    string GameVersion = "1";
    string _gameCenterPath = "GameCenter";
    string _sceneLobby = "[3]Lobby";
    string _sceneRoom = "[4]Room";

    LobbyUI LobbyUI;
    List<RoomItem> RoomItemsList = new List<RoomItem>();
    float _nextUpdateTime = 1f;
    float _timeBetweenUpdate = 1.5f;

    public static PhotonManager Instance { get { return p_instance; } }
  

    void Awake()
    {
        Init();
    }

    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            IsConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = GameVersion;
        }

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        if (currentSceneName == _sceneLobby)
        {
            AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
            AudioClip audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
            Managers.Sound.Play(audioClip, Define.Sound.Bgm);
        }
    }

    public void LeaveRoom()
    {
        Debug.Log("[LeaveRoom()] Call PhotonNetwork.LeaveRoom()");
        PhotonNetwork.LeaveRoom();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _sceneLobby)
        {
            LobbyUI = GameObject.Find("Lobby UI").GetComponent<LobbyUI>();
        }
    }

    public void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in RoomItemsList)
        {
            Destroy(item.gameObject);
        }
        RoomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            if (room.PlayerCount == 0)
                continue;

            RoomItem newRoom = Instantiate(LobbyUI.RoomItemPrefab, LobbyUI.ContentObject);
            newRoom.SetRoomName(room.Name, room.PlayerCount);
            RoomItemsList.Add(newRoom);
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

   

    IEnumerator LoadNextScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                break;
            }
            else
                yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        if (sceneName == _sceneRoom)
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
        if (IsConnecting)
        {
            IsConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        IsConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MAX_PLAYERS_PER_ROOM });
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[JoinLobby()] Load Lobby Scene");
        StartCoroutine(LoadNextScene(_sceneLobby));
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadNextScene(_sceneRoom));
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[OnLeftRoom()] LoadScene(2)");
        SceneManager.LoadScene(2);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //UpdateRoomList(roomList); // > 2번 이상 불러올 경우가 생김

        if (Time.time >= _nextUpdateTime)
        {
            UpdateRoomList(roomList);
            _nextUpdateTime = Time.time + _timeBetweenUpdate;
        }

        if (roomList.Count == 0 && PhotonNetwork.InLobby)
        {
            RoomItemsList.Clear();
        }
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
