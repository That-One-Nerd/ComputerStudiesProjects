using ArgumentBase;
using System.Text;
using System;
using System.IO;
using CipherCrypt.Writers;

namespace CipherCrypt;

public class CipherArgumentBase<TSelf, TKey> : CipherArgumentBase<TSelf>
    where TSelf : CipherArgumentBase<TSelf>, new()
{
    [IsParameter(1, "The key to use when ciphering.")]
    public TKey Key { get; set; }
}

public class CipherArgumentBase<TSelf> : ArgumentBase<TSelf>
    where TSelf : CipherArgumentBase<TSelf>, new()
{
    [IsVariable("text", "Specify raw text to apply the cipher to.")]
    public string? Plaintext { get; set; }

    [IsVariable("infile", "Specify a file path to apply the cipher to.")]
    public string? InputFile { get; set; }

    [IsFlag("inbuf", "Specify the input to come from the console buffer.")]
    public bool InputFromConsole { get; set; }

    [IsVariable("outfile", "Specify an output file path to write the ciphered text to.")]
    public string? OutputFile { get; set; }

    [IsFlag("no-punc", "Disable punctuation and special characters.")]
    public bool NoPunctuation { get; set; }

    [IsFlag("normalized", "Removes all non-alphabetic characters and places letters in groups of 5.")]
    public bool Normalized { get; set; }

    public (TextReader?, TextWriter) ConfigureIO()
    {
        TextReader? input;
        if (Plaintext is not null) input = new StringReader(Plaintext);
        else if (InputFile is not null) input = new StreamReader(InputFile);
        else if (InputFromConsole) input = Console.In;
        else input = null;

        TextWriter output;
        if (OutputFile is not null) output = new StreamWriter(OutputFile);
        else output = Console.Out;

        if (Normalized) output = new NormalizedTextWriter(output);
        else if (NoPunctuation) output = new NoPunctuationTextWriter(output);

        return (input, output);
    }

    public static void PrintArgs()
    {
        string[] inputs = ["-text", "-infile", "--inbuf"];
        StringBuilder inputsStr = new("  \e[97mInputs:\n");
        int maxLength = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            string name = inputs[i];
            if (name.Length > maxLength) maxLength = name.Length;
        }
        for (int i = 0; i < inputs.Length; i++)
        {
            string name = inputs[i];
            string desc;
            if (VariableDescriptions.TryGetValue(name, out string? temp)) desc = temp;
            else if (FlagDescriptions.TryGetValue(name, out temp)) desc = temp;
            else throw new();

            inputsStr.Append($"    \e[32m")
                     .Append(name)
                     .Append(new string(' ', maxLength - name.Length + 2))
                     .Append("\e[91m- \e[37m")
                     .Append(desc)
                     .Append("\e[0m\n");
        }
        Console.WriteLine(inputsStr);

        PrintVariables();
        PrintFlags();
    }
}
