using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Info;

namespace Tests
{

    [TestClass]
    public class CloneTests : TestBase
    {
        private ScriptLanguage language = ScriptLanguage.Jass;
        private string name = "TestProject";
        private string directory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Temp");
        Project project;

        [TestInitialize]
        public void BeforeEach()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        [TestCleanup]
        public void AfterEach()
        {
            if (project != null)
            {
                Project.Close();
            }
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        [TestMethod]
        public void Clone_IfThenElse_Test()
        {
            // Arrange
            string TriggerSleepAction = "TriggerSleepAction";

            var ifThenElse = new IfThenElse();
            var eca1 = new ECA(TriggerSleepAction);
            var params1 = new List<Parameter>()
            {
                new Value()
                {
                    value = "0.00"
                }
            };
            eca1.function.parameters = params1;
            eca1.SetParent(ifThenElse.Then, 0);

            // Act
            var clone = ifThenElse.Clone();

            // Assert
            var clonedECA = clone.Then.Elements[0] as ECA;
            Assert.AreEqual(1, clone.Then.Elements.Count);
            Assert.AreEqual(TriggerSleepAction, clonedECA.function.value);
        }

        [TestMethod]
        public void Clone_ActionDefinition_Test()
        {
            // Arrange
            var projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            var explorerElement = new ExplorerElement(ExplorerElementEnum.ActionDefinition);
            var actionDefinition = new ActionDefinition(explorerElement);
            explorerElement.actionDefinition = actionDefinition;
            var parameterDef = new ParameterDefinition();
            var variable = new Variable();
            variable.War3Type = War3Type.Get("integer");
            variable.InitialValue = new Parameter();
            var localVar = new LocalVariable(variable);
            var eca = new ECA();

            parameterDef.SetParent(actionDefinition.Parameters, 0);
            localVar.SetParent(actionDefinition.LocalVariables, 0);
            eca.SetParent(actionDefinition.Actions, 0);

            // Act
            var clone = explorerElement.Clone();

            // Assert
            Assert.AreEqual(1, clone.actionDefinition.Parameters.Elements.Count());
            Assert.AreEqual(1, clone.actionDefinition.LocalVariables.Elements.Count());
            Assert.AreEqual(1, clone.actionDefinition.Actions.Elements.Count());
            Assert.IsTrue(clone.actionDefinition.Parameters.Elements[0] is ParameterDefinition);
            Assert.IsTrue(clone.actionDefinition.LocalVariables.Elements[0] is LocalVariable);
            Assert.IsTrue(clone.actionDefinition.Actions.Elements[0] is ECA);
        }
    }
}
