using Xunit;
using EnigmaConsoleApp;
using System;
using System.IO;

namespace EnigmaConsoleApp.Tests;

public class EnigmaMachineUnitTests
{
    /// <summary>
    /// Tests that info read in from files correctly allows for a new
    /// EnigmaMachine to be initialised.
    /// </summary>
    [Fact]
    public void TestReadingFromFile()
    {
        var directory = Directory.GetCurrentDirectory();
        
        if (!Directory.Exists(Path.Combine(directory, "Rotor Settings")))
        {
            // this is dumb
            // TODO: find a better method than this
            directory = Directory.GetParent(directory)?.ToString();
            directory = Directory.GetParent(directory)?.ToString();
            directory = Directory.GetParent(directory)?.ToString();
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
            Assert.NotNull(em);
        } catch(Exception e)
        {
            Assert.Fail(e.StackTrace);
        }
    }
}