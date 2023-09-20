using DG.Tweening;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Title View에 관리를 담당합니다.
/// </summary>
public class UITitleView : UIView
{
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private Button continuingBtn;
    [SerializeField] private Button exitBtn;

    [SerializeField] private Image fadeImg;

    void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Title, this);

        gameStartBtn.onClick.AddListener(() => StartBtn());
        exitBtn.onClick.AddListener(() => Application.Quit());
    }

    void StartBtn()
    {
        fadeImg.DOFade(1f, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync("Stage1Scene");
        });

    }

    public override void HidePanel()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowPanel()
    {
        throw new System.NotImplementedException();
    }
}
