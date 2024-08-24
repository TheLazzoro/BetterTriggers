using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTriggers.Logging
{
    internal class SessionDTO
    {
        public string MachineName { get; set; }
        public string SystemLanguage { get; set; }
        public Version AppVersion { get; set; }
        public Version GameVersion { get; set; }
    }
}
