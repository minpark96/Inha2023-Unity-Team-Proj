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
    private RoomUI _roomUI;

    #endregion

    #region Private Fields

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;

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
        if (scene.name == "Arena")
        {
            Debug.Log("Arena Scene 로드 완료 후 초기화");
            //InitArenaUI();
        }
    }

    void Start()
    {
        InitRoomUI();
    }

    void InitRoomUI()
    {
        Debug.Log("InitRoomUI() / name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        _roomUI = GameObject.Find("Control Panel").transform.GetComponent<RoomUI>();

        if (PhotonNetwork.IsMasterClient)
        {
            _roomUI.IsReady = true;
            _roomUI.SetButtonActive("ready", false);
            _roomUI.AddButtonEvent("play", PhotonManager.Instance.LoadArena);
            _roomUI.UpdateReadyCountText(_roomUI.IsReady);
            _roomUI.SetButtonInteractable("play", false);
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
    void EnteredRoom()
    {
        Debug.Log("[master received] EnteredRoom(void)");

        _roomUI.UpdateReadyCountText(_roomUI.PlayerReadyCount);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    [PunRPC]
    void UpdateCount(int count)
    {
        Debug.Log("[except master received] UpdateCount(int): " + count);

        _roomUI.UpdateReadyCountText(count);
    }

    [PunRPC]
    void PlayerReady(bool isReady)
    {
        Debug.Log("[master received] PlayerReady(void): " + isReady);

        _roomUI.UpdateReadyCountText(isReady);
        UpdateMasterStatus();
        photonView.RPC("UpdateCount", RpcTarget.Others, _roomUI.PlayerReadyCount);
    }

    #endregion
}
