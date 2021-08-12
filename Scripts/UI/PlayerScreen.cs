using Godot;
using System;

namespace UI
{

    public class PlayerScreen : Control
    {
        [Export]
        private readonly string HEALTH_NAME;

        [Export]
        private readonly string HEALTH_MAX_NAME;

        [Export]
        private readonly string ATTACK_NAME;

        [Export]
        private readonly string DEFFENSE_NAME;

        private (Label health, Label maxHealt) _lbl_health;

        private Label _lbl_attack;
        private Label _lbl_deffense;

        #region Godot Methods

        public override void _EnterTree()
        {
            this.InitHealthLabels();
            this.InitAtc_Def_Labels();           
        }

        #endregion
        #region initializers
        private void InitHealthLabels()
        {
            _lbl_health.health = this.TryGetFromChild_Rec<Label>(HEALTH_NAME);
            _lbl_health.maxHealt = this.TryGetFromChild_Rec<Label>(HEALTH_MAX_NAME);
        }



        private void InitAtc_Def_Labels()
        {
            _lbl_attack = this.TryGetFromChild_Rec<Label>(ATTACK_NAME);
            _lbl_deffense = this.TryGetFromChild_Rec<Label>(DEFFENSE_NAME);
        }


        #endregion

        #region AttackComp stats



        public void UpdateHealth(in int health)
        {
            this._lbl_health.health.Text = health.ToString();

            int maxHealt = int.Parse(_lbl_health.maxHealt.Text);

            if (health <= maxHealt * 0.25f)
            {
                _lbl_health.health.SelfModulate = Colors.Red;
            }
            else
            {
                _lbl_health.health.SelfModulate = Colors.White;
            }
        }

        public void UpdateHealth(in int health, in int maxHealth)
        {
            this._lbl_health.maxHealt.Text = maxHealth.ToString();
            this.UpdateHealth(health);
        }

        public void UpdateMaxHealth(int maxHealt)
        {
            int health = int.Parse(_lbl_health.health.Text);
            this.UpdateHealth(health, maxHealt);
        }

        public void UpdateAttack(in int attack)
        {
            _lbl_attack.Text = attack.ToString();
        }

        public void UpdateDeffense(in int deffense)
        {
            _lbl_deffense.Text = deffense.ToString();
        }

        #endregion

    }
}
