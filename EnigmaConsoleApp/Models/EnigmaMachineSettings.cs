using System;

namespace EnigmaConsoleApp.Models
{
    public class EnigmaMachineSettings
    {
        public string? EnigmaName { get; set; }
        public int[]? Rotors { get; set; }
        public int[]? StartPositions { get; set; }
        public string[]? PlugSettings { get; set; }
        public string? Reflector { get; set; }
    }
}