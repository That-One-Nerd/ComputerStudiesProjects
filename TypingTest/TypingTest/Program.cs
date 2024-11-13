/**********722871**********
 * Date: 10/25/2024
 * Programmer: Kyle Gilbert
 * Program Name: TypingTest
 * Program Description: Gives you a real typing challenge with obscure and large words.
 **************************/

using System.Text;

namespace TypingTest;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Start typing to begin.\n");

        string dataset = PickRandomDataset();
        Console.WriteLine("\x1b[90m" + dataset);
        Console.SetCursorPosition(0, 2);

        bool cancel = false;
        Console.CancelKeyPress += (o, e) =>
        {
            if (!cancel)
            {
                e.Cancel = true;
                cancel = true;
            }
        };

        int index = 0;
        Console.Title = "";
        bool started = false;
        DateTime startTime = DateTime.MinValue;
        DateTime endTime = DateTime.MaxValue;
        TimeSpan timer = TimeSpan.FromMinutes(1);
        int charsTyped = 0, mistakesTyped = 0, wordsTyped = 0,
            lenTyped = 0;
        CancellationTokenSource token = new();
        while (index < dataset.Length && DateTime.Now < endTime && !cancel)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (!started)
            {
                startTime = DateTime.Now;
                endTime = startTime + timer;
                started = true;
                DisplayTimer(timer, token.Token);
            }
            char expected = dataset[index];
            if (key.KeyChar == expected)
            {
                lock (LOCK)
                {
                    int prevLeft = Console.CursorLeft;
                    Console.Write("\x1b[92m" + expected);
                    if (prevLeft == Console.WindowWidth - 1) Console.WriteLine();
                }
                charsTyped++;
                lenTyped++;
                index++;
                if (expected == ' ' || index == dataset.Length - 1) wordsTyped++;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                lock (LOCK)
                {
                    int curLeft = Console.CursorLeft, curTop = Console.CursorTop;
                    Console.Write("\x1b[91m" + key.KeyChar);
                    Console.SetCursorPosition(curLeft, curTop);
                }
                charsTyped++;
                mistakesTyped++;
            }
        }
        token.Cancel();
        endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        int prevTop = Console.CursorTop;
        Console.Write(new string(' ', dataset.Length - lenTyped));
        Console.SetCursorPosition(0, prevTop);

        double wpm = wordsTyped / duration.TotalMinutes,
               cpm = (charsTyped - mistakesTyped) / duration.TotalMinutes,
               accuracy = 100.0 * (charsTyped - mistakesTyped) / charsTyped;

        Console.WriteLine($"\x1b[0m\n\nGood work! You typed {wordsTyped} words in {duration.Minutes}:{duration.Seconds:00}:{duration.Milliseconds:000}!");
        Console.WriteLine($"Characters per minute: {cpm:0.00}, Words per minute: {wpm:0.00}.");
        Console.WriteLine($"Accuracy: {accuracy:0.00}%");

        while (true) Console.ReadKey(true);
    }

    private static readonly object LOCK = new();

    private static string PickRandomDataset()
    {
        StreamReader reader = new("words.txt");
        string[] lines = reader.ReadToEnd().Split('\n', StringSplitOptions.TrimEntries);
        reader.Close();

        Random rand = new();
        int chars = 0;
        StringBuilder result = new();
        while (chars < 500)
        {
            int index = rand.Next(0, lines.Length);
            string word = lines[index];

            chars += word.Length + 1;
            result.Append(word + " ");
        }
        return result.ToString();
    }
    private static async void DisplayTimer(TimeSpan max, CancellationToken cancel)
    {
        DateTime start = DateTime.Now;
        while (!cancel.IsCancellationRequested)
        {
            DateTime cur = DateTime.Now;
            TimeSpan delta = cur - start;
            lock (LOCK)
            {
                int prevLeft = Console.CursorLeft, prevTop = Console.CursorTop;
                Console.SetCursorPosition(0, 0);
                Console.Write($"\x1b[0mTimer: {delta.Minutes}:{delta.Seconds:00}.{delta.Milliseconds:000} / {max.Minutes}:{max.Seconds:00}");
                Console.SetCursorPosition(prevLeft, prevTop);
            }
            await Task.Delay(100);
        }
    }
}