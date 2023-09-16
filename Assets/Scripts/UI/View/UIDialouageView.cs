using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dialouage View에 관리를 담당합니다.
/// </summary>
public class UIDialouageView : UIView
{
    [SerializeField] private TextMeshProUGUI ContentTxt;
    [SerializeField] private float typingSpeed;
    [SerializeField] private string printContent; //#출력할 내용

    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Dialouage, this);
    }

    protected override void Start()
    {
        base.Start();

        SceneManager.LoadScene("UIScene");
        SceneManager.LoadScene("Stage1Scene", LoadSceneMode.Additive);
    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }


    public IEnumerator TypingEffect()
    {
        //TODD : 한 글자씩 타이핑 or 문단마다 fade in/out

        yield return null;
    }
}
