using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Dialouage View에 관리를 담당합니다.
/// </summary>
public class UIDialouageView : UIView
{
    [SerializeField] private TextMeshProUGUI ContentTxt;
    [SerializeField] private float typingSpeed;

    public override void HidePanel()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowPanel()
    {
        throw new System.NotImplementedException();
    }


    public IEnumerator TypingEffect()
    {
        //TODD : 한 글자씩 타이핑 or 문단마다 fade in/out

        yield return null;
    }
}
