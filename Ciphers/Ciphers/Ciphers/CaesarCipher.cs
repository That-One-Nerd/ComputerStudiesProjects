using ArgumentBase;
using System;
using System.IO;

namespace CipherCrypt.Ciphers;

public class CaesarCipher : ICipher
{
    public static string Name => "caesar";

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

        if (args.PrintTable) PrintTable(encrypt, args.Key, output);

        int cKey;
        while ((cKey = input.Read()) != -1)
        {
            char c = char.ToLower((char)cKey);
            if (c >= 'a' && c <= 'z')
            {
                int cVal = c - 'a';
                int newVal = encrypt ? (cVal + args.Key) : (cVal - args.Key);
                while (newVal < 0) newVal += 26;
                while (newVal >= 26) newVal -= 26;
                output.Write((char)(newVal + (encrypt ? 'A' : 'a')));
            }
            else output.Write(c);
        }

        output.Flush();
    }

    private static void PrintUsage()
    {
        Console.WriteLine($"\n  \e[97mUSAGE: \e[90mcphr [mode] \e[36m{Name} \e[33m[key] \e[32m[input] \e[90m[extra?...]\e[0m\n");
        Arguments.PrintArgs();
    }
    private static void PrintTable(bool encrypt, int key, TextWriter output)
    {
        // ╔═╤═╗
        // ║a│b║
        // ╟─┼─╢
        // ║D│E║
        // ╚═╧═╝

        output.Write("╔═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╤═╗\n║");
        for (int i = 0; i < 26; i++)
        {
            char baseC = encrypt ? 'a' : 'A';
            output.Write(((char)(baseC + i)).ToString());
            if (i == 25) output.Write("║");
            else output.Write("│");
        }
        output.Write("\n╟─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─┼─╢\n║");
        for (int i = 0; i < 26; i++)
        {
            int newVal;
            char baseC;
            if (encrypt)
            {
                baseC = 'A';
                newVal = i + key;
            }
            else
            {
                baseC = 'a';
                newVal = i - key;
            }
            while (newVal < 0) newVal += 26;
            while (newVal >= 26) newVal -= 26;
            output.Write(((char)(baseC + newVal)).ToString());
            if (i == 25) output.Write("║");
            else output.Write("│");
        }
        output.Write("\n╚═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╧═╝\n\n");
    }

    private class Arguments : CipherArgumentBase<Arguments, int>
    {
        [IsFlag("print-table", "Print the cipher table before generation.")]
        public bool PrintTable { get; set; }
    }
}
