using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 ui는 ui object를 상속받습니다.
/// </summary>
public interface UIObject
{
    public void Enter();

    public void Exit();
}
