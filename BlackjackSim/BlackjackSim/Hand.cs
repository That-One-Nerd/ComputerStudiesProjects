using System.Collections.Generic;
using System.Linq;

namespace BlackjackSim;

public class Hand
{
    public IPerson player;
    public List<Card> cards = [];
    public double bet;

    public HandStatus status;

    public Hand(IPerson player)
    {
        this.player = player;
    }

    public bool IsBlackjack() => cards.Count == 2 &&
        ((cards[0] == ValueKind.Ace && cards[1].GetValue(false) == 10) ||
         (cards[1] == ValueKind.Ace && cards[0].GetValue(false) == 10));
    public int GetValue()
    {
        // Count all non-aces first.
        int aceCount = cards.Count(x => x == ValueKind.Ace);
        int sum = 0;
        foreach (Card c in cards)
        {
            if (c == ValueKind.Ace) continue;
            sum += c.GetValue(false);
        }

        // You will never count two aces as 11, so we don't need to
        // search too deep. All other aces count as 1, the last one
        // is the only one we need to compare for.
        for (int i = 0; i < aceCount; i++)
        {
            if (i < aceCount - 1) sum += 1;
            else
            {
                if (sum >= 21) sum += 1;
                else sum += 11;
            }
        }

        return sum;
    }
}

public enum HandStatus
{
    Incomplete,
    Won,
    WonBlackjack,
    Push,
    Lose
}
