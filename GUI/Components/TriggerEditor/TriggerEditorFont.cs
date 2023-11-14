using BetterTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public static class TriggerEditorFont
    {
        private static FontFamily Default { get; } = new FontFamily();
        private static FontFamily Verdana { get; } = new FontFamily("Verdana");


        public static FontFamily GetTreeItemFont()
        {
            return Default;
        }

        public static FontFamily GetParameterFont()
        {
            var settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
                return Default;

            return Verdana;
        }

        public static double GetTreeItemFontSize()
        {
            var settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
                return 12;

            return 12;
        }

        public static double GetParameterFontSize()
        {
            var settings = EditorSettings.Load();
            if (settings.triggerEditorMode == 1)
                return 12;

            return 18;
        }

    }
}
