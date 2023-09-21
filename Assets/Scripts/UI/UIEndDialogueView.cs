using DG.Tweening;

using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunshower.UI
{
    public class UIEndDialogueView : UIView
    {
        [SerializeField] private Image endImg;
        [SerializeField] private Image fadeImg;
        [SerializeField] private GameObject creditImg;
        [SerializeField] private Image fadeImg2;
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
            yield return new WaitForSeconds(65f);

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

            yield return new WaitForSeconds(3f);

            creditImg.SetActive(true);

            yield return new WaitForSeconds(25f);

            fadeImg2.DOFade(1f, 3f);

        }
    }
}
