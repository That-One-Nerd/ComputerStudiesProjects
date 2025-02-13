using System;
using System.IO;
using System.Text;

namespace CipherCrypt.Writers;

public class NormalizedTextWriter : TextWriter
{
    public override Encoding Encoding => underlying.Encoding;

    private readonly TextWriter underlying;

    private int written;

    public NormalizedTextWriter(TextWriter underlying)
    {
        this.underlying = underlying;
    }

    public override void Write(string? value) => underlying.Write(value);
    public override void Write(char value)
    {
        value = char.ToUpper(value);
        if (value >= 'A' && value <= 'Z')
        {
            underlying.Write(value);
            written++;
            if (written % 5 == 0) underlying.Write(' ');
        }
    }
}
