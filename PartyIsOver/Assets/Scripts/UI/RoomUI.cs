using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class RoomUI : MonoBehaviour
{
    string _roomName = "[4]Room";
    string _arenaName = "[5]Arena";

    public Text PlayerNameText;

    public int PlayerReadyCount = 1;

    public bool SkillChange = false;
    public Image SkillChangeButton;
    public Sprite Skill1;
    public Sprite Skill2;
    public Text SkillName;

    public GameObject PlayButton;
    public Sprite GamePlayOff;
    public Sprite GamePlayOn;
    public bool CanPlay;

    public GameObject ReadyButton;
    public Sprite ReadyOff;
    public Sprite ReadyOn;
    public bool IsReady;

    public GameObject SpawnPoint;
    public int ActorNumber;

    public delegate void ChangeSkillEvent(bool isChange);
    public event ChangeSkillEvent OnChangeSkiilEvent;
    public delegate void LoadArena();
    public event LoadArena OnLoadArena;
    public delegate void ReadyEvent(bool isReady);
    public event ReadyEvent OnReadyEvent;
    public delegate void LeaveRoom();
    public event LeaveRoom OnLeaveRoom;

    void Start()
    {
        PlayerNameText.text = PhotonNetwork.NickName;
        IsReady = false;

        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;

        SpawnPoint = GameObject.Find("Spawn Point");

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            SpawnPoint.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    }


    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.F5))
                    {
                        OnClickReady();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickLeaveRoom();
                    }
                }
                break;
            case Define.KeyboardEvent.PointerDown:
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        OnClickSkillChange();
                    }
                }
                break;
        }
    }

    public void ChangeMasterButton(bool canPlay)
    {
        if (canPlay)
            PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOn;
        else
            PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOff;
    }

    public void UpdatePlayerNumber(bool[] playerJoinedList)
    {
        for (int i = 0; i < playerJoinedList.Length; i++)
        {
            if (playerJoinedList[i])
            {
                SpawnPoint.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                SpawnPoint.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

   
    public void OnClickReady()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (CanPlay)
            {
                OnLoadArena();
                PhotonNetwork.LoadLevel(_arenaName);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            IsReady = !IsReady;
            OnReadyEvent(IsReady);

            if (IsReady == false)
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOff;
                ReadyButton.GetComponentInChildren<Text>().text = "ÁØºñ! (F5)";

            }
            else
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOn;
                ReadyButton.GetComponentInChildren<Text>().text = "ÁØºñÇØÁ¦! (F5)";
            }
        }
    }

    public void UnsubscribeKeyboardEvent()
    {
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
    }


    public void OnClickLeaveRoom()
    {
        if (SceneManager.GetActiveScene().name != _arenaName)
        {
            OnLeaveRoom();

            //PhotonManager.Instance.LeaveRoom();
        }
    }

    public void OnClickSkillChange()
    {
        if (SceneManager.GetActiveScene().name != _roomName)
        {
            Managers.Input.KeyboardAction -= OnKeyboardEvent;
            return;
        }

        SkillChange = !SkillChange;
        OnChangeSkiilEvent(SkillChange);
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        if (SkillChange)
        {
            SkillChangeButton.sprite = Skill1;
            SkillName.text = "Â÷Â¡ ½ºÅ³\n\n\n\n³É³ÉÆÝÄ¡";
        }
        else
        {
            SkillChangeButton.sprite = Skill2;
            SkillName.text = "Â÷Â¡ ½ºÅ³\n\n\n\nÇÙÆÝÄ¡";
        }
    }
}
