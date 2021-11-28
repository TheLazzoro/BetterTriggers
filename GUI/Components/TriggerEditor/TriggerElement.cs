using DataAccess.Data;
using DataAccess.Natives;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public abstract class TriggerElement : TreeViewItem
    {
        public TextBlock paramTextBlock;
        protected List<Parameter> parameters;
        protected string paramText;
        protected EnumCategory category;

        public TriggerElement()
        {
            this.paramTextBlock = new TextBlock();
            this.paramTextBlock.FontSize = 18;
            this.paramTextBlock.Margin = new Thickness(0, 0, 5, 0);
            this.paramTextBlock.Foreground = Brushes.White;
        }

        protected void FormatParameterText(TextBlock textBlock, List<Parameter> parameters)
        {
            textBlock.Inlines.Clear();

            RecurseParameters(textBlock, parameters, paramText);

            TreeViewManipulator.SetTreeViewItemAppearance(this, textBlock.Text, category);
        }

        private void RecurseParameters(TextBlock textBlock, List<Parameter> parameters, string paramText)
        {
            int paramIndex = 0;

            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] != '%')
                {
                    Run run = new Run(paramText[i].ToString());
                    run.FontFamily = new FontFamily("Verdana");

                    textBlock.Inlines.Add(run);
                }
                else
                {
                    if (parameters[paramIndex] is Constant)
                    {
                        var index = paramIndex; // copy current iterated index to prevent referenced values in hyperlink.click delegate
                        var hyperlink = CreateHyperlink(textBlock, parameters[paramIndex].name, parameters, index);
                        hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
                        paramIndex++;
                    }
                    else if (parameters[paramIndex] is Function) // recurse if parameter is a function
                    {
                        var index = paramIndex;
                        var hyperlink = CreateHyperlink(textBlock, "(", parameters, index);
                        hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));

                        var function = (Function)parameters[paramIndex];
                        paramIndex++;

                        RecurseParameters(textBlock, function.parameters, function.funcText); // recurse
                        textBlock.Inlines.Add(")");
                    }
                    else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set. Redundant?
                    {
                        var index = paramIndex;
                        var hyperlink = CreateHyperlink(textBlock, parameters[paramIndex].name, parameters, index);
                        hyperlink.Foreground = Brushes.Red;
                        paramIndex++;
                    }
                }
            }
        }

        private Hyperlink CreateHyperlink(TextBlock textBlock, string hyperlinkText, List<Parameter> parameters, int paramIndex)
        {
            Run run = new Run(hyperlinkText); // idk why it's called run
            Hyperlink hyperlink = new Hyperlink(run);
            hyperlink.Tag = parameters;


            // Create an underline text decoration. Default is 'underline'.
            TextDecoration underline = new TextDecoration();
            underline.PenOffset = 1; // Underline offset
            underline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            TextDecorationCollection decorations = new TextDecorationCollection();
            decorations.Add(underline);
            hyperlink.TextDecorations = decorations;

            // Hyperlink font
            hyperlink.FontFamily = new FontFamily("Verdana");


            hyperlink.Click += delegate { Hyperlink_Click(hyperlink, paramIndex); };
            hyperlink.GotFocus += delegate { hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0)); };
            hyperlink.LostFocus += delegate
            {
                if(parameters[paramIndex] is Constant || parameters[paramIndex] is Function)
                    hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 255));
                else
                    hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            };

            textBlock.Inlines.Add(hyperlink); // adds the clickable parameter text

            return hyperlink;
        }

        // We need to pass in the list of parameters to we can change the selected parameter on the list.
        private void Hyperlink_Click(Hyperlink clickedHyperlink, int paramIndex)
        {
            var parameters = (List<Parameter>)clickedHyperlink.Tag;
            var window = new ParameterWindow(parameters[paramIndex].returnType.type);
            window.Title = parameters[paramIndex].name;
            window.ShowDialog();

            if (window.isOK)
            {
                parameters[paramIndex] = window.selectedParameter; // set parameter on window close.
                FormatParameterText(this.paramTextBlock, this.parameters);
            }
        }
    }
}
