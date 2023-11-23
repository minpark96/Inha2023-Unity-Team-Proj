using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    // UI
    public GameObject CreateRoomPanel;
    public Sprite PrivateOn;
    public Sprite PrivateOff;
    public Image PrivateButton;
    public InputField RoomInputField;

    // RoomItem Prefab
    public RoomItem RoomItemPrefab;
    public Transform ContentObject;


    private bool _isClicked;



    private void Start()
    {
        CreateRoomPanel.SetActive(false);
    }


    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void OnClickLeaveRoom()
    {
        PhotonManager.Instance.LeaveRoom();
    }
    public void OnClickRandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public void OnClickCreatePopup()
    {
        CreateRoomPanel.SetActive(true);
    }
    public void OnClickCreate()
    {
        CreateRoomPanel.SetActive(false);

        if (RoomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(RoomInputField.text, new RoomOptions() { MaxPlayers = 6 });
        }
    }
    public void OnClickCancel()
    {
        CreateRoomPanel.SetActive(false);
    }
    public void OnClickPrivate()
    {
        _isClicked = !_isClicked;

        if (_isClicked)
            PrivateButton.sprite = PrivateOn;
        else
            PrivateButton.sprite = PrivateOff;
    }

}
