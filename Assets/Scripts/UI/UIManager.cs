using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType
{
    Title = 0,
    Dialouage,
    Lobby,
    Stage1,
    Stage2,
    Setting
}

/// <summary>
/// UI Panel을 등록하여 On/Off 관련된 관리합니다.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public PanelType panelType;

    private void Start()
    {

    }

    private void Update()
    {

    }
}
