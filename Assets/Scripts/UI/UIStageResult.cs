using DG.Tweening;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunshower.UI
{
    public class UIStageResult : MonoBehaviour
    {
        [SerializeField] private CanvasGroup stageClear;
        [SerializeField] private CanvasGroup stageFail;
        [SerializeField] private Ease ease = Ease.OutBounce;
        [SerializeField] private Vector2 move;

        private bool alreadyOpen = false;

        private void Awake()
        {
            stageClear.gameObject.SetActive(false);
            stageFail.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ShowResultPanel(StageResult result)
        {
            if (alreadyOpen)
            {
                return;
            }

            var transform = GetComponent<RectTransform>();
            var canvasGroup = GetComponent<CanvasGroup>();

            var targetPanel = result switch
            {
                StageResult.Clear => stageClear,
                StageResult.Fail => stageFail,
                _ => throw new System.NotImplementedException()
            };

            transform.anchoredPosition = move;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);
            targetPanel.gameObject.SetActive(true);

            transform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(ease);
            canvasGroup.DOFade(1f, 0.5f);

            alreadyOpen = true;

            if (result == StageResult.Clear)
            {
                DOTween.Sequence()
                    .SetDelay(3f)
                    .AppendCallback(() => GoToNext());
            }
        }

        private void GoToNext()
        {
            var stage = Stage.Instance;
            if (stage == null)
            {
                return;
            }

            if (stage.NextStageNumber > 0)
            {
                SceneManager.LoadSceneAsync($"Stage{stage.NextStageNumber}Scene");
            }
            else
            {
                SceneManager.LoadSceneAsync("EndDialogueScene");
            }
        }

        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Exit()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
