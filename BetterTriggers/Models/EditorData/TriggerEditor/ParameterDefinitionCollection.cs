using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Utility;
using ICSharpCode.Decompiler.DebugInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Models.EditorData
{
    public class ParameterDefinitionCollection : TriggerElementCollection
    {
        public ParameterDefinitionCollection(TriggerElementType Type) : base(Type) { }

        public void CreateParameterDefinition(ExplorerElement explorerElement)
        {
            var definition = new ParameterDefinition();
            definition.Name = GenerateParameterDefName();
            definition.Id = GenerateId();

            CommandTriggerElementCreate command = new CommandTriggerElementCreate(explorerElement, definition, this, Elements.Count);
            command.Execute();
        }

        public void RenameParameterDefinition(ExplorerElement explorerElement, ParameterDefinition parameterDefinition)
        {
            string newName = parameterDefinition.RenameText;
            if (newName == parameterDefinition.Name)
                return;

            if (string.IsNullOrEmpty(newName))
            {
                throw new Exception("Name cannot be empty.");
            }

            foreach (ParameterDefinition def in Elements)
            {
                if (def.Name == newName)
                    throw new Exception($"Parameter with name '{newName}' already exists.");
            }

            CommandTriggerElementRename command = new CommandTriggerElementRename(explorerElement, parameterDefinition, newName);
            command.Execute();
        }

        public ParameterDefinition? GetByReference(ParameterDefinitionRef paramDefRef)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                var element = (ParameterDefinition)Elements[i];
                if (element.Id == paramDefRef.ParameterDefinitionId)
                {
                    return element;
                }
            }

            return null;
        }

        private int GenerateId()
        {
            int id = 0;
            bool exists = true;
            while (exists)
            {
                exists = false;
                foreach (ParameterDefinition item in Elements)
                {
                    if(item.Id == id)
                    {
                        exists = true;
                    }
                }
                id = RandomUtil.GenerateInt();
            }
            return id;
        }

        private string GenerateParameterDefName()
        {
            string name = "Untitled Parameter";
            int i = 0;
            bool validName = false;
            while (!validName)
            {
                validName = true;
                foreach (ParameterDefinition element in Elements)
                {
                    if (element.Name == name)
                    {
                        validName = false;
                        name = "Untitled Parameter " + i;
                    }
                }
                i++;
            }
            return name;
        }
    }
}
