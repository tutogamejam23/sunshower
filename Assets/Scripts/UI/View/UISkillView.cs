using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        private static readonly int YeobulID = 1000;
        private static readonly int YeougusulID = 1001;
        private static readonly int KaengID = 1002;
        private static readonly int YeoubiID = 1003;

        private readonly Dictionary<int, Skill> _skills = new();

        private bool _targetingGround;

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
            if (player != null)
            {
                yeoubul.interactable = _skills[YeobulID].CanUse();
                yeougusul.interactable = _skills[YeougusulID].CanUse();
                kaeng.interactable = _skills[KaengID].CanUse();
                yeoubi.interactable = _skills[YeoubiID].CanUse();
            }
            else
            {
                yeoubul.interactable = false;
                yeougusul.interactable = false;
                kaeng.interactable = false;
                yeoubi.interactable = false;
            }
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

            yeoubul.enabled = !_targetingGround;
            kaeng.enabled = !_targetingGround;
            yeoubi.enabled = !_targetingGround;
        }

        private void OnPlayerSpawned(object sender, Player e)
        {
            foreach (var skill in e.SkillManager.Skills)
            {
                _skills.Add(skill.Info.ID, skill);
            }

            // yeoubul.onClick.AddListener(() => Yeoubul());
            // yeougusul.onClick.AddListener(() => Yeougusul());
            // kaeng.onClick.AddListener(() => Kaeng());
            // yeoubi.onClick.AddListener(() => Yeoubi());
        }

        public void Yeoubul()
        {
            Debug.Log("Yeoubul");
            var skill = _skills[YeobulID];
            if (!skill.CanUse())
            {
                yeoubul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                return;
            }
            skill.Execute();
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
            var skill = _skills[KaengID];
            if (!skill.CanUse())
            {
                yeoubul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                return;
            }
            skill.Execute();
        }

        public void Yeoubi()
        {
            var skill = _skills[YeoubiID];
            if (!skill.CanUse())
            {
                yeoubul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                return;
            }
            skill.Execute();
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
}
