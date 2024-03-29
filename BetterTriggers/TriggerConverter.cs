using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using System.Threading;
using War3Net.Build.Info;
using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using BetterTriggers.Containers;
using War3Net.Build;

namespace BetterTriggers.WorldEdit
{
    public class TriggerConverter
    {
        public event Action<string> OnExplorerElementImported;

        private string mapPath;
        private string mapPathProjectToImportInto;
        private MapTriggers triggers;
        private MapInfo mapInfo;
        private ScriptLanguage language;
        private Dictionary<uint, string> triggerStrings = new Dictionary<uint, string>(); // [wts key, trigger string]

        private string rootComment = string.Empty;
        private string rootHeader = string.Empty;
        private List<string> wctStrings = new List<string>();
        private int wctIndex = 0;

        Dictionary<int, string> triggerPaths = new Dictionary<int, string>(); // [triggerId, our path in the filesystem]
        Dictionary<string, int> variableIds = new Dictionary<string, int>(); // [name, variableId]
        Dictionary<string, int> triggerIds = new Dictionary<string, int>(); // [name, triggerId]

        Dictionary<int, ExplorerElement> explorerVariables = new Dictionary<int, ExplorerElement>(); // [id, variable]
        Dictionary<string, ExplorerElement> explorerVariables_byName = new Dictionary<string, ExplorerElement>(); // [name, variable]

        Dictionary<int, War3ProjectFileEntry> projectFilesEntries = new Dictionary<int, War3ProjectFileEntry>(); // [id, file entry in the project]

        public TriggerConverter(string mapPath)
        {
            this.mapPath = mapPath;
            Load(mapPath);
        }

        public TriggerConverter(string mapPath, string mapPathProjectToImportInto)
        {
            this.mapPath = mapPath;
            this.mapPathProjectToImportInto = mapPathProjectToImportInto;
            Load(mapPath);
        }

        private void Load(string mapPath)
        {
            CustomMapData.Load(mapPath);

            var map = CustomMapData.MPQMap;
            //var map = Map.Open(mapPath);
            if (map.Triggers == null)
                return;

            triggers = map.Triggers;
            var customTextTriggers = map.CustomTextTriggers;
            rootComment = customTextTriggers.GlobalCustomScriptComment;
            if (customTextTriggers.GlobalCustomScriptCode != null)
            {
                if (customTextTriggers.GlobalCustomScriptCode.Code.Length > 0)
                    rootHeader = customTextTriggers.GlobalCustomScriptCode.Code.Replace("\0", ""); // remove NUL char
            }
            customTextTriggers.CustomTextTriggers.ForEach(item =>
            {
                wctStrings.Add(item.Code.Replace("\0", "")); // remove NUL char
            });
            mapInfo = map.Info;
            language = mapInfo.ScriptLanguage;
            var wts = map.TriggerStrings;
            wts.Strings.ForEach(trigStr => triggerStrings.TryAdd(trigStr.Key, trigStr.Value));

            // Prepare all trigger items
            if (triggers != null)
            {
                // First, gather all variables names and ids
                for (int i = 0; i < triggers.Variables.Count; i++)
                {
                    var variable = triggers.Variables[i];
                    variableIds.Add(variable.Name, variable.Id);
                    if (triggers.SubVersion != null)
                    {
                        explorerVariables.Add(variable.Id, CreateVariable(variable));
                    }
                    else
                    {
                        explorerVariables_byName.Add(variable.Name, CreateVariable(variable));
                    }
                }

                // Then, gather all trigger names and ids
                for (int i = 0; i < triggers.TriggerItems.Count; i++)
                {
                    var triggerItem = triggers.TriggerItems[i];
                    if (triggerItem.Type != TriggerItemType.Gui)
                        continue;

                    string name = "gg_trg_" + triggerItem.Name.TrimEnd().Replace(" ", "_");
                    string nameFormatted = Ascii.ReplaceNonASCII("gg_trg_" + triggerItem.Name.TrimEnd().Replace(" ", "_")); // Again, War3Net auto-formats names for parameters. See comment further down in the parameter method.
                    if (triggers.SubVersion == null) // legacy format. Id's probably didn't exist in older formats
                    {
                        int newId = RandomUtil.GenerateInt();
                        triggerIds.TryAdd(name, newId);
                        triggerIds.TryAdd(nameFormatted, newId);
                    }
                    else
                    {
                        triggerIds.TryAdd(name, triggerItem.Id);
                        triggerIds.TryAdd(nameFormatted, triggerItem.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an entire map's triggers to a Better Triggers project.
        /// </summary>
        /// <returns>Project file path.</returns>
        public string Convert(string projectDestinationDir)
        {
            return ConvertAllTriggers(projectDestinationDir);
        }

        public List<ExplorerElement> ConvertAll_NoWrite()
        {
            return ConvertSelectedTriggers(triggers.TriggerItems);
        }

        /// <summary>
        /// Used for unit test purposes.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ImportIntoCurrentProject(List<TriggerItem> itemsToImport)
        {
            if (Project.CurrentProject == null)
            {
                throw new Exception("Cannot import when no project is open.");
            }

            var convertedElements = ConvertSelectedTriggers(itemsToImport);
            WriteConvertedTriggers(convertedElements);
        }

        /// <summary>
        /// Writes new triggers to a current project
        /// </summary>
        public void WriteConvertedTriggers(List<ExplorerElement> elements)
        {
            // Write to disk
            var project = Project.CurrentProject;
            project.EnableFileEvents(false);
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element.ElementType == ExplorerElementEnum.Folder)
                    element.Save();
                else
                {
                    string folder = Path.GetDirectoryName(element.GetPath());
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                        project.OnCreateElement(folder, false); // We manually create UI elements
                        OnExplorerElementImported?.Invoke(folder);
                    }
                    element.Save();
                }
                string path = element.GetPath();
                project.OnCreateElement(path, false); // We manually create UI elements
                OnExplorerElementImported?.Invoke(path);
            }
            project.EnableFileEvents(true);

            CustomMapData.Load(mapPathProjectToImportInto);
            CustomMapData.ReloadMapData();
        }


        private void ResolveIdCollisions(Project project, List<ExplorerElement> triggerElementsToImport)
        {
            List<ExplorerElement> dummyElements = new ();
            // Resolve collisions
            for (int i = 0; i < triggerElementsToImport.Count; i++)
            {
                var element = triggerElementsToImport[i];

                // Resolve name collision

                string dirLocation = Path.GetDirectoryName(element.GetPath());
                switch (element.ElementType)
                {
                    case ExplorerElementEnum.Trigger:
                        string triggerName = project.Triggers.GenerateTriggerName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, triggerName));
                        break;
                    case ExplorerElementEnum.GlobalVariable:
                        string variableName = project.Variables.GenerateName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, variableName + ".var"));
                        break;
                    case ExplorerElementEnum.Script:
                        string scriptName = project.Scripts.GenerateName(element);
                        element.SetPath(Path.Combine(dirLocation, scriptName));
                        break;
                    case ExplorerElementEnum.Folder:
                        string folderName = project.Folders.GenerateName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, folderName));
                        break;
                    default:
                        break;
                }

                if (element.ElementType != ExplorerElementEnum.Trigger && element.ElementType != ExplorerElementEnum.GlobalVariable)
                    continue;

                // Resolve id-collisions

                int id = element.GetId();
                bool idAlreadyExists = false;
                bool isVariable = false;
                switch (element.ElementType)
                {
                    case ExplorerElementEnum.Trigger:
                        idAlreadyExists = project.Triggers.Contains(id);
                        break;
                    case ExplorerElementEnum.GlobalVariable:
                        idAlreadyExists = project.Variables.Contains(id);
                        isVariable = true;
                        break;
                    default:
                        break;
                }

                if (idAlreadyExists)
                {
                    int oldId = id;
                    int newId = isVariable ? project.Variables.GenerateId() : project.Triggers.GenerateId();
                    if (element.ElementType == ExplorerElementEnum.Trigger)
                    {
                        element.trigger.Id = newId;
                    }
                    else if (element.ElementType == ExplorerElementEnum.GlobalVariable)
                    {
                        element.variable.Id = newId;
                    }

                    foreach (var trigger in triggerElementsToImport)
                    {
                        if (trigger.ElementType == ExplorerElementEnum.Trigger)
                        {
                            var functions = Triggers.GetFunctionsFromTrigger(trigger);
                            foreach (var function in functions)
                            {
                                foreach (var parameter in function.parameters)
                                {
                                    if (!isVariable && parameter is TriggerRef triggerRef)
                                    {
                                        if (triggerRef.TriggerId == oldId)
                                        {
                                            triggerRef.TriggerId = newId;
                                        }
                                    }
                                    else if (isVariable && parameter is VariableRef variableRef)
                                    {
                                        if (variableRef.VariableId == oldId)
                                        {
                                            variableRef.VariableId = newId;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                /*  hack
                    We add the elements to the container AFTER checking for duplicate id's
                    However, this will cause bugs when writing to files afterwards,
                    since they will be added twice into the container.
                    Therefore, we need to remove these dummy elements after generating all id's.
                */
                dummyElements.Add(element);
                if (element.ElementType == ExplorerElementEnum.GlobalVariable)
                {
                    project.Variables.AddVariable(element);
                }
                else if (element.ElementType == ExplorerElementEnum.Trigger)
                {
                    project.Triggers.AddTrigger(element);
                }
            }

            // Check file paths before writing
            for (int i = 0; i < triggerElementsToImport.Count; i++)
            {
                var element = triggerElementsToImport[i];
                string path = element.GetPath();
                if (File.Exists(path) || Directory.Exists(path))
                {
                    throw new Exception($"Could not properly rename file or folder for conversion.{Environment.NewLine}'{path}' already exists in the project.{Environment.NewLine}{Environment.NewLine}Import cancelled.");
                }
            }

            // remove dummy elements
            dummyElements.ForEach(el =>
            {
                project.RemoveElementFromContainer(el);
            });
        }


        private List<ExplorerElement> ConvertSelectedTriggers(List<TriggerItem> selectedTriggers)
        {
            var project = Project.CurrentProject;
            if (project == null)
            {
                throw new Exception("Cannot import when no project is active.");
            }

            var root = project.GetRoot();
            string targetDir = FileSystemUtil.FormatFileOrDirectoryName(Path.Combine(root.GetPath(), mapInfo.MapName + "_Imported"));
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            triggerPaths.Add(0, targetDir); // root path for the imported triggers

            var triggerElementsToImport = new List<ExplorerElement>();
            for (int i = 0; i < selectedTriggers.Count; i++)
            {
                var triggerItem = selectedTriggers[i];
                if (triggerItem is DeletedTriggerItem || triggerItem.Type is TriggerItemType.RootCategory)
                    continue;

                ExplorerElement explorerElement = CreateExplorerElement(triggerItem);
                explorerElement = FormatExplorerElement(explorerElement, triggerItem.Name, triggerItem.ParentId);
                if (explorerElement == null)
                    continue;

                triggerPaths.TryAdd(triggerItem.Id, explorerElement.GetPath());
                triggerElementsToImport.Add(explorerElement);
            }

            ResolveIdCollisions(project, triggerElementsToImport);

            return triggerElementsToImport;
        }



        private string ConvertAllTriggers(string fullPath)
        {
            string projectPath = Project.Create(language, Path.GetFileName(fullPath), Path.GetDirectoryName(fullPath), false);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(File.ReadAllText(projectPath));
            string src = Path.Combine(Path.GetDirectoryName(projectPath), "src");

            project.War3MapDirectory = mapPath;
            project.Comment = rootComment;
            project.Header = rootHeader;
            triggerPaths.Add(0, src);

            if (triggers != null)
            {
                /// Write to disk (local method)
                void WriteToProjectFileAndDisk(ExplorerElement explorerElement, int id, int parentId)
                {
                    War3ProjectFileEntry entry = new War3ProjectFileEntry()
                    {
                        isEnabled = explorerElement.IsEnabled,
                        isInitiallyOn = explorerElement.IsInitiallyOn,
                        path = explorerElement.GetRelativePath(),
                    };

                    War3ProjectFileEntry parentEnty;
                    projectFilesEntries.TryGetValue(parentId, out parentEnty);
                    if (parentEnty == null)
                        project.Files.Add(entry);
                    else
                        parentEnty.Files.Add(entry);

                    projectFilesEntries.TryAdd(id, entry);

                    explorerElement.Save();
                }

                List<ExplorerElement> elements = new();

                // run through legacy format for variables
                if (triggers.SubVersion == null)
                {
                    for (int i = 0; i < triggers.Variables.Count; i++)
                    {
                        var variableItem = triggers.Variables[i];
                        ExplorerElement variable = CreateVariable(variableItem);
                        int newId = RandomUtil.GenerateInt(); // Legacy variable format always has id=0, so we need to generate new id's
                        variable.variable.Id = newId;
                        variableIds.Remove(variableItem.Name);
                        variableIds.Add(variableItem.Name, newId);
                        ExplorerElement explorerElement = FormatExplorerElement(variable, variableItem.Name, variableItem.ParentId);

                        triggerPaths.TryAdd(variableItem.Id, explorerElement.GetPath());

                        elements.Add(explorerElement);
                        WriteToProjectFileAndDisk(explorerElement, variableItem.Id, variableItem.ParentId);
                    }
                }

                // run through the rest
                for (int i = 0; i < triggers.TriggerItems.Count; i++)
                {
                    var triggerItem = triggers.TriggerItems[i];
                    if (triggerItem is DeletedTriggerItem || triggerItem.Type is TriggerItemType.RootCategory)
                        continue;

                    ExplorerElement explorerElement = CreateExplorerElement(triggerItem);
                    if (explorerElement == null)
                        continue;

                    triggerPaths.TryAdd(triggerItem.Id, explorerElement.GetPath());

                    elements.Add(explorerElement);
                    WriteToProjectFileAndDisk(explorerElement, triggerItem.Id, triggerItem.ParentId);
                }
            }

            File.WriteAllText(projectPath, JsonConvert.SerializeObject(project, Formatting.Indented));

            return projectPath;
        }

        private ExplorerElement CreateExplorerElement(TriggerItem triggerItem)
        {
            ExplorerElement explorerElement = null;

            switch (triggerItem.Type)
            {
                case TriggerItemType.RootCategory:
                    break;
                case TriggerItemType.UNK1:
                    break;
                case TriggerItemType.Category:
                    explorerElement = CreateFolder(triggerItem as TriggerCategoryDefinition);
                    break;
                case TriggerItemType.Gui:
                    var triggerDef = triggerItem as TriggerDefinition;
                    if (triggerDef.IsComment) // Triggers can be converted to comments in vanilla WE
                    {
                        break;
                    }
                    explorerElement = CreateTrigger(triggerDef);
                    break;
                case TriggerItemType.Comment:
                    break;
                case TriggerItemType.Script:
                    explorerElement = CreateScript(triggerItem as TriggerDefinition, wctStrings[wctIndex]);
                    wctIndex++;
                    break;
                case TriggerItemType.Variable:
                    ExplorerElement explorerElementVariable = GetVariable(triggerItem);
                    explorerElement = explorerElementVariable;
                    break;
                case TriggerItemType.UNK7:
                    break;
                default:
                    break;
            }

            return FormatExplorerElement(explorerElement, triggerItem.Name, triggerItem.ParentId);
        }

        private ExplorerElement CreateExplorerElementVariable(VariableDefinition variableDefinition)
        {
            string name = variableDefinition.Name;
            int id = variableDefinition.Id;
            string extension = ".var";

            ExplorerElement explorerElementVariable = GetVariable(variableDefinition);

            return FormatExplorerElement(explorerElementVariable, name, variableDefinition.ParentId);
        }

        private ExplorerElement FormatExplorerElement(ExplorerElement explorerElement, string name, int parentId)
        {
            if (explorerElement == null)
                return null;

            string parentPath;
            triggerPaths.TryGetValue(parentId, out parentPath);
            if (parentPath == null) // could not find the element's location, put it in root.
            {
                triggerPaths.TryGetValue(0, out parentPath);
            }

            List<char> invalidPathChars = Path.GetInvalidPathChars().ToList();
            invalidPathChars.AddRange(Path.GetInvalidFileNameChars());

            int i = 0;
            while (i < invalidPathChars.Count)
            {
                name = name.Replace(invalidPathChars[i].ToString(), "");
                i++;
            }

            string extension = FileSystemUtil.GetExtension(explorerElement, language);
            name = name.TrimStart().TrimEnd();
            name = name.TrimEnd('.'); // files/dirs cannot end with '.'
            string suffix = string.Empty;
            bool ok = false;
            i = 0;
            string finalName = string.Empty;
            while (!ok)
            {
                finalName = name + suffix;
                if (explorerElement.ElementType == ExplorerElementEnum.Folder && !Directory.Exists(Path.Combine(parentPath, finalName + extension)))
                    ok = true;
                else if (explorerElement.ElementType != ExplorerElementEnum.Folder && !File.Exists(Path.Combine(parentPath, finalName + extension)))
                    ok = true;

                suffix = i.ToString();
                i++;
            }
            explorerElement.SetPath(Path.Combine(parentPath, finalName + extension));

            return explorerElement;
        }


        private ExplorerElement CreateFolder(TriggerCategoryDefinition triggerCategory)
        {
            if (triggerCategory == null)
                return null;

            ExplorerElement folder = new ExplorerElement(ExplorerElementEnum.Folder);
            return folder;
        }

        private ExplorerElement CreateVariable(VariableDefinition variableDefinition)
        {
            int arrSize = variableDefinition.ArraySize == 0 ? 1 : variableDefinition.ArraySize;
            Parameter initialValue = new Parameter();
            if (TriggerData.ConstantExists(variableDefinition.InitialValue))
                initialValue = new Preset { value = variableDefinition.InitialValue };
            else if (variableDefinition.InitialValue != "")
                initialValue = new Value { value = variableDefinition.InitialValue };

            ExplorerElement variable = new ExplorerElement(ExplorerElementEnum.GlobalVariable)
            {
                variable = new Variable()
                {
                    Id = variableDefinition.Id,
                    Name = variableDefinition.Name,
                    ArraySize = new int[] { arrSize, 1 },
                    InitialValue = initialValue,
                    IsArray = variableDefinition.IsArray,
                    IsTwoDimensions = false,
                    Type = variableDefinition.Type
                }
            };
            return variable;
        }

        private ExplorerElement CreateScript(TriggerDefinition triggerDefinition, string script)
        {
            ExplorerElement element = new ExplorerElement(ExplorerElementEnum.Script);
            element.IsEnabled = triggerDefinition.IsEnabled;
            element.script = script;

            return element;
        }

        private ExplorerElement CreateTrigger(TriggerDefinition triggerDefinition)
        {
            if (triggerDefinition == null)
                return null;

            ExplorerElement explorerElementTrigger = new ExplorerElement(ExplorerElementEnum.Trigger);
            Trigger trigger = new Trigger();
            explorerElementTrigger.trigger = trigger;
            explorerElementTrigger.IsEnabled = triggerDefinition.IsEnabled;
            explorerElementTrigger.IsInitiallyOn = triggerDefinition.IsInitiallyOn;

            if (triggers.SubVersion == null) // legacy format
            {
                string name = "gg_trg_" + triggerDefinition.Name.TrimEnd().Replace(" ", "_");
                int newId;
                triggerIds.TryGetValue(name, out newId);
                trigger.Id = newId;
            }
            else
            {
                trigger.Id = triggerDefinition.Id;
            }
            trigger.Comment = triggerDefinition.Description;
            trigger.Script = wctStrings[wctIndex];
            wctIndex++;
            if (triggerDefinition.IsCustomTextTrigger)
            {
                trigger.IsScript = triggerDefinition.IsCustomTextTrigger;
                trigger.RunOnMapInit = triggerDefinition.RunOnMapInit;
                return explorerElementTrigger;
            }

            List<TriggerFunction> Events = new List<TriggerFunction>();
            List<TriggerFunction> Conditions = new List<TriggerFunction>();
            List<TriggerFunction> Actions = new List<TriggerFunction>();

            triggerDefinition.Functions.ForEach(function =>
            {
                switch (function.Type)
                {
                    case TriggerFunctionType.Event:
                        Events.Add(function);
                        break;
                    case TriggerFunctionType.Condition:
                        Conditions.Add(function);
                        break;
                    case TriggerFunctionType.Action:
                        Actions.Add(function);
                        break;
                    case TriggerFunctionType.Call:
                        throw new Exception("Attempted to create a 'Parameter' as 'TriggerElement'.");
                    default:
                        break;
                }
            });

            CreateSubElements(explorerElementTrigger.trigger.Events, Events);
            CreateSubElements(explorerElementTrigger.trigger.Conditions, Conditions);
            CreateSubElements(explorerElementTrigger.trigger.Actions, Actions);

            return explorerElementTrigger;
        }

        private void CreateSubElements(TriggerElementCollection triggerElements, List<TriggerFunction> triggerFunctions)
        {
            triggerFunctions.ForEach(function =>
            {
                ECA te = TriggerElementFactory.Create(function.Name);
                te.IsEnabled = function.IsEnabled;
                te.function.parameters = CreateParameters(function.Parameters);

                triggerElements.Elements.Add(te);

                if (te is IfThenElse)
                {
                    IfThenElse special = (IfThenElse)te;

                    List<TriggerFunction> If = function.ChildFunctions.Where(f => f.Branch == 0).ToList();
                    List<TriggerFunction> Then = function.ChildFunctions.Where(f => f.Branch == 1).ToList();
                    List<TriggerFunction> Else = function.ChildFunctions.Where(f => f.Branch == 2).ToList();
                    CreateSubElements(special.If, If);
                    CreateSubElements(special.Then, Then);
                    CreateSubElements(special.Else, Else);
                }
                else if (te is AndMultiple)
                {
                    AndMultiple special = (AndMultiple)te;
                    CreateSubElements(special.And, function.ChildFunctions);
                }
                else if (te is OrMultiple)
                {
                    OrMultiple special = (OrMultiple)te;
                    CreateSubElements(special.Or, function.ChildFunctions);
                }
                else if (te is ForGroupMultiple)
                {
                    ForGroupMultiple special = (ForGroupMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is ForForceMultiple)
                {
                    ForForceMultiple special = (ForForceMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is ForLoopAMultiple)
                {
                    ForLoopAMultiple special = (ForLoopAMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is ForLoopBMultiple)
                {
                    ForLoopBMultiple special = (ForLoopBMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is ForLoopVarMultiple)
                {
                    ForLoopVarMultiple special = (ForLoopVarMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is EnumDestructablesInRectAllMultiple)
                {
                    EnumDestructablesInRectAllMultiple special = (EnumDestructablesInRectAllMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is EnumDestructiblesInCircleBJMultiple)
                {
                    EnumDestructiblesInCircleBJMultiple special = (EnumDestructiblesInCircleBJMultiple)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
                else if (te is EnumItemsInRectBJ)
                {
                    EnumItemsInRectBJ special = (EnumItemsInRectBJ)te;
                    CreateSubElements(special.Actions, function.ChildFunctions);
                }
            });
        }

        private List<Parameter> CreateParameters(List<TriggerFunctionParameter> foreignParameters)
        {
            List<Parameter> parameters = new List<Parameter>();
            for (int i = 0; i < foreignParameters.Count; i++)
            {
                var foreignParam = foreignParameters[i];

                Parameter parameter = null;
                string value = foreignParam.Value;

                // War3Net thingy:
                // Some functions (boolexpr) have an empty name? Dunno how many more
                if (string.IsNullOrEmpty(value) && foreignParam.Type == TriggerFunctionParameterType.Function)
                    value = foreignParam.Function.Name;

                switch (foreignParam.Type)
                {
                    case TriggerFunctionParameterType.Preset:
                        parameter = new Preset()
                        {
                            value = foreignParam.Value,
                        };
                        break;
                    case TriggerFunctionParameterType.Variable:
                        List<Parameter> arrayIndex = new List<Parameter>();
                        if (foreignParam.ArrayIndexer == null)
                        {
                            arrayIndex.Add(new Value() { value = "0" });
                            arrayIndex.Add(new Value() { value = "0" });
                        }
                        else
                        {
                            var list = new List<TriggerFunctionParameter>();
                            list.Add(foreignParam.ArrayIndexer);

                            arrayIndex = CreateParameters(list);
                            arrayIndex.Add(new Value() { value = "0" });
                        }




                        // In our editor regions, cameras, units etc. are considered values, not variables.
                        // Also, War3Net does not include 'gg' prefixes in variable names.
                        if (foreignParam.Value.StartsWith("gg_unit_"))
                            parameter = new Value() { value = foreignParam.Value.Replace("gg_unit_", "") };
                        else if (foreignParam.Value.StartsWith("gg_item_"))
                            parameter = new Value() { value = foreignParam.Value.Replace("gg_item_", "") };
                        else if (foreignParam.Value.StartsWith("gg_dest_"))
                            parameter = new Value() { value = foreignParam.Value.Replace("gg_dest_", "") };
                        else if (foreignParam.Value.StartsWith("gg_rct_"))
                        {
                            // This madness exists because of cyrillic and other non-ASCII chars.
                            // Old versions of WE (other language versions too?) accept non-ASCII chars
                            // for variable names in WE, but references in WTG are underscore formatted.
                            // ... So we need to search and replace using the Ascii util.

                            /* War3Net is doing something unexpected, where all parameters using
                             * non-ASCII symbols are being auto-translated, but that also means parameters
                             * automatically don't match the name of region they're referencing.
                             * 
                             * So we have to look through all actual regions, do the translation ourselves,
                             * so we can find the region. Once the region is found the parameter
                             * can then use the region's original name.
                             */


                            var val = foreignParam.Value.Replace("gg_rct_", "");
                            var regions = Regions.GetAll();
                            for (int r = 0; r < regions.Count; r++)
                            {
                                var region = regions[r];
                                if (val == Ascii.ReplaceNonASCII(region.ToString().Replace(" ", "_")))
                                {
                                    val = region.Name.Replace("gg_rct_", "");
                                    break;
                                }
                            }
                            parameter = new Value() { value = val };
                        }
                        else if (foreignParam.Value.StartsWith("gg_cam_"))
                        {
                            var val = foreignParam.Value.Replace("gg_cam_", "");
                            var cameras = Cameras.GetAll();
                            for (int c = 0; c < cameras.Count; c++)
                            {
                                var camera = cameras[c];
                                if (val == Ascii.ReplaceNonASCII(camera.ToString().Replace(" ", "_")))
                                {
                                    val = camera.Name.Replace("gg_cam_", "");
                                    break;
                                }
                            }
                            parameter = new Value() { value = val };
                        }
                        else if (foreignParam.Value.StartsWith("gg_snd_"))
                        {
                            var val = foreignParam.Value.Replace("gg_snd_", "");
                            var sounds = Sounds.GetAll();
                            for (int c = 0; c < sounds.Count; c++)
                            {
                                var sound = sounds[c];
                                if (val == Ascii.ReplaceNonASCII(sound.ToString().Replace(" ", "_")))
                                {
                                    val = sound.Name.Replace("gg_snd_", "");
                                    break;
                                }
                            }
                            parameter = new Value() { value = val };
                        }

                        if (parameter != null)
                            break;

                        int id = 0;
                        variableIds.TryGetValue(foreignParam.Value, out id);
                        if (id != 0)
                        {
                            parameter = new VariableRef() { arrayIndexValues = arrayIndex, VariableId = id };
                            break;
                        }
                        triggerIds.TryGetValue(foreignParam.Value, out id);
                        parameter = new TriggerRef() { TriggerId = id };

                        break;
                    case TriggerFunctionParameterType.Function:
                        Function f = new Function();
                        if (value == "DoNothing" && foreignParam.Function != null) // special case for 'ForGroup' single action
                        {
                            f.value = foreignParam.Function.Name;
                            f.parameters = CreateParameters(foreignParam.Function.Parameters);
                            parameter = f;
                            break;
                        }

                        f.value = value;
                        f.parameters = CreateParameters(foreignParam.Function.Parameters);
                        parameter = f;
                        break;
                    case TriggerFunctionParameterType.String:
                        if (value.StartsWith("TRIGSTR"))
                        {
                            string[] split = value.Split("_");
                            string key = split[1];
                            triggerStrings.TryGetValue(uint.Parse(key), out value);
                        }
                        parameter = new Value() { value = value };
                        break;
                    case TriggerFunctionParameterType.Undefined:
                        parameter = new Parameter() { value = value };
                        break;
                    default:
                        break;
                }
                if (parameter != null)
                    parameters.Add(parameter);
            }

            return parameters;
        }


        private ExplorerElement GetVariable(TriggerItem triggerItem)
        {
            ExplorerElement variable;
            if (triggers.SubVersion is not null)
            {
                explorerVariables.TryGetValue(triggerItem.Id, out variable);
            }
            else
            {
                explorerVariables_byName.TryGetValue(triggerItem.Name, out variable);
            }

            return variable;
        }

        private ExplorerElement GetVariable(VariableDefinition variableDef)
        {
            ExplorerElement variable;
            if (triggers.SubVersion is not null)
            {
                explorerVariables.TryGetValue(variableDef.Id, out variable);
            }
            else
            {
                explorerVariables_byName.TryGetValue(variableDef.Name, out variable);
            }

            return variable;
        }
    }
}