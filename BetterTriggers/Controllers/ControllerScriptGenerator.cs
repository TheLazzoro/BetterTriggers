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
using War3Net.Common.Extensions;

namespace BetterTriggers.Controllers
{
    public class ControllerScriptGenerator
    {
        List<Variable> variables = new List<Variable>();
        List<string> scripts = new List<string>();
        List<Trigger> triggers = new List<Trigger>();
        Dictionary<string, Value> generatedUnitVars = new Dictionary<string, Value>();
        CultureInfo enUS = new CultureInfo("en-US");

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
                else if (File.Exists(element.GetPath()))
                {
                    if (Path.GetExtension(element.GetPath()) == ".trg")
                    {
                        string json = File.ReadAllText(element.GetPath());
                        Trigger trig = JsonConvert.DeserializeObject<Trigger>(json);
                        triggers.Add(trig);
                    }
                    else if (Path.GetExtension(element.GetPath()) == ".j")
                    {
                        scripts.Add(File.ReadAllText(element.GetPath()) + System.Environment.NewLine);
                    }
                    else if (Path.GetExtension(element.GetPath()) == ".var")
                    {
                        string json = File.ReadAllText(element.GetPath());
                        var variable = JsonConvert.DeserializeObject<Variable>(json);
                        variable.Name = Path.GetFileNameWithoutExtension(element.GetPath()); // hack
                        variables.Add(variable);
                    }
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
                Variable variable = variables[i];
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
                }
            }
            script.Append("globals" + System.Environment.NewLine);
            foreach (KeyValuePair<string, Value> kvp in generatedVarNames)
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
            CreateItems(script);
            CreateRegions(script);
            CreateCameras(script);
            CreateTriggers(script);

            // Append scripts
            for (int i = 0; i < scripts.Count; i++)
            {
                script.Append(scripts[i] + System.Environment.NewLine);
            }

            // Generate trigger scripts
            for (int i = 0; i < triggers.Count; i++)
            {
                var trig = triggers[i];
                for (int t = 0; t < trig.Actions.Count; t++)
                {
                    script.Append($"call {trig.Actions[t].function.identifier}(");
                    RecurseParameters(script, trig.Actions[t].function.parameters);
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
            script.Append("//*\n");
            script.Append("//*  Unit Creation\n");
            script.Append("//*\n");

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

        private void CreateItems(StringBuilder script)
        {
            script.Append("//*\n");
            script.Append("//*  Item Creation\n");
            script.Append("//*\n");

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
            script.Append("//*\n");
            script.Append("//*  Regions\n");
            script.Append("//*\n");

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
            script.Append("//*\n");
            script.Append("//*  Cameras\n");
            script.Append("//*\n");

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



        private void CreateTriggers(StringBuilder script)
        {
            script.Append("//*\n");
            script.Append("//*  Triggers\n");
            script.Append("//*\n");
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
