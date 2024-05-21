using System;
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
    // the layout of the wires. Each index is treated as an input value, with the
    // output value being the value stored at the index. E.g. [1,0,2] means that 
    // the wiring is as follows: A -> B, B -> A, C -> C 
    private int[] wires;
    // the position of the notch on the rotor
    private int notch;
    // the current position of the rotor (what letter is at the top of the rotor)
    private int currentPosition;
    // the position at which the rotor will cause the rotor next to it to turn over
    private int turnOver;
    // the offset of the rotor, 
    private int offset;

#region Constructors

    /// <summary>
    /// Initializes a new rotor.
    /// </summary>
    /// <param name="wireSettings">The wire connections, encoded as a string.</param>
    /// <param name="notchPosition">The position of the notch on the rotor.</param>
    /// <param name="turnOverPosition">The turnover position of the rotor.</param>
    public Rotor(string wireSettings, int notchPosition, int turnOverPosition)
    {
        this.notch = notchPosition;
        this.turnOver = turnOverPosition;
        SetRotorWiring(wireSettings);
        this.currentPosition = 0;
        this.offset = 0;
    }

    /// <summary>
    /// Initializes a new rotor.
    /// </summary>
    /// <param name="settings">The settings of the rotor to be set to.</param>
    public Rotor(EngimaRotorSetting settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (settings.Notch is null || settings.TurnOver is null 
        || settings.TurnOver is null || settings.Wires is null)
            throw new ArgumentException("Provided rotor settings has property that is null.");
        this.notch = settings.Notch![0];
        this.turnOver = settings.TurnOver![0];
        SetRotorWiring(settings.Wires!);
        this.currentPosition = 0;
        this.offset = 0;
    }

#endregion

#region Public Methods

    /// <summary>
    /// Moves the rotor forward one position.
    /// </summary>
    /// <returns><c>True</c> if the rotor has turned over, else <c>False</c></returns>
    public bool Rotate()
    {
        currentPosition = (short)((currentPosition + 1) % 27);
        return (currentPosition - 1) == turnOver;
    }

    /// <summary>
    /// Gets the output wire at the given position.
    /// </summary>
    /// <param name="input">The position of the input.</param>
    /// <returns>The output wire's position.</returns>
    public int GetOutputWire(int input)
    {
        return wires[input];
    }

#endregion
    
#region Private Methods
    
    /// <summary>
    /// Set the rotor wiring, based on an encoded string.
    /// </summary>
    /// <param name="wireSettings">The wire settings as a string.</param>
    private void SetRotorWiring(string wireSettings)
    {
        char[] ws = wireSettings.ToCharArray();
        wires = new int[26];
        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)ws[i] % 65;
    }
#endregion
}

public class EnigmaMachine
{
    private readonly Rotor[] rotors;
    private int[]? plugs;
    // UKW-B Reflector Setting
    private int[] reflector;

#region Constructors

    /// <summary>
    /// The default constructor. Will initialise an Enigma Machine with some default
    /// settings.
    /// </summary>
    public EnigmaMachine() 
    { 
        rotors = new Rotor[3];
        rotors[0] = new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ",24,16);
        rotors[1] = new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE",12,4);
        rotors[2] = new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO",3,21);
        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
        SetPlugs(["ZA"]);
    }

    public EnigmaMachine(Rotor[] rotors, string[] plugSettings)
    {
        this.rotors = rotors;
        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
        SetPlugs(plugSettings);
    }

    public EnigmaMachine(EngimaRotorSettings rotorSettings, EnigmaMachineSettings machineSettings)
    {

        if (rotorSettings == null)
            throw new ArgumentException("Provided rotor settings is null.");
        if (machineSettings == null || machineSettings.Rotors == null || 
        machineSettings.StartPositions == null)
            throw new ArgumentException("Provided machine setting is null, or a property is null.");

        rotors = new Rotor[3];

        for (int i=0;i<3;i++)
        {
            int selectedRotor = machineSettings.Rotors[i];
            rotors[i] = new Rotor(rotorSettings.RotorSettings![selectedRotor]);
        }

        SetReflectorWiring("YRUHQSLDPXNGOKMIEBFZCWVJAT");
    }

#endregion

#region Public Methods

    /// <summary>
    /// Takes in a message and encodes it, returning the encoded message.
    /// </summary>
    /// <param name="message">The message to encode.</param>
    /// <returns>The encoded message.</returns>
    public string EncryptString(string message)
    {
        string encryptedMessage = "";
        foreach (char c in message)
            encryptedMessage += PressChar(c);
        return encryptedMessage;
    }

    /// <summary>
    /// Presses the given character on the Enigma Machine's keyboard, outputing the
    /// key that is lit up and rotating the machine's rotors.
    /// </summary>
    /// <param name="c">The character that was pressed.</param>
    /// <returns>The key that was lit up.</returns>
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

#endregion

#region Private Methods

    /// <summary>
    /// Sets the machine's plugboard.
    /// </summary>
    /// <param name="plugSettings">The plugboard settings.</param>
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

    /// <summary>
    /// Sets the reflector's wiring.
    /// </summary>
    /// <param name="reflectorWiring">The reflect wiring settings.</param>
    private void SetReflectorWiring(string reflectorWiring)
    {
        char[] rw = reflectorWiring.ToCharArray();
        reflector = new int[26];
        for (int i=0;i<reflector.Length;i++)
            reflector[i] = (int)rw[i]%65;
    }

    /// <summary>
    /// Moves the rotors position(s) by one.
    /// </summary>
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

#endregion
}