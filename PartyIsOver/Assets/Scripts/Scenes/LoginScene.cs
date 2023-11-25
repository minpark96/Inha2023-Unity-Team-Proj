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

    // @Scene        ϰ  LoingScene           ϸ    
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
    }

    public void OnClickConnect()
    {
        if (UsernameInput.text.Length >= 1)
        {
            //        ̸   Է            ǥ  
            PhotonNetwork.NickName = UsernameInput.text; //        г        
            ButtonText.text = "Connecting ...";

            //          
            PhotonNetwork.ConnectUsingSettings();

            //      ȯ    ʿ 
            PhotonNetwork.AutomaticallySyncScene = true;

        }
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
    public override void OnConnectedToMaster()
    {
        //     Ͱ   κ      
        Managers.Scene.LoadScene(Define.Scene.Lobby);
    }
}