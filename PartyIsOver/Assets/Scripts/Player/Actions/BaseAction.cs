using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�׼��� ���̽��� �Ǵ� �߻�Ŭ����

public abstract class BaseAction
{
    //������ �ش��ϴ� �̸��� Action�� �̺�Ʈ�� ���ε�
    public BaseAction(ActionController actions, Define.ActionEventName eventName)
    {
        actions.BindActionEvent(eventName, HandleActionEvent);
    }

    //�׼��� Invoke ������ ���� ����Ǵ� �Լ�
    protected virtual bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext actionContext)
    {
        return true;
    }
}
