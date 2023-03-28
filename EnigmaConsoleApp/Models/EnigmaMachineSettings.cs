using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnigmaConsoleApp.Models
{
    public class EnigmaMachineSettings
    {
        public int[]? Rotors { get; set; }
        public int[]? StartPositions { get; set; }
        public string[]? PlugSettings { get; set; }
    }
}