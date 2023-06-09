using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using EnigmaConsoleApp.Models;

namespace EnigmaConsoleApp;

// HOW DOES THE ENIGMA MACHINE WORK?

// KEY COMPONENTS
// * Keyboard
// * Lamp board
// * Three rotors with 26 positions each, and a reflector
// * A plug board with up to ten cables (this is the ammount used by the Germans, but you could use more)

// ROTORS
// Each rotor has wiring in it that links an input letter to a different output letter.
// Signals come into the rotors from the keyboard, go throught the rotors, are then sent through a reflector, 
// back through the rotors again and then to the lamp board.
// NOTE: the reflector wires the letters in pairs.
// Each rotor can be set to one of it's 26 positions.
// Each time a key is pressed the first rotor in the circuit is rotated.
// Once the first rotor has completed a full rotation, the second rotor is rotated once.
// Once the second rotor has completed a full rotation, the third rotor is rotated once

// NOTE: Ring numbers and notch are linked, therefore we only need to know the ring position.

// ENGIMA SETTINGS
// * Rotor Order (each Enigma machine came with 5 seperate rotors)
// * Ring Order (this is both the order of the numbers and the notch that determines when the next ring will be rotated)
// * Rotor Starting Position 
// * Plug Board Settings

public class Rotor
{
    private int[] wires;
    private int notch;
    private int currentPosition;
    private int turnOver;
    private int offset;

    public Rotor(string wireSettings, int notchPosition, int turnOverPosition, int ringIndex, int ringPosition)
    {
        this.notch = notchPosition;
        this.currentPosition = ringPosition;
        this.turnOver = turnOverPosition;
        this.offset = ringIndex;
        SetRotorWiring(wireSettings);
    }

    private void SetRotorWiring(string wireSettings)
    {
        char[] ws = wireSettings.ToCharArray();
        wires = new int[26];
        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)ws[i] % 65;
    }

    /// RETURNS: True if the rotor has been turned over
    public bool Rotate()
    {
        currentPosition = (short)((currentPosition + 1) % 27);
        return (currentPosition - 1) == turnOver;
    }

    public int GetOutputWire(int input)
    {
        return wires[input];
    }
}

public class EnigmaMachine
{
    private Rotor[] rotors;
    private int[]? plugs;
    // UKW-B Reflector Setting
    private int[] reflector;

    public EnigmaMachine() 
    { 
        rotors = new Rotor[3];
        rotors[0] = new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ",24,16,0,0);
        rotors[1] = new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE",12,4,0,0);
        rotors[2] = new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO",3,21,0,0);
        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
        SetPlugs(new string[]{"ZA"});
    }

    public EnigmaMachine(Rotor[] rotors, string[] plugSettings)
    {
        this.rotors = rotors;
        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
        SetPlugs(plugSettings);
    }

    public EnigmaMachine(string enigmaRotorFile, string engimaSettingsFile)
    {            
        try
        {
            string jsonRotorString = File.ReadAllText(enigmaRotorFile);
            EngimaRotorSettings ers = JsonSerializer.Deserialize<EngimaRotorSettings>(jsonRotorString)!;
            string jsonMachineString = File.ReadAllText(engimaSettingsFile);
            EnigmaMachineSettings ems = JsonSerializer.Deserialize<EnigmaMachineSettings>(jsonMachineString)!;

            if (ers == null)
                throw new ArgumentException("Provided rotor setting file does not cotain RotorSettings.");
            if (ems.Rotors == null || ems.StartPositions == null)
                throw new ArgumentException("Provided machine setting file is not formateted correctly.");

            rotors = new Rotor[3];

            for (int i=0;i<3;i++)
            {
                int selectedRotor = ems.Rotors[i];
                string? rotorWireSettings = ers?.RotorSettings[selectedRotor].Wires;
                int notchPosition = ers.RotorSettings[selectedRotor].Notch.ToCharArray()[0];
                int turnOverPosition = ers.RotorSettings[selectedRotor].TurnOver.ToCharArray()[0];
                int ringPosition = ems.StartPositions[i];
                    
                rotors[i] = new Rotor(rotorWireSettings,notchPosition,turnOverPosition,0,ringPosition);
            }
        } catch(Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return;
        }
        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
    }

    public string EncryptString(string message)
    {
        string encryptedMessage = "";
        foreach (char c in message)
            encryptedMessage += PressChar(c);
        return encryptedMessage;
    }

    public char PressChar(char c)
    {
        UpdateRotorPositions();
            
        int output = (int)c%65;
        for (int i=0;i<rotors.Length;i++)
            output = rotors[i].GetOutputWire(output);
        output = reflector[output];
        for (int i=rotors.Length-1;i>=0;i--)
            output = rotors[i].GetOutputWire(output);

        return (char)(output+65);
    }

    private void SetPlugs(string[] plugSettings)
    {
        if (plugSettings == null)
            return;
        plugs = new int[26];
        foreach (var plug in plugSettings)
        {
            if (plug.Length != 2)
                continue;
                
            var p = plug.ToCharArray();
            int p1 = p[0]%65;
            int p2 = p[1]%65;

            plugs[p1] = p2;
            plugs[p2] = p1;
        }
    }

    private void SetReflectorWiring(string reflectorWiring)
    {
        char[] rw = reflectorWiring.ToCharArray();
        reflector = new int[26];
        for (int i=0;i<reflector.Length;i++)
            reflector[i] = (int)rw[i]%65;
    }

    private void UpdateRotorPositions()
    {
        bool turnedOver = rotors[0].Rotate();
        for (int i=1;i<rotors.Length;i++)
        {
            if (!turnedOver)
                break;
            turnedOver = rotors[i].Rotate();
        }
    }
}