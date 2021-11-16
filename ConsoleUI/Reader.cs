using System;
using BetterTriggers.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers
{
    public static class Reader
    {
        public static string ReadCommonJ()
        {
            // primitive types are not defined in common.j
            new Types("boolean");
            new Types("integer");
            new Types("real");
            new Types("string");
            new Types("void");


            string output = string.Empty;

            string filePath = @"C:\Users\Lasse Dam\Documents\Warcraft III\JassHelper\common.j";
            foreach (string line in File.ReadLines(filePath))
            {
                int i = 0;
                bool jumpToNextLine = false;
                string keyword = string.Empty;
                while (i < line.Length && !jumpToNextLine)
                {
                    string str = line.Substring(i, 1);

                    if (str != " ")
                        keyword += str;
                    else if (keyword == "constant" && keyword + line.Substring(i, 7) == "constant native")
                    {
                        keyword = "constant native";
                        i += 6;
                    }

                    switch (keyword)
                    {
                        case "type":
                            ReadType(line.Substring(i + 2, line.Length - i - 2));
                            jumpToNextLine = true;
                            break;
                        case "constant native":
                            ReadCondition(line.Substring(i + 2, line.Length - i - 2));
                            jumpToNextLine = true;
                            break;
                        default:
                            break;
                    }

                    i++;
                }
            }

            output = File.ReadAllText(filePath);

            return output;
        }

        private static void ReadType(string line)
        {
            int i = 0;
            bool finished = false;
            string typeName = string.Empty;

            while (!finished)
            {
                string str = line.Substring(i, 1);
                if (str != " ")
                    typeName += str;
                else
                    finished = true;

                i++;
            }

            Console.WriteLine("Type: " + typeName);
            var newType = new Types(typeName);
        }

        private static void ReadCondition(string line)
        {
            string name = string.Empty;
            int i = 0;

            // traverse and set condition name
            while (i < line.Length)
            {
                string str = line.Substring(i, 1);

                if (str == " ")
                    break;
                else
                    name += str;

                i++;
            }

            string takes = string.Empty; // just for checking
            while (i < line.Length)
            {
                string str = line.Substring(i, 1);

                if (takes == "takes")
                    break;
                else if (str != " ")
                    takes += str;

                i++;
            }

            // traverse and set parameters
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            string type = string.Empty;
            string typeIdentifier = string.Empty;
            bool isTypeSet = false;
            bool isIdentifierSet = false;

            while (i < line.Length)
            {
                string str = line.Substring(i, 1);

                if (!isTypeSet && str != " " && str != ",")
                    type += str;
                else if (isTypeSet && !isIdentifierSet && str == " ")
                    isIdentifierSet = true;
                else if(str != " ")
                    typeIdentifier += str;

                if (!isTypeSet && Types.collection[type] != null){
                    isTypeSet = true;
                    i++;
                }

                if (isTypeSet && isIdentifierSet)
                {
                    parameters.Add(new Tuple<string, string>(type, typeIdentifier));
                    type = string.Empty;
                    typeIdentifier = string.Empty;
                    isTypeSet = false;
                    isIdentifierSet = false;
                }

                if(type == "nothing") // reset type so we can get to the "returns" part
                    type = string.Empty;

                if (type == "returns") // break to go to return type
                    break;

                i++;
            }

            i++;

            // traverse and set return type
            string returnType = string.Empty;
            while (i < line.Length)
            {
                string str = line.Substring(i, 1);

                if (str != " ")
                    returnType += str;

                i++;
            }

            var condition = new Condition(name, parameters, returnType);

            Console.WriteLine("");
            Console.WriteLine("Condition:");
            Console.WriteLine($"Name: {name}");
            for (int p = 0; p < parameters.Count; p++)
            {
                Console.WriteLine($"Parameter: {parameters[p].Item1}, identifier: {parameters[p].Item2}");
            }
            Console.WriteLine($"Return type: {returnType}");
        }
    }
}
