using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 Panel의 On/Off 기능을 담당한다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIView : MonoBehaviour, UIObject
{
    private CanvasGroup CG;

    public abstract void ShowPanel();

    public abstract void HidePanel();

    protected virtual void Start()
    {
        CG = GetComponent<CanvasGroup>();
    }

    public void SetCG(bool isActive, bool blockRaycast = true)
    {
        CG.alpha = isActive ? 1 : 0;
        CG.interactable = isActive;
        CG.blocksRaycasts = isActive && blockRaycast;
    }

    public bool isActive()
    {
        return CG.alpha != 0;
    }

    public void Enter()
    {
        ShowPanel();
    }

    public void Exit()
    {
        HidePanel();
    }
}
