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

        //Get<Text>((int)Texts.ScoreText).text = "Bind Test"; // 원래 쓰는 법
        GetText((int)Texts.ScoreText).text = "Bind Test"; // 간략화시켜서 쓰는법

        // 이벤트 추가 형식
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        AddUIEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag); // 원래 사용방식

        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked); // extension을 추가해서 사용된 방식
    }

    int _score = 0;
    public void OnButtonClicked(PointerEventData data)
    {
        _score++;
        GetText((int)Texts.ScoreText).text = $"점수 : {_score}"; // 간략화시켜서 쓰는법
    }


}
