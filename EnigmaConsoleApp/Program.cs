﻿using System;
using System.IO;

namespace EnigmaConsoleApp;

class EnigmaConsoleApp
{
    public static void Main(string[] args)
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

        EnigmaMachine em = new EnigmaMachine(rotorSettingsPath, machineSettingsPath);
            
        Console.WriteLine(em.EncryptString("THEQUICKBROWNFOXJUMPEDOVERTHELAZYDOG"));
    }
}