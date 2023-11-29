using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomItem : MonoBehaviour
{
    public Text RoomName;
    public Text MemberNumber;
    public Image RoomPrefabImage;
    public Sprite PublicImage;
    public Sprite PrivateImage;


    LobbyUI Lobby;

    private void Start()
    {
        Lobby = FindObjectOfType<LobbyUI>();

        if (Lobby.IsClicked)
            RoomPrefabImage.sprite = PrivateImage;
        else
            RoomPrefabImage.sprite = PublicImage;
    }

    public void SetRoomName(string roomName, int member)
    {
        RoomName.text = roomName;
        MemberNumber.text = member.ToString() + " / 6";
    }

    public void OnClickItem()
    {
        Lobby.OnClickJoinRoom(RoomName.text);
    }
}