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
    [Tooltip("ÇÃ·¹ÀÌ¾î ÀÌ¸§ Ç¥½Ã ÅØ½ºÆ®")]
    [SerializeField]
    private TMP_Text _playerNameText;


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


    string _arenaName = "PO_Map_KYH";


    void Start()
    {
        Init();
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
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

                    if(Input.GetKeyDown(KeyCode.Tab))
                    {
                        OnClickSkillChange();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickLeaveRoom();
                    }
                }
                break;

            case Define.KeyboardEvent.PointerUp:
                break;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (CanPlay)
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOn;
            else
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOff;
        }
    }

    void Init()
    {
        _playerNameText.text = PhotonNetwork.NickName;
    }


    public void OnClickReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(CanPlay)
                PhotonNetwork.LoadLevel(_arenaName);
        }
        else
        {
            Ready = !Ready;

            if (Ready)
                ReadyButton.GetComponentInChildren<Image>().sprite = ReadyOn;
            else
                ReadyButton.GetComponentInChildren<Image>().sprite = ReadyOff;
        }
    }

   

    public void OnClickLeaveRoom()
    {
        //PhotonManager.Instance.LeaveRoom();

        PhotonNetwork.LeaveRoom();
    }

    public void OnClickSkillChange()
    {
        SkillChange = !SkillChange;

        if (SkillChange)
        {
            SkillChangeButton.sprite = Skill1;
            SkillName.text = "Â÷Â¡ ½ºÅ³\n\n\n³É³ÉÆÝÄ¡";
        }
        else
        {
            SkillChangeButton.sprite = Skill2;
            SkillName.text = "Â÷Â¡ ½ºÅ³\n\n\nÇÙÆÝÄ¡";
        }
    }

}
