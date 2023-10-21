using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    //// 버튼에 텍스트를 연동시키는 과정을 자동화
    // : Bind 함수에서 검색한 개체를 나중에 Get함수를 사용하여 검색할 수 있다

    // GameObject의 자식을 돌면서 type와 이름에 맞는 게임 오브젝트를 objects에 저장
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects); // enum에 있는 개수만큼 _objects dictionary에 저장

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            // Bind 실패시
            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    // 저장된 개체를 검색하여 반환
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false) // dictionary에 없는 object
            return null;

        return objects[idx] as T;
    }

    // 자주 사용하는 기능들 빼놓기
    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }


    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }


        evt.OnDragHandler += ((PointerEventData data) => { evt.gameObject.transform.position = data.position; });
    }

}
