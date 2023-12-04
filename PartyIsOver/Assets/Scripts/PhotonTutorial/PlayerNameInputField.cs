using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerNameInputField : MonoBehaviour
{
    public GameObject PlaceHolder;
    public InputField InputField;

    void Start()
    {
        PlaceHolder.SetActive(true);
        InputField.onValueChanged.AddListener(SetPlayerName);
    }
       
    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            PlaceHolder.SetActive(true);

            Debug.LogError("Player Name is null or empty");
            return;
        }

        PlaceHolder.SetActive(false);

        PhotonNetwork.NickName = value;
    }

}
