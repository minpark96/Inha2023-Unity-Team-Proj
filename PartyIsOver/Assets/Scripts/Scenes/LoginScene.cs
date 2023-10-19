using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : BaseScene
{
    public InputField UsernameInput;
    public Text ButtonText;

    // @Scene을 생성하고 LoingScene을 붙혀 사하면 됨
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
    }

    public void OnClickConnect()
    {
        if (UsernameInput.text.Length >= 1)
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

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
    public override void OnConnectedToMaster()
    {
        //마스터가 로비 연결
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}
