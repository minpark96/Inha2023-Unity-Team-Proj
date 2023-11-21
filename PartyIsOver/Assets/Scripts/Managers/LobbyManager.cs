using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // : Start
    public Text RoomName;

    public GameObject LobbyPanel;
    public GameObject RoomPanel;

    // : RoomItem
    // RoomItem(방 제목) 리스트
    List<RoomItem> RoomItemsList = new List<RoomItem>();
    public RoomItem RoomItemPrefab;
    public Transform ContentObject; // Inspector상에 있는 viewport > content

    //OnRoomListUpdate가 2번 이상 불리는 것을 방지할 타이머
    public float TimeBetweenUpdate = 1.5f;
    float NextUpdateTime;


    // : PlayerItem
    // PlayerItem(플레이어 정보) 리스트
    List<PlayerItem> PlayerItemsList = new List<PlayerItem>();
    public PlayerItem PlayerItemPrefab;
    public Transform PlayerItemParent; // Inspector상에 있는 PlayerListing


    // : Game Start
    // 게임 시작 버튼
    public GameObject PlayButton;



    // Lobby Panel


    // Room Panel



    // Create Room Panel
    public GameObject CreateRoomPanel;
    public void OnClickOK()
    {
        CreateRoomPanel.SetActive(false);
        // 방만들기
    }

    public void OnClickCancel()
    {
        CreateRoomPanel.SetActive(false);
    }

    public InputField RoomInputField;

    public void OnClickCreate()
    {
        if (RoomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(RoomInputField.text, new RoomOptions() { MaxPlayers = 6 });

        }
    }


    public Sprite PrivateOn;
    public Sprite PrivateOff;
    public Sprite PrivateButton;
    private bool _isClicked;

    public void OnClickPrivate()
    {
        _isClicked = !_isClicked;

        if(_isClicked)
            PrivateButton = PrivateOn;
        else
            PrivateButton = PrivateOff;
    }



    private void Start()
    {
        PhotonNetwork.JoinLobby();
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);

    }

   

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);
        RoomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
    }

    // : RoomItem
    // 로비에 방 목록 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //UpdateRoomList(roomList); // > 2번 이상 불러올 경우가 생김

        if (Time.time >= NextUpdateTime)
        {
            UpdateRoomList(roomList);
            NextUpdateTime = Time.time + TimeBetweenUpdate;
        }

        if (roomList.Count == 0 && PhotonNetwork.InLobby)
        {
            RoomItemsList.Clear();
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        // 변경사항이 있는 room list 갱신
        // 리스트에 있는 내용들 전부 삭제 후 
        foreach (RoomItem item in RoomItemsList)
        {
            Destroy(item.gameObject);
        }
        RoomItemsList.Clear();

        // 새롭게 추가
        foreach (RoomInfo room in list)
        {
            if (room.PlayerCount == 0)
                continue;

            RoomItem newRoom = Instantiate(RoomItemPrefab, ContentObject);
            newRoom.SetRoomName(room.Name);
            RoomItemsList.Add(newRoom);
        }
    }

    // 방에 참가하기
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // 방에서 나가기
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        RoomPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }


    // : PlayerItem
    // 로비에 플레이어 아이콘 업데이트
    void UpdatePlayerList()
    {
        // 변경사항이 있는 player list 갱신
        // 리스트에 있는 내용들 전부 삭제 후 
        foreach (PlayerItem item in PlayerItemsList)
        {
            Destroy(item.gameObject);
        }
        PlayerItemsList.Clear();

        // 방에 있는지 확인 후
        if(PhotonNetwork.CurrentLobby == null)
        {
            return;
        }

        // 새롭게 추가
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(PlayerItemPrefab, PlayerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);

            ////현재 방에 있는 player가 서버에 있는 Localplayer면 변화를 불러오기
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

            PlayerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    // : Game Start
    private void Update()
    {
        // 방장 기능 && 최소 입장인원 충족시만 게임 시작 버튼 활성화
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
    }
    
}
