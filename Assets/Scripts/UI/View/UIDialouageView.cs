using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    [SerializeField] private string[] printContent; //#출력할 내용

    private int currentIndex = 0; // 현재 출력 중인 문자열의 인덱스
    private readonly StringBuilder currentText = new(); // 현재까지 타이핑된 텍스트

    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Dialouage, this);
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(TypeText());
    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }


    private IEnumerator TypeText()
    {
        // 배열에서 문자열을 한 글자씩 출력
        for (int i = 0; i < printContent[currentIndex].Length; i++)
        {
            currentText.Append(printContent[currentIndex][i]);
            ContentTxt.text = currentText.ToString();
            yield return new WaitForSeconds(typingSpeed);
        }

        // 다음 문자열을 출력하기 위해 인덱스 증가
        currentIndex++;

        // 모든 문자열을 출력한 경우 다음 작업을 수행하거나 반복 여부를 확인할 수 있습니다.
        if (currentIndex < printContent.Length)
        {
            // 다음 문자열 출력을 시작
            StartCoroutine(TypeText());
        }
        else
        {
            // 모든 문자열을 출력한 후에 실행할 작업을 수행 (예: 다음 장면으로 전환)
            SceneManager.LoadScene("Stage1Scene", LoadSceneMode.Single);
        }
    }
}
