using GUI.Components.TriggerExplorer;
using Model;
using Model.Data;
using Model.Natives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GUI.Controllers
{
    public class ControllerScriptGenerator
    {
        List<Variable> variables = new List<Variable>();
        List<string> scripts = new List<string>();
        List<Trigger> triggers = new List<Trigger>();

        public string GenerateScript(TriggerExplorer triggerExplorer)
        {
            var root = triggerExplorer.map;
            GatherTriggerElements(root);
            string script = GenerateScript();

            return script;
        }

        private void GatherTriggerElements(ExplorerElement parent)
        {
            string script = string.Empty;

            // Gather all explorer elements

            for (int i = 0; i < parent.Items.Count; i++)
            {
                var element = (ExplorerElement)parent.Items[i];

                if (Directory.Exists(element.FilePath))
                    GatherTriggerElements(element);
                else if (File.Exists(element.FilePath))
                {
                    if (Path.GetExtension(element.FilePath) == ".trg")
                    {
                        string json = File.ReadAllText(element.FilePath);
                        Trigger trig = JsonConvert.DeserializeObject<Trigger>(json);
                        triggers.Add(trig);
                    }
                    else if (Path.GetExtension(element.FilePath) == ".j")
                    {
                        scripts.Add(File.ReadAllText(element.FilePath) + System.Environment.NewLine);
                    }
                    else if (Path.GetExtension(element.FilePath) == ".var")
                    {
                        string json = File.ReadAllText(element.FilePath);
                        var variable = JsonConvert.DeserializeObject<Variable>(json);
                        variables.Add(variable);
                    }
                }
            }
        }

        private string GenerateScript()
        {
            string script = string.Empty;

            // ---- Generate script ---- //

            List<Variable> InitGlobals = new List<Variable>();

            // Global variables
            for (int i = 0; i < variables.Count; i++)
            {
                Variable variable = variables[i];
                if (variable.InitialValue != null)
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

            script += GenerateUnits();

            // main
            script += "function main takes nothing returns nothing" + System.Environment.NewLine;
            script += "endfunction" + System.Environment.NewLine;

            return script;
        }

        private string GenerateUnits()
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
                    script += constant.codeText;
                }
                if (i < parameters.Count - 1)
                    script += ",";
            }

            return script;
        }
    }
}
