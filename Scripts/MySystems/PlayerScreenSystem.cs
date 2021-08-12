using Godot;
using System;

namespace MySystems
{

    public class PlayerScreenSystem : System_Base
    {
        private UI.PlayerScreen _playerScreen;

        private readonly string PLAYER_PATH = "res://Scenes/UI/PlayerScreen.tscn";

        private void Init()
        {
            //first, cheeck if a playerscreen already exists
            Node n = MyManager.NodeManager.GetParent();
            _playerScreen = n.TryGetFromChild_Rec<UI.PlayerScreen>();

            if (_playerScreen != null)
            {
                Messages.Print("yelooooow, is it me");
            }
            else
            {
                PackedScene sc = GD.Load<PackedScene>(PLAYER_PATH);
                _playerScreen = sc.Instance<UI.PlayerScreen>();
                n.AddChild(_playerScreen);
                n.CallDeferred("add_child", _playerScreen);
            }
        }

        #region AttackComp methods
        public void UpdateHealth(in int health) => _playerScreen.CallDeferred("UpdateHealth", health); /*_playerScreen.UpdateHealth(health);*/

        public void UpdateMaxHealth(in int maxHealt) => _playerScreen.CallDeferred("UpdateMaxHealth", maxHealt); /*_playerScreen.UpdateMaxHealth(maxHealt);*/

        public void UpdateAttack(in int attack) => _playerScreen.CallDeferred("UpdateAttack", attack); /*_playerScreen.UpdateAttack(attack);*/

        public void UpdateDeffense(in int deffense) => _playerScreen.CallDeferred("UpdateDeffense", deffense);/*_playerScreen.UpdateDeffense(deffense);*/
        #endregion


        #region SystemMethods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            this.Init();
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
        }
        #endregion
    }
}
