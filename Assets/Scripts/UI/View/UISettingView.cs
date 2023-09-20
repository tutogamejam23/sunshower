using System.Collections;
using System.Collections.Generic;
using Sunshower;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Setting View에 관리를 담당합니다.
/// </summary>
public class UISettingView : UIView
{
    [SerializeField] private Button _quitButton;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private AudioMixer _audioMixer;

    private void Awake()
    {
        _bgmSlider.value = _audioMixer.GetFloat("Volume_BGM", out var bgmVolume) ? bgmVolume : 0f;
        _bgmSlider.onValueChanged.AddListener((value) =>
        {
            _audioMixer.SetFloat("Volume_BGM", Mathf.Log10(value) * 20);
            Debug.Log(_audioMixer.GetFloat("Volume_BGM", out var bgmVolume) ? bgmVolume : 0f);
        });

        _sfxSlider.value = _audioMixer.GetFloat("Volume_SFX", out var sfxVolume) ? sfxVolume : 0f;
        _sfxSlider.onValueChanged.AddListener((value) =>
        {
            _audioMixer.SetFloat("Volume_SFX", Mathf.Log10(value) * 20);
        });

        _quitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("TitleScene");
        });
    }

    protected override void Start()
    {
        base.Start();


    }

    public override void HidePanel()
    {
    }

    public override void ShowPanel()
    {
    }
}
