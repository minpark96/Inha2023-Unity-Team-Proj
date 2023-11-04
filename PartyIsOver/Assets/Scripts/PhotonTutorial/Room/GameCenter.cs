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

        // UI 초기화 부분 나중에 뺄 것, Init에서 초기화 하면 안됨
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
            _buttonPlay.interactable = false;

            if (photonView.IsMine)
            {
                _buttonPlay.onClick.AddListener(PhotonManager.Instance.LoadArena);
                Debug.Log("방장 전용 플레이 버튼 이벤트 등록");
            }

            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }
        else
        {
            _buttonPlay.gameObject.SetActive(false);

            _buttonReady.onClick.AddListener(Ready);
            Debug.Log("클라이언트 전용 준비 버튼 이벤트 등록");
            Debug.Log("Init(): 방장에게 플레이어 입장 알림");
            photonView.RPC("PlayerEntered", RpcTarget.MasterClient);
        }

        ShowPlayerName();
        TogglePlayerStatus();
    }

    void ShowPlayerName()
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
        Debug.Log("GetPlayerName(): " + PhotonNetwork.NickName);
        _playerNameText.text = PhotonNetwork.NickName;
    }

    void TogglePlayerStatus()
    {
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
        if (_isReady)
        {
            Debug.Log("Ready");
            _playerStatusText.text = "ready";
        }
        else
        {
            Debug.Log("Unready");
            _playerStatusText.text = "Unready";
        }
    }
    void UpdatePlayerReadyCountText()
    {
        Debug.Log("UpdatePlayerReadyCountText()");
        Debug.Log("name: " + gameObject.name + ", ViewId: " + photonView.ViewID + ", IsMine?: " + photonView.IsMine);
        if (_playerReadyCountText == null)
            Debug.Log("_playerReadyCountText is null");
        if (photonView.IsMine)
            _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    #endregion

    #region Public Methods

    public void Ready()
    {
        _isReady = !_isReady;
        photonView.RPC("PlayerReady", RpcTarget.MasterClient, _isReady);
        TogglePlayerStatus();
    }

    [PunRPC]
    public void PlayerEntered()
    {
        Debug.Log("[only master] PlayerEntered");
        UpdatePlayerReadyCountText();
        photonView.RPC("UpdateReady", RpcTarget.Others, _playerReadyCount);
    }

    [PunRPC]
    public void PlayerReady(bool isReady)
    { 
        Debug.Log("[only master] PlayerReady");
        _playerReadyCount += (isReady ? 1 : -1);
        UpdatePlayerReadyCountText();
        photonView.RPC("UpdateReady", RpcTarget.Others, _playerReadyCount);
        _buttonPlay.interactable = (_playerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount);
    }

    [PunRPC]
    public void UpdateReady(int playerReadyCount)
    {
        Debug.Log("[except master] UpdateReady()");
        _playerReadyCount = playerReadyCount;
        UpdatePlayerReadyCountText();
    }

    #endregion
}
