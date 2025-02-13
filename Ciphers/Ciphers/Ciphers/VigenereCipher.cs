using ArgumentBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CipherCrypt.Ciphers;

public class VigenereCipher : ICipher
{
    public static string Name => "vigenere";

    public static void Cipher(bool encrypt, string[] argsStr)
    {
        Arguments args = Arguments.Parse(argsStr);
        if (!args.anyParams)
        {
            PrintUsage();
            return;
        }
        args.Key = FormatKey(args.Key, args.WithUnderscore);

        (TextReader? input, TextWriter output) = args.ConfigureIO();
        if (input is null)
        {
            PrintUsage();
            return;
        }

        int cRaw;
        int keyIndex = 0;
        int modBy = args.WithUnderscore ? 27 : 26;
        while ((cRaw = input.Read()) != -1)
        {
            char c = char.ToLower((char)cRaw);
            char k = args.Key[keyIndex];

            if (c >= 'a' && c <= 'z' || (args.WithUnderscore && c == '_'))
            {
                int cVal = c == '_' ? 26 : c - 'a';
                int kVal = k == '_' ? 26 : k - 'A';
                if (encrypt)
                {
                    int newVal = cVal + kVal;
                    while (newVal < 0) newVal += modBy;
                    while (newVal >= modBy) newVal -= modBy;
                    if (newVal == 26) output.Write('_');
                    else output.Write((char)(newVal + 'A'));
                }
                else
                {
                    int newVal = cVal - kVal;
                    while (newVal < 0) newVal += modBy;
                    while (newVal >= modBy) newVal -= modBy;
                    if (newVal == 26) output.Write('_');
                    else output.Write((char)(newVal + 'a'));
                }
                keyIndex = (keyIndex + 1) % args.Key.Length;
            }
            else output.Write(c);
        }

        output.Flush();
    }

    private static string FormatKey(string str, bool withUnderscore)
    {
        StringBuilder result = new();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (char.IsAsciiLetter(c) || (withUnderscore && c == '_'))
            {
                result.Append(char.ToUpper(c));
            }
        }
        return result.ToString();
    }

    private static void PrintUsage()
    {
        Console.WriteLine($"\n  \e[97mUSAGE: \e[90mcphr [mode] \e[36m{Name} \e[33m[key] \e[32m[input] \e[90m[extra?...]\e[0m\n");
        Arguments.PrintArgs();
    }

    private class Arguments : CipherArgumentBase<Arguments, string>
    {
        [IsFlag("with-underscore", "Whether to add an underscore to the cipher table.")]
        public bool WithUnderscore { get; set; }
    }
}
