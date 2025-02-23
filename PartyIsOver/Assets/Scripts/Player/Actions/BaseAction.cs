using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 액션의 베이스가 되는 추상클래스

public abstract class BaseAction
{
    // 생성과 동시에 해당하는 이름의 Action에 이벤트를 바인딩
    public BaseAction(ActionController actions, Define.ActionEventName eventName)
    {
        actions.BindActionEvent(eventName, HandleActionEvent);
    }

    // 액션이 Invoke 됐을때 실제 실행되는 함수, 상속 후 재정의해서 사용한다.
    // 애니메이션 데이터와, 애니메이션을 실행시키는 AnimationPlayer,
    // 플레이어의 신체 부위를 관리하는 bodyHandler, 플레이어의 상태 정보를 담고 있는 ActionContext를 인자로 받는다.
    protected virtual bool HandleActionEvent(AnimationData animData, AnimationPlayer animPlayer, BodyHandler bodyHandler, in PlayerActionContext actionContext)
    {
        return true;
    }
}
