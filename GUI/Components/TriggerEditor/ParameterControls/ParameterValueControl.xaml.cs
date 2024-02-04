using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.War3Data;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ParameterValueControl : UserControl, IParameterControl
    {
        public EventHandler SelectionChanged;
        public EventHandler OK;
        private IValueControl valueControl;

        public ParameterValueControl(string returnType)
        {
            InitializeComponent();

            List<Value_Saveable> values;
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
                case "string":
                case "animationname":
                case "attachpoint":
                case "stringnoformat":
                case "aiscript":
                case "anyfile":
                case "preloadfile":
                case "frametype":
                    this.valueControl = new ValueControlString(returnType);
                    break;
                case "unitcode":
                    this.valueControl = new ValueControlUnitTypes();
                    break;
                case "abilcode":
                case "heroskillcode":
                    var abilities = AbilityTypes.GetAll();
                    values = abilities.Select(a => new Value_Saveable() { value = a.AbilCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, returnType);
                    break;
                case "buffcode":
                    var buffs = BuffTypes.GetAll();
                    values = buffs.Select(b => new Value_Saveable() { value = b.BuffCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "buffcode");
                    break;
                case "destructablecode":
                    var destructibles = DestructibleTypes.GetAll();
                    values = destructibles.Select(d => new Value_Saveable() { value = d.DestCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructablecode");
                    break;
                case "doodadcode":
                    var doodads = DoodadTypes.GetAll();
                    values = doodads.Select(d => new Value_Saveable() { value = d.DoodCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "doodadcode");
                    break;
                case "techcode":
                    var upgrades = UpgradeTypes.GetAll();
                    values = upgrades.Select(up => new Value_Saveable() { value = up.UpgradeCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "techcode");
                    break;
                case "itemcode":
                    var itemTypes = ItemTypes.GetAll();
                    values = itemTypes.Select(i => new Value_Saveable() { value = i.ItemCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "itemcode");
                    break;
                case "item":
                    var items = Units.GetMapItemsAll();
                    values = items.Select(i => new Value_Saveable() { value = $"{i.ToString()}_{i.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "item");
                    break;
                case "unit":
                    var units = Units.GetAll();
                    values = units.Select(u => new Value_Saveable() { value = $"{u.ToString()}_{u.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "unit");
                    break;
                case "destructable":
                    var mapDestructibles = Destructibles.GetAll();
                    values = mapDestructibles.Select(dest => new Value_Saveable() { value = $"{dest.ToString()}_{dest.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructable");
                    break;
                case "rect":
                    var rects = Regions.GetAll();
                    values = rects.Select(rect => new Value_Saveable() { value = rect.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "rect");
                    break;
                case "camerasetup":
                    var cameras = Cameras.GetAll();
                    values = cameras.Select(c => new Value_Saveable() { value = c.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "camerasetup");
                    break;
                case "sound":
                    var sound = Sounds.GetSoundsAll();
                    values = sound.Select(s => new Value_Saveable() { value = s.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "sound");
                    break;
                case "musicfile":
                    var music = Sounds.GetMusicAll();
                    values = music.Select(m => new Value_Saveable() { value = m.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "musicfile");
                    break;
                case "modelfile": // TODO:
                    this.valueControl = new ValueControlModels();
                    break;
                case "imagefile": // TODO:
                    this.valueControl = new ValueControlIcons();
                    break;
                case "trigger":
                    var triggers = Project.CurrentProject.Triggers.GetAll();
                    values = triggers.Select(trig => new Value_Saveable() { value = trig.GetId().ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "trigger");
                    break;
                case "scriptcode":
                    this.valueControl = new ValueControlScript();
                    break;
                case "frameevents":
                    this.valueControl = new ValueControlFrameEvents();
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

            valueControl.SelectionChanged += ValueControl_SelectionChanged;
            valueControl.OK += ValueControl_OK;
        }


        private void ValueControl_SelectionChanged(object sender, EventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        private void ValueControl_OK(object sender, EventArgs e)
        {
            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(Parameter_Saveable parameter)
        {
            valueControl.SetDefaultSelection(parameter);
        }

        public int GetElementCount()
        {
            return valueControl.GetElementCount();
        }

        public Parameter_Saveable GetSelectedItem()
        {
            if (valueControl.GetSelected() == null)
                return null;

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
