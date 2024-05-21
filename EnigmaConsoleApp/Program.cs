using System;
using System.IO;
using System.Text.Json;
using EnigmaConsoleApp.Models;

namespace EnigmaConsoleApp;

class EnigmaConsoleApp
{
    public static void Main(string[] args)
    {
        string directory = Directory.GetCurrentDirectory();
        if (!Directory.Exists(Path.Combine(directory, "Rotor Settings")))
        {
            // this is dumb
            // TODO: find a better method than this
            directory = Directory.GetParent(directory)?.ToString()!;
            directory = Directory.GetParent(directory)?.ToString()!;
            directory = Directory.GetParent(directory)?.ToString()!;
        }

        string rotorSettingsPath = Path.Combine(directory, "Rotor Settings", "Enigma1Rotors.json");
        string machineSettingsPath = Path.Combine(directory, "Enigma Settings", "Enigma1Settings1.json");

        try
        {
            string jsonRotorString = File.ReadAllText(rotorSettingsPath);
            EngimaRotorSettings ers = JsonSerializer.Deserialize<EngimaRotorSettings>(jsonRotorString)!;
            string jsonMachineString = File.ReadAllText(machineSettingsPath);
            EnigmaMachineSettings ems = JsonSerializer.Deserialize<EnigmaMachineSettings>(jsonMachineString)!;

            EnigmaMachine em = new EnigmaMachine(ers, ems);
            Console.WriteLine(em.EncryptString("THEQUICKBROWNFOXJUMPEDOVERTHELAZYDOG"));
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
    }
}