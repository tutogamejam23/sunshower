using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// Dialouage View에 관리를 담당합니다.
/// </summary>
public class UIStartDialogueView : UIView
{
    [SerializeField] private Button skipBtn;
    [SerializeField] private Image fadeImg;
    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Dialouage, this);
        skipBtn.onClick.AddListener(() => SkipBtn());
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        yield return new WaitForSeconds(43f);

        // SceneManager.LoadScene("UIScene");
        SceneManager.LoadScene("Stage1Scene", LoadSceneMode.Single);
    }

    void SkipBtn()
    {
        fadeImg.DOFade(1f, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Stage1Scene", LoadSceneMode.Single);
        });

    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }
}
