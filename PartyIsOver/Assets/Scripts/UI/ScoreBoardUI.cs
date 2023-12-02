using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class ScoreBoardUI : MonoBehaviour
{
    public bool isSetup;

    public GameObject Info;
    public Sprite PointStar;

    private GameObject _scoreBoardPanel;
    private GameObject[] _portrait = new GameObject[6];
    private GameObject[] _score = new GameObject[6];
    private Text[] _nickName = new Text[6];
    private int _playerNumber;


    public void ScoreBoardSetup()
    {
        if (isSetup)
            return;

        _scoreBoardPanel = GameObject.Find("ScoreBoard Panel");
        _playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        for (int i = 0; i < _playerNumber; i++)
        {
            Info.transform.GetChild(i).gameObject.SetActive(true);
            _portrait[i] = Info.transform.GetChild(i).GetChild(0).gameObject;
            _score[i] = Info.transform.GetChild(i).GetChild(1).gameObject;
            _nickName[i] = Info.transform.GetChild(i).GetChild(2).GetComponent<Text>();
        }

        Managers.Input.KeyboardAction -= OnKeyboardEvents;
        Managers.Input.KeyboardAction += OnKeyboardEvents;
        _scoreBoardPanel.SetActive(false);
    }

    public void ChangeScoreBoard(int[] score, string[] name, int[] rank)
    {
        _playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        for (int i = 0; i < _playerNumber; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                _portrait[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _playerNumber; i++)
        {
            _portrait[i].transform.GetChild(rank[i] - 1).gameObject.SetActive(true);
        }

        for (int i = 0; i < _playerNumber; i++)
        {
            for (int j = 0; j < score[i]; j++)
            {
                _score[i].transform.GetChild(j).GetComponent<Image>().sprite = PointStar;
            }
        }

        for (int i = 0; i < _playerNumber; i++)
        {
            _nickName[i].text = name[i];
        }
    }


    void OnKeyboardEvents(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        OnClickScoreBoardON();
                    }
                }
                break;

            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        OnClickScoreBoardOFF();
                    }
                }
                break;
        }
    }

    void OnClickScoreBoardON()
    {
        _scoreBoardPanel.SetActive(true);
    }
    void OnClickScoreBoardOFF()
    {
        _scoreBoardPanel.SetActive(false);
    }
}