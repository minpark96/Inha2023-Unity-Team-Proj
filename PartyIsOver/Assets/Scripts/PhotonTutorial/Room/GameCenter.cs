using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class GameCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [SerializeField]
    private GameObject _controlPanel;

    [Tooltip("플레이어 이름 표시 텍스트")]
    [SerializeField]
    private TMP_Text _playerNameText;

    [Tooltip("플레이어 준비 상태 표시 텍스트")]
    [SerializeField]
    private TMP_Text _playerStatusText;

    [Tooltip("준비 완료 플레이어 수 표시 텍스트")]
    [SerializeField]
    private TMP_Text _playerReadyCountText;

    [Tooltip("준비/해제 버튼, 클라이언트 전용")]
    [SerializeField]
    Button _buttonReady;

    [Tooltip("시작 버튼, 방장 전용")]
    [SerializeField]
    Button _buttonPlay;

    #endregion

    #region Private Fields

    bool _isReady = false;

    #endregion

    #region Public Fields

    public static GameObject LocalGameCenterInstance = null;
    public int _playerReadyCount = 1;

    #endregion

    #region Private Methods

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalGameCenterInstance = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        if (_controlPanel == null)
            _controlPanel = GameObject.Find("Control Panel");

        if (_buttonReady == null)
            _buttonReady = _controlPanel.transform.GetChild(0).GetComponent<Button>();

        if (_buttonPlay == null)
            _buttonPlay = _controlPanel.transform.GetChild(1).GetComponent<Button>();

        

        if (photonView.IsMine)
        {
            Debug.Log("Init()");
            
            if (_playerNameText == null)
                _playerNameText = _controlPanel.transform.GetChild(2).GetComponent<TMP_Text>();

            if (_playerStatusText == null)
                _playerStatusText = _controlPanel.transform.GetChild(3).GetComponent<TMP_Text>();

            if (_playerReadyCountText == null)
                _playerReadyCountText = _controlPanel.transform.GetChild(4).GetComponent<TMP_Text>();

        }

        // 방장은 Play 버튼만 표시, 이외는 Ready 버튼만 표시
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            _buttonReady.gameObject.SetActive(false);
            _isReady = true;

            _buttonPlay.onClick.AddListener(PhotonManager.Instance.LoadArena);
            Debug.Log("방장 전용 플레이 버튼 이벤트 등록");

            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();

            ShowPlayerName();
            TogglePlayerStatus();
        }
        else if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            _buttonPlay.gameObject.SetActive(false);

            _buttonReady.onClick.AddListener(Ready);
            Debug.Log("클라이언트 전용 준비 버튼 이벤트 등록");
            
            ShowPlayerName();
            TogglePlayerStatus();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Init(): 방장에게 플레이어 입장 알림");
            photonView.RPC("PlayerEntered", RpcTarget.MasterClient);
        }
    }

    void ShowPlayerName()
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
        Debug.Log("GetPlayerName(): " + PhotonNetwork.NickName);
        _playerNameText.text = PhotonNetwork.NickName;
    }

    void TogglePlayerStatus()
    {
        if (_isReady)
        {
            Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
            Debug.Log("Ready");
            _playerStatusText.text = "ready";
        }
        else
        {
            Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
            Debug.Log("Unready");
            _playerStatusText.text = "Unready";
        }
    }

    #endregion

    #region Public Methods

    public void Ready()
    {
        if (!_isReady)
        {
            Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
            Debug.Log("Alert Ready");
            _isReady = true;
            photonView.RPC("PlayerReady", RpcTarget.MasterClient, _isReady);
            TogglePlayerStatus();
        }
        else
        {
            Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
            Debug.Log("Alert Unready");
            _isReady = false;
            photonView.RPC("PlayerReady", RpcTarget.MasterClient, _isReady);
            TogglePlayerStatus();
        }
    }

    [PunRPC]
    public void PlayerEntered()
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        Debug.Log("[only master] PlayerEntered");

        if (_playerReadyCountText == null)
        {
            Debug.Log("_playerReadyCountText null임");
        }

        Debug.Log("[only master] _playerReadyCount: " + _playerReadyCount);
        Debug.Log("[only master] PhotonNetwork.CurrentRoom.PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (photonView.IsMine)
        {
            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }

        photonView.RPC("UpdateReady", RpcTarget.Others, _playerReadyCount);
    }

    [PunRPC]
    public void PlayerReady(bool isReady)
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        Debug.Log("[only master] PlayerReady");

        if (_playerReadyCountText == null)
        {
            Debug.Log("_playerReadyCountText null임");
        }

        if (isReady)
        {
            _playerReadyCount++;
        }
        else
        {
            _playerReadyCount--;
        }

        Debug.Log("[only master] _playerReadyCount: " + _playerReadyCount);
        Debug.Log("[only master] PhotonNetwork.CurrentRoom.PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (photonView.IsMine)
        {
            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }

        photonView.RPC("UpdateReady", RpcTarget.All, _playerReadyCount);
    }

    [PunRPC]
    public void UpdateReady(int playerReadyCount)
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        Debug.Log("[except master] UpdateReady");

        if (_playerReadyCountText == null)
        {
            Debug.Log("_playerReadyCountText null임");
        }
        
        _playerReadyCount = playerReadyCount;

        Debug.Log("[except master] _playerReadyCount: " + _playerReadyCount);
        Debug.Log("[except master] PhotonNetwork.CurrentRoom.PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (photonView.IsMine)
        {
            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }
    }

    #endregion
}
