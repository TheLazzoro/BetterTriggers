using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        Project _project;
        string commandName = "Modify Parameter";
        ECA eca;
        ExplorerElement explorerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld;
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(Project project, ExplorerElement explorerElement, ECA eca, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.eca = eca;
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.oldParameter = paramCollection[this.paramIndex];
        }

        public void Execute()
        {
            paramCollection[paramIndex] = paramToAdd;

            // Special case
            if (eca is SetVariable)
            {
                Parameter setVarParam = eca.function.parameters[0];
                Parameter value = eca.function.parameters[1];
                bool doResetValue = false;

                if (paramCollection[paramIndex] == setVarParam && setVarParam is VariableRef && oldParameter is VariableRef)
                {
                    var setVarParamRef = setVarParam as VariableRef;
                    var setVarParamRefOld = oldParameter as VariableRef;
                    var newVar = _project.Variables.GetByReference(setVarParamRef, explorerElement);
                    var oldVar = _project.Variables.GetByReference(setVarParamRefOld, explorerElement);
                    if (!Types.AreTypesEqual(newVar.War3Type.Type, oldVar.War3Type.Type))
                    {
                        doResetValue = true;
                    }

                    // Copy array index parameters
                    if (newVar.IsArray == oldVar.IsArray && newVar.IsTwoDimensions == oldVar.IsTwoDimensions)
                    {
                        setVarParamRef.arrayIndexValues[0] = setVarParamRefOld.arrayIndexValues[0].Clone();
                        setVarParamRef.arrayIndexValues[1] = setVarParamRefOld.arrayIndexValues[1].Clone();
                    }
                }
                else if (paramCollection[paramIndex] == setVarParam && setVarParam is VariableRef && oldParameter is not VariableRef)
                {
                    var setVarParamRef = setVarParam as VariableRef;
                    var newVar = _project.Variables.GetByReference(setVarParamRef, explorerElement);
                    var valueReturnType = TriggerData.GetReturnType(value.value);
                    if (valueReturnType != newVar.War3Type.Type)
                    {
                        doResetValue = true;
                    }
                }

                if (doResetValue)
                {
                    setVarValueOld = value;
                    setVarValueNew = new Parameter()
                    {
                        value = null,
                    };
                    paramCollection[paramIndex + 1] = setVarValueNew;
                }
            }

            _project.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
            _project.CommandManager.AddCommand(this);
            eca.IsSelected = true;
        }

        public void Redo()
        {
            // 'SetVariable' special case
            if (setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueNew;
            }

            paramCollection[paramIndex] = paramToAdd;
            _project.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
            eca.IsSelected = true;
        }

        public void Undo()
        {
            // 'SetVariable' special case
            if (setVarValueOld != null)
            {
                paramCollection[paramIndex + 1] = setVarValueOld;
            }

            paramCollection[paramIndex] = oldParameter;
            _project.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
            eca.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
