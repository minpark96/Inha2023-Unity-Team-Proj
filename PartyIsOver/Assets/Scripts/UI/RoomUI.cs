using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public GameObject ReadyButton;
    public Sprite ReadyOff;
    public Sprite ReadyOn;
    public bool Ready;


    string _arenaName = "MJTest";


    void Start()
    {
        Init();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PlayerReadyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOn;

                if (Input.GetKeyDown(KeyCode.F5))
                {
                    PhotonNetwork.LoadLevel(_arenaName);
                }
            }
            else
                PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOff;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Ready = !Ready;

                if (Ready)
                    ReadyButton.GetComponentInChildren<Image>().sprite = ReadyOn;
                else
                    ReadyButton.GetComponentInChildren<Image>().sprite = ReadyOff;

                if (Ready)
                    PlayerReadyCount++;
                else
                    PlayerReadyCount--;
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            OnClickSkillChange();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickLeaveRoom();
        }

    }

    void Init()
    {
        _playerNameText.text = PhotonNetwork.NickName;
    }


    public void OnClickLeaveRoom()
    {
        PhotonManager.Instance.LeaveRoom();
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
