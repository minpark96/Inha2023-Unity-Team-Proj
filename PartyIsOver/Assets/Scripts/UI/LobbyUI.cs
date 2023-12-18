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
    public InputField RoomInputField;

    // RoomItem Prefab
    public RoomItem RoomItemPrefab;
    public Transform ContentObject;


    private void Start()
    {
        CreateRoomPanel.SetActive(false);
        EnterPasswordPanel.SetActive(false);

        PhotonManager.Instance.LobbyUI = GameObject.Find("Canvas_Lobby").GetComponent<LobbyUI>();
    }


    public void OnClickRandomJoin()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickLeaveLobby()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonManager.Instance.LeaveLobby();
    }


    public void OnClickCreatePopup()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CreateRoomPanel.SetActive(true);
    }

    public void OnClickCreate()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
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
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CreateRoomPanel.SetActive(false);
    }
}