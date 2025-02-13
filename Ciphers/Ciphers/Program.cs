/**********722871**********
 * Programmer: Kyle Gilbert
 * Date: 2/11/2025
 * Program Name: Ciphers
 * Program Description: A command-line tool for encrypting and decryption various ciphers.
 **************************/

using ArgumentBase;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CipherCrypt;

internal static class Program
{
    public static readonly Dictionary<string, MethodInfo> ciphers;
    static Program()
    {
        ciphers = [];
        Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in allTypes)
        {
            Type? cipherBase = t.GetInterface("ICipher");
            if (cipherBase is null) continue;

            string name = (string)t.GetProperty("Name")!.GetMethod!.Invoke(null, null)!;
            MethodInfo method = t.GetMethod("Cipher")!;

            ciphers.Add(name, method);
        }
    }

    public static void Main(string[] argsStr)
    {
        if (argsStr.Length < 2)
        {
            PrintUsage();
            return;
        }

        Arguments args = Arguments.Parse(argsStr[..2]);

        if (ciphers.TryGetValue(args.CipherName, out MethodInfo? cipher))
        {
            cipher.Invoke(null, [args.Mode == EncryptionMode.Encrypt, argsStr[2..]]);
        }
        else
        {
            PrintUsage();
            return;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("\n  \e[97mUSAGE: \e[95mcphr \e[93m[mode] \e[36m[ciphername] \e[37m[args...]\e[0m\n");
        Arguments.PrintParameters();
    }

    private class Arguments : ArgumentBase<Arguments>
    {
        [IsParameter(1, "Whether to encrypt or decrypt the input.")]
        public EncryptionMode Mode { get; set; }

        [IsParameter(2, "The name of the cipher to use.")]
        public string CipherName { get; set; }
    }
}
