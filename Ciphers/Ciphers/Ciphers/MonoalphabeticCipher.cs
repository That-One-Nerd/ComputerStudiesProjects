using ArgumentBase;
using System;
using System.Collections.Generic;
using System.IO;

namespace CipherCrypt.Ciphers;

public class MonoalphabeticCipher : ICipher
{
    public static string Name => "monoalpha";

    public static void Cipher(bool encrypt, string[] argsStr)
    {
        Arguments args = Arguments.Parse(argsStr);
        if (!args.anyParams)
        {
            PrintUsage();
            return;
        }

        (TextReader? input, TextWriter output) = args.ConfigureIO();
        if (input is null)
        {
            PrintUsage();
            return;
        }

        Dictionary<char, char> convert = MakeConversionTable(encrypt, args.Key);
        if (args.PrintTable) PrintTable(convert, output);

        int cRaw;
        while ((cRaw = input.Read()) != -1)
        {
            char c = (char)cRaw;
            if (encrypt) c = char.ToLower(c);
            else c = char.ToUpper(c);

            if ((c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z'))
            {
                char cOther = convert[c];
                output.Write(cOther);
            }
            else output.Write(c);
        }

        output.Flush();
    }

    private static Dictionary<char, char> MakeConversionTable(bool encrypt, string key)
    {
        // Slightly scuffed. Could the second loop be made better?
        key = key.ToUpper();
        string added = "";
        Dictionary<char, char> result = [];

        for (int i = 0; result.Count < 26 && i < key.Length; i++)
        {
            char c = key[i];
            if (!char.IsAsciiLetter(c)) continue;
            else if (added.Contains(c)) continue;
            added += c;
            if (encrypt) result.Add((char)('a' + result.Count), c);
            else result.Add(c, (char)('a' + result.Count));
        }
        for (int i = 0; result.Count < 26; i++)
        {
            char c = (char)('A' + i);
            if (added.Contains(c)) continue;
            if (encrypt) result.Add((char)('a' + result.Count), c);
            else result.Add(c, (char)('a' + result.Count));
        }

        return result;
    }
    private static void PrintTable(Dictionary<char, char> convert, TextWriter output)
    {
        // ╔═╤═╗
        // ║a│b║
        // ╟─┼─╢
        // ║D│E║
        // ╚═╧═╝

        output.Write("╔═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╗\n║");
        int i = 0;
        foreach (char c in convert.Keys)
        {
            output.Write(c.ToString());
            if (i == 25) output.Write("║");
            else output.Write("│");
            i++;
        }
        output.Write("\n╟─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─╢\n║");
        i = 0;
        foreach (char c in convert.Values)
        {
            output.Write(c.ToString());
            if (i == 25) output.Write("║");
            else output.Write("│");
            i++;
        }
        output.Write("\n╚═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╝\n\n");
    }

    private static void PrintUsage()
    {
        Console.WriteLine($"\n  \e[97mUSAGE: \e[90mcphr [mode] \e[36m{Name} \e[33m[key] \e[32m[input] \e[90m[extra?...]\e[0m\n");
        Arguments.PrintArgs();
    }

    private class Arguments : CipherArgumentBase<Arguments, string>
    {
        [IsFlag("print-table", "Print the cipher table before generation.")]
        public bool PrintTable { get; set; }
    }
}
