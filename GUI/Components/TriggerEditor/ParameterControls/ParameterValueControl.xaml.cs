using BetterTriggers.WorldEdit;
using Model.SaveableData;
using Model.War3Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterValueControl : UserControl, IParameterControl
    {
        private ListViewItem selectedItem;
        private IValueControl valueControl;

        public ParameterValueControl(string returnType)
        {
            InitializeComponent();

            List<Value> values;
            switch (returnType)
            {
                case "boolean":
                    this.valueControl = new ValueControlBoolean();
                    break;
                case "integer":
                    this.valueControl = new ValueControlInteger();
                    break;
                case "real":
                    this.valueControl = new ValueControlReal();
                    break;
                case "StringExt":
                    this.valueControl = new ValueControlString();
                    break;
                case "unitcode":
                    this.valueControl = new ValueControlUnitTypes();
                    break;
                case "abilcode":
                    var abilities = AbilityTypes.GetAbilitiesAll();
                    values = abilities.Select(a => new Value() { identifier = a.AbilCode, returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "buffcode":
                    var buffs = BuffData.GetBuffsAll();
                    values = buffs.Select(b => new Value() { identifier = b.BuffCode, returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "destructablecode":
                    var destructibles = DestructibleTypes.GetDestructiblesTypesAll();
                    values = destructibles.Select(d => new Value() { identifier = d.DestCode, returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "techcode":
                    var upgrades = UpgradeTypes.GetUpgradesAll();
                    values = upgrades.Select(up => new Value() { identifier = up.UpgradeCode, returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "itemcode":
                    var items = ItemData.GetItemsAll();
                    values = items.Select(i => new Value() { identifier = i.ItemCode, returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "unit":
                    var units = Units.Load();
                    values = units.Select(u => new Value() { identifier = u.ToString(), returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "destructable":
                    var mapDestructibles = Destructibles.Load();
                    values = mapDestructibles.Select(dest => new Value() { identifier = dest.ToString(), returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "rect":
                    var rects = Regions.Load();
                    values = rects.Select(rect => new Value() { identifier = rect.ToString(), returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "camerasetup":
                    var cameras = Cameras.Load();
                    values = cameras.Select(c => new Value() { identifier = c.ToString(), returnType = returnType }).ToList();
                    this.valueControl = new ValueControlGeneric(values);
                    break;
                case "modelfile":
                    this.valueControl = new ValueControlModels();
                    break;
                default:
                    break;
            }

            if (this.valueControl == null)
                return;

            var control = (UserControl)this.valueControl;
            this.grid.Children.Add(control);
            Grid.SetRow(control, 0);
            Grid.SetColumn(control, 0);
            control.Margin = new Thickness(10, 10, 10, 10);
            control.HorizontalAlignment = HorizontalAlignment.Stretch;
            control.VerticalAlignment = VerticalAlignment.Stretch;
            control.VerticalContentAlignment = VerticalAlignment.Stretch;
            control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        public int GetElementCount()
        {
            return valueControl.GetElementCount();
        }

        public Parameter GetSelectedItem()
        {
            return valueControl.GetSelected();
        }

        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
        }

        public bool ValueControlExists()
        {
            return valueControl != null;
        }
    }
}
