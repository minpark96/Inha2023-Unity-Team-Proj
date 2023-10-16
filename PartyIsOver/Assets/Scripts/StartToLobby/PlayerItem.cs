using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    // Player 이름
    public Text PlayerName;

    // 화면에 보여지는 Player 정보
    Image BackgroundImage;
    public Color HighlightColor;
    public GameObject LeftArrowButton;
    public GameObject RightArrowButton;


    ExitGames.Client.Photon.Hashtable PlayerProperties = new ExitGames.Client.Photon.Hashtable();
    

    private void Start()
    {
        //BackgroundImage = GetComponent<Image>();
    }

    // Player의 이름 가져오기
    public void SetPlayerInfo(Player player)
    {
        PlayerName.text = player.NickName;
    }

    public void ApplyLocalChanges()
    {
        //BackgroundImage.color = HighlightColor;
        LeftArrowButton.SetActive(true);
        RightArrowButton.SetActive(true);
    }


}
