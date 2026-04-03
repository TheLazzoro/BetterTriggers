using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using System.IO;
using System.Windows;
using War3Net.Build.Info;

namespace GUI.Components.BlizzardScripts
{
    public partial class BlizzardJWindow : Window
    {
        public BlizzardJWindow(Project project)
        {
            InitializeComponent();

            var commonJ = File.ReadAllText(TriggerData.pathCommonJ);
            var blizzardJ = File.ReadAllText(TriggerData.pathBlizzardJ);
            var textEditorC = new TextEditor(project, commonJ, ScriptLanguage.Jass, isReadonly: true);
            var textEditorB = new TextEditor(project, blizzardJ, ScriptLanguage.Jass, isReadonly: true);
            gridCommonJ.Children.Add(textEditorC);
            gridBlizzardJ.Children.Add(textEditorB);
        }
    }
}
