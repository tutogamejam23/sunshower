using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace Sunshower.UI
{
    public class UIPlayerInfo : MonoBehaviour
    {
        [SerializeField] private Image hpImg;
        [SerializeField] private Image hpDiffImg;
        [SerializeField] private Image hpValueImg;

        private Tween _hpChangedTween;

        private void Awake()
        {
            if (Stage.Instance != null)
            {
                Stage.Instance.PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
            }
        }

        private void OnPlayerSpawned(object sender, Player spawnedPlayer)
        {
            spawnedPlayer.OnHPChanged += OnPlayerHPChanged;
        }

        private void OnPlayerHPChanged(object sender, (int previous, int current) changedHP)
        {

            var player = sender as Player;
            hpValueImg.fillAmount = (float)changedHP.current / player.Data.HP;
            if (player.LogEnabled)
            {
                Debug.Log($"HP Changed: {changedHP.previous} -> {changedHP.current}");
            }

            if (changedHP.current < changedHP.previous)
            {
                hpDiffImg.color = Color.red;
                if (_hpChangedTween != null && _hpChangedTween.IsPlaying())
                {
                    _hpChangedTween.Complete();
                }
                _hpChangedTween = hpImg.rectTransform.DOShakePosition(0.3f, strength: 5f);
            }
            else if (changedHP.current > changedHP.previous)
            {
                hpDiffImg.color = Color.green;
                //hpImg.rectTransform.DOShakePosition(0.3f);
            }
        }

        private void Update()
        {
            if (hpValueImg.fillAmount != hpDiffImg.fillAmount)
            {
                hpDiffImg.fillAmount = Mathf.Lerp(hpDiffImg.fillAmount, hpValueImg.fillAmount, Time.deltaTime * 5f);
            }
        }
    }
}
