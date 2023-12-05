using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class RoomUI : MonoBehaviour
{
    [Tooltip("플레이어 이름 표시 텍스트")]
    [SerializeField]
    private TMP_Text _playerNameText;

    private bool _hasCheckedTime;

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
    public bool Ready;
    public bool ReadyIsClicked;

    public GameObject SpawnPoint;
    public int PlayerCount;
    public int ActorNumber;

    string _arenaName = "PO_Map_KYH";

    void Start()
    {
        Init();
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;

        SpawnPoint = GameObject.Find("Spawn Point");
        PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        for (int i = 0; i < PlayerCount; i++)
        {
            SpawnPoint.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void Init()
    {
        _playerNameText.text = PhotonNetwork.NickName;
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

                    //if (Input.GetKeyDown(KeyCode.Escape))
                    //{
                    //    OnClickLeaveRoom();
                    //}
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

    void Update()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (CanPlay)
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOn;
            else
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOff;
        }
    }

    public void UpdatePlayerNumber(int totalPlayerNumber)
    {
        for (int i = 0; i < totalPlayerNumber; i++)
        {
            SpawnPoint.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

   
    public void OnClickReady()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if(CanPlay)
            {
                PhotonNetwork.LoadLevel(_arenaName);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            Ready = !Ready;

            if (!Ready)
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOn;
                ReadyButton.GetComponentInChildren<Text>().text = "준비! (F5)";

            }
            else
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOff;
                ReadyButton.GetComponentInChildren<Text>().text = "준비해제! (F5)";
            }
        }
    }



    //public void OnClickLeaveRoom()
    //{
    //    if(SceneManager.GetActiveScene().name != _arenaName)
    //        PhotonManager.Instance.LeaveRoom();
    //}

    public void OnClickSkillChange()
    {
        if (!SkillChangeButton || !SkillName)
            return;

        SkillChange = !SkillChange;
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("Effect/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        if (SkillChange)
        {
            SkillChangeButton.sprite = Skill1;
            SkillName.text = "차징 스킬\n\n\n\n냥냥펀치";
        }
        else
        {
            SkillChangeButton.sprite = Skill2;
            SkillName.text = "차징 스킬\n\n\n\n핵펀치";
        }
    }




}
