using BetterTriggers.WorldEdit;
using System.IO;
using System.Windows;
using War3Net.Build.Info;

namespace GUI.Components.BlizzardScripts
{
    public partial class BlizzardJWindow : Window
    {
        public BlizzardJWindow()
        {
            InitializeComponent();

            var commonJ = File.ReadAllText(TriggerData.pathCommonJ);
            var blizzardJ = File.ReadAllText(TriggerData.pathBlizzardJ);
            var textEditorC = new TextEditor(commonJ, ScriptLanguage.Jass, isReadonly: true);
            var textEditorB = new TextEditor(blizzardJ, ScriptLanguage.Jass, isReadonly: true);
            gridCommonJ.Children.Add(textEditorC);
            gridBlizzardJ.Children.Add(textEditorB);
        }
    }
}
