using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PanelType
{
    Title = 0,
    Dialouage,
    Lobby,
    Stage1,
    Stage2,
    Setting,
    Skill,
}

/// <summary>
/// UI Panel을 등록하여 On/Off 관련된 관리합니다.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    Dictionary<PanelType, UIView> panels = new Dictionary<PanelType, UIView>();
    UIView curUI;

    public PanelType PanelType { get; set; }

    /// <summary>
    /// 여러 View을 등록한다.
    /// </summary>
    /// <param name="panelState"></param>
    /// <param name="panel"></param>
    public void RegisterPanel(PanelType panelState, UIView panel)
    {
        if (!panels.ContainsKey(panelState))
        {
            panels.Add(panelState, panel);
            Debug.Log(panel.name);
        }
    }

    /// <summary>
    /// 알맞게 View을 Enter/Exit을 한다. 
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangePanel(PanelType nextState)
    {
        if (curUI != null && PanelType == nextState) // 패널 상태가 같으면 해당 패널 숨기기
        {
            curUI.Exit();
            PanelType = PanelType.Title; // 현재 상태 초기화
            return;
        }
        else if (curUI != null) // 다른 패널 상태일 때는 현재 패널 숨기기
        {
            curUI.Exit();
        }

        curUI = panels[nextState];
        PanelType = nextState;

        curUI.Enter();
    }
}
