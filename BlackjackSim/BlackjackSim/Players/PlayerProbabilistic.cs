using System.Collections.Generic;

namespace BlackjackSim.Players;

public class PlayerProbabilistic : PlayerBase
{
    public double BetValue { get; set; } = 100;

    public PlayerProbabilistic() : base("Probabilistic Choices") { }

    private readonly List<Card> seenCards = [], remainingCards = [];
    private Card dealerCard;

    public override void OnShoeReset(int decks)
    {
        seenCards.Clear();
        remainingCards.Clear();
        for (int i = 0; i < decks; i++) remainingCards.AddRange(Card.GetDeck());
    }
    public override void OnSeenCard(Card card, bool yours)
    {
        seenCards.Add(card);
        remainingCards.Remove(card);
    }
    public override void InitialVisibleDealerCard(Card card) => dealerCard = card;

    public override double PlaceInitialBet() => BetValue;
    public override double MakeInsuranceBet()
    {
        // Calculate odds that the dealer's hand equals 21.
        Hand temp = new(this)
        {
            cards = [dealerCard]
        };

        int matches = 0, doesntMatch = 0;
        for (int i = 0; i < remainingCards.Count; i++)
        {
            Card added = remainingCards[i];
            temp.cards.Add(added);
            if (temp.GetValue() == 21) matches++;
            else doesntMatch++;
            temp.cards.Remove(added);
        }

        double trueOdds = (double)matches / (matches + doesntMatch);
        double weightedOdds = ApplyBias(trueOdds, 0.65);
        return BetValue * 0.5 * weightedOdds; // Bet proportionally.
    }

    public override bool ShouldDouble(Hand hand) => false; // TODO
    public override bool ShouldSplit(Hand hand) => false; // TODO: How??
    public override bool ShouldHit(Hand hand)
    {
        // Determine odds that the next card will bust.
        Hand temp = new(this)
        {
            cards = new(hand.cards),
        };

        int willBust = 0, wontBust = 0;
        for (int i = 0; i < remainingCards.Count; i++)
        {
            Card added = remainingCards[i];
            temp.cards.Add(added);
            if (temp.GetValue() > 21) willBust++;
            else wontBust++;
            temp.cards.Remove(added);
        }

        if ((double)willBust / (willBust + wontBust) >= 0.5) return false;
        else return true;
    }

    private static double ApplyBias(double x, double bias)
    {
        double k = (1 - bias) * (1 - bias) * (1 - bias); // Better than pow.
        return x * k / (x * k - x + 1);
    }
}
