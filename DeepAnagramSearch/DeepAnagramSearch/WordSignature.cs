using System.Text;

namespace DeepAnagramSearch;

public class WordSignature : IEquatable<WordSignature>
{
    public int Length { get; private set; }

    private readonly int[] counts;

    private WordSignature()
    {
        counts = new int[26];
    }

    public int this[char index]
    {
        get
        {
            if (index >= 'a' && index <= 'z') return counts[index - 'a'];
            else if (index >= 'A' && index <= 'Z') return counts[index - 'A'];
            else return -1;
        }
    }

    public bool Contains(WordSignature inner)
    {
        for (int i = 0; i < counts.Length; i++)
        {
            if (inner.counts[i] > counts[i]) return false;
        }
        return true;
    }
    public bool Equals(WordSignature? other)
    {
        if (other is null) return false;
        for (int i = 0; i < 26; i++)
        {
            if (counts[i] != other.counts[i]) return false;
        }
        return true;
    }

    public override string ToString()
    {
        StringBuilder result = new();
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > 0)
            {
                result.Append((char)('a' + i));
                result.Append(counts[i]);
            }
        }
        return result.ToString();
    }

    public static WordSignature FromPhrase(string phrase)
    {
        WordSignature result = new();
        for (int i = 0; i < phrase.Length; i++)
        {
            char c = phrase[i];
            if (c >= 'a' && c <= 'z')
            {
                result.counts[c - 'a']++;
                result.Length++;
            }
            else if (c >= 'A' && c <= 'Z')
            {
                result.counts[c - 'A']++;
                result.Length++;
            }
        }
        return result;
    }

    public static WordSignature operator +(WordSignature a, WordSignature b)
    {
        WordSignature result = new();
        for (int i = 0; i < result.counts.Length; i++)
        {
            result.counts[i] = a.counts[i] + b.counts[i];
        }
        result.Length = a.Length + b.Length;
        return result;
    }
    public static WordSignature operator -(WordSignature a, WordSignature b)
    {
        WordSignature result = new();
        for (int i = 0; i < result.counts.Length; i++)
        {
            result.counts[i] = a.counts[i] - b.counts[i];
        }
        result.Length = a.Length - b.Length;
        return result;
    }
}
