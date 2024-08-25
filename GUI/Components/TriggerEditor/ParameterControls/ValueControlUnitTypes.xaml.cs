using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using BetterTriggers.Models.War3Data;
using System.Windows.Input;
using BetterTriggers.Models.EditorData;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    enum CategoryType
    {
        Unit,
        Hero,
        Building,
        Special
    }

    enum CategoryRace
    {
        Human,
        Orc,
        Undead,
        NightElf,
        Neutral,
        Naga,
    }

    enum CategoryFamily
    {
        Melee,
        Campaign,
        Custom
    }

    public partial class ValueControlUnitTypes : UserControl, IValueControl, ISearchablesObserverList
    {
        private List<UnitType> unitData = new List<UnitType>();
        private CategoryRace selectedRace = CategoryRace.Human;
        private Value selectedType;
        private int elementCount;
        private ButtonUnitType selectedButton;
        private UnitType defaultSelected;

        private Searchables searchables;

        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlUnitTypes()
        {
            InitializeComponent();

            unitData = UnitTypes.GetAll();
            comboboxRace.SelectedIndex = 0;

            this.KeyDown += ValueControlUnitTypes_KeyDown;
        }

        private void ValueControlUnitTypes_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter || selectedType != null)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            // TODO:
            int selectedIndex = 0;
            int i = 0;
            bool found = false;
            while (!found && i < unitData.Count)
            {
                if (unitData[i].Id == parameter.value)
                    found = true;
                else
                    i++;
            }
            if (!found)
            {
                comboboxRace.SelectedIndex = selectedIndex;
                return;
            }

            defaultSelected = unitData[i];
            int categoryIndex = 0;
            if (defaultSelected.Race == "human")
                categoryIndex = 0;
            else if (defaultSelected.Race == "orc")
                categoryIndex = 1;
            else if (defaultSelected.Race == "undead")
                categoryIndex = 2;
            else if (defaultSelected.Race == "nightelf")
                categoryIndex = 3;
            else if (defaultSelected.Race == "naga")
                categoryIndex = 5;
            else
                categoryIndex = 4;

            comboboxRace.SelectedIndex = categoryIndex;
            SetSelectedRace(categoryIndex);
            RePopulate();
        }

        public Parameter GetSelected()
        {
            return selectedType;
        }

        public int GetElementCount()
        {
            return elementCount;
        }

        private void SetSelectedRace(int index)
        {
            // This is clean af kapp
            if (index == 0)
                selectedRace = CategoryRace.Human;
            else if (index == 1)
                selectedRace = CategoryRace.Orc;
            else if (index == 2)
                selectedRace = CategoryRace.Undead;
            else if (index == 3)
                selectedRace = CategoryRace.NightElf;
            else if (index == 4)
                selectedRace = CategoryRace.Neutral;
            else if (index == 5)
                selectedRace = CategoryRace.Naga;
        }

        private void comboboxRace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!this.IsLoaded)
            {
                return;
            }

            SetSelectedRace(comboboxRace.SelectedIndex);
            RePopulate();
        }

        private void RePopulate()
        {
            itemControlUnits.Items.Clear();
            itemControlHeroes.Items.Clear();
            itemControlBuildings.Items.Clear();
            itemControlSpecial.Items.Clear();
            elementCount = 0;

            List<Searchable> objects = new List<Searchable>();

            for (int i = 0; i < unitData.Count; i++)
            {
                var unit = unitData[i];
                var race = unit.Race;

                CategoryRace unitRace;

                if (race == "human")
                    unitRace = CategoryRace.Human;
                else if (race == "orc")
                    unitRace = CategoryRace.Orc;
                else if (race == "undead")
                    unitRace = CategoryRace.Undead;
                else if (race == "nightelf")
                    unitRace = CategoryRace.NightElf;
                else if (race == "naga")
                    unitRace = CategoryRace.Naga;
                else
                    unitRace = CategoryRace.Neutral;

                if (unitRace == selectedRace)
                {
                    string unitCategory = unit.Sort.Substring(1, 1);
                    var btn = new ButtonUnitType(unit);
                    btn.Category = unitCategory;
                    btn.Click += Btn_Click;

                    objects.Add(new Searchable()
                    {
                        Object = btn,
                        Words = new List<string>()
                            {
                                unit.Id.ToLower(),
                                unit.Name.Name.ToLower(),
                                unit.Name.Propernames.ToLower(),
                                unit.Name.EditorSuffix.ToLower()
                            }
                    });

                    elementCount++;
                }
            }

            searchables = new Searchables(objects);
            searchables.AttachList(this);

            searchables.Search("");
        }


        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedButton != null)
                selectedButton.RemoveSelectedBorder();

            var btnClicked = (ButtonUnitType)e.Source;
            selectedButton = btnClicked;
            SetSelectedType(btnClicked.UnitType);

            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        private void SetSelectedType(string unitcode)
        {
            this.selectedType = new Value()
            {
                value = unitcode,
            };
        }

        public void Update()
        {
            itemControlUnits.Items.Clear();
            itemControlHeroes.Items.Clear();
            itemControlBuildings.Items.Clear();
            itemControlSpecial.Items.Clear();

            var units = searchables.GetObjects();
            for (int i = 0; i < units.Count; i++)
            {
                var btn = (ButtonUnitType)units[i].Object;
                if (btn.isSpecial)
                    itemControlSpecial.Items.Add(btn);
                else if (btn.Category == "2")
                    itemControlUnits.Items.Add(btn);
                else if (btn.Category == "1")
                    itemControlHeroes.Items.Add(btn);
                else if (btn.Category == "3")
                    itemControlBuildings.Items.Add(btn);
                else
                    itemControlSpecial.Items.Add(btn);

                if (defaultSelected != null && defaultSelected.Id == btn.UnitType)
                {
                    selectedButton = btn;
                    btn.AddSelectedBorder();
                    SetSelectedType(btn.UnitType);
                }
            }
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchables.Search(textBoxSearch.Text);
        }
    }
}
