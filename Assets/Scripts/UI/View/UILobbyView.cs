using System;
using System.Collections;
using System.Collections.Generic;
using Sunshower;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// Lobby View에 관리를 담당합니다.
/// </summary>
public class UILobbyView : UIView
{
    [SerializeField] private Button settingBtn;
    [SerializeField] private Image hpImg;
    [SerializeField] private Image hpDiffImg;
    [SerializeField] private Image hpValueImg;
    [SerializeField] private Image processValueImg;
    [SerializeField] private TMP_Text costText;

    private Tween _hpChangedTween;
    private Tween _costChangedTween;

    private void Awake()
    {
        UIManager.Instance.RegisterPanel(PanelType.Lobby, this);
        if (Stage.Instance != null)
        {
            Stage.Instance.PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
        }
    }

    protected override void Start()
    {
        base.Start();
        costText.text = string.Empty;
    }

    private void OnPlayerSpawned(object sender, Player spawnedPlayer)
    {
        spawnedPlayer.OnHPChanged += OnPlayerHPChanged;
        spawnedPlayer.OnCostChanged += OnPlayerCostChanged;
    }

    private void OnPlayerCostChanged(object sender, int changedCost)
    {
        costText.text = changedCost.ToString();
        if (_costChangedTween != null && _costChangedTween.IsPlaying())
        {
            _costChangedTween.Complete();
        }
        _costChangedTween = costText.rectTransform.DOPunchScale(0.1f * Vector3.up, 0.3f);
    }

    private void OnPlayerHPChanged(object sender, (int previous, int current) changedHP)
    {

        var player = sender as Player;
        hpValueImg.fillAmount = (float)changedHP.current / player.Info.HP;
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

    public override void HidePanel()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowPanel()
    {
        throw new System.NotImplementedException();
    }
}
