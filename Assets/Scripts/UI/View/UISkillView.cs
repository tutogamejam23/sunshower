using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Sunshower
{
    public class UISkillView : UIView
    {
        [SerializeField] private Button yeoubul;
        [SerializeField] private Button yeougusul;
        [SerializeField] private Button kaeng;
        [SerializeField] private Button yeoubi;
        [SerializeField] private Image costImage;
        [SerializeField] private List<Sprite> costDiffTextures;
        [SerializeField] private TMP_Text costText;

        private static readonly int YeobulID = 1000;
        private static readonly int YeougusulID = 1001;
        private static readonly int KaengID = 1002;
        private static readonly int YeoubiID = 1003;

        private readonly Dictionary<int, Skill> _skills = new();

        private bool _targetingGround;
        private Tween _costChangedTween;
        private float _costDivide;

        private void Awake()
        {
            UIManager.Instance.RegisterPanel(PanelType.Skill, this);
            if (Stage.Instance != null)
            {
                Stage.Instance.PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
            }
        }

        protected override void Start()
        {
            base.Start();

            costImage.sprite = costDiffTextures[0];
            costText.text = "0";
        }

        private void Update()
        {
            if (_targetingGround)
            {
                TargetingGround();
            }

            CheckSkillUse();
        }

        private void CheckSkillUse()
        {
            var player = Stage.Instance.ActivePlayer;
            var interactable = player != null && player.CurrentState == player.PlayerIdleState;

            yeoubul.interactable = interactable;
            yeougusul.interactable = interactable;
            kaeng.interactable = interactable;
            yeoubi.interactable = interactable;
        }

        private void TargetingGround()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _targetingGround = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Ground")))
                {
                    var skill = _skills[YeougusulID];
                    skill.Manager.UsePosition = new Vector3(hit.point.x, Stage.Instance.ActivePlayer.transform.position.y, 0);
                    _targetingGround = false;
                }
                else
                {
                    yeougusul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                }
            }

            yeoubul.interactable = !_targetingGround;
            kaeng.interactable = !_targetingGround;
            yeoubi.interactable = !_targetingGround;
        }

        private void OnPlayerSpawned(object sender, Player spawnedPlayer)
        {
            var data = spawnedPlayer.Data as PlayerData;

            foreach (var skill in spawnedPlayer.SkillManager.Skills)
            {
                _skills.Add(skill.Info.ID, skill);
            }

            _costDivide = (float)data.MaxCost / costDiffTextures.Count;
            spawnedPlayer.OnCostChanged += OnPlayerCostChanged;
        }

        private void OnPlayerCostChanged(object sender, int changedCost)
        {
            costText.text = changedCost.ToString();
            if (_costChangedTween != null && _costChangedTween.IsPlaying())
            {
                _costChangedTween.Complete();
            }
            _costChangedTween = costText.rectTransform.DOPunchScale(0.2f * Vector3.up, 0.3f);

            var index = math.max((int)math.floor(changedCost / _costDivide) - 1, 0);
            costImage.sprite = costDiffTextures[index];
        }

        public void Yeoubul()
        {
            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(YeobulID);
        }

        public void Yeougusul()
        {
            if (_targetingGround)
            {
                _targetingGround = false;
                return;
            }

            var skill = _skills[YeougusulID];
            if (!skill.CanUse())
            {
                yeoubul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                return;
            }

            _targetingGround = true;

        }

        public void Kaeng()
        {
            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(KaengID);
        }

        public void Yeoubi()
        {
            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(YeoubiID);
        }

        public override void HidePanel()
        {
        }

        public override void ShowPanel()
        {
        }
    }
}
