using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Utility;
using System;

namespace BetterTriggers.Models.EditorData
{
    public class ParameterDefinitionCollection : TriggerElementCollection
    {
        public ParameterDefinitionCollection(TriggerElementType Type) : base(Type) { }

        public void CreateParameterDefinition(Project project, ExplorerElement explorerElement)
        {
            var definition = new ParameterDefinition();
            definition.Name = GenerateParameterDefName();
            definition.Id = GenerateId();

            CommandTriggerElementCreate command = new CommandTriggerElementCreate(project, explorerElement, definition, this, Elements.Count);
            command.Execute();
        }

        public void RenameParameterDefinition(Project project, ExplorerElement explorerElement, ParameterDefinition parameterDefinition)
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

            CommandTriggerElementRename command = new CommandTriggerElementRename(project, explorerElement, parameterDefinition, newName);
            command.Execute();
        }

        public override ParameterDefinitionCollection Clone()
        {
            var clone = new ParameterDefinitionCollection(ElementType);
            this.Elements.ForEach(element =>
            {
                var clonedChild = element.Clone();
                clonedChild.SetParent(clone, clone.Elements.Count);
            });
            return clone;
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

        internal string GenerateParameterDefName()
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
