using BetterTriggers.WorldEditParsers;
using Model.War3Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

    /// <summary>
    /// Interaction logic for ValueControlUnitTypes.xaml
    /// </summary>
    public partial class ValueControlUnitTypes : UserControl, IValueControl
    {
        private List<UnitType> unitData = new List<UnitType>();
        private CategoryRace selectedRace = CategoryRace.Human;
        private CategoryFamily selectedFamily = CategoryFamily.Melee;

        public ValueControlUnitTypes()
        {
            InitializeComponent();

            UnitDataParser parser = new UnitDataParser();
            unitData = parser.ParseUnitData();

            comboboxRace.SelectedIndex = 0;
            comboboxObjectFamily.SelectedIndex = 0;
        }


        private void comboboxRace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This is clean af heheh
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


        private void comboboxObjectFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboboxObjectFamily.SelectedIndex == 0)
                selectedFamily = CategoryFamily.Melee;
            else if (comboboxObjectFamily.SelectedIndex == 1)
                selectedFamily = CategoryFamily.Campaign;
            else if (comboboxObjectFamily.SelectedIndex == 2)
                selectedFamily = CategoryFamily.Custom;

            RePopulate();
        }

        private void RePopulate()
        {
            itemControlUnits.Items.Clear();
            itemControlHeroes.Items.Clear();
            itemControlBuildings.Items.Clear();
            itemControlSpecial.Items.Clear();

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

                CategoryFamily unitFamily;
                var isCampaign = unit.Sort.Substring(0, 1) == "z";
                if (isCampaign)
                    unitFamily = CategoryFamily.Campaign;
                else
                    unitFamily = CategoryFamily.Melee;


                if (unitRace == selectedRace && unitFamily == selectedFamily)
                {
                    string unitCategory = unit.Sort.Substring(1, 1);

                    if (unitCategory == "2")
                    {
                        var btn = new ButtonUnitType(unit);
                        itemControlUnits.Items.Add(btn);
                    }
                    else if (unitCategory == "1")
                    {
                        var btn = new ButtonUnitType(unit);
                        itemControlHeroes.Items.Add(btn);
                    }
                    else if (unitCategory == "3")
                    {
                        var btn = new ButtonUnitType(unit);
                        itemControlBuildings.Items.Add(btn);
                    }
                    else
                    {
                        var btn = new ButtonUnitType(unit);
                        itemControlSpecial.Items.Add(btn);
                    }
                }
            }
        }

    }
}
