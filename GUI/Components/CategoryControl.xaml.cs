using Facades.Controllers;
using GUI.Commands;
using GUI.Components;
using GUI.Components.TextEditor;
using GUI.Components.TriggerEditor;
using GUI.Components.TriggerExplorer;
using GUI.Utility;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace GUI.Components
{
    /// <summary>
    /// Interaction logic for TriggerControl.xaml
    /// </summary>
    public partial class CategoryControl : UserControl, IEditor
    {
        private ExplorerElementFolder explorerElementFolder;

        public CategoryControl(ExplorerElementFolder explorerElementFolder)
        {
            InitializeComponent();

            this.explorerElementFolder = explorerElementFolder;
        }

        public void Refresh()
        {
            
        }
        
        public string GetSaveString()
        {
            return string.Empty;
        }

        public UserControl GetControl()
        {
            return this;
        }


        public void OnElementRename(string name)
        {
            throw new NotImplementedException();
        }

        public void OnRemoteChange()
        {
            throw new NotImplementedException();
        }
    }
}
