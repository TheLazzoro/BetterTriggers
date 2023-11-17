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

namespace BetterTriggers.WorldEdit
{
    public class TriggerConverter
    {
        public event Action<string> OnExplorerElementImported;

        private string mapPath;
        private MapTriggers triggers;
        private MapInfo mapInfo;
        private ScriptLanguage language;
        private Dictionary<uint, string> triggerStrings = new Dictionary<uint, string>(); // [wts key, trigger string]

        private string rootComment = string.Empty;
        private string rootHeader = string.Empty;
        private List<string> wctStrings = new List<string>();
        private int wctIndex = 0;

        Dictionary<int, string> triggerPaths = new Dictionary<int, string>(); // [triggerId, our path in the filesystem]
        Dictionary<string, string> variableTypes = new Dictionary<string, string>(); // [name, type]
        Dictionary<string, int> variableIds = new Dictionary<string, int>(); // [name, variableId]
        Dictionary<string, int> triggerIds = new Dictionary<string, int>(); // [name, triggerId]

        Dictionary<int, ExplorerElementVariable> explorerVariables = new Dictionary<int, ExplorerElementVariable>(); // [id, variable]

        Dictionary<int, War3ProjectFileEntry> projectFilesEntries = new Dictionary<int, War3ProjectFileEntry>(); // [id, file entry in the project]

        /// <summary>
        /// Converts an entire map's triggers to a Better Triggers project.
        /// </summary>
        /// <returns>Project file path.</returns>
        public string Convert(string mapPath, string projectDestinationDir)
        {
            this.mapPath = mapPath;
            Load(mapPath);
            return ConvertAllTriggers(projectDestinationDir);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPath">Path to map we're importing from.</param>
        /// <exception cref="Exception"></exception>
        public void ImportIntoCurrentProject(string mapPath, List<TriggerItem> itemsToImport)
        {
            if (Project.CurrentProject == null)
            {
                throw new Exception("Cannot import when no project is open.");
            }

            this.mapPath = mapPath;
            Load(mapPath);
            ConvertSelectedTriggers(itemsToImport);
        }

        private void Load(string mapPath)
        {
            CustomMapData.Load(mapPath);


            string pathTriggers = "war3map.wtg";
            string pathCustomText = "war3map.wct";
            string pathInfo = "war3map.w3i";
            string pathTriggerStrings = "war3map.wts";
            if (CustomMapData.MPQMap.Triggers == null)
                return;

            triggers = CustomMapData.MPQMap.Triggers;
            var customTextTriggers = CustomMapData.MPQMap.CustomTextTriggers;
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
            mapInfo = CustomMapData.MPQMap.Info;
            language = mapInfo.ScriptLanguage;
            var wts = CustomMapData.MPQMap.TriggerStrings;
            wts.Strings.ForEach(trigStr => triggerStrings.TryAdd(trigStr.Key, trigStr.Value));

            // Prepare all trigger items
            if (triggers != null)
            {
                // First, gather all variables names and ids
                for (int i = 0; i < triggers.Variables.Count; i++)
                {
                    var variable = triggers.Variables[i];

                    variableIds.Add(variable.Name, variable.Id);
                    variableTypes.Add(variable.Name, variable.Type);

                    explorerVariables.Add(variable.Id, CreateVariable(variable));
                }

                // Then, gather all trigger names and ids
                for (int i = 0; i < triggers.TriggerItems.Count; i++)
                {
                    var triggerItem = triggers.TriggerItems[i];
                    if (triggerItem.Type != TriggerItemType.Gui)
                        continue;

                    triggerIds.TryAdd("gg_trg_" + triggerItem.Name.Replace(" ", "_"), triggerItem.Id);
                }
            }
        }

        List<IExplorerElement> triggerElementsToImport;
        private void ConvertSelectedTriggers(List<TriggerItem> selectedTriggers)
        {
            var project = Project.CurrentProject;
            if (project == null)
            {
                throw new Exception("Cannot import when no project is active.");
            }

            var root = project.GetRoot();
            string targetDir = Path.Combine(root.GetPath(), mapInfo.MapName + "_Imported");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            triggerPaths.Add(0, targetDir); // root path for the imported triggers

            triggerElementsToImport = new List<IExplorerElement>();
            for (int i = 0; i < selectedTriggers.Count; i++)
            {
                var triggerItem = selectedTriggers[i];
                if (triggerItem is DeletedTriggerItem || triggerItem.Type is TriggerItemType.RootCategory)
                    continue;

                IExplorerElement explorerElement = CreateExplorerElement(triggerItem);
                triggerPaths.TryAdd(triggerItem.Id, explorerElement.GetPath());
                if (explorerElement == null)
                    continue;

                triggerElementsToImport.Add(explorerElement);

            }

            List<IExplorerElement> dummyElements = new List<IExplorerElement>();
            // Resolve collisions
            for (int i = 0; i < triggerElementsToImport.Count; i++)
            {
                var element = triggerElementsToImport[i];

                // Resolve name collision

                string dirLocation = Path.GetDirectoryName(element.GetPath());
                switch (element)
                {
                    case ExplorerElementTrigger:
                        string triggerName = project.Triggers.GenerateTriggerName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, triggerName));
                        break;
                    case ExplorerElementVariable:
                        string variableName = project.Variables.GenerateName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, variableName + ".var"));
                        break;
                    case ExplorerElementScript:
                        string scriptName = project.Scripts.GenerateName(element as ExplorerElementScript);
                        element.SetPath(Path.Combine(dirLocation, scriptName));
                        break;
                    case ExplorerElementFolder:
                        string folderName = project.Folders.GenerateName(element.GetName());
                        element.SetPath(Path.Combine(dirLocation, folderName));
                        break;
                    default:
                        break;
                }

                if (element is ExplorerElementFolder || element is ExplorerElementScript)
                    continue;

                // Resolve id-collisions

                int id = element.GetId();
                bool doesIdExist = false;
                bool isVariable = false;
                switch (element)
                {
                    case ExplorerElementTrigger:
                        doesIdExist = project.Triggers.Contains(id);
                        break;
                    case ExplorerElementVariable:
                        doesIdExist = project.Variables.Contains(id);
                        isVariable = true;
                        break;
                    default:
                        break;
                }

                if (doesIdExist)
                {
                    int oldId = id;
                    int newId = isVariable ? project.Variables.GenerateId() : project.Triggers.GenerateId();
                    if (element is ExplorerElementTrigger t)
                    {
                        t.trigger.Id = newId;
                    }
                    else if (element is ExplorerElementVariable v)
                    {
                        v.variable.Id = newId;
                    }

                    foreach (var trigger in triggerElementsToImport)
                    {
                        if (trigger is ExplorerElementTrigger trig)
                        {
                            var functions = Triggers.GetFunctionsFromTrigger(trig);
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
                if (element is ExplorerElementVariable variable)
                {
                    project.Variables.AddVariable(variable);
                }
                else if (element is ExplorerElementTrigger trigger)
                {
                    project.Triggers.AddTrigger(trigger);
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

            // Write to disk
            project.EnableFileEvents(false);
            for (int i = 0; i < triggerElementsToImport.Count; i++)
            {
                var element = triggerElementsToImport[i];
                string path = element.GetPath();
                if (element is ExplorerElementFolder)
                    Directory.CreateDirectory(path);
                else
                {
                    var saveable = (IExplorerSaveable)element;
                    File.WriteAllText(path, saveable.GetSaveableString());
                }
                project.OnCreateElement(path, false); // We manually create UI elements
                OnExplorerElementImported?.Invoke(path);
            }
            project.EnableFileEvents(true);
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
                for (int i = 0; i < triggers.TriggerItems.Count; i++)
                {
                    var triggerItem = triggers.TriggerItems[i];
                    if (triggerItem is DeletedTriggerItem || triggerItem.Type is TriggerItemType.RootCategory)
                        continue;

                    IExplorerElement explorerElement = CreateExplorerElement(triggerItem);
                    if (explorerElement == null)
                        continue;

                    triggerPaths.TryAdd(triggerItem.Id, explorerElement.GetPath());
                    if (explorerElement is ExplorerElementFolder)
                        Directory.CreateDirectory(explorerElement.GetPath());
                    else
                    {
                        var saveable = (IExplorerSaveable)explorerElement;
                        File.WriteAllText(explorerElement.GetPath(), saveable.GetSaveableString());
                    }

                    War3ProjectFileEntry entry = new War3ProjectFileEntry()
                    {
                        isEnabled = explorerElement.GetEnabled(),
                        isInitiallyOn = explorerElement.GetInitiallyOn(),
                        path = explorerElement.GetSaveablePath(),
                    };

                    War3ProjectFileEntry parentEnty;
                    projectFilesEntries.TryGetValue(triggerItem.ParentId, out parentEnty);
                    if (parentEnty == null)
                        project.Files.Add(entry);
                    else
                        parentEnty.Files.Add(entry);

                    projectFilesEntries.TryAdd(triggerItem.Id, entry);
                }
            }

            File.WriteAllText(projectPath, JsonConvert.SerializeObject(project, Formatting.Indented));

            return projectPath;
        }

        private IExplorerElement CreateExplorerElement(TriggerItem triggerItem)
        {
            IExplorerElement explorerElement = null;
            string extension = string.Empty;

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
                    explorerElement = CreateTrigger(triggerItem as TriggerDefinition);
                    extension = ".trg";
                    break;
                case TriggerItemType.Comment:
                    break;
                case TriggerItemType.Script:
                    explorerElement = CreateScript(triggerItem as TriggerDefinition, wctStrings[wctIndex]);
                    wctIndex++;
                    extension = mapInfo.ScriptLanguage == ScriptLanguage.Jass ? ".j" : ".lua";
                    break;
                case TriggerItemType.Variable:
                    ExplorerElementVariable explorerElementVariable;
                    explorerVariables.TryGetValue(triggerItem.Id, out explorerElementVariable);
                    explorerElement = explorerElementVariable;
                    extension = ".var";
                    break;
                case TriggerItemType.UNK7:
                    break;
                default:
                    break;
            }

            if (explorerElement == null)
                return null;

            string parentPath;
            triggerPaths.TryGetValue(triggerItem.ParentId, out parentPath);
            if (parentPath == null) // could not find the element's location, put it in root.
            {
                triggerPaths.TryGetValue(0, out parentPath);
            }

            string name = triggerItem.Name;
            List<char> invalidPathChars = Path.GetInvalidPathChars().ToList();
            invalidPathChars.AddRange(Path.GetInvalidFileNameChars());

            int i = 0;
            while (i < invalidPathChars.Count)
            {
                name = name.Replace(invalidPathChars[i].ToString(), "");
                i++;
            }
            name = name.TrimStart().TrimEnd();
            name = name.TrimEnd('.'); // files/dirs cannot end with '.'
            string suffix = string.Empty;
            bool ok = false;
            i = 0;
            string finalName = string.Empty;
            while (!ok)
            {
                finalName = name + suffix;
                if (explorerElement is IExplorerSaveable && !File.Exists(Path.Combine(parentPath, finalName + extension)))
                    ok = true;
                else if (explorerElement is ExplorerElementFolder && !Directory.Exists(Path.Combine(parentPath, finalName + extension)))
                    ok = true;

                suffix = i.ToString();
                i++;
            }
            explorerElement.SetPath(Path.Combine(parentPath, finalName + extension));

            return explorerElement;
        }


        private ExplorerElementFolder CreateFolder(TriggerCategoryDefinition triggerCategory)
        {
            if (triggerCategory == null)
                return null;

            ExplorerElementFolder folder = new ExplorerElementFolder();
            return folder;
        }

        private ExplorerElementVariable CreateVariable(VariableDefinition variableDefinition)
        {
            int arrSize = variableDefinition.ArraySize == 0 ? 1 : variableDefinition.ArraySize;
            Parameter initialValue = new Parameter();
            if (TriggerData.ConstantExists(variableDefinition.InitialValue))
                initialValue = new Constant { value = variableDefinition.InitialValue };
            else if (variableDefinition.InitialValue != "")
                initialValue = new Value { value = variableDefinition.InitialValue };

            ExplorerElementVariable variable = new ExplorerElementVariable()
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

        private ExplorerElementScript CreateScript(TriggerDefinition triggerDefinition, string script)
        {
            ExplorerElementScript element = new ExplorerElementScript();
            element.isEnabled = triggerDefinition.IsEnabled;
            element.script = script;

            return element;
        }

        private ExplorerElementTrigger CreateTrigger(TriggerDefinition triggerDefinition)
        {
            if (triggerDefinition == null)
                return null;

            ExplorerElementTrigger explorerElementTrigger = new ExplorerElementTrigger();
            Trigger trigger = new Trigger();
            explorerElementTrigger.trigger = trigger;
            explorerElementTrigger.isEnabled = triggerDefinition.IsEnabled;
            explorerElementTrigger.isInitiallyOn = triggerDefinition.IsInitiallyOn;

            trigger.Id = triggerDefinition.Id;
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
                        throw new Exception("Should not reach here!");
                    default:
                        break;
                }
            });

            CreateSubElements(explorerElementTrigger.trigger.Events, Events);
            CreateSubElements(explorerElementTrigger.trigger.Conditions, Conditions);
            CreateSubElements(explorerElementTrigger.trigger.Actions, Actions);

            return explorerElementTrigger;
        }

        private void CreateSubElements(List<TriggerElement> triggerElements, List<TriggerFunction> triggerFunctions)
        {
            triggerFunctions.ForEach(function =>
            {
                ECA te = TriggerElementFactory.Create(function.Name);
                te.isEnabled = function.IsEnabled;
                te.function.parameters = CreateParameters(function.Parameters);

                triggerElements.Add(te);

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
                        parameter = new Constant()
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
                        string type = string.Empty;
                        variableIds.TryGetValue(foreignParam.Value, out id);
                        variableTypes.TryGetValue(foreignParam.Value, out type);
                        if (id != 0)
                        {
                            parameter = new VariableRef() { arrayIndexValues = arrayIndex, VariableId = id };
                            break;
                        }
                        triggerIds.TryGetValue(foreignParam.Value, out id);
                        type = "trigger";
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

    }
}