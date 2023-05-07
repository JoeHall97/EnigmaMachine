using System;
using System.Collections.Generic;

namespace EnigmaConsoleApp.Models
{
    public class EnigmaReflectorSettings
    {
        public List<EngimaReflectorSetting> EnigmaReflectors { get; set; }
    }

    public class EngimaReflectorSetting
    {
        public string? EnigmaName { get; set; }
        public List<ReflectorSetting> Reflectors { get; set; }
    }

    public class ReflectorSetting
    {
        public string? ReflectorName { get; set; }
        public string? ReflectorWiring { get; set; }
    }
}