using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using System.Threading;
using BetterTriggers.Controllers;
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

        public void Convert(string mapPath, string outputFullPath)
        {
            Load(mapPath);
            Convert(outputFullPath);
        }

        private void Load(string mapPath)
        {
            string pathTriggers = "war3map.wtg";
            string pathCustomText = "war3map.wct";
            string pathInfo = "war3map.w3i";
            string pathTriggerStrings = "war3map.wts";
            if (!File.Exists(Path.Combine(mapPath, pathTriggers)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(mapPath, pathTriggers), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                triggers = BinaryReaderExtensions.ReadMapTriggers(reader);
            }
            using (Stream s = new FileStream(Path.Combine(mapPath, pathCustomText), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var customTextTriggers = BinaryReaderExtensions.ReadMapCustomTextTriggers(reader, System.Text.Encoding.UTF8);
                rootComment = customTextTriggers.GlobalCustomScriptComment;
                rootHeader = customTextTriggers.GlobalCustomScriptCode.Code.Substring(0, customTextTriggers.GlobalCustomScriptCode.Code.Length - 1); // trim last byte
                customTextTriggers.CustomTextTriggers.ForEach(item =>
                {
                    if (item.Code != string.Empty)
                        wctStrings.Add(item.Code.Substring(0, item.Code.Length - 1)); // trim last byte
                });
            }
            using (Stream s = new FileStream(Path.Combine(mapPath, pathInfo), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                mapInfo = BinaryReaderExtensions.ReadMapInfo(reader);
                language = mapInfo.ScriptLanguage;
            }
            using (Stream s = new FileStream(Path.Combine(mapPath, pathTriggerStrings), FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(s);
                var wts = StreamReaderExtensions.ReadMapTriggerStrings(sr);
                wts.Strings.ForEach(trigStr => triggerStrings.Add(trigStr.Key, trigStr.Value));
            }
        }

        private void Convert(string fullPath)
        {
            ControllerProject controller = new ControllerProject();
            string projectPath = controller.CreateProject(language, Path.GetFileName(fullPath), Path.GetDirectoryName(fullPath), false);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(File.ReadAllText(projectPath));

            project.Comment = rootComment;
            project.Header = rootHeader;
            triggerPaths.Add(0, project.Root);

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
                    path = explorerElement.GetPath(),
                };

                War3ProjectFileEntry parentEnty;
                projectFilesEntries.TryGetValue(triggerItem.ParentId, out parentEnty);
                if (parentEnty == null)
                    project.Files.Add(entry);
                else
                    parentEnty.Files.Add(entry);

                projectFilesEntries.TryAdd(triggerItem.Id, entry);
            }

            File.WriteAllText(projectPath, JsonConvert.SerializeObject(project, Formatting.Indented));
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

            string name = triggerItem.Name;
            string suffix = string.Empty;
            int i = 0;
            bool ok = false;
            while (!ok)
            {
                name = triggerItem.Name + suffix;
                if (!File.Exists(name) || Directory.Exists(name))
                    ok = true;

                suffix = i.ToString();
                i++;
            }
            explorerElement.SetPath(Path.Combine(parentPath, name + extension));

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
            ExplorerElementVariable variable = new ExplorerElementVariable()
            {
                variable = new Variable()
                {
                    Id = variableDefinition.Id,
                    Name = variableDefinition.Name,
                    ArraySize = new int[] { variableDefinition.ArraySize, 1 },
                    InitialValue = variableDefinition.InitialValue,
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
                TriggerElement te = TriggerElementFactory.Create(function.Name);
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
                string identifier = foreignParam.Value;

                // War3Net thingy:
                // Some functions (boolexpr) have an empty name? Dunno how many more
                if (string.IsNullOrEmpty(identifier) && foreignParam.Type == TriggerFunctionParameterType.Function)
                    identifier = foreignParam.Function.Name;

                switch (foreignParam.Type)
                {
                    case TriggerFunctionParameterType.Preset:
                        parameter = new Constant()
                        {
                            identifier = foreignParam.Value,
                        };
                        break;
                    case TriggerFunctionParameterType.Variable:
                        List<Parameter> arrayIndex = new List<Parameter>();
                        if (foreignParam.ArrayIndexer == null)
                        {
                            arrayIndex.Add(new Value() { identifier = "0" });
                            arrayIndex.Add(new Value() { identifier = "0" });
                        }
                        else
                        {
                            var list = new List<TriggerFunctionParameter>();
                            list.Add(foreignParam.ArrayIndexer);

                            arrayIndex = CreateParameters(list);
                            arrayIndex.Add(new Value() { identifier = "0" });
                        }

                        // In our editor regions, cameras, units etc. are considered values, not variables.
                        if (foreignParam.Value.StartsWith("gg_unit_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_unit_", "") };
                        else if (foreignParam.Value.StartsWith("gg_item_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_item_", "") };
                        else if (foreignParam.Value.StartsWith("gg_dest_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_dest_", "") };
                        else if (foreignParam.Value.StartsWith("gg_rct_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_rct_", "") };
                        else if (foreignParam.Value.StartsWith("gg_cam_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_cam_", "") };
                        else if (foreignParam.Value.StartsWith("gg_snd_"))
                            parameter = new Value() { identifier = foreignParam.Value.Replace("gg_snd_", "") };

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
                        f.identifier = identifier;
                        f.parameters = CreateParameters(foreignParam.Function.Parameters);
                        parameter = f;
                        break;
                    case TriggerFunctionParameterType.String:
                        if(identifier.StartsWith("TRIGSTR"))
                        {
                            string[] split = identifier.Split("_");
                            string key = split[1];
                            triggerStrings.TryGetValue(uint.Parse(key), out identifier);
                        }
                        parameter = new Value() { identifier = identifier };
                        break;
                    case TriggerFunctionParameterType.Undefined:
                        parameter = new Parameter() { identifier = identifier };
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