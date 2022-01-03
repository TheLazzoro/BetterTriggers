﻿using GUI.Components.TriggerExplorer;
using GUI.Utility;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace GUI.Controllers
{
    public class ControllerScript
    {
        public void CreateScript(TabControl tabControl, TriggerExplorer triggerExplorer)
        {
            //var scriptControl = CreateScriptControl(tabControl);

            //string name = NameGenerator.GenerateScriptName();

            //TreeViewItem item = new TreeViewItem();
            //ScriptControl script = new ScriptControl();

            //triggerExplorer.CreateTreeViewItem(item, name, EnumCategory.AI);

            // folding text blocks?
            //foldingManager = FoldingManager.Install(textEditor.TextArea);
            //foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        }

        public ScriptControl CreateScriptControlWithScript(TabControl tabControl, string filePath)
        {
            var scriptControl = CreateScriptControl(tabControl);
            scriptControl.textEditor.Text = LoadScriptFromFile(filePath);

            return scriptControl;
        }

        public string LoadScriptFromFile(string filePath)
        {
            string script = string.Empty;
            if(File.Exists(filePath))
                script = File.ReadAllText(filePath);

            return script;
        }

        private ScriptControl CreateScriptControl(TabControl tabControl)
        {
            var scriptControl = new ScriptControl();
            
            // Position editor
            Grid.SetColumn(scriptControl, 1);
            Grid.SetRow(scriptControl, 2);
            Grid.SetRowSpan(scriptControl, 3);

            return scriptControl;
        }
    }
}