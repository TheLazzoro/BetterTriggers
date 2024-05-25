using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using GUI.Utility;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GUI.Components.TriggerEditor.WPF_BindingInlinesInTextBlock
{
    public class Attached
    {
        public static IEnumerable<Inline> GetInlines(DependencyObject d)
        {
            return (IEnumerable<Inline>)d.GetValue(InlinesProperty);
        }

        public static void SetInlines(DependencyObject d, IEnumerable<Inline> value)
        {
            d.SetValue(InlinesProperty, value);
        }

        // Using a DependencyProperty as the backing store for Inlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.RegisterAttached("Inlines", typeof(IEnumerable<Inline>), typeof(Attached),
                new FrameworkPropertyMetadata(OnInlinesPropertyChanged));

        private static void OnInlinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            var inlinesCollection = textBlock.Inlines;
            inlinesCollection.Clear();

            var eca = textBlock.DataContext as ECA;
            if (eca == null)
            {
                inlinesCollection.AddRange((IEnumerable<Inline>)e.NewValue);
                return;
            }

            EditorSettings editorSettings = EditorSettings.Load();
            if(editorSettings.triggerEditorMode == TriggerEditorMode.CliCli)
            {
                ParamTextBuilder paramTextBuilder = new ParamTextBuilder();
                var inlines = paramTextBuilder.GenerateParamText(ExplorerElement.CurrentToRender, eca);
                inlinesCollection.AddRange(inlines);
            }
            else
            {
                inlinesCollection.AddRange((IEnumerable<Inline>)e.NewValue);
            }
        }
    }
}
