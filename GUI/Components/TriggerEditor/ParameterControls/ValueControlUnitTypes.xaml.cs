using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Model.SaveableData;
using Model.War3Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private Searchables searchables;

        public ValueControlUnitTypes()
        {
            InitializeComponent();

            unitData = UnitTypes.GetUnitTypesAll();
            comboboxRace.SelectedIndex = 0;
        }

        public Parameter GetSelected()
        {
            return selectedType;
        }

        public int GetElementCount()
        {
            return elementCount;
        }

        private void comboboxRace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This is clean af kapp
            if (comboboxRace.SelectedIndex == 0)
                selectedRace = CategoryRace.Human;
            else if (comboboxRace.SelectedIndex == 1)
                selectedRace = CategoryRace.Orc;
            else if (comboboxRace.SelectedIndex == 2)
                selectedRace = CategoryRace.Undead;
            else if (comboboxRace.SelectedIndex == 3)
                selectedRace = CategoryRace.NightElf;
            else if (comboboxRace.SelectedIndex == 4)
                selectedRace = CategoryRace.Neutral;
            else if (comboboxRace.SelectedIndex == 5)
                selectedRace = CategoryRace.Naga;

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
                                unit.Id.ToLower()
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
            this.selectedType = new Value()
            {
                identifier = btnClicked.UnitType,
                returnType = "unitcode",
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
            }
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchables.Search(textBoxSearch.Text);
        }
    }
}
