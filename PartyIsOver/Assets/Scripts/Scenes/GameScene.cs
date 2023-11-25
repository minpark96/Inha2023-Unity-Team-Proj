using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        //Managers.UI.ShowSceneUI<UI_Inven>(); //UIManager       
    }

    public override void Clear()
    {

    }
    public void OnClickPlayButton()
    {
        //     ϴ  SceneManager           ߰          ϴ                 Ȯ   ϰ         
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

}