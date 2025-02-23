using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 커맨드 클래스의 베이스 클래스(추상 클래스)
public abstract class CommandKey
{
    // 커맨드를 실행할 대상(생성자에서 받아서 저장)
    protected Actor actor;
    
    // 커맨드가 생성될 때 시간을 기록하는 변수 (나중에 리플레이 기능등에 사용할 예정)
    public float Timestamp { get; set; }

    // 현재 미사용 생성자
    public CommandKey()
    {
        Timestamp = Time.time;
    }

    // 생성된 커맨드를 실제 실행하여 Action을 실행시키는 함수
    public virtual bool Execute(in PlayerActionContext data)
    {
        return true;
    }
}
