using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text RoomName;
    LobbyUI Lobby;

    private void Start()
    {
        Lobby = FindObjectOfType<LobbyUI>();
    }

    public void SetRoomName(string roomName)
    {
        RoomName.text = roomName;
    }

    public void OnClickItem()
    {
        Lobby.JoinRoom(RoomName.text);
    }


}
