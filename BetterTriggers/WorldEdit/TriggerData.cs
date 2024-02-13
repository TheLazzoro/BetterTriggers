using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using CASCLib;
using IniParser.Model;
using IniParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using War3Net.Build.Info;
using System.Linq;
using System.Text.RegularExpressions;
using BetterTriggers.Containers;
using War3Net.Common.Extensions;

namespace BetterTriggers.WorldEdit
{
    public class TriggerData
    {
        internal static Dictionary<string, PresetTemplate> ConstantTemplates = new Dictionary<string, PresetTemplate>();
        internal static Dictionary<string, FunctionTemplate> EventTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> ConditionTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> ActionTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> CallTemplates = new Dictionary<string, FunctionTemplate>();
        internal static Dictionary<string, FunctionTemplate> FunctionsAll = new Dictionary<string, FunctionTemplate>();

        internal static Dictionary<string, string> ParamDisplayNames = new Dictionary<string, string>();
        internal static Dictionary<string, string> ParamCodeText = new Dictionary<string, string>();
        internal static Dictionary<string, string> FunctionCategories = new Dictionary<string, string>();

        internal static List<Variable> customConstants = new List<Variable>();

        private static Dictionary<FunctionTemplate, string> Defaults = new Dictionary<FunctionTemplate, string>(); // saves the raw default values so we can operate on them later.
        internal static string customBJFunctions_Jass;
        internal static string customBJFunctions_Lua;

        public static string pathCommonJ;
        public static string pathBlizzardJ;

        public static void Load(bool isTest)
        {
            IniData data = null;

            Types.Clear();
            ConstantTemplates.Clear();
            EventTemplates.Clear();
            ConditionTemplates.Clear();
            ActionTemplates.Clear();
            CallTemplates.Clear();
            FunctionsAll.Clear();
            ParamDisplayNames.Clear();
            ParamCodeText.Clear();
            FunctionCategories.Clear();
            customConstants.Clear();
            Category.Clear();

            if (isTest)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/triggerdata.txt");
                string triggerdata = File.ReadAllText(path);
                data = IniFileConverter.GetIniData(triggerdata);

                string baseDir = Directory.GetCurrentDirectory() + "/Resources/JassHelper/";
                pathCommonJ = Path.Combine(baseDir, "common.txt");
                pathBlizzardJ = Path.Combine(baseDir, "Blizzardj.txt");
                ScriptGenerator.PathCommonJ = pathCommonJ;
                ScriptGenerator.PathBlizzardJ = pathBlizzardJ;
                ScriptGenerator.JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/clijasshelper.exe";
            }
            else
            {
                string baseDir = Directory.GetCurrentDirectory() + "/Resources/JassHelper/";
                pathCommonJ = baseDir + "common.j";
                pathBlizzardJ = baseDir + "Blizzard.j";
                ScriptGenerator.PathCommonJ = pathCommonJ;
                ScriptGenerator.PathBlizzardJ = pathBlizzardJ;
                ScriptGenerator.JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/jasshelper.exe";

                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];
                CASCFile commonJ = (CASCFile)units.Entries["common.j"];
                Casc.SaveFile(commonJ, pathCommonJ);
                units = (CASCFolder)Casc.GetWar3ModFolder().Entries["scripts"];
                CASCFile blizzardJ = (CASCFile)units.Entries["Blizzard.j"];
                Casc.SaveFile(blizzardJ, pathBlizzardJ);


                var ui = (CASCFolder)Casc.GetWar3ModFolder().Entries["ui"];
                CASCFile triggerData = (CASCFile)ui.Entries["triggerdata.txt"];
                var file = Casc.GetCasc().OpenFile(triggerData.FullName);
                var reader = new StreamReader(file);
                var text = reader.ReadToEnd();

                data = IniFileConverter.GetIniData(text);

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
                    byte[] image = new byte[stream.Length];
                    stream.CopyTo(image, 0, (int)stream.Length);

                    Category.Create(category.KeyName, image, WE_STRING, shouldDisplay);
                }

                byte[] img;
                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_map.png");
                Category.Create(TriggerCategory.TC_MAP, img, "???", false);
                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggeraction.png");
                Category.Create(TriggerCategory.TC_ACTION, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggercondition.png");
                Category.Create(TriggerCategory.TC_CONDITION_NEW, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggerevent.png");
                Category.Create(TriggerCategory.TC_EVENT, img, "Event", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/trigger-error.png");
                Category.Create(TriggerCategory.TC_ERROR, img, "Error", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/trigger-invalid.png");
                Category.Create(TriggerCategory.TC_INVALID, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_ui-editoricon-triggercategories_element.png");
                Category.Create(TriggerCategory.TC_TRIGGER_NEW, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_ui-editoricon-triggercategories_folder.png");
                Category.Create(TriggerCategory.TC_DIRECTORY, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/_editor-triggerscript.png");
                Category.Create(TriggerCategory.TC_SCRIPT, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/actions-setvariables-alpha.png");
                Category.Create(TriggerCategory.TC_LOCAL_VARIABLE, img, "???", false);

                img = File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/Resources/Icons/ui-editoricon-triggercategories_dialog.png");
                Category.Create(TriggerCategory.TC_FRAMEHANDLE, img, "Frame", true);

            }



            LoadTriggerDataFromIni(data);


            // --- LOAD CUSTOM DATA --- //

            // --- Loads in all editor versions --- //

            customBJFunctions_Jass = string.Empty;
            customBJFunctions_Lua = string.Empty;

            var textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/triggerdata_custom.txt"));
            var dataCustom = IniFileConverter.GetIniData(textCustom);
            LoadTriggerDataFromIni(dataCustom);

            textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/Globals_custom.txt"));
            dataCustom = IniFileConverter.GetIniData(textCustom);
            LoadCustomBlizzardJ(dataCustom);


            // --- Loads depending on version --- //

            if (Casc.GameVersion.Minor >= 31)
            {
                textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/triggerdata_custom_31.txt"));
                dataCustom = IniFileConverter.GetIniData(textCustom);
                LoadTriggerDataFromIni(dataCustom);

                textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/Globals_custom_31.txt"));
                dataCustom = IniFileConverter.GetIniData(textCustom);
                LoadCustomBlizzardJ(dataCustom);

                customBJFunctions_Jass += File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/FunctionDef_BT_31.txt"));
                customBJFunctions_Lua += File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/FunctionDef_BT_31_Lua.txt"));
            }
            if(Casc.GameVersion.Minor >= 32)
            {
                textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/triggerdata_custom_32.txt"));
                dataCustom = IniFileConverter.GetIniData(textCustom);
                LoadTriggerDataFromIni(dataCustom);
            }
            if (Casc.GameVersion.Minor >= 33)
            {
                textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/triggerdata_custom_33.txt"));
                dataCustom = IniFileConverter.GetIniData(textCustom);
                LoadTriggerDataFromIni(dataCustom);

                //textCustom = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/BlizzardJ_custom_33.txt"));
                //dataCustom = IniFileConverter.GetIniData(textCustom);
                //LoadCustomBlizzardJ(dataCustom);


                customBJFunctions_Jass += File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/FunctionDef_BT_33.txt"));
                customBJFunctions_Lua += File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Resources/WorldEditorData/Custom/FunctionDef_BT_33_Lua.txt"));
            }


            // --- Adds extends to types --- //

            Types.Create("agent", false, false, "Agent", string.Empty); // hack


            string[] commonJfile = File.ReadAllLines(pathCommonJ);
            List<string> types = new List<string>();
            for (int i = 0; i < commonJfile.Length; i++)
            {
                commonJfile[i] = Regex.Replace(commonJfile[i], @"\s+", " ");
                if (commonJfile[i].StartsWith("type"))
                {
                    types.Add(commonJfile[i]);
                }
            }

            types.ForEach(line =>
            {
                string[] split = line.Split(" ");
                string type = split[1];
                string extends = split[3];

                var _type = Types.Get(type);
                if(_type != null)
                    Types.Get(type).Extends = extends;
            });
        }

        private static void LoadTriggerDataFromIni(IniData data)
        {

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
                string codeText = values[2].Replace("\"", "").Replace("`", "").Replace("|", "\"");
                string displayText = Locale.Translate(values[3]);

                PresetTemplate constant = new PresetTemplate()
                {
                    value = key,
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

            // --- INIT DEFAULTS --- //
            foreach (var function in Defaults)
            {
                string[] defaultsTxt = function.Value.Split(",");
                FunctionTemplate template = function.Key;
                List<ParameterTemplate> defaults = new List<ParameterTemplate>();
                for (int i = 0; i < template.parameters.Count; i++)
                {
                    if (defaultsTxt.Length < template.parameters.Count)
                        continue;

                    string def = defaultsTxt[i];
                    ParameterTemplate oldParameter = template.parameters[i];
                    PresetTemplate constantTemplate = GetConstantTemplate(def);
                    FunctionTemplate functionTemplate = GetFunctionTemplate(def);
                    if (functionTemplate != null)
                        defaults.Add(functionTemplate);
                    else if (constantTemplate != null)
                        defaults.Add(constantTemplate);
                    else if (def != "_")
                        defaults.Add(new ValueTemplate() { value = def, returnType = oldParameter.returnType });
                    else
                        defaults.Add(new ParameterTemplate() { returnType = oldParameter.returnType });

                    /* hackfix because of Blizzard default returntype mismatch for some parameters...
                     * 'RectContainsItem' has 'GetRectCenter' for a 'rect' parameter, but returns location.
                     */
                    if (defaults[i].returnType != oldParameter.returnType)
                        defaults[i] = oldParameter;
                }

                if (defaultsTxt.Length != template.parameters.Count)
                    continue;

                template.parameters = defaults;
            }
        }


        private static void LoadFunctions(IniData data, string sectionName, Dictionary<string, FunctionTemplate> dictionary)
        {
            var section = data.Sections[sectionName];
            if (section == null)
                return;

            string name = string.Empty;
            FunctionTemplate functionTemplate = null;
            foreach (var _func in section)
            {
                string key = _func.KeyName;


                if (key.ToLower().StartsWith(string.Concat("_", name).ToLower())) // ToLower here because Blizzard typo.
                {
                    if (key.EndsWith("DisplayName"))
                    {
                        functionTemplate.name = _func.Value.Replace("\"", "");
                        ParamDisplayNames.Add(name, functionTemplate.name);
                    }
                    else if (key.EndsWith("Parameters"))
                    {
                        functionTemplate.paramText = _func.Value.Replace("\"", "");
                        ParamCodeText.Add(name, functionTemplate.paramText);
                    }
                    else if (key.EndsWith("Category"))
                    {
                        functionTemplate.category = _func.Value;
                        FunctionCategories.Add(name, functionTemplate.category);
                    }
                    else if (key.EndsWith("Defaults"))
                    {
                        string[] defaultsTxt = _func.Value.Split(",");
                        //if (defaultsTxt.Length >= 1 && defaultsTxt[0] != "" && defaultsTxt[0] != "_" && defaultsTxt[0] != "_true")
                        if (defaultsTxt.Length >= 1 && defaultsTxt[0] != "" && defaultsTxt[0] != "_true")
                            Defaults.TryAdd(functionTemplate, _func.Value);
                    }
                    else if (key.EndsWith("ScriptName"))
                    {
                        functionTemplate.scriptName = _func.Value;
                    }

                    FunctionTemplate controlValue;
                    if (!dictionary.TryGetValue(name, out controlValue))
                    {
                        dictionary.Add(name, functionTemplate);
                        FunctionsAll.Add(name, functionTemplate);
                    }
                }
                else
                {
                    string returnType = string.Empty;
                    string[] _params = _func.Value.Split(",");
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
                    // Some actions have 'nothing' as a parameter type. We don't want that.
                    parameters = parameters.Where(p => p.returnType != "nothing").ToList();
                    name = key;
                    functionTemplate = new FunctionTemplate();
                    functionTemplate.value = key;
                    functionTemplate.parameters = parameters;
                    functionTemplate.returnType = returnType;
                }
            }
        }

        /// <summary>
        /// Loads custom constants similar to 'bj_lastCreatedUnit', 'bj_lastCreatedItem' etc.
        /// </summary>
        /// <param name="iniData"></param>
        private static void LoadCustomBlizzardJ(IniData iniData)
        {
            var section = iniData.Sections["Presets"];
            foreach (var key in section)
            {
                string keyName = key.KeyName;

                string[] split = key.Value.Split(',');
                string type = split[0];

                Variable constant = new Variable()
                {
                    Name = keyName,
                    Type = type,
                };

                customConstants.Add(constant);
            }
        }




        public static string GetReturnType(string value)
        {
            if (value == null)
                return null;

            FunctionTemplate function;
            FunctionsAll.TryGetValue(value, out function);
            if (function != null)
                return function.returnType;

            PresetTemplate constant;
            ConstantTemplates.TryGetValue(value, out constant);
            return constant.returnType;
        }

        public static List<string> GetParameterReturnTypes(Function f)
        {
            List<string> list = new List<string>();

            if (f.value == "SetVariable")
            {
                VariableRef varRef = f.parameters[0] as VariableRef;
                if (varRef != null)
                {
                    Variable variable = Project.CurrentProject.Variables.GetByReference(f.parameters[0] as VariableRef);
                    if (variable != null)
                    {
                        list.Add(variable.Type);
                        list.Add(variable.Type);
                    }
                    else
                    {
                        list.Add("null");
                        list.Add("null");
                    }
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
            FunctionsAll.TryGetValue(f.value, out functionTemplate);
            if(functionTemplate != null)
                functionTemplate.parameters.ForEach(p => list.Add(p.returnType));

            return list;
        }


        private static FunctionTemplate GetFunctionTemplate(string key)
        {
            FunctionTemplate functionTemplate;
            FunctionsAll.TryGetValue(key, out functionTemplate);
            return functionTemplate;
        }

        private static PresetTemplate GetConstantTemplate(string key)
        {
            PresetTemplate constantTemplate;
            ConstantTemplates.TryGetValue(key, out constantTemplate);
            return constantTemplate;
        }

        public static List<FunctionTemplate> GetFunctionTemplatesAll()
        {
            return FunctionsAll.Select(f => f.Value).ToList();
        }

        internal static string GetConstantCodeText(string identifier, ScriptLanguage language)
        {
            string codeText = string.Empty;
            PresetTemplate constant;
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

        internal static bool ConstantExists(string value)
        {
            PresetTemplate temp;
            bool exists = ConstantTemplates.TryGetValue(value, out temp);
            return exists;
        }

        internal static bool FunctionExists(Function function)
        {
            if (function == null)
                return false;

            bool exists = false;
            exists = FunctionsAll.ContainsKey(function.value);
            return exists;
        }




        public static List<Types> LoadAllVariableTypes()
        {
            return Types.GetGlobalTypes();
        }


        public static List<FunctionTemplate> LoadAllEvents()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.EventTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                if (template.value != "InvalidECA")
                    list.Add(template.Clone());
            }
            return list;
        }

        public static List<FunctionTemplate> LoadAllCalls(string returnType)
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();

            if (returnType == "handle")
            {
                var enumerator = TriggerData.CallTemplates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var template = enumerator.Current.Value;
                    if (Types.IsHandle(template.returnType))
                    {
                        list.Add(template);
                    }
                }

                return list;
            }

            // Special case for for GUI "Matching" parameter
            bool wasBoolCall = false;
            if (returnType == "boolcall")
            {
                wasBoolCall = true;
                returnType = "boolexpr";
            }

            // Special case for GUI "Action" parameter
            else if (returnType == "code")
            {
                var enumerator = TriggerData.ActionTemplates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var template = enumerator.Current.Value;
                    if (!template.value.Contains("Multiple"))
                        list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "code");

                return list;
            }

            // Special case for GUI 'eventcall' parameter
            else if (returnType == "eventcall")
            {
                var enumerator = TriggerData.EventTemplates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var template = enumerator.Current.Value;
                    list.Add(template.Clone());
                }
                list.ForEach(call => call.returnType = "eventcall");

                return list;
            }

            var enumCalls = TriggerData.CallTemplates.GetEnumerator();
            while (enumCalls.MoveNext())
            {
                var template = enumCalls.Current.Value;
                if (returnType == template.returnType)
                    list.Add(template.Clone());
            }
            var enumConditions = TriggerData.ConditionTemplates.GetEnumerator();
            while (enumConditions.MoveNext())
            {
                var template = enumConditions.Current.Value;
                if (returnType == template.returnType || (returnType == "boolexpr" && !template.value.EndsWith("Multiple")))
                    list.Add(template.Clone());
            }
            if (wasBoolCall)
            {
                list.ForEach(call => call.returnType = "boolcall");
            }

            return list;
        }

        public static List<PresetTemplate> LoadAllConstants()
        {
            List<PresetTemplate> list = new List<PresetTemplate>();
            var enumerator = TriggerData.ConstantTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                if (template.value != "InvalidECA")
                    list.Add(template.Clone());
            }
            return list;
        }

        public static List<FunctionTemplate> LoadAllConditions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.ConditionTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                if (template.value != "InvalidECA")
                    list.Add(template.Clone());
            }
            return list;
        }

        public static List<FunctionTemplate> LoadAllActions()
        {
            List<FunctionTemplate> list = new List<FunctionTemplate>();
            var enumerator = TriggerData.ActionTemplates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var template = enumerator.Current.Value;
                if (template.value != "InvalidECA")
                    list.Add(template.Clone());
            }
            return list;
        }

        public static string GetParamDisplayName(Parameter parameter)
        {
            if (parameter is Value)
                return parameter.value;

            string displayName;
            TriggerData.ParamDisplayNames.TryGetValue(parameter.value, out displayName);
            if (displayName == null)
                displayName = parameter.value;

            return displayName;
        }

        public static string GetParamText(TriggerElement triggerElement)
        {
            string paramText = string.Empty;
            if (triggerElement is ECA)
            {
                var element = (ECA)triggerElement;
                var function = element.function;
                paramText = GetParamText(function);
            }
            else if (triggerElement is LocalVariable)
            {
                var element = (LocalVariable)triggerElement;
                paramText = element.variable.Name;
            }

            return paramText;
        }

        public static string GetParamText(Function function)
        {
            string paramText = string.Empty;
            TriggerData.ParamCodeText.TryGetValue(function.value, out paramText);
            if (paramText == null)
            {
                List<string> returnTypes = TriggerData.GetParameterReturnTypes(function);
                paramText = function.value + "(";
                for (int i = 0; i < function.parameters.Count; i++)
                {
                    var p = function.parameters[i];
                    paramText += ",~" + returnTypes[i] + ",";
                    if (i != function.parameters.Count - 1)
                        paramText += ", ";
                }
                paramText += ")";
            }

            return paramText;
        }

        public static string GetCategoryTriggerElement(TriggerElement triggerElement)
        {
            string category = string.Empty;
            if (triggerElement is ECA)
            {
                var element = (ECA)triggerElement;
                TriggerData.FunctionCategories.TryGetValue(element.function.value, out category);
            }
            else if (triggerElement is LocalVariable)
                category = TriggerCategory.TC_LOCAL_VARIABLE;

            return category;
        }
    }
}
