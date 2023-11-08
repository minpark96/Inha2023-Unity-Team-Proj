using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [SerializeField]
    RoomUI _roomUI;

    #endregion

    #region Private Fields

    string _arenaName = "SDJTest";
    // 프리팹 경로
    string _playerPath = "Ragdoll2";

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

    // 스폰 포인트 6인 기준
    public List<Vector3> SpawnPoints = new List<Vector3>
    {
        new Vector3(5f, 5f, 0f),
        new Vector3(2.5f, 5f, 4.33f),
        new Vector3(-2.5f, 5f, 4.33f),
        new Vector3(-5f, 5f, 0f),
        new Vector3(-2.5f, 5f, -4.33f),
        new Vector3(2.5f, 5f, -4.33f)
    };

    public List<Actor> Actors = new List<Actor>();

    #endregion

    #region Private Methods

    void Awake()
    {
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
        }
    }

    void InstantiatePlayer()
    {
        if (Actor.LocalPlayerInstance == null)
        {
            //Debug.LogFormat("PhotonManager.cs => We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            GameObject go = null;

            switch (PhotonNetwork.LocalPlayer.ActorNumber)
            {
                case 1:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[0]);
                    break;
                case 2:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[1]);
                    break;
                case 3:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[2]);
                    break;
                case 4:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[3]);
                    break;
                case 5:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[4]);
                    break;
                case 6:
                    go = Managers.Resource.PhotonNetworkInstantiate(_playerPath, pos: SpawnPoints[5]);
                    break;
            }

            Actor actor = go.GetComponent<Actor>();
            if (actor != null)
            {
                actor.OnPlayerHurt -= AnounceHurt;
                //actor.OnPlayerExhaust -= DecreaseStamina;
                actor.OnPlayerHurt += AnounceHurt;
                //actor.OnPlayerExhaust += DecreaseStamina;
            }
            Actors.Add(actor);
        }
    }

    void AnounceHurt(float HP, int viewID)
    {
        Debug.Log("AnounceHurt(float, int)");

        photonView.RPC("DecreseHP", RpcTarget.MasterClient, HP, viewID);
    }

    void DecreaseStamina(int amount)
    {

    }

    void LoadArena()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(_arenaName);
        }
        else
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
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
        {
            _roomUI.IsReady = true;
            _roomUI.SetButtonActive("ready", false);
            _roomUI.AddButtonEvent("play", LoadArena);
            _roomUI.UpdateReadyCountText(_roomUI.IsReady);
            _roomUI.SetButtonInteractable("play", true);
            _roomUI.SetPlayerStatus("Wait for Other Players...");
        }
        else
        {
            _roomUI.SetButtonActive("play", false);
            _roomUI.AddButtonEvent("ready", Ready);
            _roomUI.SetPlayerStatus("Unready");
            photonView.RPC("EnteredRoom", RpcTarget.MasterClient);
        }
    }

    void UpdateMasterStatus()
    {
        if (_roomUI.PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            _roomUI.SetPlayerStatus("All Players Ready!");
            _roomUI.SetButtonInteractable("play", true);
        }
        else
        {
            _roomUI.SetPlayerStatus("Wait for Other Players...");
            _roomUI.SetButtonInteractable("play", false);
        }
    }

    void Ready()
    {
        _roomUI.IsReady = !_roomUI.IsReady;
        _roomUI.SetPlayerStatus();
        photonView.RPC("PlayerReady", RpcTarget.MasterClient, _roomUI.IsReady);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Methods

    public override void OnEnable()
    {
        Debug.Log("GameCenter OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region PunRPC Methods

    [PunRPC]
    void DeacreaseHP(float HP, int viewID)
    {
        Debug.Log("[master received] DeacreaseHP(void)");

        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].Health = HP;
                break;
            }
        }

        photonView.RPC("SyncHP", RpcTarget.Others, HP, viewID);
    }

    [PunRPC]
    void SyncHP(float HP, int viewID)
    {
        Debug.Log("[except master received] SyncHP(void)");

        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].photonView.ViewID == viewID)
            {
                Actors[i].Health = HP;
                break;
            }
        }
    }

    [PunRPC]
    void EnteredRoom()
    {
        _roomUI.UpdateReadyCountText(_roomUI.PlayerReadyCount);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    [PunRPC]
    void UpdateCount(int count)
    {
        _roomUI.UpdateReadyCountText(count);
    }

    [PunRPC]
    void PlayerReady(bool isReady)
    {
        _roomUI.UpdateReadyCountText(isReady);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    #endregion
}
