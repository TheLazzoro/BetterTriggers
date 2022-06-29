using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using War3Net.Build.Audio;
using War3Net.Build.Info;
using War3Net.Build.Providers;
using War3Net.Common.Extensions;

namespace BetterTriggers
{
    internal class ScriptGenerator
    {
        ScriptLanguage language;
        List<ExplorerElementVariable> variables = new List<ExplorerElementVariable>();
        List<ExplorerElementScript> scripts = new List<ExplorerElementScript>();
        List<ExplorerElementTrigger> triggers = new List<ExplorerElementTrigger>();
        Dictionary<string, Value> generatedVarNames = new Dictionary<string, Value>();
        CultureInfo enUS = new CultureInfo("en-US");

        List<string> initialization_triggers = new List<string>();
        int nameNumber = 0;
        string newline = System.Environment.NewLine;


        // --- LANGUAGE SPECIFIC STRINGS --- //
        string separator = $"/****************************************************************************{System.Environment.NewLine}";
        string comment = "//* ";
        string globals = "globals";
        string endglobals = "endglobals";
        string functionReturnsNothing = "takes nothing returns nothing";
        string functionReturnsBoolean = "takes nothing returns boolean";
        string endfunction = "endfunction";
        string endif = "endif";
        string endloop = "endloop";
        string call = "call";
        string set = "set";
        string _null = "null";
        string notEquals = "!=";
        string function = "function";
        string fourCCStart = "";
        string fourCCEnd = "";


        ControllerTrigger controllerTrigger = new ControllerTrigger();

        internal ScriptGenerator(ScriptLanguage language)
        {
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
            endloop = "end";
            call = "";
            set = "";
            _null = "nil";
            notEquals = "~=";
            function = "";
            fourCCStart = "FourCC(";
            fourCCEnd = ")";
        }


        internal void GenerateScript()
        {
            if (ContainerProject.project == null)
                return;

            if (File.Exists(Path.Combine(ContainerProject.project.War3MapDirectory, "war3map.j")))
                File.Delete(Path.Combine(ContainerProject.project.War3MapDirectory, "war3map.j"));
            if (File.Exists(Path.Combine(ContainerProject.project.War3MapDirectory, "war3map.lua")))
                File.Delete(Path.Combine(ContainerProject.project.War3MapDirectory, "war3map.lua"));

            string scriptFile = language == ScriptLanguage.Jass ? "war3map.j" : "war3map.lua";
            string outputPath = Path.Combine(ContainerProject.project.War3MapDirectory, scriptFile);
            var inMemoryFiles = ContainerProject.projectFiles;

            SortTriggerElements(inMemoryFiles[0]); // root node.
            StringBuilder script = Generate();

            string tempPath = language == ScriptLanguage.Jass ? "Resources/vJass.j" : "Resources/Lua.lua";
            var scriptFileToInput = Path.Combine(System.IO.Directory.GetCurrentDirectory(), tempPath);
            File.WriteAllText(scriptFileToInput, script.ToString());

            if (language == ScriptLanguage.Jass)
            {
                string JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/jasshelper.exe";
                string CommonJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/common.j\"";
                string BlizzardJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/Blizzard.j\"";
                string fileOutput = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/output.j\"";
                Process p = Process.Start($"{JassHelper}", $"--scriptonly {CommonJ} {BlizzardJ} \"{scriptFileToInput}\" \"{outputPath}\"");
                p.WaitForExit();
                p.Kill();
            }
            else
            {
                File.WriteAllText(outputPath, script.ToString());
            }
        }

        private void SortTriggerElements(IExplorerElement parent)
        {
            string script = string.Empty;

            // Gather all explorer elements
            List<IExplorerElement> children = new List<IExplorerElement>();
            if (parent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)parent;
                children = root.explorerElements;
            }
            else if (parent is ExplorerElementFolder)
            {
                var root = (ExplorerElementFolder)parent;
                children = root.explorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                var element = (IExplorerElement)children[i];

                if (Directory.Exists(element.GetPath()))
                    SortTriggerElements(element);
                else if (element is ExplorerElementTrigger)
                {
                    triggers.Add(element as ExplorerElementTrigger);
                }
                else if (element is ExplorerElementScript)
                {
                    scripts.Add(element as ExplorerElementScript);
                }
                else if (element is ExplorerElementVariable)
                {
                    var variable = (ExplorerElementVariable)element;
                    variable.variable.Name = Path.GetFileNameWithoutExtension(element.GetPath()); // hack
                    variables.Add(element as ExplorerElementVariable);
                }
            }
        }

        private StringBuilder Generate()
        {
            StringBuilder script = new StringBuilder(UInt16.MaxValue);

            // ---- Generate script ---- //

            List<Variable> InitGlobals = new List<Variable>();

            // Global variables
            for (int i = 0; i < variables.Count; i++)
            {
                Variable variable = variables[i].variable.Clone();
                if (IsSomethingCode(variable.Type))
                    variable.Type = "integer"; // World Editor conversion stuff.
                else
                    InitGlobals.Add(variable);

                if (language == ScriptLanguage.Lua)
                    variable.Type = "";

                string array = string.Empty;
                string dimensions = string.Empty;
                if (variable.IsArray)
                {
                    array = " array";
                    dimensions += $"[{variable.ArraySize[0]}]";
                }
                if (variable.IsTwoDimensions)
                    dimensions += $"[{variable.ArraySize[1]}]";

                script.Append(globals + newline);
                script.Append(newline);
                if (!variable.IsArray)
                    script.Append($"{variable.Type} udg_{variable.Name} = {GetGlobalsStartValue(variable.Type)}");
                else
                    script.Append($"{variable.Type}{array} udg_{variable.Name}{dimensions}");
                script.Append(newline);
                script.Append(endglobals + newline);
            }

            // Generated variables
            ControllerTrigger controllerTrigger = new ControllerTrigger();
            var parameters = controllerTrigger.GetParametersAll();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is Value && isMapObject(parameters[i] as Value))
                {
                    Value value = (Value)parameters[i];

                    if (value.returnType == "unit")
                        generatedVarNames.TryAdd("gg_unit_" + value.identifier, value);
                    else if (value.returnType == "destructable")
                        generatedVarNames.TryAdd("gg_dest_" + value.identifier, value);
                    else if (value.returnType == "item")
                        generatedVarNames.TryAdd("gg_item_" + value.identifier, value);
                }
            }
            script.Append(globals + newline);

            // Generated map object globals 
            foreach (KeyValuePair<string, Value> kvp in generatedVarNames)
            {
                string varName = kvp.Key;
                string type = language == ScriptLanguage.Jass ? kvp.Value.returnType : "";
                script.Append($"{type} {varName} = {_null} {newline}");
            }

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                string rect = language == ScriptLanguage.Jass ? "rect" : "";
                script.Append($"{rect} gg_rect_{r.Name.Replace(" ", "_")} = {_null} {newline}");
            }
            var sounds = Sounds.GetSoundsAll();
            foreach (var s in sounds)
            {
                string sound = language == ScriptLanguage.Jass ? "sound" : "";
                script.Append($"{sound} {s.Name} = {_null} {newline}");
            }
            var music = Sounds.GetMusicAll();
            foreach (var s in music)
            {
                string _music = language == ScriptLanguage.Jass ? "string" : "";
                script.Append($"{_music} {s.Name} = {_null} {newline}");
            }
            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                string camerasetup = language == ScriptLanguage.Jass ? "camerasetup" : "";
                var cameraName = $"gg_cam_{c.Name.Replace(" ", "_")}";
                script.Append($"{camerasetup} {cameraName} = {_null} {newline}");
            }

            foreach (var trigger in triggers)
            {
                string _trigger = language == ScriptLanguage.Jass ? "trigger" : "";
                script.Append($"{_trigger} gg_trg_{trigger.GetName().Replace(" ", "_")} = {_null}{newline}");
            }

            script.Append(endglobals + newline);
            script.Append(newline);

            // Map header
            var root = (ExplorerElementRoot)ContainerProject.projectFiles[0];
            script.Append(root.project.Header + newline + newline);


            // Init global variables
            script.Append($"function InitGlobals {functionReturnsNothing}" + newline);
            for (int i = 0; i < InitGlobals.Count; i++)
            {
                var variable = InitGlobals[i];
                if (!variable.IsArray)
                    script.Append($"{set} udg_" + variable.Name + "=" + variable.InitialValue + newline);
                else if (variable.IsArray && !variable.IsTwoDimensions)
                {
                    for (int j = 0; j < variable.ArraySize[0]; j++)
                    {
                        script.Append($"{set} udg_{variable.Name}[{j}] = {variable.InitialValue}{newline}");
                    }
                }
                else if (variable.IsArray && variable.IsTwoDimensions)
                {
                    for (int j = 0; j < variable.ArraySize[0]; j++)
                    {
                        for (int k = 0; k < variable.ArraySize[1]; k++)
                        {
                            script.Append($"{set} udg_{variable.Name}[{j}][{k}] = {variable.InitialValue}{newline}");
                        }
                    }
                }
            }
            script.Append(endfunction + newline);


            CreateItemTables(script);
            GenerateUnitItemTables(script);
            CreateSounds(script);

            CreateDestructibles(script);
            CreateItems(script);
            CreateUnits(script);
            CreateRegions(script);
            CreateCameras(script);

            CreateCustomScripts(script);
            GenerateTriggers(script);
            GenerateTriggerInitialization(script);

            GenerateTriggerPlayers(script);
            GenerateCustomTeams(script);
            GenerateAllyPriorities(script);

            GenerateMain(script);
            GenerateMapConfiguration(script);


            // TODO: Feed to jasshelper here?



            return script;
        }

        private bool IsSomethingCode(string returnType)
        {
            if (returnType == "ordercode" ||
                returnType == "unitcode" ||
                returnType == "abilcode" ||
                returnType == "buffcode" ||
                returnType == "destructablecode" ||
                returnType == "doodadcode" ||
                returnType == "itemcode" ||
                returnType == "techcode"
            )
                return true;

            return false;
        }

        private string GetGlobalsStartValue(string returnType)
        {
            string value = _null;

            if (returnType == "boolean")
                value = "false";
            else if (returnType == "integer" || returnType == "real")
                value = "0";

            return value;
        }

        private bool isMapObject(Value value)
        {
            if (
                    value.returnType == "unit" ||
                    value.returnType == "destructable" ||
                    value.returnType == "item" ||
                    value.returnType == "rect" ||
                    value.returnType == "camerasetup"
                    )
                return true;
            else return false;
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
                var x = u.Position.X.ToString(enUS);
                var y = u.Position.Y.ToString(enUS);
                var angle = u.Rotation.ToString(enUS);
                var skinId = Int32Extensions.ToRawcode(u.SkinId);

                if (owner == "24")
                    owner = "PLAYER_NEUTRAL_AGGRESSIVE";
                if (owner == "27")
                    owner = "PLAYER_NEUTRAL_PASSIVE";

                var varName = $"gg_unit_{u.ToString()}_{u.CreationNumber}";

                Value value;
                if (generatedVarNames.TryGetValue(varName, out value)) // unit with generated variable
                    script.Append($"{set} {varName} = BlzCreateUnitWithSkin(Player({owner}), {fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {fourCCStart}'{skinId}'{fourCCEnd}){newline}");
                else
                    script.Append($"{call} BlzCreateUnitWithSkin(Player({owner}), {fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {fourCCStart}'{skinId}'{fourCCEnd}){newline}");
            }

            script.Append($"{endfunction}{newline}{newline}");
        }

        private void CreateDestructibles(StringBuilder script)
        {
            string real = language == ScriptLanguage.Jass ? "real" : "";

            script.Append(separator);
            script.Append($"{comment}{newline}");
            script.Append($"{comment}  Destructible Objects{newline}");
            script.Append($"{comment}{newline}");
            script.Append(separator);

            script.Append($"function CreateAllDestructables {functionReturnsNothing}{newline}");
            script.Append($"local {real} life{newline}");

            var dests = Destructibles.GetAll();
            foreach (var d in dests)
            {
                var id = d.ToString();
                var x = d.Position.X.ToString(enUS);
                var y = d.Position.Y.ToString(enUS);
                var angle = d.Rotation.ToString(enUS);
                var scale = d.Scale.X.ToString(enUS);
                var variation = d.Variation;
                var skin = Int32Extensions.ToRawcode(d.SkinId);

                var varName = $"gg_dest_{d.ToString()}_{d.CreationNumber}";

                Value value;
                if (generatedVarNames.TryGetValue(varName, out value)) // dest with generated variable
                {
                    script.Append($"{set} {varName} = BlzCreateDestructableWithSkin({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {angle}, {scale}, {variation}, {fourCCStart}'{skin}'{fourCCEnd}){newline}");
                    if (d.Life < 100)
                    {
                        script.Append($"{set} life = GetDestructableLife({varName}){newline}");
                        script.Append($"{call} SetDestructableLife({varName}, {(d.Life * 0.01).ToString(enUS)} * life){newline}");
                    }
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

            var items = Units.GetMapItemsAll();
            foreach (var i in items)
            {
                if (i.ToString() == "sloc")
                    continue;

                var id = i.ToString();
                var x = i.Position.X.ToString(enUS);
                var y = i.Position.Y.ToString(enUS);
                var skinId = Int32Extensions.ToRawcode(i.SkinId);


                script.Append($"{call} BlzCreateItemWithSkin({fourCCStart}'{id}'{fourCCEnd}, {x}, {y}, {fourCCStart}'{skinId}'{fourCCEnd}){newline}");
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

                var varName = "gg_rect_" + id;

                script.Append($"{set} {varName} = Rect({left}, {bottom}, {right}, {top}){newline}");
                if (r.WeatherType == War3Net.Build.WeatherType.None)
                    continue;

                script.Append($"{set} we = AddWeatherEffect({varName}, '{Int32Extensions.ToRawcode((int)r.WeatherType)}'){newline}");
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
                var id = $"gg_cam_{c.Name.Replace(" ", "_")}";


                script.Append($"{set} {id} = CreateCameraSetup(){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ZOFFSET, {c.ZOffset.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ROTATION, {c.Rotation.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ANGLE_OF_ATTACK, {c.AngleOfAttack.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_TARGET_DISTANCE, {c.TargetDistance.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_ROLL, {c.Roll.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_FIELD_OF_VIEW, {c.FieldOfView.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_FARZ, {c.FarClippingPlane.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_NEARZ, {c.NearClippingPlane.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_PITCH, {c.LocalPitch.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_YAW, {c.LocalYaw.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_ROLL, {c.LocalRoll.ToString(enUS)}, 0.0){newline}");
                script.Append($"{call} CameraSetupSetDestPosition({id}, {c.TargetPosition.X.ToString(enUS)}, {c.TargetPosition.Y.ToString(enUS)}, 0.0){newline}");
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

                if (cutoff >= UInt32.MaxValue)
                    cutoff = 3000;
                if (pitch >= UInt32.MaxValue)
                    pitch = 1;

                script.Append($"{set} {id} = CreateSound(\"{path}\", {looping.ToString().ToLower()}, {is3D.ToString().ToLower()}, {stopRange.ToString().ToLower()}, {fadeInRate}, {fadeOutRate}, \"{eax}\"){newline}");
                script.Append($"{call} SetSoundParamsFromLabel({id}, \"{s.SoundName}\"){newline}");
                script.Append($"{call} SetSoundDuration({id}, {s.FadeInRate}){newline}"); // TODO: This is not the duration. We should pull from CASC data.
                script.Append($"{call} SetSoundChannel({id}, {(int)s.Channel}){newline}");
                if (s.MinDistance < 100000 && s.Channel != SoundChannel.UserInterface)
                    script.Append($"{call} SetSoundDistances({id}, {s.MinDistance}, {s.MaxDistance}){newline}");
                if (cutoff != 3000)
                    script.Append($"{call} SetSoundDistanceCutoff({id}, {s.DistanceCutoff}){newline}");
                script.Append($"{call} SetSoundVolume({id}, {s.Volume}){newline}");
                if (pitch != 1)
                    script.Append($"{call} SetSoundPitch({id}, {pitch}){newline}");
            }
            foreach (var s in music)
            {
                var id = s.Name;
                script.Append($"{set} {id} = \"{s.SoundName}\"{newline}");
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
    end
	bj_lastDyingWidget=null
	DestroyTrigger(GetTriggeringTrigger())
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

                script.Append($"function UnitItemDrops_{u.CreationNumber} {functionReturnsNothing}{newline}");

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
		)
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
		)
");

                script.Append($"{newline}");

                foreach (var itemSet in u.ItemTableSets)
                {
                    script.Append($"\t\t{call} RandomDistReset(){newline}");
                    foreach (var item in itemSet.Items)
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
		)
");
                    else
                        script.Append(@"
        itemID=RandomDistChoose()
		if ( trigUnit ~= nil ) then
			UnitDropItem(trigUnit, itemID)
		else
			WidgetDropItem(trigWidget, itemID)
		end
		)
");
                }
                if (language == ScriptLanguage.Jass)
                    script.Append(@"
    endif
	    set bj_lastDyingWidget=null
	    call DestroyTrigger(GetTriggeringTrigger())
    endfunction
		)
");
                else
                    script.Append(@"
    end
	    bj_lastDyingWidget=nil
	    DestroyTrigger(GetTriggeringTrigger())
    end
		)
");


                script.Append($"{newline}");
            }

            var destructibles = Destructibles.GetAll();
            foreach (var d in destructibles)
            {
                if (d.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{d.CreationNumber} {functionReturnsNothing}{newline}");

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
		)");
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
		)");

                script.Append($"{newline}");

                foreach (var itemSets in d.ItemTableSets)
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
		)");
                    else
                        script.Append(@"
        itemID=RandomDistChoose()
		if ( trigUnit ~= nil ) then
			UnitDropItem(trigUnit, itemID)
		else
			WidgetDropItem(trigWidget, itemID)
		end
		)");
                }

                if (language == ScriptLanguage.Jass)
                    script.Append(@"(

    endif

    set bj_lastDyingWidget = null

    call DestroyTrigger(GetTriggeringTrigger())
endfunction
        )");
                else
                    script.Append(@"(

    end

    bj_lastDyingWidget = nil

    DestroyTrigger(GetTriggeringTrigger())
end
        )");

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
                if (!t.isEnabled)
                    continue;

                string triggerName = t.GetName().Replace(" ", "_");
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

            string[] playerType = new string[] { "MAP_CONTROL_USER", "MAP_CONTROL_COMPUTER", "MAP_CONTROL_NEUTRAL", "MAP_CONTROL_RESCUABLE" };
            string[] races = new string[] { "RACE_PREF_RANDOM", "RACE_PREF_HUMAN", "RACE_PREF_ORC", "RACE_PREF_UNDEAD", "RACE_PREF_NIGHTELF" };

            int index = 0;
            var players = Info.MapInfo.Players;
            foreach (var p in players)
            {
                string player = $"Player({p.Id}), ";
                script.Append($"\t{call} SetPlayerStartLocation({player + index.ToString()}){newline}");
                if (p.Flags.HasFlag(PlayerFlags.FixedStartPosition) || p.Race == PlayerRace.Selectable)
                    script.Append($"\t{call} ForcePlayerStartLocation({player + index.ToString()}){newline}");

                script.Append($"\t{call} SetPlayerColor({player} ConvertPlayerColor({index.ToString()})){newline}");
                script.Append($"\t{call} SetPlayerRacePreference({player} {races[(int)p.Race]} ){newline}");
                script.Append($"\t{call} SetPlayerRaceSelectable({player} true){newline}");
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
                script.Append($"{newline}");
                script.Append($"\t{comment} Force: {f.Name}{newline}");

                string post_state = string.Empty;
                foreach (var p in Info.MapInfo.Players)
                {
                    // something about player masks here? (HiveWE)
                    script.Append($"\t{call} SetPlayerTeam(Player({p.Id}), {current_force}){newline}");

                    if (f.Flags.HasFlag(ForceFlags.AlliedVictory))
                    {
                        script.Append($"\t{call} SetPlayerState(Player({p.Id}), PLAYER_STATE_ALLIED_VICTORY, 1){newline}");
                    }

                    foreach (var k in Info.MapInfo.Players)
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
            //string terrain_lights = Info.MapInfo.LightEnvironment.ToString(); // TODO:
            //string unit_lights = Info.MapInfo.LightEnvironment.ToString();
            if (terrain_lights == "")
                terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);
            if (unit_lights == "")
                unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(War3Net.Build.Common.Tileset.LordaeronSummer);


            script.Append($"\t{call} SetDayNightModels(\"" + terrain_lights.Replace(@"\", @"\\") + "\", \"" + unit_lights.Replace(@"\", @"\\") + $"\"){newline}");

            string sound_environment = Info.MapInfo.SoundEnvironment; // TODO: Not working
            script.Append($"\t{call} NewSoundEnvironment(\"" + sound_environment + $"\"){newline}");

            string ambient_day = "LordaeronSummerDay"; // TODO: Hardcoded
            script.Append($"\t{call} SetAmbientDaySound(\"" + ambient_day + $"\"){newline}");

            string ambient_night = "LordaeronSummerNight"; // TODO: Hardcoded
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

            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ToString() == "sloc")
                    script.Append($"\t{call} DefineStartLocation({u.OwnerId}, {u.Position.X * 128f + Info.MapInfo.PlayableMapAreaWidth}, {u.Position.Y * 128f + Info.MapInfo.PlayableMapAreaHeight}){newline}");
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
                script.Append(s.script);
                script.Append($"{newline}");
            }
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
                if (!i.isEnabled)
                    continue;

                /* TODO: support trigger to script conversion in editor.
                if (!i.custom_text.empty())
                {
                    trigger_script += i.custom_text + "{newline}";
                }
                else
                */
                //{
                script.Append(ConvertGUIToJass(i, initialization_triggers));
                //}
            }


            // TODO:
            // Search the trigger script for global unit/destructible definitons
            /*
            size_t pos = trigger_script.find("gg_unit", 0);
            while (pos != std::string::npos) {
                std::string type = trigger_script.substr(pos + 8, 4);
                std::string creation_number = trigger_script.substr(pos + 13, 4);
                unit_variables[creation_number] = type;
                pos = trigger_script.find("gg_unit", pos + 17);
            }

            pos = trigger_script.find("gg_dest", 0);
            while (pos != std::string::npos) {
                std::string type = trigger_script.substr(pos + 8, 4);
                std::string creation_number = trigger_script.substr(pos + 13, trigger_script.find_first_not_of("0123456789", pos + 13) - pos - 13);
                destructable_variables[creation_number] = type;
                pos = trigger_script.find("gg_dest", pos + 17);
            }
            */
        }

        private string ConvertGUIToJass(ExplorerElementTrigger t, List<string> initialization_triggers)
        {
            string triggerName = t.GetName().Replace(" ", "_");
            string triggerVarName = "gg_trg_" + triggerName;
            string triggerActionName = "Trig_" + triggerName + "_Actions";

            StringBuilder events = new StringBuilder();
            StringBuilder conditions = new StringBuilder();
            StringBuilder pre_actions = new StringBuilder();
            StringBuilder actions = new StringBuilder();

            events.Append($"function InitTrig_{triggerName} {functionReturnsNothing}{newline}");
            events.Append($"\t{set} {triggerVarName} = CreateTrigger(){newline}");

            actions.Append($"function {triggerActionName} {functionReturnsNothing}{newline}");

            foreach (var e in t.trigger.Events)
            {
                if (!e.isEnabled)
                    continue;

                if (e.function.identifier == "MapInitializationEvent")
                {
                    initialization_triggers.Add(triggerVarName);
                    continue;
                }

                for (int i = 0; i < e.function.parameters.Count; i++)
                {
                    TriggerElement clonedEvent = e.Clone(); // Need to insert trigger variable at index 0.
                    TriggerRef triggerRef = new TriggerRef()
                    {
                        identifier = triggerVarName,
                    };
                    clonedEvent.function.parameters.Insert(0, triggerRef);

                    string _event = ConvertTriggerElementToJass(clonedEvent, pre_actions, triggerName, false);
                    events.Append($"{_event} {newline}");
                }
            }
            foreach (var c in t.trigger.Conditions)
            {
                string condition = ConvertTriggerElementToJass(c, pre_actions, triggerName, true);
                if (condition == "")
                    continue;

                conditions.Append($"\tif (not ({condition})) then{newline}");
                conditions.Append($"\t\treturn false{newline}");
                conditions.Append($"\t{endif}{newline}");
            }
            foreach (var a in t.trigger.Actions)
            {
                actions.Append($"\t{ConvertTriggerElementToJass(a, pre_actions, triggerName, false)}{newline}");
            }
            actions.Append($"{endfunction}{newline}{newline}{newline}");

            if (conditions.ToString() != "")
            {
                conditions.Insert(0, $"function Trig_{triggerName}_Conditions {functionReturnsBoolean}{newline}");
                conditions.Append($"\treturn true{newline}");
                conditions.Append($"{endfunction}{newline}{newline}");

                events.Append($"\t{call} TriggerAddCondition({triggerVarName}, Condition({function} Trig_{triggerName}_Conditions)){newline}");
            }

            if (!t.isInitiallyOn)
                events.Append($"\t{call} DisableTrigger({triggerVarName}){newline}");

            events.Append($"\t{call} TriggerAddAction({triggerVarName}, {function} {triggerActionName}){newline}");
            events.Append($"{endfunction}{newline}{newline}");

            string finalTrigger = $"{comment} Trigger {triggerName}{newline}{separator}{pre_actions.ToString()}{conditions.ToString()}{actions.ToString()}{separator}{events.ToString()}";

            return finalTrigger;
        }


        private string ConvertTriggerElementToJass(TriggerElement t, StringBuilder pre_actions, string triggerName, bool nested)
        {
            if (!t.isEnabled)
                return "";

            Function f = t.function;
            return ConvertFunctionToJass(f, pre_actions, triggerName);
        }


        private string ConvertFunctionToJass(Function f, StringBuilder pre_actions, string triggerName)
        {
            StringBuilder script = new StringBuilder();

            int invalidParams = controllerTrigger.VerifyParameters(f.parameters);
            if (invalidParams > 0)
                return "";


            // ------------------------- //
            // --- SPECIALLY HANDLED --- //
            // ------------------------- //

            if (f.identifier == "SetVariable")
            {
                script.Append($"{set} {ConvertParametersToJass(f.parameters[0], pre_actions)} = {ConvertParametersToJass(f.parameters[1], pre_actions)}");
                return script.ToString();
            }

            else if (f.identifier == "WaitForCondition")
            {
                script.Append($"loop{newline}");
                script.Append($"exitwhen({ConvertParametersToJass(f.parameters[0], pre_actions)}{newline})");
                script.Append($"{call} TriggerSleepAction(RMaxBJ(bj_WAIT_FOR_COND_MIN_INTERVAL, {ConvertParametersToJass(f.parameters[1], pre_actions)})){newline})");
                script.Append("endloop");
                return script.ToString();
            }

            else if (f.identifier == "ForLoopAMultiple" || f.identifier == "ForLoopBMultiple")
            {
                string loopIndex = f.identifier == "ForLoopAMultiple" ? "bj_forLoopAIndex" : "bj_forLoopBIndex";
                string loopIndexEnd = f.identifier == "ForLoopAMultiple" ? "bj_forLoopAIndexEnd" : "bj_forLoopBIndexEnd";

                script.Append($"{set} {loopIndex}={ConvertParametersToJass(f.parameters[0], pre_actions)} {newline}");
                script.Append($"{set} {loopIndexEnd}={ConvertParametersToJass(f.parameters[1], pre_actions)} {newline}");
                script.Append($"loop{newline}");
                script.Append($"\texitwhen {loopIndex} > {loopIndexEnd}{newline}");

                if (f.identifier == "ForLoopAMultiple")
                {
                    ForLoopAMultiple loopA = (ForLoopAMultiple)f;
                    foreach (var action in loopA.Actions)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                    }
                }
                else
                {
                    ForLoopBMultiple loopB = (ForLoopBMultiple)f;
                    foreach (var action in loopB.Actions)
                    {
                        script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                    }
                }
                script.Append($"\t{set} {loopIndex}={loopIndex}+1{newline}");
                script.Append($"endloop");
                return script.ToString();
            }

            else if (f.identifier == "ForLoopVarMultiple")
            {
                ForLoopVarMultiple loopVar = (ForLoopVarMultiple)f;
                string variable = loopVar.parameters[0].identifier;

                script.Append($"{set} {variable} = ");
                script.Append(ConvertParametersToJass(loopVar.parameters[1], pre_actions) + $"{newline}");
                script.Append("loop{newline}");
                script.Append($"exitwhen {variable} > {ConvertParametersToJass(loopVar.parameters[2], pre_actions)}{newline}");
                foreach (var action in loopVar.Actions)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"{set} {variable} = {variable} + 1{newline}");
                script.Append($"endloop{newline}");
                return script.ToString();
            }

            else if (f.identifier == "IfThenElseMultiple")
            {
                IfThenElse ifThenElse = (IfThenElse)f;

                script.Append("if (");
                foreach (var condition in ifThenElse.If)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (ifThenElse.If.IndexOf(condition) != ifThenElse.If.Count - 1)
                        script.Append("and ");
                }
                if (ifThenElse.If.Count == 0)
                    script.Append("(true)");

                script.Append($") then{newline}");
                foreach (var action in ifThenElse.Then)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"\telse{newline}");
                foreach (var action in ifThenElse.Else)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}");
                }
                script.Append($"\t{endif}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "ForForceMultiple" || f.identifier == "ForGroupMultiple")
            {
                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"{call} {f.identifier.Substring(0, 8)}({ConvertParametersToJass(f.parameters[0], pre_actions)}, function {function_name}){newline}");

                string pre = string.Empty;
                if (f.identifier == "ForForceMultiple")
                {
                    ForForceMultiple forForce = (ForForceMultiple)f;
                    foreach (var action in forForce.Actions)
                    {
                        pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                    }
                }
                else
                {
                    ForGroupMultiple forGroup = (ForGroupMultiple)f;
                    foreach (var action in forGroup.Actions)
                    {
                        pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                    }
                }
                pre_actions.Append($"function {function_name} {functionReturnsNothing}{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}{endfunction}{newline}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "EnumDestructablesInRectAllMultiple")
            {
                EnumDestructablesInRectAllMultiple enumDest = (EnumDestructablesInRectAllMultiple)f;

                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"{call} {f.identifier.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0], pre_actions)}), function {function_name}){newline}");

                string pre = string.Empty;
                foreach (var action in enumDest.Actions)
                {
                    pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                }
                pre_actions.Append($"function {function_name} {functionReturnsNothing}{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}{endfunction}{newline}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "EnumDestructablesInCircleBJMultiple")
            {
                EnumDestructiblesInCircleBJMultiple enumDest = (EnumDestructiblesInCircleBJMultiple)f;

                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"{call} {f.identifier.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0], pre_actions)}, {ConvertParametersToJass(f.parameters[0], pre_actions)}), function {function_name}){newline}");

                string pre = string.Empty;
                foreach (var action in enumDest.Actions)
                {
                    pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                }
                pre_actions.Append($"function {function_name} {functionReturnsNothing}{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}{endfunction}{newline}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "EnumItemsInRectBJMultiple")
            {
                EnumItemsInRectBJ enumItem = (EnumItemsInRectBJ)f;

                string function_name = generate_function_name(triggerName);

                // Remove multiple
                script.Append($"{call} {f.identifier.Substring(0, 26)}({ConvertParametersToJass(f.parameters[0], pre_actions)}, {ConvertParametersToJass(f.parameters[0], pre_actions)}), function {function_name}){newline}");

                string pre = string.Empty;
                foreach (var action in enumItem.Actions)
                {
                    pre += $"\t{ConvertTriggerElementToJass(action, pre_actions, triggerName, false)}{newline}";
                }
                pre_actions.Append($"function {function_name} {functionReturnsNothing}{newline}");
                pre_actions.Append(pre);
                pre_actions.Append($"{newline}{endfunction}{newline}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "AndMultiple")
            {
                AndMultiple andMultiple = (AndMultiple)f;
                var verifiedTriggerElements = new List<TriggerElement>();
                foreach (var element in andMultiple.And)
                {
                    if (!element.isEnabled)
                        continue;

                    int emptyParams = controllerTrigger.VerifyParameters(element.function.parameters);
                    if (emptyParams == 0)
                        verifiedTriggerElements.Add(element);
                }

                if (verifiedTriggerElements.Count == 0)
                    return "";

                script.Append("(");
                foreach (var condition in verifiedTriggerElements)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (verifiedTriggerElements.IndexOf(condition) != verifiedTriggerElements.Count - 1)
                        script.Append("and ");
                }
                script.Append(")");

                return script.ToString();
            }

            else if (f.identifier == "OrMultiple")
            {
                OrMultiple orMultiple = (OrMultiple)f;
                var verifiedTriggerElements = new List<TriggerElement>();
                foreach (var element in orMultiple.Or)
                {
                    if (!element.isEnabled)
                        continue;

                    int emptyParams = controllerTrigger.VerifyParameters(element.function.parameters);
                    if (emptyParams == 0)
                        verifiedTriggerElements.Add(element);
                }

                if (verifiedTriggerElements.Count == 0)
                    return "";

                script.Append("(");
                foreach (var condition in verifiedTriggerElements)
                {
                    script.Append($"\t{ConvertTriggerElementToJass(condition, pre_actions, triggerName, true)} ");

                    if (verifiedTriggerElements.IndexOf(condition) != verifiedTriggerElements.Count - 1)
                        script.Append("or ");
                }
                script.Append(")");

                return script.ToString();
            }

            else if (f.identifier == "ReturnAction")
            {
                return $"return {newline}";
            }

            else if (f.identifier.Length >= 15 && f.identifier.Substring(0, 15) == "OperatorCompare")
            {
                return $"{ConvertParametersToJass(f.parameters[0], pre_actions)} {ConvertParametersToJass(f.parameters[1], pre_actions)} {ConvertParametersToJass(f.parameters[2], pre_actions)}";
            }

            else if (f.identifier == "IfThenElse")
            {
                script.Append($"if({ConvertParametersToJass(f.parameters[0], pre_actions, true)}) then {newline}");
                script.Append($"\t\t{ConvertFunctionToJass((Function)f.parameters[1], pre_actions, triggerName)} {newline}");
                script.Append($"\telse {newline}");
                script.Append($"\t\t{ConvertFunctionToJass((Function)f.parameters[2], pre_actions, triggerName)} {newline}");
                script.Append($"\t{endif} {newline}");

                return script.ToString();
            }

            else if (f.identifier == "ForLoopA" || f.identifier == "ForLoopB")
            {
                string loopIndex = f.identifier == "ForLoopA" ? "bj_forLoopAIndex" : "bj_forLoopBIndex";
                string loopIndexEnd = f.identifier == "ForLoopA" ? "bj_forLoopAIndexEnd" : "bj_forLoopBIndexEnd";

                script.Append($"{set} {loopIndex}={ConvertParametersToJass(f.parameters[0], pre_actions)} {newline}");
                script.Append($"{set} {loopIndexEnd}={ConvertParametersToJass(f.parameters[1], pre_actions)} {newline}");
                script.Append($"loop{newline}");
                script.Append($"\texitwhen {loopIndex} > {loopIndexEnd}{newline}");
                script.Append($"\t{ConvertFunctionToJass((Function)f.parameters[2], pre_actions, triggerName)} {newline}");
                script.Append($"\t{set} {loopIndex}={loopIndex}+1{newline}");
                script.Append($"endloop {newline}");

                return script.ToString();
            }

            else if (f.identifier == "ForLoopVar")
            {
                string variable = f.parameters[0].identifier;

                script.Append($"{set} udg_{variable} = ");
                script.Append(ConvertParametersToJass(f.parameters[1], pre_actions) + $"{newline}");
                script.Append($"loop{newline}");
                script.Append($"exitwhen udg_{variable} > {ConvertParametersToJass(f.parameters[2], pre_actions)}{newline}");
                script.Append($"\t{ConvertFunctionToJass((Function)f.parameters[3], pre_actions, triggerName)} {newline}");
                script.Append($"{set} udg_{variable} = udg_{variable} + 1{newline}");
                script.Append($"endloop{newline}");

                return script.ToString();
            }

            else if (f.identifier == "ForForce" || f.identifier == "ForGroup")
            {
                string function_name = generate_function_name(triggerName);

                script.Append($"{call} {f.identifier}({ConvertParametersToJass(f.parameters[0], pre_actions)}, function {function_name}){newline}");

                string pre = string.Empty;
                pre_actions.Append($"function {function_name} {functionReturnsNothing}{newline}");
                pre_actions.Append($"\t{ConvertFunctionToJass(f.parameters[1] as Function, pre_actions, triggerName)}{newline}");
                pre_actions.Append($"{endfunction}{newline}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "AddTriggerEvent")
            {
                Function addEvent = f.Clone(); // Need to clone because of insert operation below.
                // Otherwise it's inserted into the saveable object.

                TriggerRef triggerRef = (TriggerRef)addEvent.parameters[0];
                Function _event = (Function)addEvent.parameters[1];
                _event.parameters.Insert(0, triggerRef);
                script.Append($"{ConvertFunctionToJass(_event, pre_actions, triggerName)}{newline}");

                return script.ToString();
            }

            else if (f.identifier == "CustomScriptCode")
            {
                script.Append(f.parameters[0].identifier);
                return script.ToString();
            }

            else if (f.returnType == "boolexpr")
            {
                return $"{ConvertParametersToJass(f, pre_actions, true)}";
            }




            // --------------------- //
            // --- REGULAR CALLS --- //
            // --------------------- //

            //script = ConvertEcasToJass(f, pre_actions, triggerName, nested);
            script.Append($"{call} {ConvertParametersToJass(f, pre_actions)}");

            return script.ToString();
        }



        private string ConvertParametersToJass(Parameter parameter, StringBuilder pre_actions, bool boolexprIsOn = false)
        {
            string output = string.Empty;

            if (parameter is Function)
            {
                Function f = (Function)parameter;

                // --------------------- //
                // --- SPECIAL CALLS --- //
                // --------------------- //

                if (f.identifier == "OperatorInt" || f.identifier == "OperatorReal")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[1], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[2], pre_actions, boolexprIsOn)})";
                }

                else if (f.returnType == "boolexpr" && !boolexprIsOn)
                {
                    nameNumber++;
                    string functionName = "BoolExpr_" + nameNumber;
                    output += $"Condition(function {functionName})";

                    pre_actions.Append($"function {functionName} takes nothing returns boolean {newline}");
                    pre_actions.Append($"\tif( ");
                    pre_actions.Append(ConvertParametersToJass(f, pre_actions, true));
                    pre_actions.Append($") then {newline}");
                    pre_actions.Append($"\t\treturn true {newline}");
                    pre_actions.Append($"\t{endif} {newline}");
                    pre_actions.Append($"\treturn false {newline}");
                    pre_actions.Append($"{endfunction}{newline}{newline}");

                    return output;
                }
                else if (f.identifier == "GetBooleanAnd")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], pre_actions, boolexprIsOn)} and {ConvertParametersToJass(f.parameters[1], pre_actions, boolexprIsOn)})";
                }
                else if (f.identifier == "GetBooleanOr")
                {
                    return $"({ConvertParametersToJass(f.parameters[0], pre_actions, boolexprIsOn)} or {ConvertParametersToJass(f.parameters[1], pre_actions, boolexprIsOn)})";
                }

                // TODO: COPIED CODE
                else if (f.identifier.Length >= 15 && f.identifier.Substring(0, 15) == "OperatorCompare")
                {
                    return $"{ConvertParametersToJass(f.parameters[0], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[1], pre_actions, boolexprIsOn)} {ConvertParametersToJass(f.parameters[2], pre_actions, boolexprIsOn)}";
                }


                // --------------------- //
                // --- REGULAR CALLS --- //
                // --------------------- //

                output += f.identifier + "(";
                for (int i = 0; i < f.parameters.Count; i++)
                {
                    Parameter p = f.parameters[i];
                    output += ConvertParametersToJass(p, pre_actions, boolexprIsOn);
                    if (i != f.parameters.Count - 1)
                        output += ",";
                }
                output += ")";
            }
            else if (parameter is Constant)
            {
                Constant c = (Constant)parameter;
                if (c.returnType == "unitcommonorderEx" ||
                c.returnType == "skymodelstring" ||
                c.returnType == "unitorderptarg" ||
                c.returnType == "unitorderutarg" ||
                c.returnType == "unitordernotarg" ||
                c.returnType == "unitorderitarg"
                )
                    output += "\"" + ContainerTriggerData.GetConstantCodeText(c.identifier, language) + "\"";
                else
                    output += ContainerTriggerData.GetConstantCodeText(c.identifier, language);

            }
            else if (parameter is VariableRef)
            {
                VariableRef v = (VariableRef)parameter;
                ControllerVariable controller = new ControllerVariable();
                Variable variable = controller.GetByReference(v);

                bool isVarAsString_Real = v.returnType == "VarAsString_Real";
                if (isVarAsString_Real)
                    output += "\"";

                output += "udg_" + variable.Name;

                if (variable.IsArray)
                {
                    output += $"[{v.arrayIndexValues[0].identifier}]";
                    if (variable.IsTwoDimensions)
                        output += $"[{v.arrayIndexValues[1].identifier}]";
                }

                if (isVarAsString_Real)
                    output += "\"";
            }
            else if (parameter is TriggerRef)
            {
                TriggerRef t = (TriggerRef)parameter;
                ControllerTrigger controller = new ControllerTrigger();
                Trigger trigger = controller.GetById(t.TriggerId);
                string name = controller.GetTriggerName(trigger.Id);

                output += "gg_trg_" + name.Replace(" ", "_");
            }
            else if (parameter is Value)
            {
                Value v = (Value)parameter;
                if (v.returnType == "StringExt" || v.returnType == "modelfile" || v.returnType == "skymodelstring")
                    output += "\"" + v.identifier.Replace(@"\", @"\\") + "\"";
                else if (v.returnType == "unitcode" || v.returnType == "buffcode" || v.returnType == "abilcode" || v.returnType == "destructablecode" || v.returnType == "techcode" || v.returnType == "itemcode")
                    output += $"{fourCCStart}'{v.identifier}'{fourCCEnd}";
                else if (v.returnType == "unit")
                    output += $"gg_unit_{v.identifier.Replace(" ", "_")}";
                else if (v.returnType == "destructable")
                    output += $"gg_dest_{v.identifier.Replace(" ", "_")}";
                else if (v.returnType == "item")
                    output += $"gg_item_{v.identifier.Replace(" ", "_")}";
                else if (v.returnType == "rect")
                    output += $"gg_rect_{v.identifier.Replace(" ", "_")}";
                else if (v.returnType == "camerasetup")
                    output += $"gg_cam_{v.identifier.Replace(" ", "_")}";
                else
                    output += v.identifier;
            }

            return output;
        }


        private string generate_function_name(string triggerName)
        {
            string name = $"Trig_{triggerName}_{nameNumber}";
            nameNumber++;

            return name;
            /*
            auto time = std::chrono::high_resolution_clock::now().time_since_epoch().count();
	        return "Trig_" + trigger_name + "_" + std::to_string(time & 0xFFFFFFFF);
            */
        }
    }
}
