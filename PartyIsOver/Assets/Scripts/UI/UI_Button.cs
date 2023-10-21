using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Button : UI_Base
{

    enum Buttons
    {
        PointButton
    }

    enum Texts
    {
        PointText,
        ScoreText
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    private void Start()
    {
        // 자동 바인딩 형식
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        //Get<Text>((int)Texts.ScoreText).text = "Bind Test";
        GetText((int)Texts.ScoreText).text = "Bind Test";

        // 이벤트 추가 형식
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
       
    }


}
