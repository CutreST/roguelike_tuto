using Godot;
using System;
using Entities.Components.Items;

namespace Entities.Components
{

    public class GearComponent : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }
        private Item[] _items;

        //TODO: refence onto system, event script or separate script???? >o<!
        private AttackComp _attackComp;


        //event classto refresh UI and values

        public void EquipItem(in Item item)
        {

            if (_items[(int)item.MyType] != null)
            {
                this.UnequipItem(item.MyType);
            }

            _items[(int)item.MyType] = item;
            //refresh event, other class, maybe?
            for (int i = 0; i < item.Mods.Count; i++)
            {
                this.ModifyMod(item.Mods[i], true);
            }

        }


        private void ModifyMod(in ItemMods itemMod, in bool isAdding)
        {
            int trueValue = isAdding ? 1 * itemMod.Value : -1 * itemMod.Value;


            switch (itemMod.MyType)
            {
                case ItemMods.ModType.ATTACK:
                    _attackComp.Attack += trueValue;
                    break;
                case ItemMods.ModType.DEFFENSE:
                    _attackComp.Deffense += trueValue;
                    break;
                case ItemMods.ModType.HEALTH:
                    _attackComp.Health += trueValue;
                    break;
            }
        }

        public void UnequipItem(in Item.ItemType itemType)
        {
            Item temp = _items[(int)itemType];
            for (int i = 0; i < temp.Mods.Count; i++){
                this.ModifyMod(temp.Mods[i], false);
            }
            //refresh event
        }

        #region Godot methods
        public override void _EnterTree()
        {
            base._EnterTree();
            this.OnAwake();
        }
        #endregion


        #region Component Node methods
        public void OnAwake()
        {
            _items = new Item[(int)Item.ItemType.END];

            if (MyEntity.TryGetIComponentNode<AttackComp>(out _attackComp) == false)
            {
                Messages.GetComponentFailed("AttackComp", base.Name);
                MyEntity.TryRemoveIComponentNode(this);
                base.QueueFree();
                return;
            }
        }

        public void OnSetFree()
        {

        }

        public void Ontart()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
