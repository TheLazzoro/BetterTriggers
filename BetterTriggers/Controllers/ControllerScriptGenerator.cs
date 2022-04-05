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

namespace BetterTriggers.Controllers
{
    public class ControllerScriptGenerator
    {
        List<Variable> variables = new List<Variable>();
        List<string> scripts = new List<string>();
        List<Trigger> triggers = new List<Trigger>();

        public string GenerateScript(string outputPath)
        {
            var inMemoryFiles = ContainerProject.projectFiles;

            GatherTriggerElements(inMemoryFiles[0]); // root node.
            string script = Generate();

            var scriptFileToInput = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/vJass.j";
            File.WriteAllText(scriptFileToInput, script);

            string JassHelper = $"{System.IO.Directory.GetCurrentDirectory()}/Resources/JassHelper/jasshelper.exe";
            string CommonJ = "\""+System.IO.Directory.GetCurrentDirectory()+ "/Resources/JassHelper/common.j\"";
            string BlizzardJ = "\""+System.IO.Directory.GetCurrentDirectory()+ "/Resources/JassHelper/Blizzard.j\"";
            string fileOutput = "\""+System.IO.Directory.GetCurrentDirectory()+ "/Resources/JassHelper/output.j\"";
            Process p = Process.Start($"{JassHelper}", $"--scriptonly {CommonJ} {BlizzardJ} \"{scriptFileToInput}\" \"{outputPath}\"");
            p.WaitForExit();

            return script;
        }

        private void GatherTriggerElements(IExplorerElement parent)
        {
            string script = string.Empty;

            // Gather all explorer elements
            List<IExplorerElement> children = new List<IExplorerElement>();
            if(parent is ExplorerElementRoot)
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
                    GatherTriggerElements(element);
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

        private string Generate()
        {
            string script = string.Empty;

            // ---- Generate script ---- //

            List<Variable> InitGlobals = new List<Variable>();

            // Global variables
            for (int i = 0; i < variables.Count; i++)
            {
                Variable variable = variables[i];
                InitGlobals.Add(variable);

                script += "globals" + System.Environment.NewLine;
                script += System.Environment.NewLine;
                script += variable.Type + " " + variable.Name;
                script += System.Environment.NewLine;
                script += "endglobals" + System.Environment.NewLine;
            }

            // Init global variables
            script += "function InitGlobals takes nothing returns nothing" + System.Environment.NewLine;
            for (int i = 0; i < InitGlobals.Count; i++)
            {
                var variable = InitGlobals[i];
                script += "set " + variable.Name + "=" + variable.InitialValue + System.Environment.NewLine;
            }
            script += "endfunction" + System.Environment.NewLine;

            // Append scripts
            for (int i = 0; i < scripts.Count; i++)
            {
                script += scripts[i] + System.Environment.NewLine;
            }

            // Generate trigger scripts
            for (int i = 0; i < triggers.Count; i++)
            {
                var trig = triggers[i];
                for (int t = 0; t < trig.Actions.Count; t++)
                {
                    script += $"call {trig.Actions[t].identifier}(";
                    script += RecurseParameters(trig.Actions[t].parameters);
                    script += ")" + System.Environment.NewLine;
                }
            }

            script += CreateUnits();

            // main
            script += "function main takes nothing returns nothing" + System.Environment.NewLine;
            script += "endfunction" + System.Environment.NewLine;

            return script;
        }

        private string CreateUnits()
        {
            string script = string.Empty;

            script += "//*\n";
            script += "//*  Unit Creation\n";
            script += "//*\n";

            script += "function CreateUnits takes nothing returns nothing\n";
            script += "\tlocal unit u\n";
            script += "\tlocal integer unitID\n";
            script += "\tlocal trigger t\n";
            script += "\tlocal real life\n";

            Units unitParser = new Units();
            unitParser.ParseUnits();
            foreach (var u in unitParser.units)
            {
                if (u.Id == "sloc")
                    continue;

                var owner = u.Owner;
                var id = u.Id;
                var x = u.Position.X.ToString(new CultureInfo("en-US"));
                var y = u.Position.Y.ToString(new CultureInfo("en-US"));
                var angle = u.Angle.ToString(new CultureInfo("en-US"));
                var skinId = u.SkinId;

                script += $"call BlzCreateUnitWithSkin(Player({owner}), '{id}', {x}, {y}, {angle}, '{skinId}')\n";
            }

            script += "endfunction\n";

            return script;
        }

        private string RecurseParameters(List<Parameter> parameters)
        {
            string script = string.Empty;

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is Function)
                {
                    var func = (Function)parameters[i];
                    script += $"{func.identifier}(";
                    script += RecurseParameters(func.parameters);
                    script += ")";
                }
                else if (parameters[i] is Constant)
                {
                    var constant = (Constant)parameters[i];
                    //script += constant.codeText;
                }
                if (i < parameters.Count - 1)
                    script += ",";
            }

            return script;
        }
    }
}
