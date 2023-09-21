using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace Sunshower.UI
{
    public class UIEndDialogueView : UIView
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
            yield return new WaitForSeconds(25f);

            // SceneManager.LoadScene("UIScene");
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

        public override void HidePanel()
        {
        }

        public override void ShowPanel()
        {
        }
    }
}
