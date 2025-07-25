﻿using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Models.Templates;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using BetterTriggers.WorldEdit.GameDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using War3Net.Build.Audio;
using War3Net.Build.Extensions;
using War3Net.Build.Info;
using War3Net.Build.Providers;
using War3Net.Common.Extensions;

namespace BetterTriggers
{
    class PreActions
    {
        private List<string> preActions = new List<string>();

        public void Add(string script)
        {
            preActions.Add(script);
        }

        public string GetGeneratedActions()
        {
            string script = string.Empty;
            preActions.ForEach(pa => script += pa);
            return script;
        }
    }

    public class ScriptGenerator
    {
        public static string PathCommonJ { get; set; }
        public static string PathBlizzardJ { get; set; }
        public static string JassHelper { get; set; }
        public string GeneratedScript { get; private set; }

        Project project;
        ScriptLanguage language;
        List<ExplorerElement> variables = new List<ExplorerElement>();
        List<ExplorerElement> scripts = new List<ExplorerElement>();
        List<ExplorerElement> triggers = new List<ExplorerElement>();
        List<ExplorerElement> customDefinitions = new List<ExplorerElement>();
        Dictionary<string, Tuple<Parameter, string>> generatedVarNames = new Dictionary<string, Tuple<Parameter, string>>(); // [value, [parameter, returnType] ]
        List<string> globalVarNames = new List<string>(); // Used in an edge case (old maps) where vars are multiple defined.
        CultureInfo enUS = new CultureInfo("en-US");
        ExplorerElement currentExplorerElement;

        string triggerName = string.Empty;
        List<string> initialization_triggers = new List<string>();
        int nameNumber = 0;
        string newline = System.Environment.NewLine;


        // --- LANGUAGE SPECIFIC STRINGS --- //
        string separator = $"//****************************************************************************{System.Environment.NewLine}";
        string comment = "//* ";
        string globals = "globals";
        string endglobals = "endglobals";
        string functionReturnsNothing = "takes nothing returns nothing";
        string functionReturnsBoolean = "takes nothing returns boolean";
        string endfunction = "endfunction";
        string endif = "endif";
        string startLoop = $"loop{System.Environment.NewLine}\texitwhen ";
        string breakLoop = "";
        string endloop = "endloop";
        string call = "call";
        string set = "set";
        string _null = "null";
        string function = "function";
        string fourCCStart = "";
        string fourCCEnd = "";
        string strConcat = "+";
        string array2DLuaDefinition = "";
        string integer = "integer";



        public ScriptGenerator(ScriptLanguage language)
        {
            this.project = Project.CurrentProject;
            this.language = language;
            if (language == ScriptLanguage.Jass)
                return;

            separator = $"-----------------------------------------------------------------------------{System.Environment.NewLine}";
            comment = "-- ";
            globals = "";
            endglobals = "";
            functionReturnsNothing = "()";
            functionReturnsBoolean = "()";
            endfunction = "end";
            endif = "end";
            startLoop = $"while (true) do{newline}\tif(";
            breakLoop = $") then break end{newline}";
            endloop = "end";
            call = "";
            set = "";
            _null = "nil";
            function = "";
            fourCCStart = "FourCC(";
            fourCCEnd = ")";
            strConcat = "..";
            integer = "";

            array2DLuaDefinition = @"
function array2DLua(size1, size2, initialValue)
    local array2D = {}
    for i = 0, size1 do
        array2D[i] = {}

        for j = 0, size2 do
            array2D[i][j] = initialValue
        end
    end
    return array2D
end
            ";
        }


        internal bool GenerateScript()
        {
            bool success = true;
            if (Project.CurrentProject == null || Project.CurrentProject.war3project == null)
                return false;

            string scriptFile = language == ScriptLanguage.Jass ? "war3map.j" : "war3map.lua";
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, scriptFile);
            var inMemoryFiles = Project.CurrentProject.projectFiles;

            SortTriggerElements(inMemoryFiles[0]); // root node.
            StringBuilder script = Generate();

            string tempPath = language == ScriptLanguage.Jass ? "Resources\\vJass.j" : "Resources\\Lua.lua";
            var scriptFileToInput = Path.Combine(Directory.GetCurrentDirectory(), tempPath);
            File.WriteAllText(scriptFileToInput, script.ToString());

            if (language == ScriptLanguage.Jass)
            {
                Process p = new();
                ProcessStartInfo startInfo = new();
                startInfo.CreateNoWindow = true;
                startInfo.FileName = JassHelper;
                startInfo.ArgumentList.Add("--scriptonly");
                startInfo.ArgumentList.Add($"{PathCommonJ}");
                startInfo.ArgumentList.Add($"{PathBlizzardJ}");
                startInfo.ArgumentList.Add($"{scriptFileToInput}");
                startInfo.ArgumentList.Add($"{outputPath}");
                p.StartInfo = startInfo;
                p.Start();
                p.WaitForExit();
                success = p.ExitCode == 0;
                p.Kill();
                if (File.Exists(outputPath))
                    GeneratedScript = File.ReadAllText(outputPath);
            }
            else
            {
                File.WriteAllText(outputPath, script.ToString());
                if (File.Exists(outputPath))
                    GeneratedScript = File.ReadAllText(outputPath);
            }

            return success;
        }

        private void SortTriggerElements(ExplorerElement parent)
        {
            string script = string.Empty;

            // Gather all explorer elements
            ObservableCollection<ExplorerElement> children = new();
            if (parent.ElementType == ExplorerElementEnum.Root || parent.ElementType == ExplorerElementEnum.Folder)
            {
                children = parent.ExplorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];

                if (Directory.Exists(element.GetPath()))
                    SortTriggerElements(element);
                else
                {
                    switch (element.ElementType)
                    {
                        case ExplorerElementEnum.GlobalVariable:
                            element.variable.SuppressChangedEvent = true;
                            element.variable.Name = Path.GetFileNameWithoutExtension(element.GetPath()); // hack
                            element.variable.SuppressChangedEvent = false;
                            variables.Add(element);
                            break;
                        case ExplorerElementEnum.Script:
                            scripts.Add(element);
                            break;
                        case ExplorerElementEnum.Trigger:
                            triggers.Add(element);
                            break;
                        case ExplorerElementEnum.ActionDefinition:
                        case ExplorerElementEnum.ConditionDefinition:
                        case ExplorerElementEnum.FunctionDefinition:
                            customDefinitions.Add(element);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private StringBuilder Generate()
        {
            StringBuilder script = new StringBuilder(UInt16.MaxValue);

            // ---- Generate script ---- //

            List<Variable> InitGlobals = new List<Variable>();

            // Better Trigger custom presets
            var customPresets = TriggerData.customPresets;
            script.Append(globals + newline);
            for (int i = 0; i < customPresets.Count; i++)
            {
                var preset = customPresets[i];
                string varType = Types.GetBaseType(preset.Type);
                if (language == ScriptLanguage.Lua)
                    varType = "";
                script.Append($"{varType} {preset.Identifier} = {GetGlobalsStartValue(Types.GetBaseType(preset.Type))}{newline}");
            }
            script.Append(endglobals + newline);


            // Global variables
            for (int i = 0; i < variables.Count; i++)
            {
                Variable variable = variables[i].variable.Clone();

                InitGlobals.Add(variable);
                string varType = Types.GetBaseType(variable.War3Type.Type);
                string array = string.Empty;
                string dimensions = string.Empty;
                if (language == ScriptLanguage.Jass)
                {
                    if (variable.IsArray)
                    {
                        array = " array";
                        dimensions += $"[{variable.ArraySize[0]}]";
                    }
                    if (variable.IsTwoDimensions)
                        dimensions += $"[{variable.ArraySize[1]}]";
                }
                else if (language == ScriptLanguage.Lua)
                {
                    varType = "";
                    if (variable.IsArray && !variable.IsTwoDimensions)
                    {
                        dimensions += " = {}";
                    }
                    else if (variable.IsArray && variable.IsTwoDimensions)
                    {
                        // Maybe we should init this step in 'InitGlobal' because of desync risk?
                        dimensions += " = nil";
                    }
                }

                script.Append(globals + newline);
                script.Append(newline);
                if (!variable.IsArray)
                    script.Append($"{varType} {variable.GetIdentifierName()} = {GetGlobalsStartValue(varType)}");
                else
                    script.Append($"{varType}{array} {variable.GetIdentifierName()}{dimensions}");
                script.Append(newline);
                script.Append(endglobals + newline);
            }

            // Generated variables

            if (project.war3project.GenerateAllObjectVariables)
            {
                var units = Units.GetAll();
                units.ForEach(u =>
                {
                    var value = new Value
                    {
                        value = $"{u.ToString()}_{u.CreationNumber.ToString("D4")}"
                    };
                    generatedVarNames.TryAdd($"gg_unit_" + value.value, new Tuple<Parameter, string>(value, "unit"));
                });

                var dests = Destructibles.GetAll();
                dests.ForEach(d =>
                {
                    var value = new Value
                    {
                        value = $"{d.ToString()}_{d.CreationNumber.ToString("D4")}"
                    };
                    generatedVarNames.TryAdd($"gg_dest_" + value.value, new Tuple<Parameter, string>(value, "destructable"));
                });

                var items = Units.GetMapItemsAll();
                items.ForEach(i =>
                {
                    var value = new Value
                    {
                        value = $"{i.ToString()}_{i.CreationNumber.ToString("D4")}"
                    };
                    generatedVarNames.TryAdd($"gg_item_" + value.value, new Tuple<Parameter, string>(value, "item"));
                });
            }
            else // Generate only those referenced by parameters
            {

                var functions = project.GetFunctionsAll();
                var destructibles = Destructibles.GetAll();
                for (int i = 0; i < functions.Count; i++)
                {
                    var function = functions[i];
                    List<Parameter> parameters = function.parameters;
                    int errors = VerifyParameters(parameters);
                    if (errors > 0)
                    {
                        continue;
                    }

                    List<string> returnTypes = TriggerData.GetParameterReturnTypes(function, currentExplorerElement);
                    for (int j = 0; j < parameters.Count; j++)
                    {
                        if (parameters[j] is Value)
                        {
                            Value value = parameters[j] as Value;
                            if (value.value == "")
                                continue;

                            if (returnTypes[j] == "unit")
                                generatedVarNames.TryAdd("gg_unit_" + value.value, new Tuple<Parameter, string>(value, returnTypes[j]));
                            else if (returnTypes[j] == "destructable")
                            {
                                int creationNumber = 0;
                                int length = 1;
                                while (int.TryParse(value.value.Substring(value.value.Length - length, length), out var temp))
                                {
                                    creationNumber = int.Parse(value.value.Substring(value.value.Length - length, length));
                                    length++;
                                }
                                var dest = destructibles.FirstOrDefault(x => x.CreationNumber == creationNumber);
                                if (dest != null)
                                {
                                    dest.State &= ~War3Net.Build.Widget.DoodadState.Normal;
                                    generatedVarNames.TryAdd("gg_dest_" + value.value, new Tuple<Parameter, string>(value, returnTypes[j]));
                                }
                            }
                            else if (returnTypes[j] == "item")
                                generatedVarNames.TryAdd("gg_item_" + value.value, new Tuple<Parameter, string>(value, returnTypes[j]));
                        }
                    }
                }
            }

            var all_variables = Project.CurrentProject.Variables.GetAll();
            for (int i = 0; i < all_variables.Count; i++)
            {
                var variable = all_variables[i];
                if (variable.InitialValue is Value)
                {
                    Value value = variable.InitialValue as Value;
                    if (variable.War3Type.Type == "unit")
                        generatedVarNames.TryAdd("gg_unit_" + value.value, new Tuple<Parameter, string>(value, variable.War3Type.Type));
                    else if (variable.War3Type.Type == "destructable")
                        generatedVarNames.TryAdd("gg_dest_" + value.value, new Tuple<Parameter, string>(value, variable.War3Type.Type));
                    else if (variable.War3Type.Type == "item")
                        generatedVarNames.TryAdd("gg_item_" + value.value, new Tuple<Parameter, string>(value, variable.War3Type.Type));
                }
            }

            script.Append(globals + newline);

            // Generated map object globals 
            foreach (KeyValuePair<string, Tuple<Parameter, string>> kvp in generatedVarNames)
            {
                string varName = kvp.Key;
                string type = language == ScriptLanguage.Jass ? kvp.Value.Item2 : "";
                script.Append($"{type} {varName} = {_null} {newline}");
            }

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                if (globalVarNames.Contains(r.GetVariableName()))
                    continue;

                string rect = language == ScriptLanguage.Jass ? "rect" : "";
                script.Append($"{rect} {Ascii.ReplaceNonASCII($"gg_rct_{r.Name.Replace(" ", "_")}", true)} = {_null} {newline}");
                globalVarNames.Add(r.GetVariableName());
            }
            var sounds = Sounds.GetSoundsAll();
            foreach (var s in sounds)
            {
                if (globalVarNames.Contains("gg_snd_" + s.Name))
                    continue;

                string sound = language == ScriptLanguage.Jass ? "sound" : "";
                script.Append($"{sound} {Ascii.ReplaceNonASCII(s.Name.Replace(" ", "_"), true)} = {_null} {newline}");
                globalVarNames.Add(s.Name);
            }
            var music = Sounds.GetMusicAll();
            foreach (var s in music)
            {
                if (globalVarNames.Contains("gg_snd_" + s.Name))
                    continue;

                string _music = language == ScriptLanguage.Jass ? "string" : "";
                script.Append($"{_music} {Ascii.ReplaceNonASCII(s.Name.Replace(" ", "_"), true)} = {_null} {newline}");
                globalVarNames.Add(s.Name);
            }
            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                if (globalVarNames.Contains(c.GetVariableName()))
                    continue;

                string camerasetup = language == ScriptLanguage.Jass ? "camerasetup" : "";
                var cameraName = Ascii.ReplaceNonASCII($"gg_cam_{c.Name.Replace(" ", "_")}", true);
                script.Append($"{camerasetup} {cameraName} = {_null} {newline}");
                globalVarNames.Add(c.GetVariableName());
            }

            foreach (var trigger in triggers)
            {
                string _trigger = language == ScriptLanguage.Jass ? "trigger" : "";
                script.Append($"{_trigger} {Ascii.ReplaceNonASCII($"gg_trg_{trigger.GetName().Replace(" ", "_")}", true)} = {_null}{newline}");
            }

            script.Append(endglobals + newline);
            script.Append(newline);

            // Map header
            script.Append(Project.CurrentProject.war3project.Header + newline + newline);



            GenerateLuaGlobals(script);



            // Init global variables
            script.Append($"function InitGlobals {functionReturnsNothing}" + newline);
            for (int i = 0; i < InitGlobals.Count; i++)
            {
                var variable = InitGlobals[i];

                string luaGlobalPrefix = string.Empty;
                if (language == ScriptLanguage.Lua)
                {
                    if (realVarEventVariables.ContainsKey(variable.Name))
                    {
                        luaGlobalPrefix = "globals.";
                    }
                }

                string initialValue = ConvertParametersToJass(variable.InitialValue, variable.War3Type.Type, new PreActions() /* hack */ );
                if (string.IsNullOrEmpty(initialValue))
                    initialValue = GetTypeInitialValue(variable.War3Type.Type);

                if (initialValue == "null")
                    continue;


                if (!variable.IsArray && !string.IsNullOrEmpty(initialValue))
                    script.Append($"\t{set} {luaGlobalPrefix}{variable.GetIdentifierName()} =" + initialValue + newline);
                else if (variable.IsArray && !variable.IsTwoDimensions && !string.IsNullOrEmpty(initialValue))
                {
                    for (int j = 0; j <= variable.ArraySize[0]; j++)
                    {
                        script.Append($"\t{set} {luaGlobalPrefix}{variable.GetIdentifierName()}[{j}] = {initialValue}{newline}");
                    }
                }
                else if (variable.IsArray && variable.IsTwoDimensions)
                {
                    if (language == ScriptLanguage.Lua)
                    {
                        script.Append($"\t{luaGlobalPrefix}{variable.GetIdentifierName()} = array2DLua({variable.ArraySize[0]}, {variable.ArraySize[1]}, {initialValue}){newline}");
                        continue;
                    }

                    for (int j = 0; j <= variable.ArraySize[0]; j++)
                    {
                        for (int k = 0; k <= variable.ArraySize[1]; k++)
                        {
                            script.Append($"\t{set} {luaGlobalPrefix}{variable.GetIdentifierName()}[{j}][{k}] = {initialValue}{newline}");
                        }
                    }
                }
            }
            script.Append(endfunction + newline);


            script.Append(array2DLuaDefinition);
            GenerateBetterTriggersFunctions(script);

            CreateItemTables(script);
            GenerateUnitItemTables(script);
            CreateSounds(script);

            CreateDestructibles(script);
            CreateItems(script);
            CreateUnits(script);
            CreateRegions(script);
            CreateCameras(script);

            CreateCustomScripts(script);
            GenerateCustomDefinitions(script);
            GenerateTriggers(script);
            GenerateTriggerInitialization(script);

            GenerateTriggerPlayers(script);
            GenerateCustomTeams(script);
            GenerateAllyPriorities(script);

            GenerateMain(script);
            GenerateMapConfiguration(script);


            return script;
        }

        private void GenerateBetterTriggersFunctions(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Better Triggers Functions{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            if (language == ScriptLanguage.Jass)
                script.Append(TriggerData.customBJFunctions_Jass);
            else
                script.Append(TriggerData.customBJFunctions_Lua);

        }

        /// <summary>
        /// Lua mode requires variables to be wrapped inside of a special "class" like structure
        /// when being used for 'TriggerRegisterVariableEvent'.
        /// </summary>
        private Dictionary<string, Variable> realVarEventVariables = new Dictionary<string, Variable>();
        private void GenerateLuaGlobals(StringBuilder script)
        {
            if (language != ScriptLanguage.Lua)
                return;

            var functions = project.GetFunctionsAll();
            for (int i = 0; i < functions.Count; i++)
            {
                var function = functions[i];
                if (function.value != "TriggerRegisterVariableEvent")
                    continue;

                VariableRef varRef = (VariableRef)function.parameters[0];
                Variable variable = Project.CurrentProject.Variables.GetById(varRef.VariableId);
                realVarEventVariables.TryAdd(variable.Name, variable);
            }
            script.Append(separator);
            script.Append($"globals(function(_ENV){newline}");
            script.Append($"bt_genericFrameEvent = 0.0 {newline}"); // HACK. Hardcoded
            realVarEventVariables.ToList().ForEach(v =>
            {
                script.Append($"{v.Value.GetIdentifierName()} = {GetGlobalsStartValue(v.Value.War3Type.Type)}{newline}");
            });
            script.Append($"end){newline}");
            script.Append($"{newline}");
        }


        private string GetGlobalsStartValue(string returnType)
        {
            string value = _null;

            if (returnType == "boolean")
                value = "false";
            else if (returnType == "integer")
                value = "0";
            else if (returnType == "real")
                value = "0.00";

            return value;
        }


        /// <summary>
        /// TODO: We need to work more on initial variable values.
        /// This is a hardcoded quick fix.
        /// </summary>
        private string GetTypeInitialValue(string returnType)
        {
            string value = _null;

            if (returnType == "boolean")
                value = "false";
            else if (returnType == "integer")
                value = "0";
            else if (returnType == "real")
                value = "0.00";
            else if (returnType == "group")
                value = "CreateGroup()";
            else if (returnType == "force")
                value = "CreateForce()";
            else if (returnType == "timer")
                value = "CreateTimer()";
            else if (returnType == "dialog")
                value = "DialogCreate()";

            return value;
        }

        private bool IsSomethingCode(string returnType)
        {
            if (
                returnType == "unitcode" ||
                returnType == "buffcode" ||
                returnType == "abilcode" ||
                returnType == "heroskillcode" ||
                returnType == "destructablecode" ||
                returnType == "doodadcode" ||
                returnType == "techcode" ||
                returnType == "itemcode"
                )
                return true;

            return false;
        }

        private void CreateUnits(StringBuilder script)
        {
            string unit = language == ScriptLanguage.Jass ? "unit" : "";
            string integer = language == ScriptLanguage.Jass ? "integer" : "";
            string trigger = language == ScriptLanguage.Jass ? "trigger" : "";
            string real = language == ScriptLanguage.Jass ? "real" : "";

            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Unit Creation{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateAllUnits {functionReturnsNothing}{newline}");
            script.Append($"\tlocal {unit} u{newline}");
            script.Append($"\tlocal {integer} unitID{newline}");
            script.Append($"\tlocal {trigger} t{newline}");
            script.Append($"\tlocal {real} life{newline}");

            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ToString() == "sloc")
                    continue;

                var id = u.ToString();
                var owner = u.OwnerId.ToString();
                var x = u.Position.X.ToString("0.000", enUS);
                var y = u.Position.Y.ToString("0.000", enUS);
                var angle = ((180 / Math.PI) * u.Rotation).ToString("0.000", enUS); // radians to degrees
                var skinId = Int32Extensions.ToRawcode(u.SkinId);

                if (owner == "24")
                    owner = "PLAYER_NEUTRAL_AGGRESSIVE";
                if (owner == "27")
                    owner = "PLAYER_NEUTRAL_PASSIVE";

                var varName = $"gg_unit_{u.ToString()}_{u.CreationNumber.ToString("D4")}";

                Tuple<Parameter, string> value;
                if (!generatedVarNames.TryGetValue(varName, out value)) // unit with generated variable
                    varName = "u";

                if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_32)
                {
                    script.Append($"\t{set} {varName} = BlzCreateUnitWithSkin(Player({owner}), {fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {fourCCStart}'{skinId}'{fourCCEnd}){newline}");
                }
                else
                {
                    script.Append($"\t{set} {varName} = CreateUnit(Player({owner}), {fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}){newline}");
                }

                if (u.IsGoldMine())
                    script.Append($"\t{call} SetResourceAmount({varName}, {u.GoldAmount}){newline}");

                if (u.WaygateDestinationRegionId != -1)
                {
                    var destinationRect = Regions.GetAll().Where(region => region.CreationNumber == u.WaygateDestinationRegionId).SingleOrDefault();
                    if (destinationRect is not null)
                    {
                        string regionVar = Ascii.ReplaceNonASCII($"gg_rct_{destinationRect.ToString().Replace(" ", "_")}", true);
                        script.Append($"\t{call} WaygateSetDestination({varName}, GetRectCenterX({regionVar}), GetRectCenterY({regionVar})){newline}");
                        script.Append($"\t{call} WaygateActivate({varName}, true){newline}");
                    }
                }

                if (u.CustomPlayerColorId != -1)
                    script.Append($"\t{call} SetUnitColor({varName}, ConvertPlayerColor({u.CustomPlayerColorId})){newline}");

                if (u.HP != -1)
                {
                    script.Append($"\t{set} life = GetUnitState({varName}, UNIT_STATE_LIFE){newline}");
                    script.Append($"\t{call} SetUnitState({varName}, UNIT_STATE_LIFE, {u.HP}* life){newline}");
                }

                if (u.MP != -1)
                    script.Append($"\t{call} SetUnitState({varName}, UNIT_STATE_MANA, {u.MP}){newline}");

                if (u.HeroLevel != 1)
                    script.Append($"\t{call} SetHeroLevel({varName}, {u.HeroLevel}, false){newline}");

                if (u.HeroStrength != 0)
                    script.Append($"\t{call} SetHeroStr({varName}, {u.HeroStrength}, true){newline}");

                if (u.HeroAgility != 0)
                    script.Append($"\t{call} SetHeroAgi({varName}, {u.HeroAgility}, true){newline}");

                if (u.HeroIntelligence != 0)
                    script.Append($"\t{call} SetHeroInt({varName}, {u.HeroIntelligence}, true){newline}");

                float range;
                if (u.TargetAcquisition != -1f)
                {
                    if (u.TargetAcquisition == -2f)
                        range = 200f;
                    else
                        range = u.TargetAcquisition;

                    script.Append($"\t{call} SetUnitAcquireRange({varName}, {range}){newline}");
                }

                foreach (var j in u.AbilityData)
                {
                    for (uint k = 0; k < j.HeroAbilityLevel; k++)
                    {
                        script.Append($"\t{call} SelectHeroSkill({varName}, {fourCCStart}'{j.ToString()}'{fourCCEnd}){newline}");
                    }

                    ModifiedAbilityDataExtensions.TryGetOrderOffString(j, out string orderOffString);
                    //if (j.IsAutocastActive)
                    if (orderOffString != null)
                    {
                        script.Append($"\t{call} IssueImmediateOrder({varName}, \"{orderOffString}\"){newline}");
                    }

                }

                foreach (var j in u.InventoryData)
                {
                    script.Append($"\t{call} UnitAddItemToSlotById({varName}, {fourCCStart}'{Int32Extensions.ToRawcode(j.ItemId)}'{fourCCEnd}, {j.Slot}){newline}");
                }

                if (u.HasItemTableSets())
                {
                    script.Append($"\t{set} t = CreateTrigger(){newline}");
                    script.Append($"\t{call} TriggerRegisterUnitEvent(t, {varName}, EVENT_UNIT_DEATH){newline}");
                    script.Append($"\t{call} TriggerRegisterUnitEvent(t, {varName}, EVENT_UNIT_CHANGE_OWNER){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} UnitItemDrops_{u.CreationNumber.ToString("D4")}){newline}");
                }

                if (u.MapItemTableId != -1 && Info.MapInfo.RandomItemTables != null)
                {
                    var mapItemTable = Info.MapInfo.RandomItemTables[u.MapItemTableId];
                    script.Append($"\t{set} t = CreateTrigger(){newline}");
                    script.Append($"\t{call} TriggerRegisterUnitEvent(t, {varName}, EVENT_UNIT_DEATH){newline}");
                    script.Append($"\t{call} TriggerRegisterUnitEvent(t, {varName}, EVENT_UNIT_CHANGE_OWNER){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} ItemTable_{u.MapItemTableId}){newline}");
                }
            }

            script.Append($"{endfunction}{newline}{newline}");
        }

        private void CreateDestructibles(StringBuilder script)
        {
            string real = language == ScriptLanguage.Jass ? "real" : "";
            string destructible = language == ScriptLanguage.Jass ? "destructable" : "";
            string trigger = language == ScriptLanguage.Jass ? "trigger" : "";

            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Destructible Objects{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateAllDestructables {functionReturnsNothing}{newline}");
            script.Append($"\tlocal {trigger} t{newline}");
            script.Append($"local {real} life{newline}");
            script.Append($"local {destructible} d{newline}");

            var dests = Destructibles.GetAll();
            foreach (var d in dests)
            {
                // This flag tells the map to create the destructible regardless of the script.
                if (d.State.HasFlag(War3Net.Build.Widget.DoodadState.Normal))
                    continue;

                var id = d.ToString();
                var x = d.Position.X.ToString("0.000", enUS);
                var y = d.Position.Y.ToString("0.000", enUS);
                var angle = ((180 / Math.PI) * d.Rotation).ToString("0.000", enUS); // radians to degrees
                var scale = d.Scale.X.ToString("0.000", enUS);
                var variation = d.Variation;
                var skin = Int32Extensions.ToRawcode(d.SkinId);

                var varName = $"gg_dest_{d.ToString()}_{d.CreationNumber.ToString("D4")}";
                Tuple<Parameter, string> value;
                if (!generatedVarNames.ContainsKey(varName)) // dest with generated variable
                    varName = "d";

                if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_32)
                {
                    script.Append($"{set} {varName} = BlzCreateDestructableWithSkin({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {scale}, {variation}, {fourCCStart}'{skin}'{fourCCEnd}){newline}");
                }
                else
                {
                    script.Append($"{set} {varName} = CreateDestructable({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {scale}, {variation}){newline}");
                }
                if (d.Life < 100)
                {
                    script.Append($"{set} life = GetDestructableLife({varName}){newline}");
                    script.Append($"{call} SetDestructableLife({varName}, {(d.Life * 0.01).ToString(enUS)} * life){newline}");
                }

                if (d.HasItemTableSets())
                {
                    script.Append($"\t{set} t = CreateTrigger(){newline}");
                    script.Append($"\t{call} TriggerRegisterDeathEvent(t, {varName}){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} SaveDyingWidget){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} DestructableItemDrops_{d.CreationNumber.ToString("D4")}){newline}");
                }

                if (d.MapItemTableId != -1 && Info.MapInfo.RandomItemTables != null)
                {
                    var mapItemTable = Info.MapInfo.RandomItemTables[d.MapItemTableId];
                    script.Append($"\t{set} t = CreateTrigger(){newline}");
                    script.Append($"\t{call} TriggerRegisterDeathEvent(t, {varName}){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} SaveDyingWidget){newline}");
                    script.Append($"\t{call} TriggerAddAction(t, {function} ItemTable_{d.MapItemTableId}){newline}");
                }
            }

            script.Append($"{endfunction}{newline}{newline}");
        }

        private void CreateItems(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Item Creation{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateAllItems {functionReturnsNothing}{newline}");
            if (language == ScriptLanguage.Jass)
                script.Append($"\tlocal item i = {_null}{newline}");
            else
                script.Append($"\tlocal i = {_null}{newline}");

            var items = Units.GetMapItemsAll();
            foreach (var i in items)
            {
                if (i.ToString() == "sloc")
                    continue;

                var id = i.ToString();
                var x = i.Position.X.ToString("0.000", enUS);
                var y = i.Position.Y.ToString("0.000", enUS);
                var skinId = Int32Extensions.ToRawcode(i.SkinId);

                var varName = $"gg_item_{i.ToString()}_{i.CreationNumber.ToString("D4")}";

                Tuple<Parameter, string> value;
                if (!generatedVarNames.TryGetValue(varName, out value)) // unit with generated variable
                    varName = "i";

                if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_32)
                {
                    script.Append($"\t{set} {varName} = BlzCreateItemWithSkin({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {fourCCStart}'{skinId}'{fourCCEnd}){newline}");
                }
                else
                {
                    script.Append($"\t{set} {varName} = CreateItem({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}){newline}");
                }
            }

            script.Append($"{endfunction}{newline}{newline}");
        }


        private void CreateRegions(StringBuilder script)
        {
            string weathereffect = language == ScriptLanguage.Jass ? "weathereffect" : "";

            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Regions{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateRegions {functionReturnsNothing}{newline}");
            script.Append($"local {weathereffect} we{newline}");
            script.Append($"{newline}");

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                var id = r.Name.Replace(" ", "_");
                var left = r.Left;
                var bottom = r.Bottom;
                var right = r.Right;
                var top = r.Top;

                string varName = Ascii.ReplaceNonASCII($"gg_rct_{id}", true);

                script.Append($"{set} {varName} = Rect({left}, {bottom}, {right}, {top}){newline}");
                if (r.WeatherType == War3Net.Build.WeatherType.None)
                    continue;

                script.Append($"{set} we = AddWeatherEffect({varName}, {fourCCStart}'{Int32Extensions.ToRawcode((int)r.WeatherType)}'{fourCCEnd}){newline}");
                script.Append($"{call} EnableWeatherEffect(we, true){newline}");
            }


            script.Append($"{endfunction}{newline}{newline}");
        }


        private void CreateCameras(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Cameras{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateCameras {functionReturnsNothing}{newline}");
            script.Append($"{newline}");

            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                var id = Ascii.ReplaceNonASCII($"gg_cam_{c.Name.Replace(" ", "_")}", true);


                script.Append($"{set} {id} = CreateCameraSetup(){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ZOFFSET, {c.ZOffset.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ROTATION, {c.Rotation.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ANGLE_OF_ATTACK, {c.AngleOfAttack.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_TARGET_DISTANCE, {c.TargetDistance.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ROLL, {c.Roll.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_FIELD_OF_VIEW, {c.FieldOfView.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_FARZ, {c.FarClippingPlane.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_NEARZ, {c.NearClippingPlane.ToString("0.000", enUS)}, 0.0){newline}");
                if (WarcraftStorageReader.GameVersion >= WarcraftVersion._1_31)
                {
                    script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_PITCH, {c.LocalPitch.ToString("0.000", enUS)}, 0.0){newline}");
                    script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_YAW, {c.LocalYaw.ToString("0.000", enUS)}, 0.0){newline}");
                    script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_ROLL, {c.LocalRoll.ToString("0.000", enUS)}, 0.0){newline}");
                }
                script.Append($"{call} CameraSetupSetDestPosition({id}, {c.TargetPosition.X.ToString("0.000", enUS)}, {c.TargetPosition.Y.ToString("0.000", enUS)}, 0.0){newline}");
                script.Append($"{newline}");
            }

            script.Append($"{endfunction}{newline}{newline}");
        }


        private void CreateSounds(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Sounds{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function InitSounds {functionReturnsNothing}{newline}");
            script.Append($"{newline}");

            var sounds = Sounds.GetSoundsAll();
            var music = Sounds.GetMusicAll();
            foreach (var s in sounds)
            {
                var id = s.Name;
                var path = s.FilePath;
                var looping = s.Flags.HasFlag(SoundFlags.Looping);
                var is3D = s.Flags.HasFlag(SoundFlags.Is3DSound);
                var stopRange = s.Flags.HasFlag(SoundFlags.StopWhenOutOfRange);
                var fadeInRate = s.FadeInRate;
                var fadeOutRate = s.FadeInRate;
                var eax = s.EaxSetting;
                var cutoff = s.DistanceCutoff;
                var pitch = s.Pitch;
                var maxDistance = s.MaxDistance;

                if (cutoff >= UInt32.MaxValue)
                    cutoff = 3000;
                if (pitch >= UInt32.MaxValue)
                    pitch = 1;
                if (maxDistance >= UInt32.MaxValue)
                    maxDistance = 10000;

                script.Append($"{set} {id} = CreateSound(\"{path.Replace(@"\", @"\\")}\", {looping.ToString().ToLower()}, {is3D.ToString().ToLower()}, {stopRange.ToString().ToLower()}, {fadeInRate}, {fadeOutRate}, \"{eax}\"){newline}");
                script.Append($"{call} SetSoundParamsFromLabel({id}, \"{s.SoundName}\"){newline}");
                script.Append($"{call} SetSoundDuration({id}, {s.FadeInRate}){newline}"); // TODO: This is not the duration. We should pull from CASC data.
                script.Append($"{call} SetSoundChannel({id}, {(int)s.Channel}){newline}");
                if (s.MinDistance < 100000 && s.Channel != SoundChannel.UserInterface)
                    script.Append($"{call} SetSoundDistances({id}, {s.MinDistance}, {maxDistance}){newline}");
                if (cutoff != 3000)
                    script.Append($"{call} SetSoundDistanceCutoff({id}, {s.DistanceCutoff}){newline}");
                script.Append($"{call} SetSoundVolume({id}, {s.Volume}){newline}");
                if (pitch != 1)
                    script.Append($"{call} SetSoundPitch({id}, {pitch.ToString("0.000", enUS)}){newline}");
            }
            foreach (var s in music)
            {
                var id = s.Name;
                var path = s.FilePath;
                script.Append($"{set} {id} = \"{path.Replace(@"\", @"\\")}\"{newline}");
            }

            script.Append($"{endfunction}{newline}{newline}");
        }


        private void CreateItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Map Item Tables{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append(newline);

            var itemTables = Info.MapInfo.RandomItemTables;
            foreach (var table in itemTables)
            {
                script.Append($"function ItemTable_{table.Index} {functionReturnsNothing}{newline}");
                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    local widget trigWidget= null
	local unit trigUnit= null
	local integer itemID= 0
	local boolean canDrop= true
	set trigWidget=bj_lastDyingWidget
	if ( trigWidget == null ) then
		set trigUnit=GetTriggerUnit()
	endif
	if ( trigUnit != null ) then
		set canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() != null ) then
			set canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		endif
	endif

    if ( canDrop ) then
");
                else
                    script.Append(@"
    local trigWidget= nil
	local trigUnit= nil
	local itemID= 0
	local canDrop= true
	trigWidget=bj_lastDyingWidget
	if ( trigWidget == nil ) then
		trigUnit=GetTriggerUnit()
	end
	if ( trigUnit ~= nil ) then
		canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() ~= nil ) then
			canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		end
	end

    if ( canDrop ) then
");

                script.Append($"{newline}");

                foreach (var itemSets in table.ItemSets)
                {
                    script.Append($"\t\t{call} RandomDistReset(){newline}");
                    foreach (var item in itemSets.Items)
                    {
                        script.Append($"\t\t{call} RandomDistAddItem({fourCCStart}'{Int32Extensions.ToRawcode(item.ItemId)}'{fourCCEnd}, {item.Chance}){newline}");
                    }

                    if (language == ScriptLanguage.Jass)
                        script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
");
                    else
                        script.Append(@"
        itemID=RandomDistChoose()
		if ( trigUnit ~= nil ) then
			UnitDropItem(trigUnit, itemID)
		else
			WidgetDropItem(trigWidget, itemID)
		end
");
                }

                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    endif
	set bj_lastDyingWidget=null
	call DestroyTrigger(GetTriggeringTrigger())
endfunction
                ");
                else
                    script.Append(@"
    end
	bj_lastDyingWidget=nil
	DestroyTrigger(GetTriggeringTrigger())
end
                ");
            }

            script.Append($"{newline}");
        }


        private void GenerateUnitItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Unit Item Tables{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);


            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{u.CreationNumber.ToString("D4")} {functionReturnsNothing}{newline}");

                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    local widget trigWidget= null
	local unit trigUnit= null
	local integer itemID= 0
	local boolean canDrop= true
	set trigWidget=bj_lastDyingWidget
	if ( trigWidget == null ) then
		set trigUnit=GetTriggerUnit()
	endif
	if ( trigUnit != null ) then
		set canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() != null ) then
			set canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		endif
	endif
	if ( canDrop ) then
");
                else
                    script.Append(@"
    local trigWidget= nil
	local trigUnit= nil
	local itemID= 0
	local canDrop= true
	trigWidget=bj_lastDyingWidget
	if ( trigWidget == nil ) then
		trigUnit=GetTriggerUnit()
	end
	if ( trigUnit ~= nil ) then
		canDrop=not IsUnitHidden(trigUnit)
		if ( canDrop and GetChangingUnit() ~= nil ) then
			canDrop=( GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE) )
		end
	end
	if ( canDrop ) then
");

                script.Append($"{newline}");

                foreach (var itemSet in u.ItemTableSets)
                {
                    script.Append($"\t\t{call} RandomDistReset(){newline}");
                    foreach (var item in itemSet.Items)
                    {
                        if (item.ItemId != 0)
                            script.Append($"\t\t{call} RandomDistAddItem({fourCCStart}'{Int32Extensions.ToRawcode(item.ItemId)}'{fourCCEnd}, {item.Chance}){newline}");
                    }

                    if (language == ScriptLanguage.Jass)
                        script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		
");
                    else
                        script.Append(@"
        itemID=RandomDistChoose()
		if ( trigUnit ~= nil ) then
			UnitDropItem(trigUnit, itemID)
		else
			WidgetDropItem(trigWidget, itemID)
		end
		
");
                }
                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    endif
	    set bj_lastDyingWidget=null
	    call DestroyTrigger(GetTriggeringTrigger())
    endfunction
");
                else
                    script.Append(@"
    end
	    bj_lastDyingWidget=nil
	    DestroyTrigger(GetTriggeringTrigger())
    end
");


                script.Append($"{newline}");
            }

            var destructibles = Destructibles.GetAll();
            foreach (var d in destructibles)
            {
                if (d.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function DestructableItemDrops_{d.CreationNumber.ToString("D4")} {functionReturnsNothing}{newline}");

                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    local widget trigWidget = null
    local unit trigUnit = null
    local integer itemID = 0
    local boolean canDrop = true
    set trigWidget = bj_lastDyingWidget
    if (trigWidget == null) then
      set trigUnit = GetTriggerUnit()
    endif
    if (trigUnit != null) then
      set canDrop = not IsUnitHidden(trigUnit)
        if (canDrop and GetChangingUnit() != null ) then
           set canDrop = (GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE))
        endif
    endif

    if (canDrop) then
		");
                else
                    script.Append(@"
    local trigWidget = nil
    local trigUnit = nil
    local itemID = 0
    local canDrop = true
    trigWidget = bj_lastDyingWidget
    if (trigWidget == nil) then
      trigUnit = GetTriggerUnit()
    end
    if (trigUnit ~= nil) then
      canDrop = not IsUnitHidden(trigUnit)
        if (canDrop and GetChangingUnit() ~= nil ) then
           canDrop = (GetChangingUnitPrevOwner() == Player(PLAYER_NEUTRAL_AGGRESSIVE))
        end
    end

    if (canDrop) then
		");

                script.Append($"{newline}");

                foreach (var itemSets in d.ItemTableSets)
                {
                    script.Append($"\t\t{call} RandomDistReset(){newline}");
                    foreach (var item in itemSets.Items)
                    {
                        if (item.ItemId != 0)
                            script.Append($"\t\t{call} RandomDistAddItem({fourCCStart}'{Int32Extensions.ToRawcode(item.ItemId)}'{fourCCEnd}, {item.Chance}){newline}");
                    }

                    if (language == ScriptLanguage.Jass)
                        script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		");
                    else
                        script.Append(@"
        itemID=RandomDistChoose()
		if ( trigUnit ~= nil ) then
			UnitDropItem(trigUnit, itemID)
		else
			WidgetDropItem(trigWidget, itemID)
		end
		");
                }

                if (language == ScriptLanguage.Jass)
                    script.Append(@"

    endif

    set bj_lastDyingWidget = null

    call DestroyTrigger(GetTriggeringTrigger())
endfunction
        ");
                else
                    script.Append(@"

    end

    bj_lastDyingWidget = nil

    DestroyTrigger(GetTriggeringTrigger())
end
        ");

                script.Append($"{newline}");
            }
        }



        private void GenerateTriggerInitialization(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Triggers{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function InitCustomTriggers {functionReturnsNothing}{newline}");

            foreach (var t in triggers)
            {
                if (!t.IsEnabled)
                    continue;

                string triggerName = Ascii.ReplaceNonASCII(t.GetName().Replace(" ", "_"), true);
                script.Append($"\t{call} InitTrig_{triggerName}(){newline}");
            }
            script.Append($"{endfunction}{newline}{newline}");
            script.Append($"{newline}");

            script.Append($"function RunInitializationTriggers {functionReturnsNothing}{newline}");
            foreach (var t in initialization_triggers)
            {
                script.Append($"\t{call} ConditionalTriggerExecute({t}){newline}");
            }
            script.Append($"{endfunction}{newline}{newline}");
        }



        private void GenerateTriggerPlayers(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Players{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function InitCustomPlayerSlots {functionReturnsNothing}{newline}");

            string[] playerType = new string[] { "MAP_CONTROL_NONE", "MAP_CONTROL_USER", "MAP_CONTROL_COMPUTER", "MAP_CONTROL_NEUTRAL", "MAP_CONTROL_RESCUABLE" };
            string[] races = new string[] { "RACE_PREF_RANDOM", "RACE_PREF_HUMAN", "RACE_PREF_ORC", "RACE_PREF_UNDEAD", "RACE_PREF_NIGHTELF" };

            int index = 0;
            var players = Info.MapInfo.Players;
            foreach (var p in players)
            {
                string player = $"Player({p.Id}), ";
                script.Append($"\t{call} SetPlayerStartLocation({player + index.ToString()}){newline}");
                if (p.Flags.HasFlag(PlayerFlags.FixedStartPosition) || p.Race == PlayerRace.Selectable)
                    script.Append($"\t{call} ForcePlayerStartLocation({player + index.ToString()}){newline}");

                script.Append($"\t{call} SetPlayerColor({player} ConvertPlayerColor({p.Id})){newline}");
                script.Append($"\t{call} SetPlayerRacePreference({player} {races[(int)p.Race]} ){newline}");
                string raceIsSelectable = p.Flags.HasFlag(PlayerFlags.RaceSelectable) ? "true" : "false";
                script.Append($"\t{call} SetPlayerRaceSelectable({player} {raceIsSelectable}){newline}");
                script.Append($"\t{call} SetPlayerController({player} {playerType[(int)p.Controller]} ){newline}");

                if (p.Controller == PlayerController.Rescuable)
                {
                    foreach (var j in players)
                    {
                        if (j.Race == PlayerRace.Human) // why is this here, HiveWE folks?
                            script.Append($"\t{call} SetPlayerAlliance({player} Player({j.Id}), ALLIANCE_RESCUABLE, true){newline}");
                    }
                }

                script.Append($"{newline}");
                index++;
            }

            script.Append($"{endfunction}{newline}{newline}");
        }



        private void GenerateCustomTeams(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Custom Teams{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function InitCustomTeams {functionReturnsNothing}{newline}");

            int current_force = 0;
            var forces = Info.MapInfo.Forces;
            foreach (var f in forces)
            {
                List<PlayerData> forcePlayers = new List<PlayerData>();
                for (int p = 0; p < 32; p++)
                {
                    if (f.Players[p] == true)
                    {
                        int i = 0;
                        while (Info.MapInfo.Players.Count > i)
                        {
                            if (Info.MapInfo.Players[i].Id == p)
                            {
                                forcePlayers.Add(Info.MapInfo.Players[i]);
                                break;
                            }

                            i++;
                        }
                    }
                }

                script.Append($"{newline}");
                script.Append($"\t{comment} Force: {f.Name}{newline}");

                string post_state = string.Empty;
                foreach (var p in forcePlayers)
                {
                    // something about player masks here? (HiveWE)
                    script.Append($"\t{call} SetPlayerTeam(Player({p.Id}), {current_force}){newline}");

                    if (f.Flags.HasFlag(ForceFlags.AlliedVictory))
                    {
                        script.Append($"\t{call} SetPlayerState(Player({p.Id}), PLAYER_STATE_ALLIED_VICTORY, 1){newline}");
                    }

                    foreach (var k in forcePlayers)
                    {
                        // something about player masks here? (HiveWE)
                        if (p.Id != k.Id)
                        {
                            if (f.Flags.HasFlag(ForceFlags.Allied))
                                post_state += $"\t{call} SetPlayerAllianceStateAllyBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareVision))
                                post_state += $"\t{call} SetPlayerAllianceStateVisionBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareUnitControl))
                                post_state += $"\t{call} SetPlayerAllianceStateControlBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                            if (f.Flags.HasFlag(ForceFlags.ShareAdvancedUnitControl))
                                post_state += $"\t{call} SetPlayerAllianceStateFullControlBJ(Player({p.Id}), Player({k.Id}), true){newline}";
                        }
                    }
                }
                if (post_state != string.Empty)
                {
                    script.Append(post_state);
                }

                script.Append($"{newline}");
                current_force++;
            }

            script.Append($"{endfunction}{newline}{newline}");
        }


        private void GenerateAllyPriorities(StringBuilder script)
        {
            script.Append($"function InitAllyPriorities {functionReturnsNothing}{newline}");

            Dictionary<int, int> player_to_startloc = new Dictionary<int, int>();

            int current_player = 0;
            foreach (var p in Info.MapInfo.Players)
            {
                player_to_startloc[p.Id] = current_player;
                current_player++;
            }

            current_player = 0;
            foreach (var p in Info.MapInfo.Players)
            {
                string player_text = string.Empty;

                int current_index = 0;
                foreach (var j in Info.MapInfo.Players)
                {
                    if (p.EnemyLowPriorityFlags == 1 && p.Id != j.Id)
                    {
                        player_text += $"\t{call} SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_LOW){newline}";
                        current_index++;
                    }
                    else if (p.EnemyHighPriorityFlags == 1 && p.Id != j.Id)
                    {
                        player_text += $"\t{call} SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_HIGH){newline}";
                        current_index++;
                    }
                }

                player_text = $"\t{call} SetStartLocPrioCount({current_player}, {current_index}){newline}";
                script.Append(player_text);
                current_player++;
            }

            script.Append($"{endfunction}{newline}{newline}");
        }



        private void GenerateMain(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Main Initialization{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function main {functionReturnsNothing}{newline}");

            string camera_bounds = $"\t{call} SetCameraBounds(" +
                (Info.MapInfo.CameraBounds.BottomLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.BottomLeft.Y - 256f) + " + GetCameraMargin(CAMERA_MARGIN_BOTTOM), " +

                (Info.MapInfo.CameraBounds.TopRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.TopRight.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.TopLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.TopLeft.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.BottomRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.BottomRight.Y - 256f) + $" + GetCameraMargin(CAMERA_MARGIN_BOTTOM)){newline}";

            script.Append(camera_bounds);

            string terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            string unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            if (terrain_lights == "")
                terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);
            if (unit_lights == "")
                unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);


            script.Append($"\t{call} SetDayNightModels(\"" + terrain_lights.Replace(@"\", @"\\") + "\", \"" + unit_lights.Replace(@"\", @"\\") + $"\"){newline}");

            if (Info.MapInfo.MapFlags.HasFlag(MapFlags.HasTerrainFog))
                script.Append($"\t{call} SetTerrainFogEx({(int)Info.MapInfo.FogStyle}, {Info.MapInfo.FogStartZ.ToString(enUS)}, {Info.MapInfo.FogEndZ.ToString(enUS)}, {Info.MapInfo.FogDensity.ToString(enUS)}, {((float)Info.MapInfo.FogColor.R / 256).ToString(enUS)}, {((float)Info.MapInfo.FogColor.G / 256).ToString(enUS)}, {((float)Info.MapInfo.FogColor.B / 256).ToString(enUS)}){newline}");

            string sound_environment = Info.MapInfo.SoundEnvironment; // TODO: Not working
            script.Append($"\t{call} NewSoundEnvironment(\"" + sound_environment + $"\"){newline}");


            War3Net.Build.Common.Tileset tileset = Info.MapInfo.Tileset;
            string ambient_day = "LordaeronSummerDay";
            string ambient_night = "LordaeronSummerNight";
            switch (tileset)
            {
                case War3Net.Build.Common.Tileset.Ashenvale:
                    ambient_day = "AshenvaleDay";
                    ambient_night = "AshenvaleNight";
                    break;
                case War3Net.Build.Common.Tileset.Barrens:
                    ambient_day = "BarrensDay";
                    ambient_night = "BarrensNight";
                    break;
                case War3Net.Build.Common.Tileset.BlackCitadel:
                    ambient_day = "BlackCitadelDay";
                    ambient_night = "BlackCitadelNight";
                    break;
                case War3Net.Build.Common.Tileset.Cityscape:
                    ambient_day = "CityScapeDay";
                    ambient_night = "CityScapeNight";
                    break;
                case War3Net.Build.Common.Tileset.Dalaran:
                    ambient_day = "DalaranDay";
                    ambient_night = "DalaranNight";
                    break;
                case War3Net.Build.Common.Tileset.DalaranRuins:
                    ambient_day = "DalaranRuinsDay";
                    ambient_night = "DalaranRuinsNight";
                    break;
                case War3Net.Build.Common.Tileset.Dungeon:
                    ambient_day = "DungeonDay";
                    ambient_night = "DungeonNight";
                    break;
                case War3Net.Build.Common.Tileset.Felwood:
                    ambient_day = "FelwoodDay";
                    ambient_night = "FelwoodNight";
                    break;
                case War3Net.Build.Common.Tileset.IcecrownGlacier:
                    ambient_day = "IceCrownDay";
                    ambient_night = "IceCrownNight";
                    break;
                case War3Net.Build.Common.Tileset.LordaeronFall:
                    ambient_day = "LordaeronFallDay";
                    ambient_night = "LordaeronFallNight";
                    break;
                case War3Net.Build.Common.Tileset.LordaeronSummer:
                    ambient_day = "LordaeronSummerDay";
                    ambient_night = "LordaeronSummerNight";
                    break;
                case War3Net.Build.Common.Tileset.LordaeronWinter:
                    ambient_day = "LordaeronWinterDay";
                    ambient_night = "LordaeronWinterNight";
                    break;
                case War3Net.Build.Common.Tileset.Northrend:
                    ambient_day = "NorthrendDay";
                    ambient_night = "NorthrendNight";
                    break;
                case War3Net.Build.Common.Tileset.Outland:
                    ambient_day = "BlackCitadelDay";
                    ambient_night = "BlackCitadelNight";
                    break;
                case War3Net.Build.Common.Tileset.SunkenRuins:
                    ambient_day = "SunkenRuinsDay";
                    ambient_night = "SunkenRuinsNight";
                    break;
                case War3Net.Build.Common.Tileset.Underground:
                    ambient_day = "DungeonCaveDay";
                    ambient_night = "DungeonCaveNight";
                    break;
                case War3Net.Build.Common.Tileset.Village:
                    ambient_day = "VillageDay";
                    ambient_night = "VillageNight";
                    break;
                case War3Net.Build.Common.Tileset.VillageFall:
                    ambient_day = "VillageFallDay";
                    ambient_night = "VillageFallNight";
                    break;
                default:
                    break;
            }

            byte waterRed = Info.MapInfo.WaterTintingColor.R;
            byte waterGreen = Info.MapInfo.WaterTintingColor.G;
            byte waterBlue = Info.MapInfo.WaterTintingColor.B;
            byte waterAlpha = Info.MapInfo.WaterTintingColor.A;
            script.Append($"\t{call} SetWaterBaseColor({waterRed}, {waterGreen}, {waterBlue}, {waterAlpha}){newline}");
            script.Append($"\t{call} SetAmbientDaySound(\"" + ambient_day + $"\"){newline}");
            script.Append($"\t{call} SetAmbientNightSound(\"" + ambient_night + $"\"){newline}");

            script.Append($"\t{call} SetMapMusic(\"Music\", true, 0){newline}");
            script.Append($"\t{call} InitSounds(){newline}");
            script.Append($"\t{call} CreateRegions(){newline}");
            script.Append($"\t{call} CreateCameras(){newline}");
            script.Append($"\t{call} CreateAllDestructables(){newline}");
            script.Append($"\t{call} CreateAllItems(){newline}");
            script.Append($"\t{call} CreateAllUnits(){newline}");
            script.Append($"\t{call} InitBlizzard(){newline}");

            script.Append($"\t{call} InitGlobals(){newline}");
            script.Append($"\t{call} InitCustomTriggers(){newline}");
            script.Append($"\t{call} RunInitializationTriggers(){newline}");

            script.Append($"{endfunction}{newline}{newline}");
        }



        private void GenerateMapConfiguration(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Map Configuration{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function config {functionReturnsNothing}{newline}");

            script.Append($"\t{call} SetMapName(\"{Info.MapInfo.MapName}\"){newline}");
            script.Append($"\t{call} SetMapDescription(\"{Info.MapInfo.MapDescription}\"){newline}");
            script.Append($"\t{call} SetPlayers({Info.MapInfo.Players.Count}){newline}");
            script.Append($"\t{call} SetTeams({Info.MapInfo.Forces.Count}){newline}");
            script.Append($"\t{call} SetGamePlacement(MAP_PLACEMENT_TEAMS_TOGETHER){newline}");

            script.Append($"{newline}");

            var units = Units.GetMapStartLocations();
            foreach (var u in units)
            {
                //script.Append($"\t{call} DefineStartLocation({u.OwnerId}, {u.Position.X * 128f + Info.MapInfo.PlayableMapAreaWidth}, {u.Position.Y * 128f + Info.MapInfo.PlayableMapAreaHeight}){newline}");
                script.Append($"\t{call} DefineStartLocation({u.OwnerId}, {u.Position.X}, {u.Position.Y}){newline}");
            }

            script.Append($"{newline}");

            script.Append($"\t{call} InitCustomPlayerSlots(){newline}");
            if (Info.MapInfo.MapFlags.HasFlag(MapFlags.UseCustomForces))
                script.Append($"\t{call} InitCustomTeams(){newline}");
            else
            {
                foreach (var p in Info.MapInfo.Players)
                {
                    script.Append($"\t{call} SetPlayerSlotAvailable(Player({p.Id}), MAP_CONTROL_USER){newline}");
                }
                script.Append($"\t{call} InitGenericPlayerSlots(){newline}");
            }
            script.Append($"\t{call} InitAllyPriorities(){newline}");
            script.Append($"{endfunction}{newline}{newline}");
        }



        private void CreateCustomScripts(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Custom Script Code{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            foreach (var s in scripts)
            {
                currentExplorerElement = s;
                if (!s.IsEnabled)
                    continue;

                script.Append(s.script);
                script.Append($"{newline}");
            }
        }


        private string GenerateLocalVariableDefinitions(ExplorerElement ex, StringBuilder sb)
        {
            localVariables = new List<Variable>();
            localVariableDecl = new StringBuilder();
            globalLocalCarries = new List<Variable>();
            localVariableDecl = new StringBuilder();
            StringBuilder localVariableInit = new StringBuilder();
            int elementId = -1;

            TriggerElementCollection localVariableCollection = null;

            switch (ex.ElementType)
            {
                case ExplorerElementEnum.Trigger:
                    localVariableCollection = ex.trigger.LocalVariables;
                    elementId = ex.trigger.Id;
                    break;
                case ExplorerElementEnum.ActionDefinition:
                    localVariableCollection = ex.actionDefinition.LocalVariables;
                    elementId = ex.actionDefinition.Id;
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    localVariableCollection = ex.conditionDefinition.LocalVariables;
                    elementId = ex.conditionDefinition.Id;
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    localVariableCollection = ex.functionDefinition.LocalVariables;
                    elementId = ex.functionDefinition.Id;
                    break;
            }

            foreach (LocalVariable localVar in localVariableCollection.Elements)
            {
                var v = localVar.variable;
                localVariables.Add(v);
                string name = v.GetIdentifierName();
                string initialValue = ConvertParametersToJass(v.InitialValue, v.War3Type.Type, new PreActions());
                string type = language == ScriptLanguage.Jass ? Types.GetBaseType(v.War3Type.Type) : string.Empty;
                initialValue = string.IsNullOrEmpty(initialValue) ? $" = {GetTypeInitialValue(type)}" : initialValue.Insert(0, " = ");


                localVariableInit.Append($"\tlocal {type} {name}{initialValue}{newline}"); // used on first declaration
                localVariableDecl.Append($"\tlocal {type} {name}{newline}"); // used in special trigger elements

                Variable globalCarry = new Variable();
                globalCarry.Name = $"{name}_c_{elementId}";
                globalCarry.War3Type = War3Type.Get(type);
                globalCarry.IsArray = localVar.variable.IsArray;
                globalLocalCarries.Add(globalCarry);
                sb.Insert(0, $"{endglobals}{newline}");
                if (language == ScriptLanguage.Lua)
                {
                    sb.Insert(0, $" = nil{newline}");
                }
                else
                {
                    sb.Insert(0, $"{newline}");
                }
                sb.Insert(0, $"{type} {globalCarry.Name}");
                sb.Insert(0, $"{globals}{newline}");
            }


            return localVariableInit.ToString();
        }

        private void GenerateCustomDefinitions(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Custom Action-, Condition-, and Function Definitions{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            foreach (var f in customDefinitions)
            {
                currentExplorerElement = f;
                script.Append(ConvertCustomDefinitionToJASS(f));
            }
        }


        private string ConvertCustomDefinitionToJASS(ExplorerElement ex)
        {
            ParameterDefinitionCollection parameters = null;
            TriggerElementCollection actions = null;
            string returnType = "nothing";
            string functionNamePrefix = string.Empty;
            string scriptCommentTitle = string.Empty;

            switch (ex.ElementType)
            {
                case ExplorerElementEnum.ActionDefinition:
                    parameters = ex.actionDefinition.Parameters;
                    actions = ex.actionDefinition.Actions;
                    returnType = "nothing";
                    functionNamePrefix = "ActionDef_";
                    scriptCommentTitle = "Action Definition";
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    parameters = ex.conditionDefinition.Parameters;
                    actions = ex.conditionDefinition.Actions;
                    returnType = "boolean";
                    functionNamePrefix = "ConditionDef_";
                    scriptCommentTitle = "Condition Definition";
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    parameters = ex.functionDefinition.Parameters;
                    actions = ex.functionDefinition.Actions;
                    returnType = ex.functionDefinition.ReturnType.War3Type.Type;
                    functionNamePrefix = "FunctionDef_";
                    scriptCommentTitle = "Function Definition";
                    break;
                default:
                    break;
            }

            triggerName = Ascii.ReplaceNonASCII(ex.GetName().Replace(" ", "_"), true);

            StringBuilder sb_parameters = new StringBuilder();
            StringBuilder sb = new StringBuilder();


            if (language == ScriptLanguage.Jass)
                sb_parameters.Append($"takes ");
            else
                sb_parameters.Append($"(");

            if (parameters.Count() == 0)
            {
                if (language == ScriptLanguage.Jass)
                    sb_parameters.Append($"nothing returns {Types.GetBaseType(returnType)}");
                else
                    sb_parameters.Append($")");
            }
            else
            {
                for (int i = 0; i < parameters.Count(); i++)
                {
                    var parameter = (ParameterDefinition)parameters.Elements[i];
                    string parameterName = Ascii.ReplaceNonASCII(parameter.GetIdentifierName().Replace(" ", "_"), true);
                    string parameterText = language == ScriptLanguage.Jass ? $"{Types.GetBaseType(parameter.ReturnType.Type)} {parameterName}" : $"{parameterName}";
                    sb_parameters.Append(parameterText);

                    if (i < parameters.Count() - 1)
                    {
                        sb_parameters.Append(", ");
                    }
                }
                if (language == ScriptLanguage.Jass)
                    sb_parameters.Append($" returns {Types.GetBaseType(returnType)}");
                else
                    sb_parameters.Append(")");
            }

            sb.Append($"function {functionNamePrefix}{triggerName} {sb_parameters} {newline}");
            sb.Append(GenerateLocalVariableDefinitions(ex, sb));
            sb.Append(Environment.NewLine);
            PreActions pre_actions = new PreActions();
            foreach (ECA a in actions.Elements)
            {
                sb.Append($"\t{ConvertTriggerElementToJass(a, pre_actions, false)}{newline}");
            }
            sb.Append($"{endfunction}{newline}{newline}{newline}");

            string finalFunction = $"{comment} {scriptCommentTitle} {triggerName}{newline}{separator}{pre_actions.GetGeneratedActions()}{sb.ToString()}";

            return finalFunction;
        }



        private void GenerateTriggers(StringBuilder script)
        {
            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Triggers{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            foreach (var i in triggers)
            {
                currentExplorerElement = i;
                if (!i.IsEnabled)
                    continue;

                if (i.trigger.IsScript)
                {
                    script.Append($"{i.trigger.Script}{newline}");
                    if (i.trigger.RunOnMapInit)
                    {
                        string triggerVarName = "gg_trg_" + i.GetName().Replace(" ", "_");
                        initialization_triggers.Add(triggerVarName);
                    }
                }
                else
                    script.Append(ConvertGUIToJass(i, initialization_triggers));
            }
        }

        StringBuilder localVariableDecl = new();
        List<Variable> localVariables = new();
        List<Variable> globalLocalCarries = new();

        private string SetLocals()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < localVariables.Count; i++)
            {
                var global = globalLocalCarries[i];
                var local = localVariables[i];
                s.Append($"\t{set} ");
                s.Append($"{local.GetIdentifierName()}");
                s.Append($" = ");
                s.Append($"{global.Name}");
                s.Append($"{newline}");
            }
            return s.ToString();
        }

        private string CarryLocals()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < localVariables.Count; i++)
            {
                var global = globalLocalCarries[i];
                var local = localVariables[i];
                s.Append($"\t{set} ");
                s.Append($"{global.Name}");
                s.Append($" = ");
                s.Append($"{local.GetIdentifierName()}");
                s.Append($"{newline}");
            }
            return s.ToString();
        }

        private string NullLocals()
        {
            StringBuilder s = new StringBuilder();
            localVariables.ForEach(v =>
            {
                s.Append($"\t{set} ");
                s.Append($"{v.GetIdentifierName()}");
                s.Append($" = {GetGlobalsStartValue(Types.GetBaseType(v.War3Type.Type))}");
                s.Append($"{newline}");
            });
            return s.ToString();
        }

        public string ConvertGUIToJass(ExplorerElement t, List<string> initialization_triggers)
        {
            triggerName = Ascii.ReplaceNonASCII(t.GetName().Replace(" ", "_"), true);
            string triggerVarName = "gg_trg_" + triggerName;
            string triggerActionName = "Trig_" + triggerName + "_Actions";

            StringBuilder events = new StringBuilder();
            StringBuilder conditions = new StringBuilder();
            PreActions pre_actions = new PreActions();
            StringBuilder actions = new StringBuilder();
            localVariableDecl = new StringBuilder();
            localVariables = new List<Variable>();

            events.Append($"function InitTrig_{triggerName} {functionReturnsNothing}{newline}");
            events.Append($"\t{set} {triggerVarName} = CreateTrigger(){newline}");


            foreach (ECA e in t.trigger.Events.Elements)
            {
                if (!e.IsEnabled || e is InvalidECA)
                    continue;

                if (e.function.value == "MapInitializationEvent")
                {
                    initialization_triggers.Add(triggerVarName);
                    continue;
                }

                ECA clonedEvent = e.Clone(); // Need to insert trigger variable at index 0, so we clone it instead of using the original one.
                TriggerRef triggerRef = new TriggerRef()
                {
                    TriggerId = t.trigger.Id,
                    value = triggerVarName,
                };
                clonedEvent.function.parameters.Insert(0, triggerRef);

                string _event = ConvertTriggerElementToJass(clonedEvent, pre_actions, false);
                events.Append($"{_event} {newline}");
            }

            foreach (ECA c in t.trigger.Conditions.Elements)
            {
                string condition = ConvertTriggerElementToJass(c, pre_actions, true);
                if (condition == "")
                    continue;

                conditions.Append($"\tif (not ({condition})) then{newline}");
                conditions.Append($"\t\treturn false{newline}");
                conditions.Append($"\t{endif}{newline}");
            }
            actions.Insert(0, GenerateLocalVariableDefinitions(t, events));
            actions.Insert(0, $"function {triggerActionName} {functionReturnsNothing}{newline}");
            foreach (ECA a in t.trigger.Actions.Elements)
            {
                actions.Append($"\t{ConvertTriggerElementToJass(a, pre_actions, false)}{newline}");
            }
            actions.Append($"{endfunction}{newline}{newline}{newline}");

            if (conditions.ToString() != "")
            {
                conditions.Insert(0, $"function Trig_{triggerName}_Conditions {functionReturnsBoolean}{newline}");
                conditions.Append($"\treturn true{newline}");
                conditions.Append($"{endfunction}{newline}{newline}");

                events.Append($"\t{call} TriggerAddCondition({triggerVarName}, Condition({function} Trig_{triggerName}_Conditions)){newline}");
            }

            if (!t.IsInitiallyOn)
                events.Append($"\t{call} DisableTrigger({triggerVarName}){newline}");

            events.Append($"\t{call} TriggerAddAction({triggerVarName}, {function} {triggerActionName}){newline}");
            events.Append($"{endfunction}{newline}{newline}");

            string finalTrigger = $"{comment} Trigger {triggerName}{newline}{separator}{pre_actions.GetGeneratedActions()}{conditions.ToString()}{actions.ToString()}{separator}{events.ToString()}";

            return finalTrigger;
        }


        private string ConvertTriggerElementToJass(ECA t, PreActions pre_actions, bool nested)
        {
            if (!t.IsEnabled || t is InvalidECA)
                return "";
            if (VerifyParameters(t.function.parameters) > 0)
                return "";

            StringBuilder script = new StringBuilder();
            Function f = t.function;
            List<string> returnTypes = TriggerData.GetParameterReturnTypes(f, currentExplorerElement);


            if (t is ForLoopAMultiple || t is ForLoopBMultiple)
            {
                string loopIndex = f.value == "ForLoopAMultiple" ? "bj_forLoopAIndex" : "bj_forLoopBIndex";
                string loopIndexEnd = f.value == "ForLoopAMultiple" ? "bj_forLoopAIndexEnd" : "bj_forLoopBIndexEnd";

                script.Append($"{set} {loopIndex}={ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)} {newline}");
                script.Append($"{set} {loopIndexEnd}={ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)} {newline}");
                script.Append($"\t{startLoop}{loopIndex} > {loopIndexEnd}{breakLoop}{newline}");

                if (t is ForLoopAMultiple)
                {
                    ForLoopAMultiple loopA = (ForLoopAMultiple)t;
                    foreach (ECA action in loopA.Actions.Elements)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}");
                    }
                }
                else
                {
                    ForLoopBMultiple loopB = (ForLoopBMultiple)t;
                    foreach (ECA action in loopB.Actions.Elements)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}");
                    }
                }
                script.Append($"\t{set} {loopIndex}={loopIndex}+1{newline}");
                script.Append($"{endloop}");
                return script.ToString();
            }

            else if (t is ForLoopVarMultiple)
            {
                ForLoopVarMultiple loopVar = (ForLoopVarMultiple)t;
                VariableRef varRef = (VariableRef)loopVar.function.parameters[0];
                var variable = Project.CurrentProject.Variables.GetVariableById_AllLocals(varRef.VariableId);
                string varName = variable.GetIdentifierName();

                string array0 = string.Empty;
                string array1 = string.Empty;
                if (variable.IsArray)
                    array0 = $"[{ConvertParametersToJass(varRef.arrayIndexValues[0], "integer", pre_actions)}]";
                if (variable.IsTwoDimensions)
                    array1 = $"[{ConvertParametersToJass(varRef.arrayIndexValues[1], "integer", pre_actions)}]";

                script.Append($"{set} {varName}{array0}{array1} = ");
                script.Append(ConvertParametersToJass(loopVar.function.parameters[1], returnTypes[1], pre_actions) + $"{newline}");
                script.Append($"\t{startLoop}{varName}{array0}{array1} > {ConvertParametersToJass(loopVar.function.parameters[2], returnTypes[2], pre_actions)}{breakLoop}{newline}");

                foreach (ECA action in loopVar.Actions.Elements)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}");
                }
                script.Append($"\t{set} {varName}{array0}{array1} = {varName}{array0}{array1} + 1{newline}");
                script.Append($"{endloop}{newline}");

                return script.ToString();
            }

            else if (t is IfThenElse)
            {
                IfThenElse ifThenElse = (IfThenElse)t;

                List<ECA> conditions = new List<ECA>();
                ifThenElse.If.Elements.ForEach(c =>
                {
                    ECA cond = (ECA)c;
                    int emptyParams = VerifyParameters(cond.function.parameters);
                    if (cond.IsEnabled && emptyParams == 0)
                        conditions.Add(cond);
                });

                string function_name = generate_function_name(triggerName);
                string pre = string.Empty;
                pre += $"function If_{function_name} {functionReturnsBoolean}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                conditions.ForEach(cond =>
                {
                    var condition = (ECA)cond;
                    string c = ConvertTriggerElementToJass(condition, pre_actions, true);
                    /*
                        Omega edge case.
                        Maps converted from WEU to vanilla WE can have GUI custom script elements
                        in conditions! The vanilla WE can read these and they're
                        even valid elements when generating the script...
                        So... it's also supported here.

                        Actions are probably also supported in WEU,
                        but I cannot be bothered to check and add support for those.
                        With the way things work in this editor, many things would
                        need to get re-written.
                    */
                    if (condition.function.value == "CustomScriptCode")
                    {
                        pre += condition.function.parameters[0].value + newline;
                    }
                    else if (c != "")
                    {
                        pre += $"\tif (not ({c})) then{newline}";
                        pre += $"\t\treturn false{newline}";
                        pre += $"\t{endif}{newline}";
                    }
                });
                //pre += CarryLocals(); // conditions cannot modify locals
                pre += NullLocals();
                pre += $"\treturn true{newline}";
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                script.Append(CarryLocals());
                script.Append($"if(If_{function_name}()) then{newline}");

                //
                // OLD IF-CONDITIONS
                //

                //script.Append("if (");
                //List<ECA> conditions = new List<ECA>();
                //ifThenElse.If.ForEach(c =>
                //{
                //    ECA cond = (ECA)c;
                //    int emptyParams = ControllerTrigger.VerifyParameters(cond.function.parameters);
                //    if (cond.isEnabled && emptyParams == 0)
                //        conditions.Add(cond);
                //});
                //foreach (var condition in conditions)
                //{
                //    if (!condition.isEnabled)
                //        continue;

                //    script.Append($"{ConvertTriggerElementToJass(condition, pre_actions, true)} ");
                //    if (conditions.IndexOf(condition) != conditions.Count - 1)
                //        script.Append("and ");
                //}
                //if (conditions.Count == 0)
                //    script.Append("(true)");

                //script.Append($") then{newline}");

                foreach (ECA action in ifThenElse.Then.Elements)
                {
                    if (!action.IsEnabled)
                        continue;

                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}");
                }
                script.Append($"\telse{newline}");
                foreach (ECA action in ifThenElse.Else.Elements)
                {
                    if (!action.IsEnabled)
                        continue;

                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}");
                }
                script.Append($"\t{endif}{newline}");

                return script.ToString();
            }

            else if (t is ForForceMultiple || t is ForGroupMultiple)
            {
                string function_name = generate_function_name(triggerName);

                // Remove 'multiple'
                script.Append(CarryLocals());
                if (t is ForGroupMultiple) // BJ function :)))
                {
                    script.Append($"{call} {f.value.Substring(0, 8)}BJ({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {function} {function_name}){newline}");
                }
                else
                {
                    script.Append($"{call} {f.value.Substring(0, 8)}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {function} {function_name}){newline}");
                }

                string pre = string.Empty;
                string pre_content = string.Empty;
                if (f.value == "ForForceMultiple")
                {
                    ForForceMultiple forForce = (ForForceMultiple)t;
                    foreach (ECA action in forForce.Actions.Elements)
                    {
                        pre_content += $"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}";
                    }
                }
                else
                {
                    ForGroupMultiple forGroup = (ForGroupMultiple)t;
                    foreach (ECA action in forGroup.Actions.Elements)
                    {
                        pre_content += $"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}";
                    }
                }
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                pre += pre_content;
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (t is EnumDestructablesInRectAllMultiple)
            {
                EnumDestructablesInRectAllMultiple enumDest = (EnumDestructablesInRectAllMultiple)t;

                string function_name = generate_function_name(triggerName);

                // Remove 'multiple'
                script.Append($"{call} {f.value.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, function {function_name}){newline}");

                string pre = string.Empty;
                string pre_content = string.Empty;
                foreach (ECA action in enumDest.Actions.Elements)
                {
                    pre_content += $"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}";
                }
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                pre += pre_content;
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (t is EnumDestructiblesInCircleBJMultiple)
            {
                EnumDestructiblesInCircleBJMultiple enumDest = (EnumDestructiblesInCircleBJMultiple)t;

                string function_name = generate_function_name(triggerName);

                // Remove 'multiple'
                script.Append(CarryLocals());
                script.Append($"{call} {f.value.Substring(0, 27)}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)}, {function} {function_name}){newline}");

                string pre = string.Empty;
                string pre_content = string.Empty;
                foreach (ECA action in enumDest.Actions.Elements)
                {
                    pre_content += $"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}";
                }
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                pre += pre_content;
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (t is EnumItemsInRectBJ)
            {
                EnumItemsInRectBJ enumItem = (EnumItemsInRectBJ)t;

                string function_name = generate_function_name(triggerName);

                // Remove 'multiple'
                script.Append(CarryLocals());
                script.Append($"{call} {f.value.Substring(0, 17)}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {function} {function_name}){newline}");

                string pre = string.Empty;
                string pre_content = string.Empty;
                foreach (ECA action in enumItem.Actions.Elements)
                {
                    pre_content += $"\t{ConvertTriggerElementToJass(action, pre_actions, false)}{newline}";
                }
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                pre += pre_content;
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (t is AndMultiple)
            {
                AndMultiple andMultiple = (AndMultiple)t;
                List<ECA> conditions = new List<ECA>();
                andMultiple.And.Elements.ForEach(c =>
                {
                    ECA cond = (ECA)c;
                    int emptyParams = VerifyParameters(cond.function.parameters);
                    if (cond.IsEnabled && emptyParams == 0)
                        conditions.Add(cond);
                });

                string function_name = generate_function_name(triggerName);
                string pre = string.Empty;
                pre += $"function And_{function_name} {functionReturnsBoolean}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                conditions.ForEach(cond =>
                {
                    var condition = (ECA)cond;
                    string c = ConvertTriggerElementToJass(condition, pre_actions, true);
                    if (condition.function.value == "CustomScriptCode")
                    {
                        pre += condition.function.parameters[0].value + newline;
                    }
                    else if (c != "")
                    {
                        pre += $"\tif (not ({c})) then{newline}";
                        pre += $"\t\treturn false{newline}";
                        pre += $"\t{endif}{newline}";
                    }
                });
                pre += $"\treturn true{newline}";
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);
                script.Append($"And_{function_name}()");

                return script.ToString();

                //
                // OLD AND-CONDITION
                //

                //var verifiedTriggerElements = new List<ECA>();
                //foreach (ECA element in andMultiple.And)
                //{
                //    if (!element.isEnabled)
                //        continue;

                //    int emptyParams = ControllerTrigger.VerifyParameters(element.function.parameters);
                //    if (emptyParams == 0)
                //        verifiedTriggerElements.Add(element);
                //}

                //if (verifiedTriggerElements.Count == 0)
                //    return "(true)";

                //script.Append("(");
                //foreach (var condition in verifiedTriggerElements)
                //{
                //    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, true)} ");

                //    if (verifiedTriggerElements.IndexOf(condition) != verifiedTriggerElements.Count - 1)
                //        script.Append("and ");
                //}
                //script.Append(")");

                //return script.ToString();
            }

            else if (t is OrMultiple)
            {
                OrMultiple orMultiple = (OrMultiple)t;
                List<ECA> conditions = new List<ECA>();
                orMultiple.Or.Elements.ForEach(c =>
                {
                    ECA cond = (ECA)c;
                    int emptyParams = VerifyParameters(cond.function.parameters);
                    if (cond.IsEnabled && emptyParams == 0)
                        conditions.Add(cond);
                });

                string function_name = generate_function_name(triggerName);
                string pre = string.Empty;
                pre += $"function Or_{function_name} {functionReturnsBoolean}{newline}";
                pre += localVariableDecl.ToString();
                pre += SetLocals();
                conditions.ForEach(cond =>
                {
                    var condition = (ECA)cond;
                    string c = ConvertTriggerElementToJass(condition, pre_actions, true);
                    if (condition.function.value == "CustomScriptCode")
                    {
                        pre += condition.function.parameters[0].value + newline;
                    }
                    else if (c != "")
                    {
                        pre += $"\tif ({c}) then{newline}";
                        pre += $"\t\treturn true{newline}";
                        pre += $"\t{endif}{newline}";
                    }
                });
                pre += $"\treturn false{newline}";
                pre += CarryLocals();
                pre += NullLocals();
                pre += $"{newline}{endfunction}{newline}{newline}";
                pre_actions.Add(pre);
                script.Append($"Or_{function_name}()");

                return script.ToString();

                //
                // OLD OR-CONDITION
                //

                //var verifiedTriggerElements = new List<ECA>();
                //foreach (ECA element in orMultiple.Or)
                //{
                //    if (!element.isEnabled)
                //        continue;

                //    int emptyParams = ControllerTrigger.VerifyParameters(element.function.parameters);
                //    if (emptyParams == 0)
                //        verifiedTriggerElements.Add(element);
                //}

                //if (verifiedTriggerElements.Count == 0)
                //    return "(true)";

                //script.Append("(");
                //foreach (var condition in verifiedTriggerElements)
                //{
                //    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, true)} ");

                //    if (verifiedTriggerElements.IndexOf(condition) != verifiedTriggerElements.Count - 1)
                //        script.Append("or ");
                //}
                //script.Append(")");

                //return script.ToString();
            }
            else

                return ConvertFunctionToJass(f, pre_actions, nested, t);
        }


        private string ConvertFunctionToJass(Function f, PreActions pre_actions, bool nested, TriggerElement? triggerElement = null)
        {
            StringBuilder script = new StringBuilder();

            int invalidParams = VerifyParameters(f.parameters);
            if (invalidParams > 0)
                return "";


            List<string> returnTypes = TriggerData.GetParameterReturnTypes(f, currentExplorerElement);

            // ------------------------- //
            // --- SPECIALLY HANDLED --- //
            // ------------------------- //

            if (f.value == "SetVariable")
            {
                script.Append($"{set} {ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)} = {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)}");
                return script.ToString();
            }

            else if (f.value == "WaitForCondition")
            {
                script.Append($"loop{newline}");
                script.Append($"exitwhen({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, true)}){newline}");
                script.Append($"{call} TriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)})){newline}");
                script.Append($"{endloop}");
                return script.ToString();
            }

            else if (f.value == "ReturnAction")
            {
                return $"return {newline}";
            }

            else if (f.value.StartsWith("OperatorCompare"))
            {
                return $"{ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)} {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)} {ConvertParametersToJass(f.parameters[2], returnTypes[2], pre_actions)}";
            }

            else if (f.value == "IfThenElse")
            {
                script.Append($"if({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, true)}) then {newline}");
                script.Append($"\t\t{ConvertFunctionToJass((Function)f.parameters[1], pre_actions, nested)} {newline}");
                script.Append($"\telse {newline}");
                script.Append($"\t\t{ConvertFunctionToJass((Function)f.parameters[2], pre_actions, nested)} {newline}");
                script.Append($"\t{endif} {newline}");

                return script.ToString();
            }

            else if (f.value == "ForLoopA" || f.value == "ForLoopB")
            {
                string loopIndex = f.value == "ForLoopA" ? "bj_forLoopAIndex" : "bj_forLoopBIndex";
                string loopIndexEnd = f.value == "ForLoopA" ? "bj_forLoopAIndexEnd" : "bj_forLoopBIndexEnd";

                script.Append($"{set} {loopIndex}={ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)} {newline}");
                script.Append($"{set} {loopIndexEnd}={ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)} {newline}");
                script.Append($"\t{startLoop}{loopIndex} > {loopIndexEnd}{breakLoop}{newline}");
                script.Append($"\t{ConvertFunctionToJass((Function)f.parameters[2], pre_actions, nested)} {newline}");
                script.Append($"\t{set} {loopIndex}={loopIndex}+1{newline}");
                script.Append($"{endloop} {newline}");

                return script.ToString();
            }

            else if (f.value == "ForLoopVar")
            {
                VariableRef varRef = (VariableRef)f.parameters[0];
                var variable = Project.CurrentProject.Variables.GetVariableById_AllLocals(varRef.VariableId);
                string varName = variable.GetIdentifierName();

                string array0 = string.Empty;
                string array1 = string.Empty;
                if (variable.IsArray)
                    array0 = $"[{ConvertParametersToJass(varRef.arrayIndexValues[0], "integer", pre_actions)}]";
                if (variable.IsTwoDimensions)
                    array1 = $"[{ConvertParametersToJass(varRef.arrayIndexValues[1], "integer", pre_actions)}]";

                script.Append($"{set} {varName}{array0}{array1} = ");
                script.Append(ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions) + $"{newline}");
                script.Append($"\t{startLoop}{varName}{array0}{array1} > {ConvertParametersToJass(f.parameters[2], returnTypes[2], pre_actions)}{breakLoop}{newline}");
                script.Append($"\t{ConvertFunctionToJass((Function)f.parameters[3], pre_actions, nested)} {newline}");
                script.Append($"\t{set} {varName}{array0}{array1} = {varName}{array0}{array1} + 1{newline}");
                script.Append($"\t{endloop}{newline}");

                return script.ToString();
            }

            else if (f.value == "ForForce" || f.value == "ForGroup" || f.value == "EnumDestructablesInRectAll" || f.value == "EnumItemsInRectBJ")
            {
                string function_name = generate_function_name(triggerName);

                script.Append($"{call} {f.value}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {function} {function_name}){newline}");

                string pre = string.Empty;
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += $"\t{ConvertFunctionToJass(f.parameters[1] as Function, pre_actions, nested)}{newline}";
                pre += $"{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (f.value == "EnumDestructablesInCircleBJ")
            {
                string function_name = generate_function_name(triggerName);

                script.Append($"{call} {f.value}({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}, {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions)}, {function} {function_name}){newline}");

                string pre = string.Empty;
                pre += $"function {function_name} {functionReturnsNothing}{newline}";
                pre += $"\t{ConvertFunctionToJass(f.parameters[2] as Function, pre_actions, nested)}{newline}";
                pre += $"{endfunction}{newline}{newline}";
                pre_actions.Add(pre);

                return script.ToString();
            }

            else if (f.value == "AddTriggerEvent")
            {
                Function addEvent = f.Clone(); // Need to clone because of insert operation below.
                // Otherwise it's inserted into the saveable object.

                Function _event = (Function)addEvent.parameters[1];
                _event.parameters.Insert(0, addEvent.parameters[0]);
                script.Append($"{ConvertFunctionToJass(_event, pre_actions, nested)}{newline}");

                return script.ToString();
            }

            else if (f.value == "CustomScriptCode")
            {
                script.Append(f.parameters[0].value);
                return script.ToString();
            }

            else if (TriggerData.GetReturnType(f.value) == "boolexpr")
            {
                return $"{ConvertParametersToJass(f, TriggerData.GetReturnType(f.value), pre_actions, true)}";
            }

            else if (f.value == "CommentString")
            {
                return $"{newline}{comment} {f.parameters[0].value}";
            }

            // Custom
            else if (f.value == "ReturnStatement")
            {
                script.Append($"return {ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions)}");
                return script.ToString();
            }



            // --------------------- //
            // --- REGULAR CALLS --- //
            // --------------------- //


            if (!nested)
                script.Append($"{call} {ConvertParametersToJass(f, TriggerData.GetReturnType(f.value), pre_actions, false, triggerElement)}");
            else
                script.Append($"{ConvertParametersToJass(f, TriggerData.GetReturnType(f.value), pre_actions, false, triggerElement)}");


            return script.ToString();
        }



        private string ConvertParametersToJass(Parameter parameter, string returnType, PreActions pre_actions, bool boolexprIsOn = false, TriggerElement? triggerElement = null)
        {
            string output = string.Empty;

            if (parameter is Function)
            {
                Function f = (Function)parameter;
                f = f.Clone();
                var returnTypes = new List<string>();
                if (f is not FunctionDefinitionRef)
                {
                    FunctionTemplate template;
                    TriggerData.FunctionsAll.TryGetValue(f.value, out template);
                    returnTypes = TriggerData.GetParameterReturnTypes(f, currentExplorerElement);
                    if (template != null && template.scriptName != null)
                        f.value = template.scriptName; // This exists because of triggerdata.txt 'ScriptName' key.
                }

                if (returnType == "event")
                    returnTypes.Insert(0, "trigger");


                // --------------------- //
                // --- SPECIAL CALLS --- //
                // --------------------- //

                if (f.value == "IfThenElse")
                {
                    string functionName = generate_function_name(triggerName);
                    string pre = string.Empty;
                    pre += $"function {functionName} {functionReturnsNothing} {newline}";
                    pre += $"{ConvertFunctionToJass(f, pre_actions, true)}";
                    pre += $"{endfunction}{newline}{newline}";
                    pre_actions.Add(pre);

                    return $"{function} {functionName}";
                }

                else if (f.value == "OperatorInt" || f.value == "OperatorReal")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[2], returnTypes[2], pre_actions, boolexprIsOn)})";
                }

                else if (f.value == "OperatorString")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, boolexprIsOn)} {strConcat} {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions, boolexprIsOn)})";
                }

                else if (returnType == "boolexpr" && !boolexprIsOn)
                {
                    nameNumber++;
                    string functionName = "BoolExpr_" + nameNumber;
                    string pre = string.Empty;

                    pre += $"function {functionName} {functionReturnsBoolean} {newline}";
                    pre += $"\tif( ";
                    pre += ConvertParametersToJass(f, returnTypes[0], pre_actions, true);
                    pre += $") then {newline}";
                    pre += $"\t\treturn true {newline}";
                    pre += $"\t{endif} {newline}";
                    pre += $"\treturn false {newline}";
                    pre += $"{endfunction}{newline}{newline}";
                    pre_actions.Add(pre);

                    return $"Condition({function} {functionName})";
                }
                else if (f.value == "GetBooleanAnd")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, boolexprIsOn)} and {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions, boolexprIsOn)})";
                }
                else if (f.value == "GetBooleanOr")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, boolexprIsOn)} or {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions, boolexprIsOn)})";
                }

                // TODO: COPIED CODE
                else if (f.value.Length >= 15 && f.value.Substring(0, 15) == "OperatorCompare")
                {
                    return $"{ConvertParametersToJass(f.parameters[0], returnTypes[0], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[1], returnTypes[1], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[2], returnTypes[2], pre_actions, boolexprIsOn)}";
                }

                else if (f.value == "GetTriggerName")
                {
                    return $"\"{triggerName}\"";
                }

                // --------------------- //
                // --- REGULAR CALLS --- //
                // --------------------- //

                else if (triggerElement is ActionDefinitionRef actionDefRef)
                {
                    var actionDef = Project.CurrentProject.ActionDefinitions.FindById(actionDefRef.ActionDefinitionId);
                    string name = Ascii.ReplaceNonASCII(actionDef.GetName().Replace(" ", "_"), true);
                    output += "ActionDef_" + name + "(";
                }
                else if (triggerElement is ConditionDefinitionRef conditionDefRef) // TODO: Should this even be here?
                {
                    var conditionDef = Project.CurrentProject.ConditionDefinitions.FindById(conditionDefRef.ConditionDefinitionId);
                    string name = Ascii.ReplaceNonASCII(conditionDef.GetName().Replace(" ", "_"), true);
                    output += "ConditionDef_" + name + "(";
                }
                else if (f is FunctionDefinitionRef functionDefRef)
                {
                    var functionDef = Project.CurrentProject.FunctionDefinitions.FindById(functionDefRef.FunctionDefinitionId);
                    var paramCollection = functionDef.GetParameterCollection();
                    paramCollection.Elements.ForEach(p =>
                    {
                        var paramDef = (ParameterDefinition)p;
                        returnTypes.Add(paramDef.ReturnType.Type);
                    });
                    string name = Ascii.ReplaceNonASCII(functionDef.GetName().Replace(" ", "_"), true);
                    output += "FunctionDef_" + name + "(";
                }
                else
                {
                    output += f.value + "(";
                }

                for (int i = 0; i < f.parameters.Count; i++)
                {
                    Parameter p = f.parameters[i];

                    output += ConvertParametersToJass(p, returnTypes[i], pre_actions, boolexprIsOn);
                    if (i != f.parameters.Count - 1)
                        output += ",";

                }
                output += ")";
            }
            else if (parameter is Preset)
            {
                Preset c = (Preset)parameter;
                if (Types.GetBaseType(returnType) == "string")
                    output += "\"" + TriggerData.GetConstantCodeText(c.value, language) + "\"";
                else
                    output += TriggerData.GetConstantCodeText(c.value, language);

            }
            else if (parameter is VariableRef)
            {
                VariableRef v = (VariableRef)parameter;
                Variable variable = Project.CurrentProject.Variables.GetVariableById_AllLocals(v.VariableId);

                bool isVarAsString_Real = returnType == "VarAsString_Real";
                if (isVarAsString_Real)
                {
                    output += "\"";
                    output += variable.GetIdentifierName();
                }
                else if (realVarEventVariables.ContainsValue(variable))
                    output += "globals." + variable.GetIdentifierName();
                else
                    output += variable.GetIdentifierName();

                if (variable.IsArray)
                {
                    output += $"[{ConvertParametersToJass(v.arrayIndexValues[0], "integer", pre_actions, boolexprIsOn)}]";
                    if (variable.IsTwoDimensions)
                        output += $"[{ConvertParametersToJass(v.arrayIndexValues[1], "integer", pre_actions, boolexprIsOn)}]";
                }

                if (isVarAsString_Real)
                    output += "\"";
            }
            else if (parameter is TriggerRef)
            {
                TriggerRef t = (TriggerRef)parameter;
                Trigger trigger = project.Triggers.GetById(t.TriggerId).trigger;
                string name = project.Triggers.GetName(trigger.Id);

                output += "gg_trg_" + Ascii.ReplaceNonASCII(name.Replace(" ", "_"), true);
            }
            else if (parameter is ParameterDefinitionRef paramDefRef)
            {
                ParameterDefinitionCollection collection = null;
                switch (currentExplorerElement.ElementType)
                {
                    case ExplorerElementEnum.ActionDefinition:
                        collection = currentExplorerElement.actionDefinition.Parameters;
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        collection = currentExplorerElement.conditionDefinition.Parameters;
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        collection = currentExplorerElement.functionDefinition.Parameters;
                        break;
                    default:
                        break;
                }
                var paramDef = collection.GetByReference(paramDefRef);
                string name = paramDef.GetIdentifierName();
                output += Ascii.ReplaceNonASCII(name.Replace(" ", "_"), true);
            }
            else if (parameter is Value)
            {
                Value v = (Value)parameter;

                if (Types.GetBaseType(returnType) == "string")
                {
                    string value = v.value.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\r\n", "\\r\\n");
                    if (value.Length < 1024)
                    {
                        output += "\"" + value + "\"";
                        return output;
                    }

                    // prevent string literal limit.
                    output += "\"";
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i % 1024 == 0)
                        {
                            output += sb.ToString() + $"\" {strConcat} \"";
                            sb.Clear();
                        }
                        sb.Append(value[i]);
                    }
                    output += sb.ToString() + "\"";
                }
                else if (IsSomethingCode(returnType))
                    output += $"{fourCCStart}'{v.value}'{fourCCEnd}";
                else if (returnType == "unit")
                    output += $"gg_unit_{v.value.Replace(" ", "_")}";
                else if (returnType == "destructable")
                    output += $"gg_dest_{v.value.Replace(" ", "_")}";
                else if (returnType == "item")
                    output += $"gg_item_{v.value.Replace(" ", "_")}";
                else if (returnType == "rect")
                    output += Ascii.ReplaceNonASCII($"gg_rct_{v.value.Replace(" ", "_")}", true);
                else if (returnType == "camerasetup")
                    output += Ascii.ReplaceNonASCII($"gg_cam_{v.value.Replace(" ", "_")}", true);
                else if (returnType == "sound")
                {
                    if (v.value.Contains("gg_snd_"))
                        output += Ascii.ReplaceNonASCII($"{v.value.Replace(" ", "_")}", true);
                    else
                        output += Ascii.ReplaceNonASCII($"gg_snd_{v.value.Replace(" ", "_")}", true);
                }
                else
                    output += v.value;

                // TODO:
                // One would think that we need to do 'Ascii.ReplaceNonASCII' also,
                // but JassHelper for some reason does it on it's own.
                // Investigate plz.
            }

            return output;
        }

        /// <summary>
        /// Returns amount of invalid parameters.
        /// </summary>
        private int VerifyParameters(List<Parameter> parameters)
        {
            int invalidCount = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter.value == null && !(parameter is VariableRef) && !(parameter is TriggerRef) && !(parameter is ParameterDefinitionRef))
                    invalidCount++;

                if (parameter is Function)
                {
                    var function = (Function)parameter;
                    invalidCount += VerifyParameters(function.parameters);
                }
                else if (parameter is VariableRef varRef)
                {
                    var variable = Project.CurrentProject.Variables.GetVariableById_AllLocals(varRef.VariableId);
                    if (variable == null)
                        invalidCount++;
                    else
                    {
                        List<Parameter> arrays = new List<Parameter>();
                        if (variable.IsArray)
                            arrays.Add(varRef.arrayIndexValues[0]);
                        if (variable.IsArray && variable.IsTwoDimensions)
                            arrays.Add(varRef.arrayIndexValues[1]);

                        invalidCount += VerifyParameters(arrays);
                    }
                }
                else if (parameter is TriggerRef triggerRef)
                {
                    var trigger = Project.CurrentProject.Triggers.GetById(triggerRef.TriggerId);
                    if (trigger == null)
                        invalidCount++;
                }
                else if (parameter is ParameterDefinitionRef paramDefRef)
                {
                    if (currentExplorerElement == null)
                    {
                        invalidCount++;
                    }
                    else
                    {
                        ParameterDefinitionCollection collection = null;
                        switch (currentExplorerElement.ElementType)
                        {
                            case ExplorerElementEnum.ActionDefinition:
                                collection = currentExplorerElement.actionDefinition.Parameters;
                                break;
                            case ExplorerElementEnum.ConditionDefinition:
                                collection = currentExplorerElement.conditionDefinition.Parameters;
                                break;
                            case ExplorerElementEnum.FunctionDefinition:
                                collection = currentExplorerElement.functionDefinition.Parameters;
                                break;
                            default:
                                break;
                        }
                        var paramDef = collection.GetByReference(paramDefRef);
                        if (paramDef == null)
                            invalidCount++;
                    }
                }
            }

            return invalidCount;
        }

        private string generate_function_name(string triggerName)
        {
            string name = $"Trig_{triggerName}_{nameNumber}";
            nameNumber++;

            return name;
        }
    }
}
