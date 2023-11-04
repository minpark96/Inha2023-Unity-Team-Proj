using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
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
    Button _buttonReady;

    [Tooltip("시작 버튼, 방장 전용")]
    [SerializeField]
    Button _buttonPlay;

    #endregion

    //int _playerReadyCount = 0;
    //bool _isReady = false;

    public int PlayerReadyCount = 0;
    public bool IsReady = false;
    //public int PlayerReadyCount { get { return _playerReadyCount; } }
    //public bool IsReady { get { return _isReady; } set { _isReady = value; } }

    #region Private Methods

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (_buttonReady == null)
            _buttonReady = transform.GetChild(0).GetComponent<Button>();

        if (_buttonPlay == null)
            _buttonPlay = transform.GetChild(1).GetComponent<Button>();

        if (_playerNameText == null)
            _playerNameText = transform.GetChild(2).GetComponent<TMP_Text>();

        if (_playerStatusText == null)
            _playerStatusText = transform.GetChild(3).GetComponent<TMP_Text>();

        if (_playerReadyCountText == null)
            _playerReadyCountText = transform.GetChild(4).GetComponent<TMP_Text>();

        _playerNameText.text = PhotonNetwork.NickName;
    }

    #endregion

    #region Public Methods

    public void SetPlayerStatus(string text)
    {
        _playerStatusText.text = text;
    }

    public void SetPlayerStatus()
    {
        _playerStatusText.text = IsReady ? "Ready" : "Unready";
    }

    public void AddButtonEvent(string buttonType, UnityAction func)
    {
        switch (buttonType)
        {
            case "play":
                {
                    _buttonPlay.onClick.AddListener(func);
                }
                break;
            case "ready":
                {
                    _buttonReady.onClick.AddListener(func);
                }
                break;
        }
    }

    public void SetButtonActive(string buttonType, bool isActive)
    {
        switch(buttonType)
        {
            case "play":
                {
                    _buttonPlay.gameObject.SetActive(isActive);
                }
                break;
            case "ready":
                {
                    _buttonReady.gameObject.SetActive(isActive);
                }
                break;
        }
    }

    public void SetButtonInteractable(string buttonType, bool isInteractable)
    {
        switch (buttonType)
        {
            case "play":
                {
                    _buttonPlay.interactable = isInteractable;
                }
                break;
            case "ready":
                {
                    _buttonReady.interactable = isInteractable;
                }
                break;
        }
    }

    public void UpdateReadyCountText(bool isReady) 
    {
        if (_playerReadyCountText == null)
            Debug.Log("_playerReadyCountText is null");

        IsReady = isReady;
        PlayerReadyCount += (IsReady ? 1 : -1);
        _playerReadyCountText.text = PlayerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    public void UpdateReadyCountText(int count)
    {
        if (_playerReadyCountText == null)
            Debug.Log("_playerReadyCountText is null");

        PlayerReadyCount = count;
        _playerReadyCountText.text = PlayerReadyCount.ToString() + "/" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

    #endregion
}
