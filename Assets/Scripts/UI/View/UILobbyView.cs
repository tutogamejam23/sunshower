using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lobby View에 관리를 담당합니다.
/// </summary>
public class UILobbyView : UIView
{
    [SerializeField] private Button settingBtn;
    [SerializeField] private Image hpImg;
    [SerializeField] private Image processValueImg;

    public override void HidePanel()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowPanel()
    {
        throw new System.NotImplementedException();
    }
}
