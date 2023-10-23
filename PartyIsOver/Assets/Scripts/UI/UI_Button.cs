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
        // �ڵ� ���ε� ����
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        //Get<Text>((int)Texts.ScoreText).text = "Bind Test"; // ���� ���� ��
        GetText((int)Texts.ScoreText).text = "Bind Test"; // ����ȭ���Ѽ� ���¹�

        // �̺�Ʈ �߰� ����
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        AddUIEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag); // ���� �����

        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked); // extension�� �߰��ؼ� ���� ���
    }

    int _score = 0;
    public void OnButtonClicked(PointerEventData data)
    {
        _score++;
        GetText((int)Texts.ScoreText).text = $"���� : {_score}"; // ����ȭ���Ѽ� ���¹�
    }


}