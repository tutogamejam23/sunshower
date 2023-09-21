using DG.Tweening;

using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Sunshower.UI
{

    [RequireComponent(typeof(CanvasGroup))]
    public class UIGameSettings : MonoBehaviour
    {
        public Resolution[] SupportResolutions { get; private set; }

        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullScreenModeDropdown;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private AudioMixer audioMixer;

        private void Awake()
        {
            SupportResolutions = Screen.resolutions
                .Where(r => r.height >= 720)
                // .Where(r => r.refreshRateRatio.Equals(Screen.mainWindowDisplayInfo.refreshRate))
                .ToArray();

            closeButton.onClick.AddListener(Close);
            InitializeResolutionSettings(resolutionDropdown);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            InitializeFullScreenSettings();
            fullScreenModeDropdown.onValueChanged.AddListener(OnFullScreenModeChanged);
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        private void OnEnable()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            var transform = GetComponent<RectTransform>();
            transform.anchoredPosition = Vector2.up * 100f;
            transform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1f, 0.5f);

            bgmSlider.value = audioMixer.GetFloat("Volume_BGM", out var bgmVolume) ? bgmVolume : 0f;
            sfxSlider.value = audioMixer.GetFloat("Volume_SFX", out var sfxVolume) ? sfxVolume : 0f;

            closeButton.interactable = true;
        }

        private void InitializeResolutionSettings(TMP_Dropdown dropdown)
        {
            var resolutions = SupportResolutions;
            var current = Screen.currentResolution;

            var options = new List<TMP_Dropdown.OptionData>();
            var idx = 0;
            for (var i = 0; i < resolutions.Length; i++)
            {
                var resolution = resolutions[i];
                var option = new TMP_Dropdown.OptionData(resolution.ToString());
                options.Add(option);

                if (resolution.width == current.width &&
                    resolution.height == current.height &&
                    resolution.refreshRateRatio.Equals(current.refreshRateRatio))
                {
                    idx = i;
                }
            }

            dropdown.options = options;
            dropdown.SetValueWithoutNotify(idx);
        }

        private void InitializeFullScreenSettings()
        {
            var options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("전체 화면"),
                new TMP_Dropdown.OptionData("창 모드")
            };

            fullScreenModeDropdown.options = options;
            fullScreenModeDropdown.SetValueWithoutNotify(Screen.fullScreen ? 0 : 1);
        }


        private void OnResolutionChanged(int index)
        {
            var resolution = SupportResolutions[index];
            Debug.Log(resolution.ToString());
            // Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            //var current = Screen.currentResolution;
            //for (int i = 0; i < SupportResolutions.Length; i++)
            //{
            //    var r = SupportResolutions[i];
            //    if (r.width == current.width &&
            //        r.height == current.height &&
            //        r.refreshRateRatio.Equals(current.refreshRateRatio))
            //    {
            //        resolutionDropdown.value = i;
            //        break;
            //    }
            //}
            //fullScreenModeDropdown.value = Screen.fullScreen ? 0 : 1;
        }

        private void OnFullScreenModeChanged(int index)
        {
            Screen.fullScreen = index == 0;

            //var current = Screen.currentResolution;
            //for (int i = 0; i < SupportResolutions.Length; i++)
            //{
            //    var r = SupportResolutions[i];
            //    if (r.width == current.width &&
            //        r.height == current.height &&
            //        r.refreshRateRatio.Equals(current.refreshRateRatio))
            //    {
            //        resolutionDropdown.value = i;
            //        break;
            //    }
            //}
            //fullScreenModeDropdown.value = Screen.fullScreen ? 0 : 1;
        }

        private void OnBGMVolumeChanged(float value)
        {
            audioMixer.SetFloat("Volume_BGM", value);
            // Debug.Log(_audioMixer.GetFloat("Volume_BGM", out var bgmVolume) ? bgmVolume : 0f);
        }

        private void OnSFXVolumeChanged(float value)
        {
            audioMixer.SetFloat("Volume_SFX", value);
        }

        public void Close()
        {
            closeButton.interactable = false;
            DOTween.Sequence()
                .Append(GetComponent<RectTransform>().DOAnchorPos(Vector2.up * 100f, 0.5f).SetEase(Ease.OutBack))
                .Join(GetComponent<CanvasGroup>().DOFade(0f, 0.5f))
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
