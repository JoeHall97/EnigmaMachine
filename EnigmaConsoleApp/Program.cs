using System;
using System.IO;
using System.Text.Json;
using EnigmaConsoleApp.Models;

namespace EnigmaConsoleApp
{
    class EnigmaConsoleApp
    {
        public static void Main(string[] args)
        {
            EnigmaMachine em = new EnigmaMachine("Enigma1Rotors.json","");
            Console.WriteLine(em.EncryptString("THEQUICKBROWNFOXJUMPEDOVERTHELAZYDOG"));
        }
    }
}