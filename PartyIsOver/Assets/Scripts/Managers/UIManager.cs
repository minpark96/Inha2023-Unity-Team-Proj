using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용 예제
// UI_Button ui = Managers.UI.ShowPopupUI<UI_Button>(); // UI버튼 생성
// Managers.UI.ClosePopupUI(ui); // 생성한 버튼 삭제

public class UIManager
{
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }


    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // 캔버스 안에 캔버스가 있을 때, 부모 값을 무시하고 내 sort값을 갖겠다
        
        if(sort) // 팝업 UI면 sorting함
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else // 팝업이 아닌 UI면 sorting 안함
        {
            canvas.sortingOrder = 0;
        }
    }


    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // UI_Scene Prefab 불러오기
        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    // <T> : 생성할 팝업 prefab , 경로: Resources/UI/Popup/____
    public T ShowPopupUI<T> (string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        // UI_Button Prefab 불러오기
        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }


    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;
        
        // 닫으려는 팝업창이 아닐 때
        if(_popupStack.Peek() != popup)
        {
            Debug.Log("Close popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop(); // 가장 최근에 띄운 팝업창
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

}
