using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunshower.UI
{
    public class UIStageView : UIView
    {
        [SerializeField] private UIStageResult stageResultUI;

        private void Awake()
        {
            Debug.Assert(stageResultUI);
        }

        protected override void Start()
        {
            base.Start();
            if (Stage.Instance == null)
            {
                return;
            }

            Stage.Instance.EndState.OnStageEnded += OnStageEnded;
        }

        private void OnStageEnded(object sender, StageResult result)
        {
            Debug.Log("Stage Ended!");
            // stageResultUI.gameObject.SetActive(true);
            stageResultUI.ShowResultPanel(result);
        }

        public void ExitStage()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
