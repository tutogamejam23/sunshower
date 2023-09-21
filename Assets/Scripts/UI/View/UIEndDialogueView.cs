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
public class UIEndDialogueView : UIView
{
    [SerializeField] private Image endImg;
    [SerializeField] private Image fadeImg;
    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Dialouage, this);
    }

    protected override void Start()
    {
        base.Start();

        endImg.enabled = false;
        fadeImg.enabled = false;
        StartCoroutine(PlayVideo());
        StartCoroutine(ShowImage());
    }

    IEnumerator PlayVideo()
    {
        yield return new WaitForSeconds(37f);

        // SceneManager.LoadScene("UIScene");
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    IEnumerator ShowImage()
    {
        yield return new WaitForSeconds(27f);
        
        fadeImg.enabled = true;
        endImg.enabled = true;
        fadeImg.DOFade(0f, 3f);

        yield return new WaitForSeconds(5f);
        fadeImg.DOFade(1f, 3f);

    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }
}
