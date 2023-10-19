using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        //Managers.UI.ShowSceneUI<UI_Inven>(); //UIManager이 없음
    }

    public override void Clear()
    {
        
    }
    public void OnClickPlayButton()
    {
        //제공하는 SceneManager 빌드셋팅을 추가를 해줘야하는 에러가 나옴 꼭 확인하고 사용하자
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

}
