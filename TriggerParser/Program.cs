using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Categories;
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
            TriggerElementParser.ParseTriggerElements(@"C:\Users\Lasse Dam\Desktop\TriggerData\Conditions.txt", ConditionContainer.Id);
            TriggerElementParser.ParseTriggerElements(@"C:\Users\Lasse Dam\Desktop\TriggerData\Actions.txt", ActionContainer.Id);

            // Converts all parsed elements and writes a json file
            EventConverter.ConvertEvents(EventContainer.container);
            // ConditionConverter.ConvertConditions(ConditionContainer.container); // not yet functional
            ActionConverter.ConvertActions(ActionContainer.container);
        }
    }
}
