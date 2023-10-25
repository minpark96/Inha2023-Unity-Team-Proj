using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text RoomName;
    LobbyManager Manager;

    private void Start()
    {
        Manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string roomName)
    {
        RoomName.text = roomName;
    }

    public void OnClickItem()
    {
        Manager.JoinRoom(RoomName.text);
    }


}
