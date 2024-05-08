using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace BetterTriggers.Models.EditorData
{
    public class ExplorerElement : TreeNodeBase
    {
        public ExplorerElementEnum ElementType { get; private set; }
        public string path
        {
            get => _path;
            set
            {
                _path = value;
                DisplayText = Path.GetFileNameWithoutExtension(value);
            }
        }
        public ExplorerElement Parent { get; set; }
        public ObservableCollection<ExplorerElement> ExplorerElements { get; set; } = new();
        public bool IsInitiallyOn
        {
            get => _isInitiallyOn;
            set
            {
                _isInitiallyOn = value;
                OnPropertyChanged();
                OnToggleInitiallyOn?.Invoke();
            }
        }

        public bool ShouldRefreshUIElements { get; set; } // hack. I should structure my code better, but I'm tired of this project now.
        public event Action OnReload;
        public event Action OnChanged;
        public event Action OnSaved;
        public event Action OnDeleted;
        public event Action OnToggleInitiallyOn;
        private string _path;
        private bool _isInitiallyOn = true;
        public DateTime LastWrite { get; private set; }
        public long Size { get; private set; }

        public Trigger trigger;
        public Variable variable;
        public string script;
        public ActionDefinition actionDefinition;
        public ConditionDefinition conditionDefinition;
        public FunctionDefinition functionDefinition;

        public UserControl editor;


        /// <summary>Reserved for copy-pasting purposes.</summary>
        public ExplorerElement() { }

        /// <summary>Reserved for TriggerConverter and tests.</summary>
        public ExplorerElement(ExplorerElementEnum type)
        {
            ElementType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="explicitType">Only used when creating root on init. hack...</param>
        /// <exception cref="Exception"></exception>
        public ExplorerElement(string path, ExplorerElementEnum explicitType = ExplorerElementEnum.None)
        {
            this.path = path;
            string fileContent;

            if (Directory.Exists(path))
            {
                ElementType = ExplorerElementEnum.Folder;
                CategoryStr = TriggerCategory.TC_DIRECTORY;
            }
            else if (File.Exists(path))
            {
                string extension = Path.GetExtension(path);
                switch (extension)
                {
                    case ".trg":
                        ElementType = ExplorerElementEnum.Trigger;
                        CategoryStr = TriggerCategory.TC_TRIGGER_NEW;
                        fileContent = ReadFile(path);
                        var savedTrigger = JsonConvert.DeserializeObject<Trigger_Saveable>(fileContent);
                        trigger = TriggerSerializer.Deserialize(savedTrigger);
                        StoreLocalVariables();
                        Project.CurrentProject.Triggers.AddTrigger(this);
                        break;

                    case ".j":
                    case ".lua":
                        ElementType = ExplorerElementEnum.Script;
                        CategoryStr = TriggerCategory.TC_SCRIPT;
                        this.script = Project.CurrentProject.Scripts.LoadFromFile(path);
                        Project.CurrentProject.Scripts.AddScript(this);
                        break;

                    case ".var":
                        ElementType = ExplorerElementEnum.GlobalVariable;
                        CategoryStr = TriggerCategory.TC_SETVARIABLE;
                        fileContent = ReadFile(path);
                        var savedVariable = JsonConvert.DeserializeObject<Variable_Saveable>(fileContent);
                        variable = TriggerSerializer.DeserializeVariable(savedVariable);
                        variable.PropertyChanged += Variable_PropertyChanged; ;
                        Project.CurrentProject.Variables.AddVariable(this);
                        variable.Name = Path.GetFileNameWithoutExtension(GetPath());
                        break;

                    case ".act":
                        ElementType = ExplorerElementEnum.ActionDefinition;
                        CategoryStr = TriggerCategory.TC_ACTION_DEF;
                        fileContent = ReadFile(path);
                        var savedActionDef = JsonConvert.DeserializeObject<ActionDefinition_Saveable>(fileContent);
                        actionDefinition = TriggerSerializer.DeserializeActionDefinition(this, savedActionDef);
                        StoreLocalVariables();
                        Project.CurrentProject.ActionDefinitions.Add(this);
                        break;

                    case ".cond":
                        ElementType = ExplorerElementEnum.ConditionDefinition;
                        CategoryStr = TriggerCategory.TC_CONDITION_DEF;
                        fileContent = ReadFile(path);
                        var savedConditionDef = JsonConvert.DeserializeObject<ConditionDefinition_Saveable>(fileContent);
                        conditionDefinition = TriggerSerializer.DeserializeConditionDefinition(this, savedConditionDef);
                        StoreLocalVariables();
                        Project.CurrentProject.ConditionDefinitions.Add(this);
                        break;

                    case ".func":
                        ElementType = ExplorerElementEnum.FunctionDefinition;
                        CategoryStr = TriggerCategory.TC_FUNCTION_DEF;
                        fileContent = ReadFile(path);
                        var savedFunctionDef = JsonConvert.DeserializeObject<FunctionDefinition_Saveable>(fileContent);
                        functionDefinition = TriggerSerializer.DeserializeFunctionDefinition(this, savedFunctionDef);
                        StoreLocalVariables();
                        Project.CurrentProject.FunctionDefinitions.Add(this);
                        break;

                    default:
                        ElementType = ExplorerElementEnum.None;
                        CategoryStr = TriggerCategory.TC_UNKNOWN;
                        break;
                }
            }

            if (explicitType == ExplorerElementEnum.Root)
            {
                var project = Project.CurrentProject;
                DisplayText = Path.GetFileNameWithoutExtension(project.war3project.Name);
                ElementType = ExplorerElementEnum.Root;
                CategoryStr = TriggerCategory.TC_MAP;
            }


            UpdateMetadata();
        }

        private void Variable_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddToUnsaved();
        }

        private string ReadFile(string path)
        {
            string content = string.Empty;
            bool isReadyForRead = false;
            int sleepTolerance = 100;
            while (!isReadyForRead)
            {
                try
                {
                    content = File.ReadAllText(path);
                    isReadyForRead = true;
                }
                catch (Exception ex)
                {
                    if (sleepTolerance < 0)
                        throw new Exception(ex.Message);

                    Thread.Sleep(100);
                    sleepTolerance--;
                }
            }

            return content;
        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetPath()
        {
            return path;
        }

        /// <summary>
        /// Returns the relative path of this file in the 'src' directory.
        /// </summary>
        public string GetRelativePath()
        {
            string relativePath = this.path;
            while (true)
            {
                relativePath = Path.GetDirectoryName(relativePath);
                if (Path.GetFileName(relativePath) == "src")
                {
                    relativePath = relativePath + "\\";
                    relativePath = this.path.Replace(relativePath, "");
                    break;
                }
            }
            return relativePath;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public int GetId()
        {
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    return variable.Id;
                case ExplorerElementEnum.Trigger:
                    return trigger.Id;
                default:
                    throw new Exception($"Element type '{ElementType}' cannot return an ID.");
            }
        }

        public void UpdateMetadata()
        {
            if (ElementType == ExplorerElementEnum.Folder || ElementType == ExplorerElementEnum.Root)
            {
                var info = new DirectoryInfo(path);
                this.Size = info.EnumerateFiles().Sum(file => file.Length);
                this.LastWrite = info.LastWriteTime;
            }
            else if (ElementType != ExplorerElementEnum.None)
            {
                var info = new FileInfo(path);
                this.Size = info.Length;
                this.LastWrite = info.LastWriteTime;
            }
        }

        public ExplorerElement GetParent()
        {
            if (ElementType == ExplorerElementEnum.Root)
                throw new Exception("Root is the super parent");

            return Parent;
        }

        public void SetParent(ExplorerElement parent, int insertIndex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ElementType == ExplorerElementEnum.Root)
                    throw new Exception("Root is the super parent");

                Parent = parent;
                parent.GetExplorerElements().Insert(insertIndex, this);
                StoreLocalVariables();
            });
        }

        public void RemoveFromParent()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Parent.GetExplorerElements().Remove(this);
                Parent = null;
                RemoveLocalVariables();
            });
        }

        public void AddToUnsaved()
        {
            var project = Project.CurrentProject;
            if (project.IsLoading)
                return;

            project.UnsavedFiles.AddToUnsaved(this);
            OnChanged?.Invoke();
        }

        public void RemoveFromUnsaved(bool recursive = false)
        {
            if (Project.CurrentProject == null)
                return;

            Project.CurrentProject.UnsavedFiles.RemoveFromUnsaved(this);
            if (recursive && ExplorerElements.Count > 0)
            {
                for (int i = 0; i < ExplorerElements.Count; i++)
                {
                    ExplorerElements[i].RemoveFromUnsaved(true);
                }
            }
        }

        public ObservableCollection<ExplorerElement> GetExplorerElements()
        {
            if (ElementType != ExplorerElementEnum.Folder && ElementType != ExplorerElementEnum.Root)
            {
                throw new Exception("'" + path + "' is not a folder.");
            }

            return ExplorerElements;
        }

        /// <summary>
        /// Gets the collection of local variables from the <see cref="ExplorerElement"/>.
        /// The collection is either attached to a <see cref="Trigger"/>, <see cref="ActionDefinition"/>,
        /// <see cref="ConditionDefinition"/>, or <see cref="FunctionDefinition"/>.
        /// </summary>
        public TriggerElementCollection GetLocalVariables()
        {
            TriggerElementCollection localVariables = null;

            switch (ElementType)
            {
                case ExplorerElementEnum.Trigger:
                    localVariables = trigger.LocalVariables;
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    localVariables = actionDefinition.LocalVariables;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    localVariables = conditionDefinition.LocalVariables;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    localVariables = functionDefinition.LocalVariables;
                    break;
                default:
                    break;
            }

            return localVariables;
        }

        /// <summary>
        /// Gets a <see cref="ParameterDefinitionCollection"/> from the <see cref="ExplorerElement"/>.
        /// The collection is either attached to a <see cref="ActionDefinition"/>,
        /// <see cref="ConditionDefinition"/>, or <see cref="FunctionDefinition"/>.
        /// </summary>
        public ParameterDefinitionCollection GetParameterCollection()
        {
            ParameterDefinitionCollection parameterDefs = null;

            switch (ElementType)
            {
                case ExplorerElementEnum.ActionDefinition:
                    parameterDefs = actionDefinition.Parameters;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    parameterDefs = conditionDefinition.Parameters;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    parameterDefs = functionDefinition.Parameters;
                    break;
                default:
                    break;
            }

            return parameterDefs;
        }

        public ExplorerElement Clone()
        {
            ExplorerElement newElement = new ExplorerElement();
            newElement.path = new string(this.path); // we need this path in paste command.
            newElement.Parent = this.Parent;
            newElement.IsInitiallyOn = this.IsInitiallyOn;
            newElement.IsEnabled = this.IsEnabled;
            newElement.ElementType = this.ElementType;
            newElement.IconImage = new byte[IconImage.Length];
            IconImage.CopyTo(newElement.IconImage, 0);

            switch (ElementType)
            {
                case ExplorerElementEnum.Folder:
                    foreach (var element in ExplorerElements)
                    {
                        newElement.ExplorerElements.Add(element.Clone());
                    }
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    newElement.variable = this.variable.Clone();
                    break;
                case ExplorerElementEnum.Script:
                    newElement.script = new string(script);
                    break;
                case ExplorerElementEnum.Trigger:
                    newElement.trigger = this.trigger.Clone();
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    newElement.actionDefinition = this.actionDefinition.Clone();
                    newElement.actionDefinition.explorerElement = newElement;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    newElement.conditionDefinition = this.conditionDefinition.Clone();
                    newElement.conditionDefinition.explorerElement = newElement;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    newElement.functionDefinition = this.functionDefinition.Clone();
                    newElement.functionDefinition.explorerElement = newElement;
                    break;
                case ExplorerElementEnum.Root:
                    throw new Exception("Cannot clone Root.");
                default:
                    break;
            }

            return newElement;
        }

        /// <summary>
        /// Writes the content of the ExplorerElement to disk.
        /// </summary>
        public void Save()
        {
            if (ElementType == ExplorerElementEnum.Root)
                return;

            if (!Directory.Exists(Path.GetDirectoryName(this.path))) // Edge case when a folder containing the file was deleted.
            {
                return;
            }

            if (ElementType == ExplorerElementEnum.Folder)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                string fileContent = string.Empty;
                switch (ElementType)
                {
                    case ExplorerElementEnum.GlobalVariable:
                        fileContent = TriggerSerializer.SerializeVariable(variable);
                        break;
                    case ExplorerElementEnum.Script:
                        fileContent = script;
                        break;
                    case ExplorerElementEnum.Trigger:
                        fileContent = TriggerSerializer.SerializeTrigger(trigger);
                        break;
                    case ExplorerElementEnum.ActionDefinition:
                        fileContent = TriggerSerializer.SerializeActionDefinition(actionDefinition);
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        fileContent = TriggerSerializer.SerializeConditionDefinition(conditionDefinition);
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        fileContent = TriggerSerializer.SerializeFunctionDefinition(functionDefinition);
                        break;
                    default:
                        break;
                }

                File.WriteAllText(path, fileContent);
            }

            RemoveFromUnsaved();
            OnSaved?.Invoke();
        }


        public void Rename()
        {
            if (RenameText == DisplayText)
            {
                RenameBoxVisibility = Visibility.Hidden;
                return;
            }

            var project = Project.CurrentProject;
            string oldPath = GetPath();
            string formattedName = string.Empty;

            switch (ElementType)
            {
                case ExplorerElementEnum.Folder:
                formattedName = RenameText;
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    if (project.Variables.Contains(RenameText))
                        throw new Exception($"Variable '{RenameText}' already exists.");

                    formattedName = RenameText + ".var";
                    break;
                case ExplorerElementEnum.Script:
                    formattedName = RenameText + ".j";
                    break;
                case ExplorerElementEnum.Trigger:
                    if (project.Triggers.Contains(RenameText))
                        throw new Exception($"Trigger '{RenameText}' already exists.");

                    formattedName = RenameText + ".trg";
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    if (project.ActionDefinitions.Contains(RenameText))
                        throw new Exception($"Action Definition '{RenameText}' already exists.");

                    formattedName = RenameText + ".act";
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    if (project.ConditionDefinitions.Contains(RenameText))
                        throw new Exception($"Condition Definition '{RenameText}' already exists.");

                    formattedName = RenameText + ".cond";
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    if (project.FunctionDefinitions.Contains(RenameText))
                        throw new Exception($"Function Definition '{RenameText}' already exists.");

                    formattedName = RenameText + ".func";
                    break;
                default:
                    throw new Exception("Cannot rename unknown file types.");
            }

            FileSystemUtil.Rename(oldPath, formattedName);
            RenameBoxVisibility = Visibility.Hidden;
        }

        public void Delete()
        {
            FileSystemUtil.Delete(path);
        }

        public void Notify()
        {
            if (ElementType == ExplorerElementEnum.Script)
            {
                this.script = Project.CurrentProject.Scripts.LoadFromFile(GetPath());
                OnReload?.Invoke();
            }

            VerifyAndRemoveTriggerErrors();
        }

        /// <summary>
        /// Expensive action that checks for errors, updates state and re-renders UI.
        /// </summary>
        public void InvokeChange()
        {
            VerifyAndRemoveTriggerErrors();
            UpdateVariableIdentifier();
            OnChanged?.Invoke();
            AddToUnsaved();
        }

        public void InvokeDelete()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnDeleted?.Invoke();
                foreach (var element in ExplorerElements)
                {
                    element.InvokeDelete();
                }
            });
        }

        private void VerifyAndRemoveTriggerErrors()
        {
            TriggerValidator validator = new TriggerValidator(this, true);
            int errors = validator.RemoveInvalidReferences();
            HasErrors = errors > 0;
        }

        private void UpdateVariableIdentifier()
        {
            if (ElementType == ExplorerElementEnum.GlobalVariable)
            {
                variable.Name = this.GetName();
            }
        }

        public List<ExplorerElement> GetReferrers()
        {
            switch (ElementType)
            {
                case ExplorerElementEnum.GlobalVariable:
                    return Project.CurrentProject.References.GetReferrers(variable);
                case ExplorerElementEnum.Trigger:
                    return Project.CurrentProject.References.GetReferrers(trigger);
                case ExplorerElementEnum.ActionDefinition:
                    return Project.CurrentProject.References.GetReferrers(actionDefinition);
                case ExplorerElementEnum.ConditionDefinition:
                    return Project.CurrentProject.References.GetReferrers(conditionDefinition);
                case ExplorerElementEnum.FunctionDefinition:
                    return Project.CurrentProject.References.GetReferrers(functionDefinition);
                case ExplorerElementEnum.Folder:
                    return ExplorerElements.SelectMany(el => el.GetReferrers()).ToList();
                default:
                    return new List<ExplorerElement>();
            }
        }

        private void StoreLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            var localVariables = GetLocalVariables();
            if (localVariables != null)
            {
                localVariables.Elements.ForEach(e =>
                {
                    var lv = (LocalVariable)e;
                    variables.AddLocalVariable(lv);
                });
            }
        }

        private void RemoveLocalVariables()
        {
            var variables = Project.CurrentProject.Variables;
            var localVariables = GetLocalVariables();
            if (localVariables != null)
            {
                localVariables.Elements.ForEach(e =>
                {
                    var lv = (LocalVariable)e;
                    variables.RemoveLocalVariable(lv);
                });
            }
        }
    }
}