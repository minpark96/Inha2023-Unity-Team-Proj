using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField UsernameInput;
    public Text ButtonText;

    public void OnClickConnect()
    {
        if(UsernameInput.text.Length >= 1)
        {
            // 사용자 이름 입력 및 연결중 표시
            PhotonNetwork.NickName = UsernameInput.text; // 사용자 닉네임 노출
            ButtonText.text = "Connecting ...";

            // 서버 접속
            PhotonNetwork.ConnectUsingSettings();

            // 씬 전환시 필요
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }


}
