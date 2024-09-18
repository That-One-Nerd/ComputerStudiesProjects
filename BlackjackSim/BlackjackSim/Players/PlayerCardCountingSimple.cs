using System;

namespace BlackjackSim.Players;

public class PlayerCardCountingSimple : PlayerTabular // Build on top of a good system.
{
    public PlayerCardCountingSimple()
    {
        Name = "Card Counting (Simple)";
    }

    private int counter;
    private int decks;

    public override void OnShoeReset(int decks)
    {
        this.decks = decks;
        counter = 0;
    }
    public override void OnSeenCard(Card card, bool yours)
    {
        int val = card.GetValue(true);
        if (val <= 6) counter++;
        else if (val >= 10) counter--;
    }

    public override double PlaceInitialBet() => BetSize * Math.Pow(1.1, -counter);
}
