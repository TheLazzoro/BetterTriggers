using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Calls;
using TriggerParser.Categories;
using TriggerParser.Conditions;
using TriggerParser.Converter;
using TriggerParser.Params;
using TriggerParser.TriggerElements;
using TriggerParser.Types;

namespace TriggerParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //string triggerData = File.ReadAllText(@"C:\Users\Lasse Dam\Desktop\triggerdata.txt");
            TriggerCategoryParser.ParseCategories(@"C:\Users\Lasse Dam\Desktop\TriggerData\Categories.txt");
            TriggerTypeParser.ParseVariableTypes(@"C:\Users\Lasse Dam\Desktop\TriggerData\VariableTypes.txt");
            TriggerTypeParser.ParseOtherTypes(@"C:\Users\Lasse Dam\Desktop\TriggerData\OtherTypes.txt");
            TriggerParamParser.ParseParams(@"C:\Users\Lasse Dam\Desktop\TriggerData\Constants.txt");

            TriggerElementParser.ParseTriggerElements(@"C:\Users\Lasse Dam\Desktop\TriggerData\Events.txt", EventContainer.Id);
            TriggerConditionParser.ParseConditions(@"C:\Users\Lasse Dam\Desktop\TriggerData\Conditions.txt");
            TriggerElementParser.ParseTriggerElements(@"C:\Users\Lasse Dam\Desktop\TriggerData\Actions.txt", ActionContainer.Id);
            TriggerCallParser.ParseCalls(@"C:\Users\Lasse Dam\Desktop\TriggerData\Calls.txt");
            TriggerTypeParser.ParseVariableTypes(@"C:\Users\Lasse Dam\Desktop\TriggerData\VariableTypes.txt");

            // Converts all parsed elements and writes a json file
            ConstantConverter.ConvertConstants(TriggerParamContainer.container);
            EventConverter.ConvertEvents(EventContainer.container);
            ConditionConverter.ConvertConditions(TriggerConditionContainer.container);
            ActionConverter.ConvertActions(ActionContainer.container);
            CallConverter.ConvertCalls(TriggerCallContainer.container);
            TypeConverter.ConvertTypes(TriggerTypeContainer.variableTypes);
        }
    }
}
