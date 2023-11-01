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
        if (_controlPanel == null)
            _controlPanel = GameObject.Find("Control Panel");

        if (_buttonReady == null)
            _buttonReady = _controlPanel.transform.GetChild(0).GetComponent<Button>();

        if (_buttonPlay == null)
            _buttonPlay = _controlPanel.transform.GetChild(1).GetComponent<Button>();

        if (_playerNameText == null)
            _playerNameText = _controlPanel.transform.GetChild(2).GetComponent<TMP_Text>();

        if (_playerStatusText == null)
            _playerStatusText = _controlPanel.transform.GetChild(3).GetComponent<TMP_Text>();

        if (_playerReadyCountText == null)
            _playerReadyCountText = _controlPanel.transform.GetChild(4).GetComponent<TMP_Text>();

        // 방장은 Play 버튼만 표시, 이외는 Ready 버튼만 표시
        if (PhotonNetwork.IsMasterClient)
        {
            _buttonReady.gameObject.SetActive(false);
            _isReady = true;

            _buttonPlay.onClick.AddListener(PhotonManager.Instance.LoadArena);
            Debug.Log("방장 전용 플레이 버튼 이벤트 등록");
        }
        else
        {
            _buttonPlay.gameObject.SetActive(false);

            _buttonReady.onClick.AddListener(Ready);
            Debug.Log("클라이언트 전용 준비 버튼 이벤트 등록");
        }

        
            Debug.Log("Init(): 방장한테 나 들어왔다고 알려라");
            photonView.RPC("PlayerEntered", RpcTarget.MasterClient);
        
        ShowPlayerName();
        TogglePlayerStatus();
    }

    void ShowPlayerName()
    {
        Debug.Log("GetPlayerName(): " + PhotonNetwork.NickName);
        _playerNameText.text = PhotonNetwork.NickName;
    }

    void TogglePlayerStatus()
    {
        if (_isReady)
        {
            _playerStatusText.text = "ready";
        }
        else
        {
            _playerStatusText.text = "Unready";
        }
    }

    #endregion

    #region Public Methods

    public void Ready()
    {
        if (!_isReady)
        {
            _isReady = true;
            photonView.RPC("PlayerReady", RpcTarget.MasterClient, _isReady);
            TogglePlayerStatus();
        }
        else
        {
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

        _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
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

        _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        photonView.RPC("UpdateReady", RpcTarget.Others, _playerReadyCount);
    }

    [PunRPC]
    public void UpdateReady(int playerReadyCount)
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);

        Debug.Log("[except master] PlayerReady");

        if (_playerReadyCountText == null)
        {
            Debug.Log("_playerReadyCountText null임");
        }
        
        _playerReadyCount = playerReadyCount;

        Debug.Log("[except master] _playerReadyCount: " + _playerReadyCount);
        Debug.Log("[except master] PhotonNetwork.CurrentRoom.PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);

        _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    #endregion
}
