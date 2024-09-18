using System.Collections.Generic;

namespace BlackjackSim.Players;

public class PlayerTabular : PlayerBase
{
    public double BetSize { get; set; } = 100;

    public Dictionary<int, Choice[]> HardTotals = new()
    {
        { 21, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { 20, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { 19, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { 18, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { 17, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { 16, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { 15, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { 14, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { 13, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { 12, [Choice.Hit        , Choice.Hit        , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { 11, [Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  ] },
        { 10, [Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        ] },
        {  9, [Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  8, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  7, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  6, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  5, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  4, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  3, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        {  2, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
    };
    public Dictionary<ValueCombined, Choice[]> AceSoftTotals = new()
    {
        { ValueCombined.Ten  , [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { ValueCombined.Nine , [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { ValueCombined.Eight, [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.DoubleStand, Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { ValueCombined.Seven, [Choice.DoubleStand, Choice.DoubleStand, Choice.DoubleStand, Choice.DoubleStand, Choice.DoubleStand, Choice.Stand      , Choice.Stand      , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Six  , [Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Five , [Choice.Hit        , Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Four , [Choice.Hit        , Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Three, [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Two  , [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
    };
    public Dictionary<ValueCombined, Choice[]> Pairs = new()
    {
        { ValueCombined.Ace  , [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      ] },
        { ValueCombined.Ten  , [Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      , Choice.Stand      ] },
        { ValueCombined.Nine , [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Stand      , Choice.Split      , Choice.Split      , Choice.Stand      , Choice.Stand      ] },
        { ValueCombined.Eight, [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      ] },
        { ValueCombined.Seven, [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Six  , [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Five , [Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.DoubleHit  , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Four , [Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Split      , Choice.Split      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Three, [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
        { ValueCombined.Two  , [Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Split      , Choice.Hit        , Choice.Hit        , Choice.Hit        , Choice.Hit        ] },
    };

    public PlayerTabular() : base("Tabular Gameplay") { }

    private Card dealerCard;

    public override double PlaceInitialBet() => BetSize;
    public override double MakeInsuranceBet() => 0;
    public override void InitialVisibleDealerCard(Card card) => dealerCard = card;

    public override bool ShouldDouble(Hand hand)
    {
        Choice choice = GetChoice(hand);
        return choice == Choice.DoubleHit || choice == Choice.DoubleStand;
    }
    public override bool ShouldSplit(Hand hand) => GetChoice(hand) == Choice.Split;
    public override bool ShouldHit(Hand hand)
    {
        Choice choice = GetChoice(hand);
        return choice == Choice.Hit || choice == Choice.DoubleHit;
    }

    private Choice GetChoice(Hand hand)
    {
        ValueCombined dealerCard = CardToValue(this.dealerCard);
        Choice[] row;
        if (hand.cards.Count == 2)
        {
            // Check for pairs.
            ValueCombined cardA = CardToValue(hand.cards[0]),
                          cardB = CardToValue(hand.cards[1]);
            if (cardA == cardB) row = Pairs[cardA]; // Load pair table.
            else if (cardA == ValueCombined.Ace) row = AceSoftTotals[cardB]; // Load soft totals.
            else if (cardB == ValueCombined.Ace) row = AceSoftTotals[cardA]; // Load soft totals.
            else row = HardTotals[hand.GetValue()]; // Not pair or ace, load hard totals.
        }
        else row = HardTotals[hand.GetValue()]; // Load hard totals.
        return row[(int)dealerCard];
    }

    public enum Choice
    {
        Stand,
        Hit,
        DoubleHit,
        DoubleStand,
        Split
    }
    public enum ValueCombined
    {
        Two = 0,
        Three = 1,
        Four = 2,
        Five = 3,
        Six = 4,
        Seven = 5,
        Eight = 6,
        Nine = 7,
        Ten = 8,
        Ace = 9
    }
    public static ValueCombined CardToValue(Card c) => c.value switch
    {
        ValueKind.Two => ValueCombined.Two,
        ValueKind.Three => ValueCombined.Three,
        ValueKind.Four => ValueCombined.Four,
        ValueKind.Five => ValueCombined.Five,
        ValueKind.Six => ValueCombined.Six,
        ValueKind.Seven => ValueCombined.Seven,
        ValueKind.Eight => ValueCombined.Eight,
        ValueKind.Nine => ValueCombined.Nine,
        ValueKind.Ten => ValueCombined.Ten,
        ValueKind.Jack => ValueCombined.Ten,
        ValueKind.Queen => ValueCombined.Ten,
        ValueKind.King => ValueCombined.Ten,
        ValueKind.Ace => ValueCombined.Ace,
        _ => throw new("invalid card?") // Shouldn't happen under correct usage.
    };
}
