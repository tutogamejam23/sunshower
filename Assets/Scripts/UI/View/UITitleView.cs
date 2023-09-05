using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Title View에 관리를 담당합니다.
/// </summary>
public class UITitleView : UIView
{
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button continuingBtn;
    [SerializeField] private Button exitBtn;

    public override void HidePanel()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowPanel()
    {
        throw new System.NotImplementedException();
    }
}
