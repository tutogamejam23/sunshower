using DG.Tweening;

using System.Collections.Generic;

using TMPro;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

namespace Sunshower.UI
{
    public class UIPlayerSkill : MonoBehaviour
    {
        [SerializeField] private Button yeoubul;
        [SerializeField] private Button yeougusul;
        [SerializeField] private Button kaeng;
        [SerializeField] private Button yeoubi;
        [SerializeField] private Image costImage;
        [SerializeField] private List<Sprite> costDiffTextures;
        [SerializeField] private TMP_Text costText;

        private static readonly int YeobulID = 1000;
        private static readonly int KaengID = 1001;
        private static readonly int YeougusulID = 1002;
        private static readonly int YeoubiID = 1003;

        private readonly Dictionary<int, Skill> _skills = new();

        private bool _targetingGround;
        private Tween _costChangedTween;
        private float _costDivide;

        private void Awake()
        {
        }

        private void Start()
        {
            costImage.sprite = costDiffTextures[0];
            costText.text = "0";

            if (Stage.Instance != null)
            {
                Stage.Instance.PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
            }
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
            if (Stage.Instance == null)
            {
                return;
            }
            var player = Stage.Instance.ActivePlayer;
            var interactable = player != null && player.CurrentState == player.PlayerIdleState;

            yeoubul.interactable = interactable && _skills[YeobulID].CanUse();
            yeougusul.interactable = interactable && _skills[YeougusulID].CanUse();
            kaeng.interactable = interactable && _skills[KaengID].CanUse();
            yeoubi.interactable = interactable && _skills[YeoubiID].CanUse();
        }

        private void TargetingGround()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _targetingGround = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (Stage.Instance == null)
                {
                    _targetingGround = false;
                }
                else
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, LayerMask.GetMask("Ground"));
                    if (hit)
                    {
                        var skill = _skills[YeougusulID];
                        skill.Manager.UsePosition = new Vector3(hit.point.x, Stage.Instance.MobSpawner.EnemySpawnPosition.y + 1.5f, 0);
                        _targetingGround = false;
                        Stage.Instance.ActivePlayer.ExecuteSkill(YeougusulID);
                    }
                    else
                    {
                        yeougusul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                    }
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
            if (!yeoubul.interactable)
            {
                return;
            }
            if (Stage.Instance == null)
            {
                return;
            }

            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(YeobulID);
        }

        public void Yeougusul()
        {
            if (!yeougusul.interactable)
            {
                return;
            }


            if (_targetingGround)
            {
                _targetingGround = false;
                return;
            }

            if (Stage.Instance != null)
            {
                var skill = _skills[YeougusulID];
                if (!skill.CanUse())
                {
                    yeoubul.GetComponent<RectTransform>().DOShakePosition(0.3f, strength: 3f);
                    return;
                }
            }
            _targetingGround = true;

        }

        public void Kaeng()
        {
            if (!kaeng.interactable)
            {
                return;
            }
            if (Stage.Instance == null)
            {
                return;
            }

            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(KaengID);
        }

        public void Yeoubi()
        {
            if (!yeoubi.interactable)
            {
                return;
            }
            if (Stage.Instance == null)
            {
                return;
            }

            var player = Stage.Instance.ActivePlayer;
            if (player == null)
            {
                return;
            }
            player.ExecuteSkill(YeoubiID);
        }
    }
}
