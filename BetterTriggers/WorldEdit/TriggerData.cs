using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using BetterTriggers.Controllers;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using War3Net.Build.Info;

namespace BetterTriggers.WorldEdit
{
    public class TriggerData
    {
        internal static Dictionary<string, ConstantTemplate> ConstantTemplates = new Dictionary<string, ConstantTemplate>();
        internal static Dictionary<string, FunctionTemplate> EventTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> ConditionTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> ActionTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> CallTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> FunctionsAll = new Dictionary<string, FunctionTemplate>();

        internal static Dictionary<string, string> ParamDisplayNames = new Dictionary<string, string>();
        internal static Dictionary<string, string> ParamCodeText = new Dictionary<string, string>();
        internal static Dictionary<string, string> FunctionCategories = new Dictionary<string, string>();


        internal static void Load()
        {
            string baseDir = System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/";
            string pathCommonJ = baseDir + "common.j";
            string pathBlizzardJ = baseDir + "Blizzard.j";

            if (!File.Exists(pathCommonJ))
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];

                CASCFile commonJ = (CASCFile)units.Entries["common.j"];
                Casc.SaveFile(commonJ, pathCommonJ);
            }

            if (!File.Exists(pathBlizzardJ))
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];

                CASCFile blizzardJ = (CASCFile)units.Entries["Blizzard.j"];
                Casc.SaveFile(blizzardJ, pathBlizzardJ);
            }

            var ui = (CASCFolder)Casc.GetWar3ModFolder().Entries["ui"];
            CASCFile triggerData = (CASCFile)ui.Entries["triggerdata.txt"];
            var file = Casc.GetCasc().OpenFile(triggerData.FullName);
            var reader = new StreamReader(file);
            var text = reader.ReadToEnd();

            var iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);

            // --- TRIGGER CATEGORIES --- //

            var triggerCategories = data.Sections["TriggerCategories"];
            var replText = (CASCFolder)Casc.GetWar3ModFolder().Entries["replaceabletextures"];
            var worldEditUI = (CASCFolder)replText.Entries["worldeditui"];
            foreach (var category in triggerCategories)
            {
                string[] values = category.Value.Split(",");

                if (values[1] == "none")
                    continue;

                string WE_STRING = values[0];
                string texturePath = Path.GetFileName(values[1] + ".dds");
                bool shouldDisplay = true;
                if (values.Length == 3)
                    shouldDisplay = false;

                CASCFile icon = (CASCFile)worldEditUI.Entries[texturePath];
                Stream stream = Casc.GetCasc().OpenFile(icon.FullName);

                Category.Create(category.KeyName, stream, WE_STRING, shouldDisplay);
            }

            Stream s;
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_map.png", FileMode.Open);
            Category.Create("TC_MAP", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggeraction.png", FileMode.Open);
            Category.Create("TC_ACTION", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggercondition.png", FileMode.Open);
            Category.Create("TC_CONDITION_NEW", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggerevent.png", FileMode.Open);
            Category.Create("TC_EVENT", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_trigger-error.png", FileMode.Open);
            Category.Create("TC_ERROR", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_ui-editoricon-triggercategories_element.png", FileMode.Open);
            Category.Create("TC_TRIGGER_NEW", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_ui-editoricon-triggercategories_folder.png", FileMode.Open);
            Category.Create("TC_DIRECTORY", s, "???", false);
            s = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggerscript.png", FileMode.Open);
            Category.Create("TC_SCRIPT", s, "???", false);




            // --- TRIGGER TYPES (GUI VARIABLE TYPE DEFINITIONS) --- //

            var triggerTypes = data.Sections["TriggerTypes"];
            foreach (var type in triggerTypes)
            {
                string[] values = type.Value.Split(",");
                string key = type.KeyName;
                bool canBeGlobal = values[1] == "1" ? true : false;
                bool canBeCompared = values[2] == "1" ? true : false;
                string displayName = values[3];
                string baseType = null;
                if (values.Length >= 5)
                    baseType = values[4];

                Types.Create(key, canBeGlobal, canBeCompared, displayName, baseType);
            }



            // --- TRIGGER PARAMS (CONSTANTS OR PRESETS) --- //

            var triggerParams = data.Sections["TriggerParams"];
            foreach (var preset in triggerParams)
            {

                string[] values = preset.Value.Split(",");
                string key = preset.KeyName;

                string variableType = values[1];
                string codeText = values[2].Replace("\"", "").Replace("`", "");
                string displayText = Locale.Translate(values[3]);

                ConstantTemplate constant = new ConstantTemplate()
                {
                    identifier = key,
                    returnType = variableType,
                    name = displayText,
                    codeText = codeText
                };
                ConstantTemplates.Add(key, constant);
                ParamDisplayNames.Add(key, displayText);
                ParamCodeText.Add(key, codeText);
            }


            // --- TRIGGER FUNCTIONS --- //

            LoadFunctions(data, "TriggerEvents", EventTemplates);
            LoadFunctions(data, "TriggerConditions", ConditionTemplates);
            LoadFunctions(data, "TriggerActions", ActionTemplates);
            LoadFunctions(data, "TriggerCalls", CallTemplates);
        }

        private static void LoadFunctions(IniData data, string sectionName, Dictionary<string, FunctionTemplate> dictionary)
        {
            var triggerEvents = data.Sections[sectionName];
            string identifier = string.Empty;
            FunctionTemplate functionTemplate = null;
            foreach (var _event in triggerEvents)
            {
                string key = _event.KeyName;

                if (key.ToLower().StartsWith(string.Concat("_", identifier).ToLower())) // ToLower here because Blizzard typo.
                {
                    if (key.EndsWith("DisplayName"))
                    {
                        functionTemplate.name = _event.Value.Replace("\"", "");
                        ParamDisplayNames.Add(identifier, functionTemplate.name);
                    }
                    else if (key.EndsWith("Parameters"))
                    {
                        functionTemplate.paramText = _event.Value.Replace("\"", "");
                        ParamCodeText.Add(identifier, functionTemplate.paramText);
                    }
                    else if (key.EndsWith("Category"))
                    {
                        functionTemplate.category = _event.Value;
                        FunctionCategories.Add(identifier, functionTemplate.category);
                    }
                    else if (key.EndsWith("ScriptName"))
                    {
                        functionTemplate.identifier = _event.Value;
                    }

                    FunctionTemplate controlValue;
                    if (!dictionary.TryGetValue(identifier, out controlValue))
                    {
                        dictionary.Add(identifier, functionTemplate);
                        FunctionsAll.Add(identifier, functionTemplate);
                    }
                }
                else
                {

                    string returnType = string.Empty;
                    string[] _params = _event.Value.Split(",");
                    List<ParameterTemplate> parameters = new List<ParameterTemplate>();

                    if (sectionName == "TriggerEvents")
                    {
                        returnType = "event";

                        for (int i = 1; i < _params.Length; i++)
                        {
                            parameters.Add(new ParameterTemplate() { returnType = _params[i] });
                        }
                    }
                    else if (sectionName == "TriggerConditions")
                    {
                        returnType = "boolean";

                        for (int i = 1; i < _params.Length; i++)
                        {
                            parameters.Add(new ParameterTemplate() { returnType = _params[i] });
                        }
                    }
                    else if (sectionName == "TriggerActions")
                    {
                        returnType = "nothing";

                        for (int i = 1; i < _params.Length; i++)
                        {
                            parameters.Add(new ParameterTemplate() { returnType = _params[i] });
                        }
                    }
                    else if (sectionName == "TriggerCalls")
                    {
                        returnType = _params[2];
                        for (int i = 3; i < _params.Length; i++)
                        {
                            parameters.Add(new ParameterTemplate() { returnType = _params[i] });
                        }
                    }
                    identifier = key;
                    functionTemplate = new FunctionTemplate();
                    functionTemplate.identifier = key;
                    functionTemplate.parameters = parameters;
                    functionTemplate.returnType = returnType;
                }
            }
        }





        public static string GetReturnType(string identifier)
        {
            if (identifier == null)
                return null;

            FunctionTemplate function;
            FunctionsAll.TryGetValue(identifier, out function);
            if (function != null)
                return function.returnType;

            ConstantTemplate constant;
            ConstantTemplates.TryGetValue(identifier, out constant);
            return constant.returnType;
        }

        public static List<string> GetParameterReturnTypes(Function f)
        {
            List<string> list = new List<string>();

            if (f.identifier == "SetVariable")
            {
                ControllerVariable controller = new ControllerVariable();
                VariableRef varRef = f.parameters[0] as VariableRef;
                if (varRef != null)
                {
                    Variable variable = controller.GetByReference(f.parameters[0] as VariableRef);
                    list.Add(variable.Type);
                    list.Add(variable.Type);
                    return list;
                }
                else
                {
                    list.Add("null");
                    list.Add("null");
                    return list;
                }
            }

            FunctionTemplate functionTemplate;
            FunctionsAll.TryGetValue(f.identifier, out functionTemplate);
            functionTemplate.parameters.ForEach(p => list.Add(p.returnType));
            return list;
        }

        internal static string GetConstantCodeText(string identifier, ScriptLanguage language)
        {
            string codeText = string.Empty;
            ConstantTemplate constant;
            ConstantTemplates.TryGetValue(identifier, out constant);
            codeText = constant.codeText;
            if (language == ScriptLanguage.Lua)
            {
                if (constant.codeText == "!=")
                    codeText = "~=";
                else if (constant.codeText == "null")
                    codeText = "nil";
            }

            return codeText;
        }

        internal static bool ConstantExists(string identifier)
        {
            ConstantTemplate temp;
            bool exists = ConstantTemplates.TryGetValue(identifier, out temp);
            return exists;
        }
    }
}
