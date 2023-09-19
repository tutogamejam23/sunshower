using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Dialouage View에 관리를 담당합니다.
/// </summary>
public class UIDialouageView : UIView
{
    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Dialouage, this);
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        yield return new WaitForSeconds(28f);

        // SceneManager.LoadScene("UIScene");
        SceneManager.LoadScene("Stage1Scene", LoadSceneMode.Single);
    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }
}
