using System;
using System.Diagnostics.CodeAnalysis;

namespace BlackjackSim;

public readonly struct Card : IEquatable<Card>
{
    public readonly SuitKind suit;
    public readonly ValueKind value;

    public Card()
    {
        suit = SuitKind.Spades;
        value = ValueKind.Ace;
    }
    public Card(SuitKind suit, ValueKind value)
    {
        this.suit = suit;
        this.value = value;
    }
    public Card(ValueKind value, SuitKind suit)
    {
        this.suit = suit;
        this.value = value;
    }

    public static Card[] GetDeck()
    {
        SuitKind[] suits = Enum.GetValues<SuitKind>();
        ValueKind[] values = Enum.GetValues<ValueKind>();

        Card[] totalCards = new Card[suits.Length * values.Length];
        for (int s = 0; s < suits.Length; s++)
        {
            for (int v = 0; v < values.Length; v++)
            {
                totalCards[s * values.Length + v] = (suits[s], values[v]);
            }
        }
        return totalCards;
    }

    public int GetValue(bool acesAreEleven) => value switch
    {
        ValueKind.Ace => acesAreEleven ? 11 : 1,
        ValueKind.Two => 2,
        ValueKind.Three => 3,
        ValueKind.Four => 4,
        ValueKind.Five => 5,
        ValueKind.Six => 6,
        ValueKind.Seven => 7,
        ValueKind.Eight => 8,
        ValueKind.Nine => 9,
        ValueKind.Ten or ValueKind.Jack or ValueKind.Queen or ValueKind.King => 10,
        _ => 0
    };

    public override bool Equals(object? obj) => obj is Card objCard && Equals(objCard);
    public bool Equals(Card other) => suit == other.suit && value == other.value;
    public override int GetHashCode() => suit.GetHashCode() ^ value.GetHashCode();
    public override string ToString() => $"{value} of {suit}";

    public static bool operator ==(Card card, SuitKind suit) => card.suit == suit;
    public static bool operator ==(Card card, ValueKind value) => card.value == value;
    public static bool operator !=(Card card, SuitKind suit) => card.suit != suit;
    public static bool operator !=(Card card, ValueKind value) => card.value != value;

    public static implicit operator Card(ValueTuple<SuitKind, ValueKind> tuple) =>
        new(tuple.Item1, tuple.Item2);
    public static implicit operator Card(ValueTuple<ValueKind, SuitKind> tuple) =>
        new(tuple.Item1, tuple.Item2);
}
