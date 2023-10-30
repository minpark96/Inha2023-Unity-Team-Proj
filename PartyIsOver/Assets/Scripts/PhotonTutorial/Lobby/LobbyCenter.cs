using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class LobbyCenter : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

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
    //GameObject _buttonReady;
    Button _buttonReady;
    [Tooltip("시작 버튼, 방장 전용")]
    [SerializeField]
    //GameObject _buttonPlay;
    Button _buttonPlay;

    PhotonView _pv;

    #endregion

    #region Private Fields

    bool _isReady = false;

    #endregion

    #region Public Fields

    public static int _playerReadyCount = 1;

    #endregion

    #region Private Methods

    private void Start()
    {
        Init();
        Enter();
    }

    private void Init()
    {
        if (_buttonPlay == null)
        {
            _buttonPlay = transform.GetChild(1).GetComponent<Button>();
        }

        if (_buttonReady == null)
        {
            _buttonReady = transform.GetChild(0).GetComponent<Button>();
        }

        // 방장은 Play 버튼만 표시, 이외는 Ready 버튼만 표시
        if (PhotonNetwork.IsMasterClient)
        {
            _buttonReady.gameObject.SetActive(false);
            _isReady = true;

            _buttonPlay.onClick.AddListener(PhotonManager.Instance.LoadArena);
            Debug.Log("1");
        }
        else
        {
            _buttonPlay.gameObject.SetActive(false);

            _buttonReady.onClick.AddListener(Ready);
            Debug.Log("2");
        }

        Debug.Log("3");
        ShowPlayerName();
        TogglePlayerStatus();
    }

    public void Announce(PhotonView pv)
    {
        pv.RPC("Enter", RpcTarget.All);
    }

    [PunRPC]
    public void Enter()
    {
        Debug.Log("Enter");
        _playerReadyCountText.text = _playerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
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
            _playerStatusText.text = "Unready";
        }
        else
        {
            _playerStatusText.text = "Ready";
        }
    }

    #endregion

    #region Public Methods

    public void UpdatePlayerReadyCount()
    {

    }

    public void Ready()
    {
        if (!_isReady)
        {
            _isReady = true;
            _playerReadyCount++;
            TogglePlayerStatus();
        }
        else
        {
            _isReady = false;
            TogglePlayerStatus();
        }
    }

    #endregion
}
