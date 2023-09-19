using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sunshower
{
    public class UISkillView : UIView
    {
        [SerializeField] Button skil1Btn;
        [SerializeField] Button skil2Btn;
        [SerializeField] Button skil3Btn;
        [SerializeField] Button skil4Btn;

        Player player;

        private void Awake()
        {
            UIManager.Instance.RegisterPanel(PanelType.Skill, this);

            player = new Player();
            skil1Btn.onClick.AddListener(() => SetSkillBtn(new FoxFireSkill()));
            skil2Btn.onClick.AddListener(() => SetSkillBtn(new KangSkill()));

            skil4Btn.onClick.AddListener(() => SetSkillBtn(new FoxRainSkill()));
        }

        void SetSkillBtn(ISkill<Player> skill)
        {
            skill.Use(new Player());
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
