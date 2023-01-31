using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
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
                case "string":
                case "animationname":
                case "attachpoint":
                case "stringnoformat":
                case "aiscript":
                case "anyfile":
                case "preloadfile":
                case "imagefile":
                    this.valueControl = new ValueControlString(returnType);
                    break;
                case "unitcode":
                    this.valueControl = new ValueControlUnitTypes();
                    break;
                case "abilcode":
                case "heroskillcode":
                    var abilities = ControllerMapData.GetAbilitiesAll();
                    values = abilities.Select(a => new Value() { value = a.AbilCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, returnType);
                    break;
                case "buffcode":
                    var buffs = ControllerMapData.GetBuffsAll();
                    values = buffs.Select(b => new Value() { value = b.BuffCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "buffcode");
                    break;
                case "destructablecode":
                    var destructibles = ControllerMapData.GetDestTypesAll();
                    values = destructibles.Select(d => new Value() { value = d.DestCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructablecode");
                    break;
                case "doodadcode":
                    var doodads = ControllerMapData.GetDoodadTypesAll();
                    values = doodads.Select(d => new Value() { value = d.DoodCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "doodadcode");
                    break;
                case "techcode":
                    var upgrades = ControllerMapData.GetUpgradeTypesAll();
                    values = upgrades.Select(up => new Value() { value = up.UpgradeCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "techcode");
                    break;
                case "itemcode":
                    var itemTypes = ControllerMapData.GetItemTypesAll();
                    values = itemTypes.Select(i => new Value() { value = i.ItemCode }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "itemcode");
                    break;
                case "item":
                    var items = ControllerMapData.GetMapItems();
                    values = items.Select(i => new Value() { value = $"{i.ToString()}_{i.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "item");
                    break;
                case "unit":
                    var units = ControllerMapData.GetMapUnits();
                    values = units.Select(u => new Value() { value = $"{u.ToString()}_{u.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "unit");
                    break;
                case "destructable":
                    var mapDestructibles = ControllerMapData.GetMapDests();
                    values = mapDestructibles.Select(dest => new Value() { value = $"{dest.ToString()}_{dest.CreationNumber.ToString("D4")}" }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "destructable");
                    break;
                case "rect":
                    var rects = ControllerMapData.GetMapRegions();
                    values = rects.Select(rect => new Value() { value = rect.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "rect");
                    break;
                case "camerasetup":
                    var cameras = ControllerMapData.GetMapCameras();
                    values = cameras.Select(c => new Value() { value = c.ToString() }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "camerasetup");
                    break;
                case "sound":
                    var sound = ControllerMapData.GetMapSounds();
                    values = sound.Select(s => new Value() { value = s.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "sound");
                    break;
                case "musicfile":
                    var music = ControllerMapData.GetMapMusic();
                    values = music.Select(m => new Value() { value = m.Name }).ToList();
                    this.valueControl = new ValueControlGeneric(values, "musicfile");
                    break;
                case "modelfile": // TODO:
                    this.valueControl = new ValueControlModels();
                    break;
                case "trigger":
                    var triggers = ControllerTrigger.GetTriggersAll();
                    values = triggers.Select(trig => new Value() { value = trig.GetId().ToString() }).ToList();
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

        public void SetDefaultSelection(Parameter parameter)
        {
            valueControl.SetDefaultSelection(parameter);
        }

        public int GetElementCount()
        {
            return valueControl.GetElementCount();
        }

        public Parameter GetSelectedItem()
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
