using System.IO;
using System.Text;

namespace CipherCrypt.Writers;

public class NoPunctuationTextWriter : TextWriter
{
    public override Encoding Encoding => underlying.Encoding;

    private readonly TextWriter underlying;

    public NoPunctuationTextWriter(TextWriter underlying)
    {
        this.underlying = underlying;
    }

    public override void Write(string? value) => underlying.Write(value);
    public override void Write(char value)
    {
        if (char.IsPunctuation(value)) return; // No punctuation.
        else underlying.Write(value);
    }
}
