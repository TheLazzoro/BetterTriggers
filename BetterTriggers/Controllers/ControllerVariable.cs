using BetterTriggers.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Commands;
using System.Linq;

namespace BetterTriggers.Controllers
{
    public class ControllerVariable
    {
        public static bool includeLocals { get; set; } = true;

        
    }
}
