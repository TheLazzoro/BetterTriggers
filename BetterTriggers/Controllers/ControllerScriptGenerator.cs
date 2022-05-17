using BetterTriggers.Containers;
using BetterTriggers.WorldEdit;
using Model;
using Model.Data;
using Model.EditorData;
using Model.SaveableData;
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

namespace BetterTriggers.Controllers
{
    public class ControllerScriptGenerator
    {
        List<ExplorerElementVariable> variables = new List<ExplorerElementVariable>();
        List<ExplorerElementScript> scripts = new List<ExplorerElementScript>();
        List<ExplorerElementTrigger> triggers = new List<ExplorerElementTrigger>();
        Dictionary<string, Value> generatedUnitVars = new Dictionary<string, Value>();
        Dictionary<string, Value> generatedDestVars = new Dictionary<string, Value>();
        CultureInfo enUS = new CultureInfo("en-US");
        string separator = "//***************************************************************************";

        List<string> initialization_triggers = new List<string>();

        public void GenerateScript(string outputPath)
        {
            var inMemoryFiles = ContainerProject.projectFiles;

            SortTriggerElements(inMemoryFiles[0]); // root node.
            StringBuilder script = Generate();

            var scriptFileToInput = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/vJass.j";
            File.WriteAllText(scriptFileToInput, script.ToString());

            string JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/jasshelper.exe";
            string CommonJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/common.j\"";
            string BlizzardJ = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/Blizzard.j\"";
            string fileOutput = "\"" + System.IO.Directory.GetCurrentDirectory() + "/Resources/JassHelper/output.j\"";
            Process p = Process.Start($"{JassHelper}", $"--scriptonly {CommonJ} {BlizzardJ} \"{scriptFileToInput}\" \"{outputPath}\"");
            p.WaitForExit();
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
                Variable variable = variables[i].variable;
                InitGlobals.Add(variable);

                script.Append("globals" + System.Environment.NewLine);
                script.Append(System.Environment.NewLine);
                script.Append(variable.Type + " " + variable.Name);
                script.Append(System.Environment.NewLine);
                script.Append("endglobals" + System.Environment.NewLine);
            }

            // Generated variables
            ControllerTrigger controllerTrigger = new ControllerTrigger();
            var parameters = controllerTrigger.GetParametersAll();
            Dictionary<string, Value> generatedVarNames = new Dictionary<string, Value>();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is Value && (
                    parameters[i].returnType == "unit" ||
                    parameters[i].returnType == "destructable" ||
                    parameters[i].returnType == "item" ||
                    parameters[i].returnType == "rect" ||
                    parameters[i].returnType == "camerasetup"
                    ))
                {
                    Value value = (Value)parameters[i];
                    generatedVarNames.TryAdd(value.identifier, value);

                    if (value.returnType == "unit")
                        generatedUnitVars.TryAdd(value.identifier, value);
                    if (value.returnType == "destructable")
                        generatedDestVars.TryAdd(value.identifier, value);
                }
            }
            script.Append("globals" + System.Environment.NewLine);

            // Unit globals 
            foreach (KeyValuePair<string, Value> kvp in generatedVarNames)
            {
                string varName = kvp.Key;
                string type = kvp.Value.returnType;
                script.Append($"{type} {varName} = null {System.Environment.NewLine}");
            }

            // Destructible globals
            foreach (KeyValuePair<string, Value> kvp in generatedDestVars)
            {
                string varName = kvp.Key;
                string type = kvp.Value.returnType;
                script.Append($"{type} {varName} = null {System.Environment.NewLine}");
            }
            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                script.Append($"rect {r.Name.Replace(" ", "_")} = null {System.Environment.NewLine}");
            }
            var sounds = Sounds.GetAll();
            foreach (var s in sounds)
            {
                script.Append($"sound {s.Name} = null {System.Environment.NewLine}");
            }

            script.Append("endglobals" + System.Environment.NewLine);
            script.Append(System.Environment.NewLine);


            // Init global variables
            script.Append("function InitGlobals takes nothing returns nothing" + System.Environment.NewLine);
            for (int i = 0; i < InitGlobals.Count; i++)
            {
                var variable = InitGlobals[i];
                script.Append("set " + variable.Name + "=" + variable.InitialValue + System.Environment.NewLine);
            }
            script.Append("endfunction" + System.Environment.NewLine);


            CreateUnits(script);
            CreateDestructibles(script);
            CreateItems(script);
            CreateRegions(script);
            CreateCameras(script);
            CreateSounds(script);
            CreateItemTables(script);
            GenerateUnitItemTables(script);
            GenerateTriggerInitialization(script);
            GenerateTriggerPlayers(script);
            GenerateCustomTeams(script);
            GenerateAllyPriorities(script);
            GenerateMain(script);
            GenerateMapConfiguration(script);

            // Append scripts
            for (int i = 0; i < scripts.Count; i++)
            {
                script.Append(scripts[i] + System.Environment.NewLine);
            }

            // Generate trigger scripts
            for (int i = 0; i < triggers.Count; i++)
            {
                var trig = triggers[i];
                for (int t = 0; t < trig.trigger.Actions.Count; t++)
                {
                    script.Append($"call {trig.trigger.Actions[t].function.identifier}(");
                    RecurseParameters(script, trig.trigger.Actions[t].function.parameters);
                    script.Append(")" + System.Environment.NewLine);
                }
            }



            // main
            script.Append("function main takes nothing returns nothing" + System.Environment.NewLine);
            script.Append("endfunction" + System.Environment.NewLine);

            return script;
        }



        private void CreateUnits(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Unit Creation\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateUnits takes nothing returns nothing\n");
            script.Append("\tlocal unit u\n");
            script.Append("\tlocal integer unitID\n");
            script.Append("\tlocal trigger t\n");
            script.Append("\tlocal real life\n");

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

                Value value;
                if (generatedUnitVars.TryGetValue($"{u.ToString()}_{u.CreationNumber}", out value)) // unit with generated variable
                    script.Append($"set {u.ToString()}_{u.CreationNumber} = BlzCreateUnitWithSkin(Player({owner}), '{id}', {x}, {y}, {angle}, '{skinId}')\n");
                else
                    script.Append($"call BlzCreateUnitWithSkin(Player({owner}), '{id}', {x}, {y}, {angle}, '{skinId}')\n");
            }

            script.Append("endfunction\n");
        }

        private void CreateDestructibles(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Destructible Objects\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateAllDestructibles takes nothing returns nothing\n");
            script.Append("local real life\n");

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

                Value value;
                if (generatedDestVars.TryGetValue($"{d.ToString()}_{d.CreationNumber}", out value)) // dest with generated variable
                {
                    script.Append($"set {d.ToString()}_{d.CreationNumber} = BlzCreateDestructableWithSkin('{id}', {x}, {y}, {angle}, {scale}, {variation}, '{skin}')\n");
                    if (d.Life < 100)
                    {
                        script.Append($"set life = GetDestructableLife({d.ToString()}_{d.CreationNumber})\n");
                        script.Append($"call SetDestructableLife({d.ToString()}_{d.CreationNumber}, {(d.Life * 0.01).ToString(enUS)} * life)\n");
                    }
                }
            }

            script.Append("endfunction\n");
        }

        private void CreateItems(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Item Creation\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateAllItems takes nothing returns nothing\n");

            var items = Units.GetMapItemsAll();
            foreach (var i in items)
            {
                if (i.ToString() == "sloc")
                    continue;

                var id = i.ToString();
                var x = i.Position.X.ToString(enUS);
                var y = i.Position.Y.ToString(enUS);
                var skinId = Int32Extensions.ToRawcode(i.SkinId);

                script.Append($"call BlzCreateItemWithSkin('{id}', {x}, {y}, '{skinId}')\n");
            }

            script.Append("endfunction\n");
        }


        private void CreateRegions(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Regions\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateAllRegions takes nothing returns nothing\n");
            script.Append("local weathereffect we\n");
            script.Append("\n");

            var regions = Regions.GetAll();
            foreach (var r in regions)
            {
                var id = r.Name.Replace(" ", "_");
                var left = r.Left;
                var bottom = r.Bottom;
                var right = r.Right;
                var top = r.Top;

                script.Append($"set {id} = Rect({left}, {bottom}, {right}, {top})\n");
                if (r.WeatherType == War3Net.Build.WeatherType.None)
                    continue;

                script.Append($"set we = AddWeatherEffect({id}, '{Int32Extensions.ToRawcode((int)r.WeatherType)}')\n");
                script.Append($"EnableWeatherEffect(we, true)\n");
            }


            script.Append("endfunction\n");
        }


        private void CreateCameras(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Cameras\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateCameras takes nothing returns nothing\n");
            script.Append("\n");

            var cameras = Cameras.GetAll();
            foreach (var c in cameras)
            {
                var id = c.Name.Replace(" ", "_");

                script.Append($"set {id} = CreateCameraSetup()\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ZOFFSET, {c.ZOffset.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ROTATION, {c.Rotation.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ANGLE_OF_ATTACK, {c.AngleOfAttack.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_TARGET_DISTANCE, {c.TargetDistance.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_ROLL, {c.Roll.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_FIELD_OF_VIEW, {c.FieldOfView.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_FARZ, {c.FarClippingPlane.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_NEARZ, {c.NearClippingPlane.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_PITCH, {c.LocalPitch.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_YAW, {c.LocalYaw.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetField({id}, CAMERA_FIELD_LOCAL_ROLL, {c.LocalRoll.ToString(enUS)}, 0.0)\n");
                script.Append($"call CameraSetupSetDestPosition({id}, {c.TargetPosition.X.ToString(enUS)}, {c.TargetPosition.Y.ToString(enUS)}, 0.0)\n");
                script.Append("\n");
            }

            script.Append("endfunction\n");
        }


        private void CreateSounds(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Sounds\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function InitSounds takes nothing returns nothing\n");
            script.Append("\n");

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

                script.Append($"set {id} = CreateSound(\"{path}\", {looping}, {is3D}, {stopRange}, {fadeInRate}, {fadeOutRate}, \"{eax}\")\n");
                script.Append($"call SetSoundParamsFromLabel({id}, \"{s.SoundName}\")\n");
                script.Append($"call SetSoundDuration({id}, {s.FadeInRate})\n"); // TODO: This is not the duration. We should pull from CASC data.
                script.Append($"call SetSoundChannel({id}, {(int)s.Channel})\n");
                if (s.MinDistance < 100000 && s.Channel != SoundChannel.UserInterface)
                    script.Append($"call SetSoundDistances({id}, {s.MinDistance}, {s.MaxDistance})\n");
                if (cutoff != 3000)
                    script.Append($"call SetSoundDistanceCutoff({id}, {s.DistanceCutoff})\n");
                script.Append($"call SetSoundVolume({id}, {s.Volume})\n");
                if (pitch != 1)
                    script.Append($"call SetSoundPitch({id}, {pitch})\n");
            }
            foreach (var s in music)
            {
                var id = s.Name;
                script.Append($"set {id} = \"{s.SoundName}\"\n");
            }
        }


        private void CreateItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Map Item Tables\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function CreateItemTables takes nothing returns nothing\n");
            script.Append("\n");

            var itemTables = Info.MapInfo.RandomItemTables;
            foreach (var table in itemTables)
            {
                script.Append($"function ItemTable_{table.Index} takes nothing returns nothing\n");
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

                script.Append("\n");

                foreach (var itemSets in table.ItemSets)
                {
                    script.Append("\t\tcall RandomDistReset()\n");
                    foreach (var item in itemSets.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance})\n");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		)
");
                }

                script.Append(@"
    endif
	set bj_lastDyingWidget=null
	call DestroyTrigger(GetTriggeringTrigger())
endfunction
		)
                ");
            }

            script.Append("\n");
        }


        private void GenerateUnitItemTables(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Unit Item Tables\n");
            script.Append("//*\n");
            script.Append(separator);


            var units = Units.GetAll();
            foreach (var u in units)
            {
                if (u.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{u.CreationNumber} takes nothing returns nothing\n");
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

                script.Append("\n");

                foreach (var itemSet in u.ItemTableSets)
                {
                    script.Append("\t\tcall RandomDistReset()\n");
                    foreach (var item in itemSet.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance})\n");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		)
");
                }
                script.Append(@"
    endif
	    set bj_lastDyingWidget=null
	    call DestroyTrigger(GetTriggeringTrigger())
    endfunction
		)
");


                script.Append("\n");
            }

            var destructibles = Destructibles.GetAll();
            foreach (var d in destructibles)
            {
                if (d.ItemTableSets.Count == 0)
                    continue;

                script.Append($"function UnitItemDrops_{ d.CreationNumber} takes nothing returns nothing\n");
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

                script.Append("\n");

                foreach (var itemSets in d.ItemTableSets)
                {
                    script.Append("\t\tcall RandomDistReset()\n");
                    foreach (var item in itemSets.Items)
                    {
                        script.Append($"\t\tcall RandomDistAddItem('{Int32Extensions.ToRawcode(item.ItemId)}', {item.Chance})\n");
                    }

                    script.Append(@"
        set itemID=RandomDistChoose()
		if ( trigUnit != null ) then
			call UnitDropItem(trigUnit, itemID)
		else
			call WidgetDropItem(trigWidget, itemID)
		endif
		)");
                }

                script.Append(@"(

    endif

    set bj_lastDyingWidget = null

    call DestroyTrigger(GetTriggeringTrigger())
endfunction
        )");

                script.Append("\n");
            }
        }



        private void GenerateTriggerInitialization(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Triggers\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function InitCustomTriggers takes nothing returns nothing\n");

            foreach (var t in triggers)
            {
                if (!t.isEnabled)
                    continue;

                string triggerName = t.GetName().Replace(" ", "_");
                script.Append($"\tcall InitTrig_{triggerName}()\n");
            }
            script.Append($"endfunction\n");
            script.Append($"\n");

            script.Append("function RunInitializationTriggers takes nothing returns nothing\n");
            foreach (var t in initialization_triggers)
            {
                script.Append($"\tcall ConditionalTriggerExecute(\"{t}\")\n");
            }
            script.Append($"endfunction\n");
        }



        private void GenerateTriggerPlayers(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Players\n");
            script.Append("//*\n");
            script.Append(separator);

            script.Append("function InitCustomPlayerSlots takes nothing returns nothing\n");

            string[] playerType = new string[] { "MAP_CONTROL_USER", "MAP_CONTROL_COMPUTER", "MAP_CONTROL_NEUTRAL", "MAP_CONTROL_RESCUABLE" };
            string[] races = new string[] { "RACE_PREF_RANDOM", "RACE_PREF_HUMAN", "RACE_PREF_ORC", "RACE_PREF_UNDEAD", "RACE_PREF_NIGHTELF" };

            int index = 0;
            var players = Info.MapInfo.Players;
            foreach (var p in players)
            {
                string player = $"Player({p.Id}), ";
                script.Append($"\tcall SetPlayerStartLocation(\"{ player + index.ToString()}\")\n");
                if (p.Flags.HasFlag(PlayerFlags.FixedStartPosition) || p.Race == PlayerRace.Selectable)
                    script.Append($"\tcall ForcePlayerStartLocation(\"{ player + index.ToString()}\")\n");

                script.Append($"\tcall SetPlayerColor({player} ConvertPlayerColor(\"{ player + index.ToString()}\"))\n");
                script.Append($"\tcall SetPlayerRacePreference({player} {races[(int)p.Race]} )\n");
                script.Append($"\tcall SetPlayerRaceSelectable({player} true)\n");
                script.Append($"\tcall SetPlayerController({player} {playerType[(int)p.Controller]} )\n");

                if (p.Controller == PlayerController.Rescuable)
                {
                    foreach (var j in players)
                    {
                        if (j.Race == PlayerRace.Human) // why is this here, eejin?
                            script.Append($"\tcall SetPlayerAlliance({player} Player({ j.Id}), ALLIANCE_RESCUABLE, true)\n");
                    }
                }

                script.Append($"\n");
                index++;
            }

            script.Append($"endfunction\n");
        }



        private void GenerateCustomTeams(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Custom Teams\n");
            script.Append("//*\n");
            script.Append(separator);

            int current_force = 0;
            var forces = Info.MapInfo.Forces;
            foreach (var f in forces)
            {
                script.Append($"\n");
                script.Append($"\t// Force: " + f.Name + "\n");

                string post_state = string.Empty;
                foreach (var p in Info.MapInfo.Players)
                {
                    // something about player masks here? (HiveWE)
                    script.Append($"\tcall SetPlayerTeam(Player({p.Id}), {current_force})\n");

                    if (f.Flags.HasFlag(ForceFlags.AlliedVictory))
                    {
                        script.Append($"\tcall SetPlayerState(Player({p.Id}), PLAYER_STATE_ALLIED_VICTORY, 1)\n");
                    }

                    foreach (var k in Info.MapInfo.Players)
                    {
                        // something about player masks here? (HiveWE)
                        if (p.Id != k.Id)
                        {
                            if (f.Flags.HasFlag(ForceFlags.Allied))
                                post_state += $"\tcall SetPlayerAllianceStateAllyBJ(Player({p.Id}), Player({k.Id}), true)\n";
                            if (f.Flags.HasFlag(ForceFlags.ShareVision))
                                post_state += $"\tcall SetPlayerAllianceStateVisionBJ(Player({p.Id}), Player({k.Id}), true)\n";
                            if (f.Flags.HasFlag(ForceFlags.ShareUnitControl))
                                post_state += $"\tcall SetPlayerAllianceStateControlBJ(Player({p.Id}), Player({k.Id}), true)\n";
                            if (f.Flags.HasFlag(ForceFlags.ShareAdvancedUnitControl))
                                post_state += $"\tcall SetPlayerAllianceStateFullControlBJ(Player({p.Id}), Player({k.Id}), true)\n";
                        }
                    }
                }
                if (post_state != string.Empty)
                {
                    script.Append(post_state);
                }

                script.Append("\n");
                current_force++;
            }

            script.Append("endfunction\n");
        }


        private void GenerateAllyPriorities(StringBuilder script)
        {
            script.Append("function InitAllyPriorities takes nothing returns nothing\n");

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
                        player_text += $"\tcall SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_LOW)\n";
                        current_index++;
                    }
                    else if (p.EnemyHighPriorityFlags == 1 && p.Id != j.Id)
                    {
                        player_text += $"\tcall SetStartLocPrio({current_player}, {current_index}, {player_to_startloc[j.Id]}, MAP_LOC_PRIO_HIGH)\n";
                        current_index++;
                    }
                }

                player_text = $"\tcall SetStartLocPrioCount({current_player}, {current_index})\n";
                script.Append(player_text);
                current_player++;
            }

            script.Append("endfunction\n");
        }



        private void GenerateMain(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append($"//*  Main Initialization\n");
            script.Append($"//*\n");
            script.Append(separator);

            script.Append($"function main takes nothing returns nothing\n");

            string camera_bounds = "\tcall SetCameraBounds(" +
                (Info.MapInfo.CameraBounds.BottomLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.BottomLeft.Y - 256f) + " + GetCameraMargin(CAMERA_MARGIN_BOTTOM), " +

                (Info.MapInfo.CameraBounds.TopRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.TopRight.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.TopLeft.X - 512f) + " + GetCameraMargin(CAMERA_MARGIN_LEFT), " +
                (Info.MapInfo.CameraBounds.TopLeft.Y + 256f) + " - GetCameraMargin(CAMERA_MARGIN_TOP), " +

                (Info.MapInfo.CameraBounds.BottomRight.X + 512f) + " - GetCameraMargin(CAMERA_MARGIN_RIGHT), " +
                (Info.MapInfo.CameraBounds.BottomRight.Y - 256f) + " + GetCameraMargin(CAMERA_MARGIN_BOTTOM))\n";

            script.Append(camera_bounds);

            string terrain_lights = LightEnvironmentProvider.GetTerrainLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            string unit_lights = LightEnvironmentProvider.GetUnitLightEnvironmentModel(Info.MapInfo.LightEnvironment);
            script.Append($"\tcall SetDayNightModels(\"" + terrain_lights + "\", \"" + unit_lights + "\")\n");

            string sound_environment = Info.MapInfo.SoundEnvironment;
            script.Append($"\tcall NewSoundEnvironment(\"" + sound_environment + "\")\n");

            string ambient_day = "LordaeronSummerDay"; // TODO: Hardcoded
            script.Append($"\tcall SetAmbientDaySound(\"" + ambient_day + "\")\n");

            string ambient_night = "LordaeronSummerNight"; // TODO: Hardcoded
            script.Append($"\tcall SetAmbientNightSound(\"" + ambient_night + "\")\n");

            script.Append($"\tcall SetMapMusic(\"Music\", true, 0)\n");
            script.Append($"\tcall InitSounds()\n");
            script.Append($"\tcall CreateRegions()\n");
            script.Append($"\tcall CreateCameras()\n");
            script.Append($"\tcall CreateDestructables()\n");
            script.Append($"\tcall CreateItems()\n");
            script.Append($"\tcall CreateUnits()\n");
            script.Append($"\tcall InitBlizzard()\n");

            script.Append($"\tcall InitGlobals()\n");
            script.Append($"\tcall InitCustomTriggers()\n");
            script.Append($"\tcall RunInitializationTriggers()\n");

            script.Append($"endfunction\n");
        }



        private void GenerateMapConfiguration(StringBuilder script)
        {
            script.Append(separator);
            script.Append("//*\n");
            script.Append("//*  Map Configuration\n");
            script.Append("//*\n");
            script.Append(separator);



        }



        private void RecurseParameters(StringBuilder script, List<Parameter> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is Function)
                {
                    var func = (Function)parameters[i];
                    script.Append($"{func.identifier}(");
                    RecurseParameters(script, func.parameters);
                    script.Append(")");
                }
                else if (parameters[i] is Constant)
                {
                    var constant = (Constant)parameters[i];
                    //script.Append(constant.codeText;
                }
                if (i < parameters.Count - 1)
                    script.Append(",");
            }
        }
    }
}
