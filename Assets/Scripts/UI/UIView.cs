using UnityEngine;

namespace Sunshower.UI
{

    /// <summary>
    /// 모든 Panel의 On/Off 기능을 담당한다.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : MonoBehaviour, IUserInterfaceObject
    {
        private CanvasGroup CG;

        public virtual void ShowPanel()
        {
        }

        public virtual void HidePanel()
        {
        }

        protected virtual void Start()
        {
            CG = GetComponent<CanvasGroup>();
        }

        public void SetCG(bool isActive, bool blockRaycast = true)
        {
            CG.alpha = isActive ? 1 : 0;
            CG.interactable = isActive;
            CG.blocksRaycasts = isActive && blockRaycast;
        }

        public bool IsActive()
        {
            return CG.alpha != 0;
        }

        public void Enter()
        {
            ShowPanel();
        }

        public void Exit()
        {
            HidePanel();
        }
    }
}
