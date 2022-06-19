using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using GUI.Controllers;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class HyperlinkParameter : Hyperlink
    {
        private TreeViewTriggerElement treeViewTriggerElement;
        internal readonly List<Parameter> parameters;
        internal readonly int index;

        public HyperlinkParameter(TreeViewTriggerElement treeViewTriggerElement, string text, List<Parameter> parameters, int index)
        {
            this.treeViewTriggerElement = treeViewTriggerElement;
            this.parameters = parameters;
            this.index = index;

            this.GotFocus += HyperlinkParameter_GotFocus;
            this.LostFocus += HyperlinkParameter_LostFocus;
            this.Click += HyperlinkParameter_Click;

            this.Inlines.Add(text);

            // Create an underline text decoration.
            TextDecoration underline = new TextDecoration();
            underline.PenOffset = 2; // Underline offset
            underline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            TextDecorationCollection decorations = new TextDecorationCollection();
            decorations.Add(underline);
            this.TextDecorations = decorations;

            // Hyperlink font
            this.FontFamily = new FontFamily("Verdana");
            RecolorHyperlink();
        }

        /// <summary>
        /// Disables and greys out hyperlink.
        /// </summary>
        public void Disable()
        {
            this.IsEnabled = false;
            this.Inlines.Clear();
            this.Inlines.Add("Value");
            this.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var parameter = parameters[index];
            var window = new ParameterWindow(treeViewTriggerElement.triggerElement.function, parameter);
            window.Title = parameter.returnType;
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandTriggerElementParamModify command = new CommandTriggerElementParamModify(treeViewTriggerElement.triggerElement, treeViewTriggerElement.GetExplorerElementTrigger(), parameters, index, window.selectedParameter);
                command.Execute();

                ControllerTriggerTreeItem controllerTriggerTreeItem = new ControllerTriggerTreeItem(treeViewTriggerElement);
                controllerTriggerTreeItem.GenerateParamText();
                treeViewTriggerElement.UpdateTreeItem();
            }

        }

        private void HyperlinkParameter_LostFocus(object sender, RoutedEventArgs e)
        {
            RecolorHyperlink();
        }

        private void HyperlinkParameter_GotFocus(object sender, RoutedEventArgs e)
        {
            RecolorHyperlink();
        }

        private void RecolorHyperlink()
        {
            var parameter = parameters[index];
            if (this.IsFocused)
                this.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0));
            else if (parameter is Constant || parameter is Function || parameter is VariableRef || parameter is TriggerRef || parameter is Value)
                this.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
            else
                this.Foreground = new SolidColorBrush(Color.FromRgb(255, 75, 75));
        }
    }
}
