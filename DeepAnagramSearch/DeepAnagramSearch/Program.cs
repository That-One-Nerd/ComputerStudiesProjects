/**********722871**********
 * Date: 5/9/2025
 * Programmer: Kyle Gilbert
 * Program Name: DeepAnagramSearch
 * Program Description: Searches for anagrams that consist of multiple words.
 **************************/

using System.Text;

namespace DeepAnagramSearch;

internal static class Program
{
    private static readonly int minWordsLimit = 1, maxWordsLimit = 10,
                                wordsYellowMin = 3, wordsRedMin = 6;

    private static string inputPhrase = "";
    private static int maxWordSearch;

    private static long itersComputed = 0;
    private static readonly string[] words;

    private static readonly List<string> results = [];

    static Program()
    {
        using StreamReader reader = new(@"wordlist_nc.txt");
        words = reader.ReadToEnd().Replace("\r", "").Split(' ', '\n');
        /*reader.Close();

        int init = words.Length;

        Console.WriteLine("a");
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = new(words[i].Where(x => (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z')).ToArray());
        }
        Console.WriteLine("b");
        List<string> newWords = new(words.Length);
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            if (word.Count(char.IsUpper) > 1) continue;
            else if (newWords.Contains(word)) continue;
            
            newWords.Add(word);
            if (i % 10_000 == 0) Console.Title = i.ToString();
        }
        Console.WriteLine("c");
        newWords.Sort();
        Console.Write("d");

        StreamWriter writer = new("words_100k_formatted.txt");
        for (int i = 0; i < newWords.Count; i++)
        {
            writer.Write($"{newWords[i]} ");
        }
        writer.Close();

        Console.WriteLine("one");

        Console.WriteLine($"{words.Length} -> {newWords.Count}");
        words = [.. newWords];

        Console.ReadKey(true);
        Console.Clear();*/
    }

    public static void Main()
    {
        MenuInput();
        Console.Clear();

        CancellationTokenSource cancel = new();
        Task runner = DisplayLoop(cancel.Token);

        WordSignature overall = WordSignature.FromPhrase(inputPhrase);
        using StreamWriter writer = new("anagrams_out.txt");
        foreach (string anagram in GetAnagrams(maxWordSearch, overall))
        {
            string a2 = anagram.Replace("  ", " ").Trim();
            if (a2 == inputPhrase) continue;

            writer.WriteLine(a2);
            results.Insert(0, a2);
        }

        cancel.Cancel();
        runner.Wait();
    }

    private static readonly Random rand = new();
    private static IEnumerable<string> GetAnagrams(int maxWords, WordSignature overall)
    {
        if (maxWords < 1) yield break;
        string[] wordsCopy = new string[words.Length];
        Array.Copy(words, wordsCopy, words.Length);
        rand.Shuffle(wordsCopy);
        for (int i = 0; i < wordsCopy.Length; i++)
        {
            itersComputed++;
            string word = wordsCopy[i];
            if (word.Length > overall.Length) continue; // Too long to possibly be a fit.

            WordSignature thisSig = WordSignature.FromPhrase(word);
            if (maxWords == 1)
            {
                // One word, must completely fill the signature.
                if (!overall.Equals(thisSig)) continue;
                yield return word;
            }
            else
            {
                // Room for more words, we can allow a remaining signature.
                if (!overall.Contains(thisSig)) continue;
                WordSignature remaining = overall - thisSig;
                foreach (string others in GetAnagrams(maxWords - 1, remaining))
                {
                    yield return word + " " + others;
                }
            }
        }
    }

    private static async Task DisplayLoop(CancellationToken cancel)
    {
        Console.CursorVisible = false;
        Console.SetCursorPosition(6, 3);
        Console.Write($"\x1b[36mInput\x1b[97m: \x1b[32m{inputPhrase}\x1b[0m");

        Console.SetCursorPosition(6, Console.CursorTop + 1);
        Console.Write("\x1b[35mMax Words\x1b[97m: ");

        string wordsToPrint = new string('#', maxWordSearch) + new string(' ', maxWordsLimit - maxWordSearch);
        if (maxWordSearch > wordsRedMin) wordsToPrint = wordsToPrint.Insert(wordsRedMin, "\x1b[31m");
        if (maxWordSearch > wordsYellowMin) wordsToPrint = wordsToPrint.Insert(wordsYellowMin, "\x1b[33m");
        if (maxWordSearch > 0) wordsToPrint = wordsToPrint.Insert(0, "\x1b[32m");
        Console.Write(wordsToPrint);

        int top = Console.CursorTop + 2;

        WordSignature completeSig = WordSignature.FromPhrase(inputPhrase);
        while (true)
        {
            Console.SetCursorPosition(6, top);
            Console.Write($"\x1b[90mCurrent Iters: \x1b[36m{formatNum(itersComputed)} ");

            Console.SetCursorPosition(6, top + 1);
            Console.Write($"\x1b[90mComplete Signature: \x1b[32m{completeSig}");

            Console.SetCursorPosition(6, top + 3);
            Console.Write("\x1b[90mResults: ");

            if (results.Count == 0)
            {
                if (cancel.IsCancellationRequested) Console.Write("\x1b[31mNo Anagrams Found");
                else Console.Write("\x1b[33mNone Yet");
            }
            else
            {
                Console.Write($"\x1b[32m{results.Count} Anagram{(results.Count == 1 ? "" : "s")} Found\x1b[93m");
                for (int i = 0; i < int.Min(results.Count, 8); i++)
                {
                    string anagram = results[i];
                    Console.SetCursorPosition(8, top + 4 + i);
                    Console.Write($"\"{anagram}\"     ");
                }
            }

            Console.ResetColor();

            if (cancel.IsCancellationRequested) return; // End here.
            await Task.Delay(300, CancellationToken.None);
        }

        static string formatNum(long num)
        {
            if (num < 0) return $"-{formatNum(-num)}";

            if (num < 1e3) return num.ToString();
            else if (num < 1e4) return $"{num * 1e-3:0.000} thou";
            else if (num < 1e5) return $"{num * 1e-3:0.00} thou";
            else if (num < 1e6) return $"{num * 1e-3:0.0} thou";
            else if (num < 1e7) return $"{num * 1e-6:0.000} mil";
            else if (num < 1e8) return $"{num * 1e-6:0.00} mil";
            else if (num < 1e9) return $"{num * 1e-6:0.0} mil";
            else if (num < 1e10) return $"{num * 1e-9:0.000} bil";
            else if (num < 1e11) return $"{num * 1e-9:0.00} bil";
            else if (num < 1e12) return $"{num * 1e-9:0.0} bil";
            else return $"{num * 1e-12:0}t";
        }
    }

    private static void MenuInput()
    {
        StringBuilder input = new();
        int deltaChar = 0, insertPos = 0;
        int section = 0;

        int maxWords = 5;
        while (true)
        {
            Console.SetCursorPosition(6, 3);
            Console.Write($"\x1b[36mInput\x1b[97m: \x1b[32m{input}\x1b[0m");
            if (deltaChar < 0) Console.Write(new string(' ', -deltaChar));

            Console.SetCursorPosition(6, Console.CursorTop + 1);
            Console.Write("\x1b[35mMax Words\x1b[97m: ");

            string wordsToPrint = new string('#', maxWords) + new string(' ', maxWordsLimit - maxWords);
            if (maxWords > wordsRedMin) wordsToPrint = wordsToPrint.Insert(wordsRedMin, "\x1b[31m");
            if (maxWords > wordsYellowMin) wordsToPrint = wordsToPrint.Insert(wordsYellowMin, "\x1b[33m");
            if (maxWords > 0) wordsToPrint = wordsToPrint.Insert(0, "\x1b[32m");

            Console.Write(wordsToPrint);
            Console.SetCursorPosition(6, Console.CursorTop + 2);

            if (section == 2) Console.Write("\x1b[30;102m[ GO ]\x1b[0m");
            else Console.Write("\x1b[40;92m[ GO ]\x1b[0m");

            bool exit = section switch
            {
                0 => handleTopSection(),
                1 => handleMiddleSection(),
                2 => handleBottomSection(),
                _ => false
            };
            if (exit)
            {
                inputPhrase = input.ToString();
                maxWordSearch = maxWords;
                return;
            }
        }

        bool handleTopSection()
        {
            int firstLen = input.Length;
            Console.CursorVisible = true;
            Console.SetCursorPosition((13 + insertPos) % Console.WindowWidth, 3 + (13 + insertPos) / Console.WindowWidth);
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (char.IsControl(key.KeyChar))
            {
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow or ConsoleKey.Enter or ConsoleKey.Tab: section = (section + 1) % 3; break;
                    case ConsoleKey.UpArrow: section = (section - 1) % 3; break;

                    case ConsoleKey.Backspace:
                        if (input.Length == 0) break;

                        if ((key.Modifiers & ConsoleModifiers.Control) != 0)
                        {
                            // Delete one word.
                            int wordIndex = insertPos;
                            while (wordIndex > 0)
                            {
                                wordIndex--;
                                if (input[wordIndex] == ' ') break;
                            }
                            input.Remove(wordIndex, insertPos - wordIndex);
                            insertPos -= insertPos - wordIndex;
                        }
                        else
                        {
                            // Delete one character.
                            input.Remove(insertPos - 1, 1);
                            insertPos--;
                        }
                        break;

                    case ConsoleKey.Escape:
                        input.Clear();
                        insertPos = 0;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (insertPos > 0) insertPos--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (insertPos < input.Length) insertPos++;
                        break;
                }
            }
            else
            {
                input.Insert(insertPos, key.KeyChar);
                insertPos++;
            }
            deltaChar = input.Length - firstLen;
            return false;
        }
        bool handleMiddleSection()
        {
            Console.CursorVisible = true;
            int extraLines = (13 + insertPos) / Console.WindowWidth;
            Console.SetCursorPosition(17 + maxWords, 4 + extraLines);
            ConsoleKeyInfo input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.DownArrow or ConsoleKey.Enter or ConsoleKey.Tab: section = (section + 1) % 3; break;
                case ConsoleKey.UpArrow: section = (section - 1) % 3; break;

                case ConsoleKey.LeftArrow:
                    if (maxWords > minWordsLimit) maxWords--;
                    break;
                case ConsoleKey.RightArrow:
                    if (maxWords < maxWordsLimit) maxWords++;
                    break;
            }
            return false;
        }
        bool handleBottomSection()
        {
            Console.CursorVisible = false;
            ConsoleKeyInfo input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.DownArrow or ConsoleKey.Tab: section = (section + 1) % 3; break;
                case ConsoleKey.UpArrow: section = (section - 1) % 3; break;

                case ConsoleKey.Enter: return true;
            }
            return false;
        }
    }
}
