using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyUI : MonoBehaviour
{
    // UI
    public GameObject CreateRoomPanel;
    public GameObject EnterPasswordPanel;

    public Sprite PrivateOn;
    public Sprite PrivateOff;
    public Sprite PasswordOn;
    public Sprite PasswordOff;

    public Image PrivateButton;

    public InputField RoomInputField;
    public InputField Password;
    public InputField InviteCode;

    // RoomItem Prefab
    public RoomItem RoomItemPrefab;
    public Transform ContentObject;

    public bool IsClicked;

    private string _roomName;
    

    private void Start()
    {
        CreateRoomPanel.SetActive(false);
        EnterPasswordPanel.SetActive(false);

        PhotonManager.Instance.LobbyUI = GameObject.Find("Canvas_Lobby").GetComponent<LobbyUI>();
    }

  
    public void JoinRoom(string roomName)
    {
        EnterPasswordPanel.SetActive(true);
        _roomName = roomName;
    }
    public void OnClickJoinRoomCancel()
    {
        EnterPasswordPanel.SetActive(false);
    }


    public void OnClickRandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnClickLeaveLobby()
    {
        PhotonManager.Instance.LeaveLobby();
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
            if(IsClicked)
            {
                RoomOptions roomOptions =
                new RoomOptions()
                {
                    IsVisible = true,
                    IsOpen = false,
                    MaxPlayers = 6,
                };

                roomOptions.CustomRoomProperties = new Hashtable();
                roomOptions.CustomRoomProperties.Add("password", Password.textComponent.text);

                PhotonNetwork.CreateRoom(RoomInputField.text, roomOptions);
            }
            else
            {
                RoomOptions roomOptions =
                new RoomOptions()
                {
                    IsVisible = true,
                    IsOpen = true,
                    MaxPlayers = 6,
                };

                PhotonNetwork.CreateRoom(RoomInputField.text, roomOptions);
            }
        }
    }
    public void OnClickJoinRoom()
    {
        string inviteCode = InviteCode.textComponent.text;
        object[] invitation = new object[] { inviteCode };
        PhotonNetwork.JoinRoom(_roomName, (string[])invitation);
    }
    public void OnClickCancel()
    {
        CreateRoomPanel.SetActive(false);
    }


    public void OnClickPrivate()
    {
        IsClicked = !IsClicked;

        if (IsClicked)
        {
            PrivateButton.sprite = PrivateOn;
            Password.image.sprite = PasswordOn;
            Password.textComponent.gameObject.SetActive(true);
            Password.textComponent.text = "";
        }
        else
        {
            PrivateButton.sprite = PrivateOff;
            Password.image.sprite = PasswordOff;
            Password.textComponent.gameObject.SetActive(false);
            Password.textComponent.text = "";
        }
    }
}