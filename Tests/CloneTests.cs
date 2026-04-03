using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Info;

namespace Tests
{

    [TestClass]
    public class CloneTests : TestBase
    {
        private ScriptLanguage language = ScriptLanguage.Jass;
        private static string parentFolder = "TestProjectsClone";
        private static string directory = Path.Combine(Directory.GetCurrentDirectory(), "TempClone");
        private string projectFolder;
        private string name;
        Project project;


        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING PROJECT TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");

            var parentDir = Path.Combine(directory, parentFolder);
            if (Directory.Exists(parentDir))
                Directory.Delete(parentDir, true);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            name = "Project-" + Guid.NewGuid().ToString();
            var parentDir = Path.Combine(directory, parentFolder);
            projectFolder = Path.Combine(parentDir, name);
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }
        }

        [TestCleanup]
        public void AfterEach()
        {
            if (project != null)
            {
                project.Close();
            }
        }

        [TestMethod]
        public void Clone_IfThenElse_Test()
        {
            // Arrange
            string TriggerSleepAction = "TriggerSleepAction";

            var ifThenElse = new IfThenElse(project);
            var eca1 = new ECA(project, TriggerSleepAction);
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
            var projectPath = Project.Create(language, name, projectFolder);
            project = Project.Load(projectPath);
            var explorerElement = new ExplorerElement(project, ExplorerElementEnum.ActionDefinition);
            var actionDefinition = new ActionDefinition(project, explorerElement);
            explorerElement.actionDefinition = actionDefinition;
            var parameterDef = new ParameterDefinition(project);
            var variable = new Variable();
            variable.War3Type = War3Type.Get("integer");
            variable.InitialValue = new Parameter();
            var localVar = new LocalVariable(project, variable);
            var eca = new ECA(project);

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
