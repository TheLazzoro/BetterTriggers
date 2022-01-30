using Model.Containers;
using Model.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggerParser.Params;
using TriggerParser.TriggerElements;

namespace TriggerParser.Converter
{
    public static class ConstantConverter
    {
        public static void ConvertConstants(List<TriggerParam> constants)
        {
            foreach (var item in constants)
            {
                ConstantTemplate constant = new ConstantTemplate()
                {
                    identifier = item.key,
                    name = item.displayText,
                    codeText = item.codeText,
                    returnType = item.variableType,
                };

                ContainerConstants.AddConstant(constant);
            }

            string json = JsonConvert.SerializeObject(ContainerConstants.GetAllTypes());
            File.WriteAllText(@"C:\Users\Lasse Dam\Desktop\ParseTest\constants.json", json);
        }
    }
}
