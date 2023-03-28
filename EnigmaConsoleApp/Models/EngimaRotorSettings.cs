using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace EnigmaConsoleApp.Models
{
    public class EngimaRotorSettings
    {
        public IList<EngimaRotorSetting>? RotorSettings { get; set; }
    }

    public class EngimaRotorSetting
    {
        public string? Wires { get; set; }
        public string? Notch { get; set; }
        public string? TurnOver { get; set; }
    }
}