using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomItem : MonoBehaviour
{
    public Text RoomName;
    public Text MemberNumber;

    LobbyUI Lobby;

    private void Start()
    {
        Lobby = FindObjectOfType<LobbyUI>();
    }

    public void SetRoomName(string roomName, int member)
    {
        RoomName.text = roomName;
        MemberNumber.text = member.ToString() + " / 6";
    }

    public void OnClickItem()
    {
        Lobby.JoinRoom(RoomName.text);
    }


}
