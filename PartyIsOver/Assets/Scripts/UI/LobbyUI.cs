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
    public bool hasPassword;
    

    private void Start()
    {
        CreateRoomPanel.SetActive(false);
        EnterPasswordPanel.SetActive(false);

        PhotonManager.Instance.LobbyUI = GameObject.Find("Canvas_Lobby").GetComponent<LobbyUI>();
    }

    //public void OnClickJoinRoomCancel()
    //{
    //    EnterPasswordPanel.SetActive(false);
    //}


    public void OnClickRandomJoin()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickLeaveLobby()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonManager.Instance.LeaveLobby();
    }


    public void OnClickCreatePopup()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CreateRoomPanel.SetActive(true);
    }

    public void OnClickCreate()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CreateRoomPanel.SetActive(false);

        if (RoomInputField.text.Length >= 1)
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

    public void OnClickJoinRoom(string roomName)
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CreateRoomPanel.SetActive(false);
    }


    //public void OnClickPrivate()
    //{
    //    IsClicked = !IsClicked;

    //    if (IsClicked)
    //    {
    //        PrivateButton.sprite = PrivateOn;
    //        Password.image.sprite = PasswordOn;
    //        Password.textComponent.gameObject.SetActive(true);
    //        Password.textComponent.text = "";
    //    }
    //    else
    //    {
    //        PrivateButton.sprite = PrivateOff;
    //        Password.image.sprite = PasswordOff;
    //        Password.textComponent.gameObject.SetActive(false);
    //        Password.textComponent.text = "";
    //    }
    //}

}