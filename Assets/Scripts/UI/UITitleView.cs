using DG.Tweening;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunshower.UI
{

    /// <summary>
    /// Title View에 관리를 담당합니다.
    /// </summary>
    public class UITitleView : UIView
    {
        [SerializeField] private Image fadeImg;

        private void Awake()
        {
            UIManager.Instance.RegisterPanel(PanelType.Title, this);
        }

        public void LoadScene(string sceneName)
        {
            fadeImg.DOFade(1f, 0.5f).OnComplete(() =>
            {
                SceneManager.LoadSceneAsync(sceneName);
            });
        }

        public void Quit()
        {
            Application.Quit();
        }

        public override void HidePanel()
        {
        }

        public override void ShowPanel()
        {
        }
    }
}
