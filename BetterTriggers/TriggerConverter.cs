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

namespace BetterTriggers.WorldEdit
{
    public class TriggerConverter
    {
        private MapTriggers triggers;
        private MapInfo mapInfo;
        private ScriptLanguage language;

        private string rootComment = string.Empty;
        private string rootHeader = string.Empty;
        private List<string> wctStrings = new List<string>();
        private int wctIndex = 0;

        Dictionary<int, string> triggerPaths = new Dictionary<int, string>(); // [triggerId, our path in the filesystem]


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
                rootHeader = customTextTriggers.GlobalCustomScriptCode.Code;
                customTextTriggers.CustomTextTriggers.ForEach(item =>
                {
                    if (item.Code != string.Empty) wctStrings.Add(item.Code);
                });
            }
            using (Stream s = new FileStream(Path.Combine(mapPath, pathInfo), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                mapInfo = BinaryReaderExtensions.ReadMapInfo(reader);
                language = mapInfo.ScriptLanguage;
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
            }

            File.WriteAllText(projectPath, JsonConvert.SerializeObject(project));
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
                    break;
                case TriggerItemType.Variable:
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
            while(!ok)
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
            
            triggerDefinition.Functions.ForEach( function => {
                TriggerElement te = new TriggerElement();
                te.isEnabled = function.IsEnabled;
                te.function = FunctionFactory.Create(function.Name);
                CreateParameters(te.function.parameters, function.Parameters);

                switch (function.Type)
                {
                    case TriggerFunctionType.Event:
                        trigger.Events.Add(te);
                        break;
                    case TriggerFunctionType.Condition:
                        trigger.Conditions.Add(te);
                        break;
                    case TriggerFunctionType.Action:
                        trigger.Actions.Add(te);
                        break;
                    case TriggerFunctionType.Call:
                        throw new Exception("Should not reach here!");
                    default:
                        break;
                }
            });

            return explorerElementTrigger;
        }

        private void CreateParameters(List<Parameter> parameters, List<TriggerFunctionParameter> foreignParameters)
        {
            foreach (var foreignParam in foreignParameters)
            {
                string identifier = foreignParam.Value;
                //string returnType = foreignParam.

                switch (foreignParam.Type)
                {
                    case TriggerFunctionParameterType.Preset:
                        break;
                    case TriggerFunctionParameterType.Variable:
                        break;
                    case TriggerFunctionParameterType.Function:
                        break;
                    case TriggerFunctionParameterType.String:
                        break;
                    case TriggerFunctionParameterType.Undefined:
                        break;
                    default:
                        break;
                }
            }
        }

        private ExplorerElementFolder CreateFolder(TriggerCategoryDefinition triggerCategory)
        {
            if (triggerCategory == null)
                return null;

            ExplorerElementFolder folder = new ExplorerElementFolder();
            return folder;
        }
    }
}