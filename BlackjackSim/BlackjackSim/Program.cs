/*****722871
 * Date: 9/18/2024
 * Programmer: Kyle
 * Program Name: Blackjack
 * Program Description: Simulates casino blackjack as accurately as possible.
 */

using BlackjackSim.Dealers;
using BlackjackSim.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackSim;

public static class Program
{
    public static async Task Main()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.Unicode;
        DealerBase dealer = new DealerStandard()
        {
            BlackjackPayment = 1.5f,
            WinPayment = 1.0f,
            DrawTo = 16,
        };
        await SimulatePlayerGraph(dealer, new PlayerCardCountingSimple());
        Console.ResetColor();
    }

    public static async Task SimulatePlayerGraph(DealerBase dealer, PlayerBase player)
    {
        Game game = new(dealer, player);
        List<GraphSegment> moneyGraph = [];
        int rounds = 0;
        while (player.Money > 1)
        {
            game.PlayRound(false);
            moneyGraph.Add(new(player.Money, player.DeltaMoneyThisRound));
            RenderOnce(dealer, player, moneyGraph);

            rounds++;
            await Task.Delay(20);
        }
    }
    public static Task SimulatePlayerGraphFast(DealerBase dealer, PlayerBase player, int iters)
    {
        Game game = new(dealer, player);
        List<GraphSegment> moneyGraph = [];

        for (int r = 0; r < iters; r++)
        {
            moneyGraph.Clear();
            player.Money = player.InitialMoney;

            int roundsThisTime = 0;
            while (player.Money > 1 && roundsThisTime < 10_000 && r < iters)
            {
                game.PlayRound(false);
                lock (moneyGraph)
                {
                    moneyGraph.Add(new(player.Money, player.DeltaMoneyThisRound));
                }
                r++;
                roundsThisTime++;
            }
            RenderOnce(dealer, player, moneyGraph);
        }

        return Task.CompletedTask;
    }

    private static int lastHandsWon, lastHandsLost, lastHandsPushed, lastHandsDoubled, lastHandsSplit, lastHandsBlackjacked, lastInsuranceWins, lastInsuranceLosses, insuranceShowDirection;
    private static void RenderOnce(DealerBase dealer, PlayerBase player, List<GraphSegment> segments)
    {
        Console.SetCursorPosition(0, 1);
        Console.ResetColor();
        Console.WriteLine($"\x1b[37m  Player Funds: \x1b[97m${player.Money:0.00}  ");

        double moneyBefore = player.Money - player.DeltaMoneyThisRound;
        double percent = (player.Money / moneyBefore - 1) * 100;
        string color = player.DeltaMoneyThisRound > 0 ? "\x1b[32m" : (player.DeltaMoneyThisRound < 0 ? "\x1b[31m" : "\x1b[37m");
        Console.WriteLine($"\x1b[37m    Delta: {color}${player.DeltaMoneyThisRound,8:0.00}  ({percent,5:0.0}%)    ");

        Console.WriteLine($"\n\x1b[37m  Hands Won:    {(player.HandsWon > lastHandsWon ? "\x1b[92m+" : "\x1b[90m ")}{player.HandsWon,4}  ({100.0 * player.HandsWon / player.HandsPlayed,4:0.0}%) ");
        Console.WriteLine($"\x1b[37m  Hands Lost:   {(player.HandsLost > lastHandsLost ? "\x1b[91m+" : "\x1b[90m ")}{player.HandsLost,4}  ({100.0 * player.HandsLost / player.HandsPlayed,4:0.0}%) ");
        Console.WriteLine($"\x1b[37m  Hands Pushed: {(player.HandsPushed > lastHandsPushed ? "\x1b[97m+" : "\x1b[90m ")}{player.HandsPushed,4}  ({100.0 * player.HandsPushed / player.HandsPlayed,4:0.0}%) ");
        Console.WriteLine($"\x1b[37m  Blackjacks:   {(player.HandsBlackjacked > lastHandsBlackjacked ? "\x1b[33m+" : "\x1b[90m ")}{player.HandsBlackjacked,4}  ({100.0 * player.HandsBlackjacked / player.HandsPlayed,4:0.0}%) ");

        Console.WriteLine($"\n\x1b[37m  Hands Doubled: {(player.HandsDoubled > lastHandsDoubled ? "\x1b[97m+" : "\x1b[90m ")}{player.HandsDoubled,3}  ({100.0 * player.HandsDoubled / player.HandsPlayed,4:0.0}%) ");
        Console.WriteLine($"\x1b[37m  Hands Split:   {(player.HandsSplit > lastHandsSplit ? "\x1b[97m+" : "\x1b[90m ")}{player.HandsSplit,3}  ({100.0 * player.HandsSplit / player.HandsPlayed,4:0.0}%) ");

        percent = 100 * dealer.HouseEdge - 100;
        Console.WriteLine($"\n\x1b[37m  House Edge: {(percent > 0 ? "\x1b[31m" : "\x1b[32m")}{percent,5:0.0}%  ");

        int centerX = (int)(Console.WindowWidth * 0.5);
        Console.SetCursorPosition(centerX, 4);
        string winWrite = $"\x1b[37mInsurance Wins:   {(player.InsuranceWon > lastInsuranceWins ? "\x1b[92m+" : "\x1b[90m ")}{player.InsuranceWon,4}  ({100.0 * player.InsuranceWon / player.InsurancePlayed,4:0.0}%) ";
        Console.Write(winWrite);
        if (player.InsuranceWon > lastInsuranceWins) insuranceShowDirection = 1;
        
        Console.SetCursorPosition(centerX, 5);
        string loseWrite = $"\x1b[37mInsurance Losses: {(player.InsuranceLost > lastInsuranceLosses ? "\x1b[91m+" : "\x1b[90m ")}{player.InsuranceLost,4}  ({100.0 * player.InsuranceLost / player.InsurancePlayed,4:0.0}%) ";
        Console.Write(loseWrite);
        if (player.InsuranceLost > lastInsuranceLosses) insuranceShowDirection = -1;
        
        if (insuranceShowDirection > 0)
        {
            Console.Write(new string(' ', 10));
            Console.SetCursorPosition(centerX + 33, 4);
            Console.Write($" ↑ ${player.InsuranceDelta:0.00}");
        }
        else if (insuranceShowDirection < 0)
        {
            Console.Write($" ↓ ${-player.InsuranceDelta:0.00}");
            Console.SetCursorPosition(centerX + 33, 4);
            Console.Write(new string(' ', 10));
        }

        lastHandsWon = player.HandsWon;
        lastHandsLost = player.HandsLost;
        lastHandsPushed = player.HandsPushed;
        lastHandsDoubled = player.HandsDoubled;
        lastHandsSplit = player.HandsSplit;
        lastHandsBlackjacked = player.HandsBlackjacked;
        lastInsuranceWins = player.InsuranceWon;
        lastInsuranceLosses = player.InsuranceLost;

        StringBuilder[] total, local;
        lock (segments)
        {
            total = GenerateGraph(segments.Count >= 10000 ? segments[^10000..] : segments, 40, 15);
            local = GenerateGraph(segments.Count >= 40 ? segments[^40..] : segments, 40, 15);
        }
        RenderTwoGraphs(total, local, 1, 40, 40);
    }

    public static StringBuilder[] GenerateGraph(IEnumerable<GraphSegment> segments, int width,
        int height, bool render = false)
    {
        StringBuilder[] result = new StringBuilder[height];
        Dictionary<int, List<(int x, int d)>> points = [];

        // Locate min and max of the graph.
        double maxMoney = int.MinValue;
        double minMoney = int.MaxValue;
        int count = 0;
        foreach (GraphSegment seg in segments)
        {
            if (seg.money > maxMoney) maxMoney = seg.money;
            else if (seg.money < minMoney) minMoney = seg.money;
            count++;
        }

        // Converts the graph segments into list of X points by Y.
        // Used for the stringbuilders, since they're grouped by Y
        // rather than X.
        int index = 0;
        foreach (GraphSegment seg in segments)
        {
            // Change lerp scale.
            int x = (int)((double)index / count * width);
            int y = (int)((seg.money - minMoney) / (maxMoney - minMoney) * (height * 2));

            y = height * 2 - y;

            if (points.TryGetValue(y, out List<(int x, int d)>? p)) p.Add((x, Math.Sign(seg.delta)));
            else points.Add(y, [(x, Math.Sign(seg.delta))]);
            index++;
        }

        // Sort the lists. Slows it down a little but I got issues with negative spacing.
        foreach (KeyValuePair<int, List<(int x, int d)>> xVals in points)
            xVals.Value.Sort((a, b) => a.x.CompareTo(b.x));

        // Generate the lines. Two sets of y-values can fit on each
        // line, so that's what we'll do.
        for (int i = 0; i < height; i++)
        {
            StringBuilder line = new("\x1b[0m"); // Start with reset.
            List<(int x, int d)> upper, lower;
            if (!points.TryGetValue(i * 2, out upper!)) upper = [];
            if (!points.TryGetValue(i * 2 + 1, out lower!)) lower = [];

            int curPos = 0;

            // The color only changes when we hit a point.
            List<(int x, int d)> totalHits = [.. upper.Concat(lower).Distinct(DirectionComparer.Instance)];
            foreach ((int x, int d) tuple in totalHits)
            {
                if (curPos > tuple.x) continue; // Why???
                line.Append(new string(' ', tuple.x - curPos));
                bool top = upper.Contains(tuple), bot = lower.Contains(tuple);

                if (tuple.d > 0) line.Append("\x1b[92m");
                else if (tuple.d < 0) line.Append("\x1b[91m");
                else line.Append("\x1b[37m");

                if (top && bot) line.Append('█');
                else if (top) line.Append('▀');
                else if (bot) line.Append('▄');
                else line.Append(' ');
                curPos = tuple.x + 1;
            }
            line.Append(new string(' ', width - curPos));
            result[i] = line;
        }

        if (render) RenderOneGraph(result, width, height);
        return result;
    }

    public static void RenderOneGraph(StringBuilder[] lines, int width, int height)
    {
        // Render the graph.
        int top = Console.WindowHeight - height - 1;
        int left = (Console.WindowWidth - width - 1) / 2;
        for (int y = 0; y < height; y++)
        {
            Console.SetCursorPosition(left, top + y);
            Console.Write(lines[y]);
        }
    }
    public static void RenderTwoGraphs(StringBuilder[] linesA, StringBuilder[] linesB,
        int spacing, int widthA, int widthB)
    {
        int height = Math.Max(linesA.Length, linesB.Length);
        StringBuilder[] total = new StringBuilder[height];

        int totalWidth = widthA + spacing + widthB;

        // Combine graphs.
        for (int i = 0; i < height; i++)
        {
            StringBuilder combined;
            if (i < linesA.Length) combined = linesA[i];
            else combined = new(new string(' ', widthA));
            combined.Append(new string(' ', spacing));
            if (i < linesB.Length) combined.Append(linesB[i]);
            total[i] = combined;
        }
        // Render the graph.
        int top = Console.WindowHeight - height - 1;
        int left = (Console.WindowWidth - totalWidth - 1) / 2;
        for (int y = 0; y < height; y++)
        {
            Console.SetCursorPosition(left, top + y);
            Console.Write(total[y]);
        }
    }
    
    public class DirectionComparer : IEqualityComparer<(int x, int d)>
    {
        public static readonly DirectionComparer Instance = new();

        public bool Equals((int x, int d) a, (int x, int d) b) => a.x == b.x;
        public int GetHashCode((int x, int d) item) =>
            item.x.GetHashCode() ^ item.d.GetHashCode();
    }

    public readonly struct GraphSegment
    {
        public readonly double money;
        public readonly double delta;

        public GraphSegment(double money, double delta)
        {
            this.money = money;
            this.delta = delta;
        }
    }
}
